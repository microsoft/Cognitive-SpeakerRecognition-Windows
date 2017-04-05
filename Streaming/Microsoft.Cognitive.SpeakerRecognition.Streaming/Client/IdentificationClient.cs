// <copyright file="IdentificationClient.cs" company="Microsoft">
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

namespace  Microsoft.Cognitive.SpeakerRecognition.Streaming.Client
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using  Microsoft.Cognitive.SpeakerRecognition.Streaming.Result;
    using Microsoft.ProjectOxford.SpeakerRecognition;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;

    /// <summary>
    /// Identification client
    /// Performs the identification against SpeakerRecognition service
    /// </summary>
    internal class IdentificationClient
    {
        private Guid[] speakerIds;
        private Action<RecognitionResult> resultCallback;

        /// <summary>
        /// Initializes a new instance of the IdentificationClient class.
        /// </summary>
        /// <param name="speakerIds"> Speaker IDs for identification</param>
        /// <param name="callback">Value callback action consisted of identification result, request ID and second sequence number</param>
        public IdentificationClient(Guid[] speakerIds, Action<RecognitionResult> callback)
        {
            this.speakerIds = speakerIds;
            this.resultCallback = callback;
        }

        /// <summary>
        /// Gets or sets the endpoint used by the client to perform the identification.
        /// </summary>
        public string ServiceURI
        {
            get; set;
        }

        /// <summary>
        /// Identify a stream of audio
        /// </summary>
        /// <param name="stream">Audio buffer to be recognized</param>
        /// <param name="serviceClient">Client used in identifying the streamed audio wave</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="requestId">Request ID</param>
        public async Task IdentifyStreamAsync(Stream stream, SpeakerIdentificationServiceClient serviceClient, Guid clientId, int requestId)
        {
            try
            {
                OperationLocation processPollingLocation;
                processPollingLocation = await serviceClient.IdentifyAsync(stream, this.speakerIds, forceShortAudio: true).ConfigureAwait(false);

                IdentificationOperation identificationResponse = null;
                int numOfRetries = int.Parse(ConfigurationManager.AppSettings["NumberOfPollingRetries"]);
                TimeSpan timeBetweenRetries = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["TimeSpanBetweenPollingRetries"]));
                while (numOfRetries > 0)
                {
                    await Task.Delay(timeBetweenRetries);
                    identificationResponse = await serviceClient.CheckIdentificationStatusAsync(processPollingLocation);

                    if (identificationResponse.Status == Status.Succeeded)
                    {
                        var result = new RecognitionResult(identificationResponse.ProcessingResult, clientId, requestId);
                        this.resultCallback(result);
                        break;
                    }
                    else if (identificationResponse.Status == Status.Failed)
                    {
                        var failureResult = new RecognitionResult(false, identificationResponse.Message, requestId);
                        this.resultCallback(failureResult);
                        return;
                    }

                    numOfRetries--;
                }

                if (numOfRetries <= 0)
                {
                    var failureResult = new RecognitionResult(false, "Request timeout.", requestId);
                    this.resultCallback(failureResult);
                    return;
                }
            }
            catch (Exception ex)
            {
                var result = new RecognitionResult(false, ex.Message, requestId);
                this.resultCallback(result);
            }
        }
    }
}
