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
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Audio;
using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Client;
using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Result;

namespace SPIDIdentificationStreaming_WPF_Samples
{
    /// <summary>
    /// Interaction logic for IdentifyFilePage.xaml
    /// </summary>
    public partial class StreamPage : Page
    {
        private string _selectedFile = "";
        private static readonly int headerSize = 44;
        private static readonly int bufferSize = 32000;
        private static readonly int windowSize = 5;
        private static readonly int stepSize = 1;

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

        private byte[] Readbytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private async void _streamBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            DisplayAudio();

            try
            {
                if (_selectedFile == "")
                    throw new Exception("No File Selected.");

                window.Log("Processing File...");
                Profile[] selectedProfiles = SpeakersListPage.SpeakersList.GetSelectedProfiles();
                Guid[] testProfileIds = new Guid[selectedProfiles.Length];
                for (int i = 0; i < testProfileIds.Length; i++)
                {
                    testProfileIds[i] = selectedProfiles[i].ProfileId;
                }


                var recoClientId = Guid.NewGuid();
                var audioFormat = new AudioFormat(AudioEncoding.PCM, 1, 16000, 16, new AudioContainer(AudioContainerType.RAW));
                using (Stream audioStream = new FileStream(_selectedFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] m_Bytes = Readbytes(audioStream);
                    using(var clientfactory = new ClientFactory())
                    using (var recoClient = clientfactory.CreateRecognitionClient(recoClientId, testProfileIds, stepSize, windowSize, audioFormat, this.WriteResults, this._serviceClient))
                    {
                        for (int i = headerSize; i < m_Bytes.Length; i += bufferSize)
                        {
                            var buffer = m_Bytes.Skip(i).Take(Math.Min(bufferSize, m_Bytes.Length - i)).ToArray();
                            await recoClient.StreamAudioAsync(buffer).ConfigureAwait(false);
                            await Task.Delay(1000);
                        }
                        await Task.Delay(10000);
                        await recoClient.EndStreamAudioAsync().ConfigureAwait(false);
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

        private void WriteResults(IRecognitionResult identification)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                if (identification.Failed)
                {
                    window.Log("Request " + identification.RequestId + " error message:"+identification.FailureMsg);
                    return;
                }
                var identificationResult = identification.Value;
                _identificationResultTxtBlk.Text = identificationResult.IdentifiedProfileId.ToString();
                _identificationConfidenceTxtBlk.Text = identificationResult.Confidence.ToString();
                _identificationRequestIdTxtBlk.Text = identification.RequestId.ToString();
                window.Log("Request " + identification.RequestId + ": Profileid: " + identificationResult.IdentifiedProfileId);

                _identificationResultStckPnl.Visibility = Visibility.Visible;
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SpeakersListPage.SpeakersList.SetMultipleSelectionMode();
        }
    }
}
