// <copyright file="StreamPage.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
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

namespace SPIDIdentificationStreaming_WPF_Samples
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Cognitive.SpeakerRecognition.Streaming.Audio;
    using Microsoft.Cognitive.SpeakerRecognition.Streaming.Client;
    using Microsoft.Cognitive.SpeakerRecognition.Streaming.Result;
    using Microsoft.ProjectOxford.SpeakerRecognition;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for IdentifyFilePage.xaml
    /// </summary>
    public partial class StreamPage : Page
    {
        private string selectedFile = string.Empty;

        private SpeakerIdentificationServiceClient serviceClient;
        private RecognitionClient recognitionClient;

        Task streamingTask;
        CancellationTokenSource tokenSource;

        /// <summary>
        /// Initializes a new instance of the StreamPage class
        /// </summary>
        public StreamPage()
        {
            this.InitializeComponent();

            this._speakersListFrame.Navigate(SpeakersListPage.SpeakersList);

            MainWindow window = (MainWindow)Application.Current.MainWindow;
            this.serviceClient = new SpeakerIdentificationServiceClient(window.ScenarioControl.SubscriptionKey);
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
            this.selectedFile = openFileDialog.FileName;
        }

        private async void StreamAudio(int windowSize, int stepSize, Guid[] testProfileIds, CancellationToken token)
        {
            // Delay between passing audio chunks to the client in milliseconds
            int requestDelay = int.Parse(ConfigurationManager.AppSettings["RequestsDelay"]);

            // Unique id of the recognition client. Returned in the callback to relate results with clients in case of having several clients using the same callback
            var recognitionClientId = Guid.NewGuid();

            // Audio format of the recognition audio
            // Supported containers: WAV and RAW (no header)
            // Supported format: Encoding = PCM, Channels = Mono (1), Rate = 16k, Bits per sample = 16
            var audioFormat = new AudioFormat(AudioEncoding.PCM, 1, 16000, 16, new AudioContainer(AudioContainerType.WAV));
            using (Stream audioStream = File.OpenRead(this.selectedFile))
            {
                // Client factory is used to create a recognition client
                // Recognition client can be used for one audio only. In case of having several audios, a separate client should be created for each one
                var clientfactory = new ClientFactory();
                using (var recognitionClient = clientfactory.CreateRecognitionClient(recognitionClientId, testProfileIds, stepSize, windowSize, audioFormat, this.WriteResults, this.serviceClient))
                {
                    var chunkSize = 32000;
                    var buffer = new byte[chunkSize];
                    var bytesRead = 0;

                    while ((bytesRead = audioStream.Read(buffer, 0, buffer.Length)) > 0 && !token.IsCancellationRequested)
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

        private async void _streamBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            try
            {
                if(_windowSzBx.Text == string.Empty)
                {
                    throw new Exception("No window size was entered.");
                }
                // Window size in seconds
                int windowSize = int.Parse(_windowSzBx.Text);

                if (_stepSzBx.Text == string.Empty)
                {
                    throw new Exception("No step size was entered.");
                }
                // Amount of seconds needed for sending a request to server
                // If set to 1, the client will send a request to the server for every second recieved from the user
                // If set to 2, the client will send a request to the server for every 2 seconds recieved from the user
                int stepSize = int.Parse(_stepSzBx.Text);                

                if (this.selectedFile == string.Empty)
                {
                    throw new Exception("No File Selected.");
                } 

                Profile[] selectedProfiles = SpeakersListPage.SpeakersList.GetSelectedProfiles();
                Guid[] testProfileIds = new Guid[selectedProfiles.Length];
                for (int i = 0; i < testProfileIds.Length; i++)
                {
                    testProfileIds[i] = selectedProfiles[i].ProfileId;
                    window.Log("Speaker Profile Id: " + testProfileIds[i] + " has been selected for streaming.");
                }

                window.Log("Processing File...");
                this.DisplayAudio();

                tokenSource = new CancellationTokenSource();
                var ct = tokenSource.Token;

                streamingTask = Task.Factory.StartNew(() => StreamAudio(windowSize, stepSize, testProfileIds, ct));
                streamingTask.Wait();
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
            mediaPlayer.Source = new Uri(this.selectedFile);
            mediaPlayer.Play();
            _mediaElementStckPnl.Visibility = Visibility.Visible;
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            mediaPlayer.Volume = (double)volumeSlider.Value;
        }

        private void OnMouseDownStopMedia(object sender, MouseButtonEventArgs args)
        {
            this.StopPlayer();
        }

        private void StopPlayer()
        {
            mediaPlayer.Stop();
            tokenSource?.Cancel();
        }

        private void WriteResults(RecognitionResult recognitionResult)
        {
            Dispatcher.Invoke((Action)delegate
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                if (!recognitionResult.Succeeded)
                {
                    window.Log("Request " + recognitionResult.RequestId + " error message: " + recognitionResult.FailureMsg);
                    return;
                }

                var identificationResult = recognitionResult.Value;
                _identificationResultTxtBlk.Text = identificationResult.IdentifiedProfileId == Guid.Empty ? "Unknown" : identificationResult.IdentifiedProfileId.ToString();
                _identificationConfidenceTxtBlk.Text = identificationResult.Confidence.ToString();
                _identificationRequestIdTxtBlk.Text = recognitionResult.RequestId.ToString();
                var result = identificationResult.IdentifiedProfileId == Guid.Empty ? "Unknown" : identificationResult.IdentifiedProfileId.ToString();
                window.Log("Request " + recognitionResult.RequestId + ": Profile id: " + result);

                _identificationResultStckPnl.Visibility = Visibility.Visible;
            });
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(async delegate
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                this.serviceClient = new SpeakerIdentificationServiceClient(window.ScenarioControl.SubscriptionKey);
                await SpeakersListPage.SpeakersList.UpdateAllSpeakersAsync().ConfigureAwait(false);
                SpeakersListPage.SpeakersList.SetSingleSelectionMode();
            });
        }
        private void Page_UnLoaded(object sender, RoutedEventArgs e)
        {
            this.StopPlayer();
        }
    }
}
