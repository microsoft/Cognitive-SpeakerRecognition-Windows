// <copyright file="MainWindow.xaml.cs" company="Microsoft">
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
    using System.Windows;
    using SampleUserControlLibrary;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this._scenariosControl.SampleTitle = "Speaker Streaming Sample";
            this._scenariosControl.SampleScenarioList = new Scenario[]
            {
                new Scenario{ Title = "Enroll Speakers", PageClass = typeof(EnrollSpeakersPage) },
                new Scenario{ Title = "Stream File", PageClass = typeof(StreamPage) },
            };

            this._scenariosControl.Disclaimer = string.Empty;
            this._scenariosControl.ClearLog();
        }

        /// <summary>
        /// Gets the sample scenario control
        /// </summary>
        public SampleScenarios ScenarioControl
        {
            get
            {
                return this._scenariosControl;
            }
        }

        /// <summary>
        /// Writes a message in the status area
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Log(string message)
        {
            this._scenariosControl.Log(message);
        }
    }
}
