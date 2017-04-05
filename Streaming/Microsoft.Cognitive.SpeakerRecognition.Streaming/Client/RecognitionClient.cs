// <copyright file="RecognitionClient.cs" company="Microsoft">
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
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Audio;
    using Microsoft.ProjectOxford.SpeakerRecognition;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
    using Result;    

    /// <summary>
    /// Speaker Identification-Streaming and Recognition Client
    /// Organizes the communication between the identification client which performs actual identification and Audio processor module 
    /// which generates audio buffers to be streamed based on the input user-configurations for step size and window size
    /// </summary>
    public class RecognitionClient : IDisposable
    {
        private readonly int defaultDelayBetweenRequests = 250;

        private AudioProcessor audioProcessor;
        private int requestID;
        private IdentificationClient idClient;

        private CancellationTokenSource requestingTaskCancelletionTokenSource;
        private Task requestingTask;
        private AudioFormatHandler audioFormatHandler;
        private SpeakerIdentificationServiceClient serviceClient;
        
        /// <summary>
        /// Initializes a new instance of the RecognitionClient class.
        /// </summary>
        /// <param name="clientId">ID associated with all requests related to this client</param>
        /// <param name="speakerIds">Speaker IDs for identification</param>
        /// <param name="stepSize">Step size in seconds</param>
        /// <param name="windowSize">Number of seconds sent per request</param>
        /// <param name="audioFormat">Audio format</param>
        /// <param name="resultCallback">Value callback action consisted of identification result, client ID and request ID</param>
        /// <param name="serviceClient">Client used in identifying the streamed audio file</param>
        internal RecognitionClient(Guid clientId, Guid[] speakerIds, int stepSize, int windowSize, AudioFormat audioFormat, Action<RecognitionResult> resultCallback, SpeakerIdentificationServiceClient serviceClient)
        {
            this.ClientId = clientId;
            this.SpeakerIds = speakerIds;
            this.StepSize = stepSize;
            this.WindowSize = windowSize;
            this.requestID = 0;
            this.AudioFormat = audioFormat;
            this.audioFormatHandler = new AudioFormatHandler(audioFormat);
            this.serviceClient = serviceClient;

            this.audioProcessor = new AudioProcessor(this.WindowSize, this.StepSize, this.audioFormatHandler);
            this.idClient = new IdentificationClient(this.SpeakerIds, resultCallback);

            this.requestingTaskCancelletionTokenSource = new CancellationTokenSource();
            this.requestingTask = Task.Run(async () => 
            {
                await SendingRequestsTask(requestingTaskCancelletionTokenSource.Token).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Gets or sets ID associated with all requests related to this client
        /// </summary>
        public Guid ClientId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets speaker IDs for identification
        /// </summary>
        public Guid[] SpeakerIds
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets step size in seconds
        /// </summary>
        public int StepSize
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets number of seconds sent per request
        /// </summary>
        public int WindowSize
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets recognition audio format
        /// </summary>
        public AudioFormat AudioFormat
        {
            get; set;
        }

        /// <summary>
        /// Streams audio to recognition service
        /// </summary>
        /// <param name="audioBytes">Audio bytes to be sent for recognition</param>
        /// <param name="offset">The position in the audio from where the stream should begin</param>
        /// <param name="length">The length of audio that should be streamed starting from the offset position</param>
        public async Task StreamAudioAsync(byte[] audioBytes, int offset, int length)
        {
            await this.audioProcessor.AppendAsync(audioBytes, offset, length).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a signal to the recognition client that the audio stream ended
        /// </summary>
        public async Task EndStreamAudioAsync()
        {
            await this.audioProcessor.CompleteAsync().ConfigureAwait(false);
        }        

        /// <summary>
        /// Disposes the client
        /// </summary>
        public void Dispose()
        {
            if (!this.audioProcessor.IsCompleted)
            {
                // If audio processor hasn't been completed yet,
                // cancel the requesting task first.
                this.requestingTaskCancelletionTokenSource.Cancel();
            }

            this.requestingTask.Wait();
        }

        private async Task SendingRequestsTask(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var audio = await this.audioProcessor.GetNextRequestAsync().ConfigureAwait(false);
                if (audio != null)
                {
                    int reqId = this.GetCurrentRequestId();

                    var forgettableTask = Task.Run(async () =>
                    {
                        using (var stream = new MemoryStream(audio))
                        {
                            await idClient.IdentifyStreamAsync(stream, this.serviceClient, this.ClientId, reqId).ConfigureAwait(false);
                        }
                    });
                }
                else
                {
                    if (this.audioProcessor.IsCompleted)
                    {
                        break;
                    }
                }

                await Task.Delay(this.defaultDelayBetweenRequests).ConfigureAwait(false);
            }
        }

        private int GetCurrentRequestId()
        {
            return Interlocked.Increment(ref this.requestID);
        }
    }
}
