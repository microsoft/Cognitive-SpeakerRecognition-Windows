// <copyright file="SpeakersListPage.xaml.cs" company="Microsoft">
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
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.ProjectOxford.SpeakerRecognition;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;

    /// <summary>
    /// Interaction logic for SpeakersListPage.xaml
    /// </summary>
    public partial class SpeakersListPage : Page
    {
        private static SpeakersListPage speakersList = new SpeakersListPage();
        private bool speakersLoaded = false;        
        private SpeakerIdentificationServiceClient serviceClient;

        private SpeakersListPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the only instance of the Speakers List Page
        /// </summary>
        public static SpeakersListPage SpeakersList
        {
            get
            {
                return speakersList;
            }
        }

        /// <summary>
        /// Adds a speaker profile to the speakers list
        /// </summary>
        /// <param name="speaker">The speaker profile to add</param>
        public void AddSpeaker(Profile speaker)
        {
            Dispatcher.Invoke((Action)delegate
            {
                this._speakersListView.Items.Add(speaker);
            });
        }

        /// <summary>
        /// Retrieves all the speakers asynchronously and adds them to the list
        /// </summary>
        /// <returns>Task to track the status of the asynchronous task.</returns>
        public async Task UpdateAllSpeakersAsync()
        {
            try
            {
                UpdateServiceClient();
                this.LogToMainWindow("Retrieving All Profiles...");
                Profile[] allProfiles = await this.serviceClient.GetProfilesAsync().ConfigureAwait(false);
                this.LogToMainWindow("All Profiles Retrieved.");

                Dispatcher.Invoke((Action)delegate
                {
                    this._speakersListView.Items.Clear();
                });

                foreach (Profile profile in allProfiles)
                {
                    this.AddSpeaker(profile);
                }
                    
                this.speakersLoaded = true;
            }
            catch (GetProfileException ex)
            {
                this.LogToMainWindow("Error Retrieving Profiles: " + ex.Message);
            }
            catch (Exception ex)
            {
                this.LogToMainWindow("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Enables single selection mode for the speakers list
        /// </summary>
        public void SetSingleSelectionMode()
        {
            this._speakersListView.SelectionMode = System.Windows.Controls.SelectionMode.Single;
        }

        /// <summary>
        /// Enables multiple selection mode for the speakers list
        /// </summary>
        public void SetMultipleSelectionMode()
        {
            this._speakersListView.SelectionMode = SelectionMode.Multiple;
        }

        /// <summary>
        /// Gets the selected profiles from the speakers list
        /// </summary>
        /// <returns>An array of the selected identification profiles</returns>
        public Profile[] GetSelectedProfiles()
        {
            UpdateServiceClient();
            if (this._speakersListView.SelectedItems.Count == 0)
            {
                throw new Exception("No Speakers Selected.");
            }
                
            Profile[] result = new Profile[this._speakersListView.SelectedItems.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = this._speakersListView.SelectedItems[i] as Profile;
            }
                
            return result;
        }

        private async void _UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.UpdateAllSpeakersAsync().ConfigureAwait(false);
        }

        private void LogToMainWindow(string logString)
        {
            Dispatcher.Invoke((Action)delegate
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                window.Log(logString);
            });
        }

        private void UpdateServiceClient()
        {
            Dispatcher.Invoke((Action)delegate
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                this.serviceClient = new SpeakerIdentificationServiceClient(window.ScenarioControl.SubscriptionKey);
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
