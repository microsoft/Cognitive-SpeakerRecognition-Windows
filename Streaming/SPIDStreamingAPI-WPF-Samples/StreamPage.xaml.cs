// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
// 
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/Cognitive-SpeakerRecognition-Windows
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Audio;
using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Client;
using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Result;

namespace SPIDIdentificationStreaming_WPF_Samples
{
    /// <summary>
    /// Interaction logic for IdentifyFilePage.xaml
    /// </summary>
    public partial class StreamPage : Page
    {
        private string _selectedFile = "";

        private SpeakerIdentificationServiceClient _serviceClient;

        /// <summary>
        /// Constructor to initialize the Identify File page
        /// </summary>
        public StreamPage()
        {
            InitializeComponent();

            _speakersListFrame.Navigate(SpeakersListPage.SpeakersList);

            MainWindow window = (MainWindow)Application.Current.MainWindow;
            _serviceClient = new SpeakerIdentificationServiceClient(window.ScenarioControl.SubscriptionKey);
        }

        private void _loadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WAV Files(*.wav)|*.wav";
            bool? result = openFileDialog.ShowDialog(window);

            if (!(bool)result)
            {
                window.Log("No File Selected.");
                return;
            }
            window.Log("File Selected: " + openFileDialog.FileName);
            _selectedFile = openFileDialog.FileName;
        }

        private async void _streamBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            
            try
            {
                // Window size in seconds
                int windowSize = int.Parse(_windowSzBx.Text);

                // Amount of seconds needed for sending a request to server
                // If set to 1, the client will send a request to the server for every second recieved from the user
                // If set to 2, the client will send a request to the server for every 2 seconds recieved from the user
                int stepSize = int.Parse(_stepSzBx.Text);

                // Delay between passing audio chunks to the client in milliseconds
                int requestDelay = int.Parse(ConfigurationManager.AppSettings["RequestsDelay"]);

                if (_selectedFile == "")
                    throw new Exception("No File Selected.");

                window.Log("Processing File...");
                DisplayAudio();

                Profile[] selectedProfiles = SpeakersListPage.SpeakersList.GetSelectedProfiles();
                Guid[] testProfileIds = new Guid[selectedProfiles.Length];
                for (int i = 0; i < testProfileIds.Length; i++)
                {
                    testProfileIds[i] = selectedProfiles[i].ProfileId;
                }

                // Unique id of the recognition client. Returned in the callback to relate results with clients in case of having several clients using the same callback
                var recognitionClientId = Guid.NewGuid();

                // Audio format of the recognition audio
                // Supported containers: WAV and RAW (no header)
                // Supported format: Encoding = PCM, Channels = Mono (1), Rate = 16k, Bits per sample = 16
                var audioFormat = new AudioFormat(AudioEncoding.PCM, 1, 16000, 16, new AudioContainer(AudioContainerType.WAV));
                using (Stream audioStream = File.OpenRead(_selectedFile))
                {
                    // Client factory is used to create a recognition client
                    // Recognition client can be used for one audio only. In case of having several audios, a separate client should be created for each one
                    var clientfactory = new ClientFactory();
                    using (var recognitionClient = clientfactory.CreateRecognitionClient(recognitionClientId, testProfileIds, stepSize, windowSize, audioFormat, this.WriteResults, this._serviceClient))
                    {
                        var chunkSize = 32000;
                        var buffer = new byte[chunkSize];
                        var bytesRead = 0;

                        while ((bytesRead = audioStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            // You can send any number of bytes not limited to 1 second
                            // If the remaining bytes of the last request are smaller than 1 second, it gets ignored
                            await recognitionClient.StreamAudioAsync(buffer, 0, bytesRead).ConfigureAwait(false);

                            // Simulates live streaming
                            // It's recommended to use a one second delay to guarantee receiving responses in the correct order
                            await Task.Delay(requestDelay).ConfigureAwait(false);
                        }

                        await recognitionClient.EndStreamAudioAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (IdentificationException ex)
            {
                window.Log("Speaker Identification Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                window.Log("Error: " + ex.Message);
            }
        }

        private async void DisplayAudio()
        {
            mediaPlayer.Source = new Uri(_selectedFile);
            mediaPlayer.Play();
            _mediaElementStckPnl.Visibility = Visibility.Visible;
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            mediaPlayer.Volume = (double)volumeSlider.Value;
        }

        private void WriteResults(RecognitionResult recognitionResult)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                if (!recognitionResult.Succeeded)
                {
                    window.Log("Request " + recognitionResult.RequestId + " error message: " + recognitionResult.FailureMsg);
                    return;
                }
                var identificationResult = recognitionResult.Value;
                _identificationResultTxtBlk.Text = identificationResult.IdentifiedProfileId.ToString();
                _identificationConfidenceTxtBlk.Text = identificationResult.Confidence.ToString();
                _identificationRequestIdTxtBlk.Text = recognitionResult.RequestId.ToString();
                window.Log("Request " + recognitionResult.RequestId + ": Profile id: " + identificationResult.IdentifiedProfileId);

                _identificationResultStckPnl.Visibility = Visibility.Visible;
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SpeakersListPage.SpeakersList.SetMultipleSelectionMode();
        }
    }
}
