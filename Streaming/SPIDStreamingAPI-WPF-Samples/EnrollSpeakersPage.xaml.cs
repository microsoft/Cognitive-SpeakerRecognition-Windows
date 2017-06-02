// <copyright file="EnrollSpeakersPage.xaml.cs" company="Microsoft">
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
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.ProjectOxford.SpeakerRecognition;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
    using Microsoft.Win32;    

    /// <summary>
    /// Interaction logic for EnrollSpeakersPage.xaml
    /// </summary>
    public partial class EnrollSpeakersPage : Page
    {
        private string selectedFile = string.Empty;
        private SpeakerIdentificationServiceClient serviceClient;

        /// <summary>
        /// Initializes a new instance of the EnrollSpeakersPage class.
        /// </summary>
        public EnrollSpeakersPage()
        {
            this.InitializeComponent();

            this._speakersListFrame.Navigate(SpeakersListPage.SpeakersList);

            MainWindow window = (MainWindow)Application.Current.MainWindow;
            this.serviceClient = new SpeakerIdentificationServiceClient(window.ScenarioControl.SubscriptionKey);
        }

        private async void _addBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            try
            {
                LogToMainWindow("Creating Speaker Profile...");
                CreateProfileResponse creationResponse = await this.serviceClient.CreateProfileAsync(_localeCmb.Text).ConfigureAwait(false);
                LogToMainWindow("Speaker Profile Created: " + creationResponse.ProfileId + ".");
                LogToMainWindow("Retrieving The Created Profile...");
                Profile profile = await this.serviceClient.GetProfileAsync(creationResponse.ProfileId).ConfigureAwait(false);
                LogToMainWindow("Speaker Profile Retrieved.");
                SpeakersListPage.SpeakersList.AddSpeaker(profile);
            }
            catch (CreateProfileException ex)
            {
                LogToMainWindow("Profile Creation Error: " + ex.Message);
            }
            catch (GetProfileException ex)
            {
                LogToMainWindow("Error Retrieving The Profile: " + ex.Message);
            }
            catch (Exception ex)
            {
                LogToMainWindow("Error: " + ex.Message);
            }
        }

        private void _loadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WAV Files(*.wav)|*.wav";
            bool? result = openFileDialog.ShowDialog(window);

            if (!(bool)result)
            {
                LogToMainWindow("No File Selected.");
                return;
            }

            LogToMainWindow("File Selected: " + openFileDialog.FileName);
            this.selectedFile = openFileDialog.FileName;
        }

        private async void _enrollBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            try
            {
                if (this.selectedFile == string.Empty)
                {
                    throw new Exception("No File Selected.");
                }                    

                LogToMainWindow("Enrolling Speaker...");
                Profile[] selectedProfiles = SpeakersListPage.SpeakersList.GetSelectedProfiles();

                OperationLocation processPollingLocation;
                using (Stream audioStream = File.OpenRead(this.selectedFile))
                {
                    this.selectedFile = string.Empty;
                    processPollingLocation = await this.serviceClient.EnrollAsync(audioStream, selectedProfiles[0].ProfileId, ((sender as Button) == _enrollShortAudioBtn)).ConfigureAwait(false);
                }

                EnrollmentOperation enrollmentResult;
                int numOfRetries = 10;
                TimeSpan timeBetweenRetries = TimeSpan.FromSeconds(5.0);

                while (numOfRetries > 0)
                {
                    await Task.Delay(timeBetweenRetries);
                    enrollmentResult = await this.serviceClient.CheckEnrollmentStatusAsync(processPollingLocation).ConfigureAwait(false);

                    if (enrollmentResult.Status == Status.Succeeded)
                    {
                        break;
                    }
                    else if (enrollmentResult.Status == Status.Failed)
                    {
                        throw new EnrollmentException(enrollmentResult.Message);
                    }

                    numOfRetries--;
                }

                if (numOfRetries <= 0)
                {
                    throw new EnrollmentException("Enrollment operation timeout.");
                }

                LogToMainWindow("Speaker Profile Id: " + selectedProfiles[0].ProfileId + " Enrollment Done.");

                await SpeakersListPage.SpeakersList.UpdateAllSpeakersAsync();
            }
            catch (EnrollmentException ex)
            {
                LogToMainWindow("Enrollment Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                LogToMainWindow("Error: " + ex.Message);
            }
        }

        private void LogToMainWindow(string logString)
        {
            Dispatcher.Invoke((Action)delegate
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                window.Log(logString);
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
    }
}