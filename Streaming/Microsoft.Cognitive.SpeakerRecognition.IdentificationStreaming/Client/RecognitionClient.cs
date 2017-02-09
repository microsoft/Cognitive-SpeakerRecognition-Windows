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

namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Client
{
    using Microsoft.ProjectOxford.SpeakerRecognition;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
    using System;
    using System.Linq;
    using System.IO;
    using System.Threading.Tasks;
    using System.Threading;
    using Interface.Client;
    using Interface.Result;
    using Interface.Audio;
    using Audio;

    /// <summary>
    /// Speaker Identification-Streaming and Recognition Client
    /// </summary>
    public class RecognitionClient : IRecognitionClient
    {
        private AudioProcessor audioProcessor;
        private int requestID;
        IIdentificationClient idClient;

        private readonly int DefaultDelayBetweenRequests = 250;

        private CancellationTokenSource RequestingTaskCancelletionTokenSource;
        private Task RequestingTask;
        private AudioFormatHandler AudioFormatHandler;
        private SpeakerIdentificationServiceClient serviceClient;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientId">Id associated with all requests related to this client</param>
        /// <param name="speakerIds">Speaker ids for identification</param>
        /// <param name="stepSize">Step size in seconds</param>
        /// <param name="windowSize">Number of seconds sent per request</param>
        /// <param name="audioFormat">Audio format</param>
        /// <param name="resultCallback">Value callback action consisted of identification result, client id and request id</param>
        /// <param name="serviceClient">Client used in identifying the streamed audio file</param>
        internal RecognitionClient(Guid clientId, Guid[] speakerIds, int stepSize, int windowSize, IAudioFormat audioFormat, Action<IRecognitionResult> resultCallback, SpeakerIdentificationServiceClient serviceClient)
        {
            this.ClientId = clientId;
            this.SpeakerIds = speakerIds;
            this.StepSize = stepSize;
            this.WindowSize = windowSize;
            this.requestID = 0;
            this.AudioFormat = audioFormat;
            this.AudioFormatHandler = new AudioFormatHandler(audioFormat);
            this.serviceClient = serviceClient;

            audioProcessor = new AudioProcessor(this.WindowSize, this.StepSize, this.AudioFormatHandler);
            idClient = new IdentificationClient(this.SpeakerIds, resultCallback);

            this.RequestingTaskCancelletionTokenSource = new CancellationTokenSource();
            this.RequestingTask = Task.Run(async () => {
                await SendingRequestsTask(RequestingTaskCancelletionTokenSource.Token).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Id associated with all requests related to this client
        /// </summary>
        public Guid ClientId
        {
            get; set;
        }

        /// <summary>
        /// Speaker ids for identification
        /// </summary>
        public Guid[] SpeakerIds
        {
            get; set;
        }

        /// <summary>
        /// Step size in seconds
        /// </summary>
        public int StepSize
        {
            get; set;
        }

        /// <summary>
        /// Number of seconds sent per request
        /// </summary>
        public int WindowSize
        {
            get; set;
        }

        /// <summary>
        /// Recognition audio format
        /// </summary>
        public IAudioFormat AudioFormat
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
            var audioBuffer = audioBytes.Skip(offset).Take(Math.Min(length, audioBytes.Length-offset)).ToArray();
            await audioProcessor.AppendAsync(audioBuffer).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a signal to the recognition client that the audio stream ended
        /// </summary>
        public async Task EndStreamAudioAsync()
        {
            await audioProcessor.CompleteAsync().ConfigureAwait(false);
        }

        private async Task SendingRequestsTask(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                var audio = await audioProcessor.GetNextRequestAsync().ConfigureAwait(false);
                if(audio != null)
                {
                    int reqId = GetCurrentRequestId();

                    var forgettableTask = Task.Run(async () => {
                        using (var stream = new MemoryStream(audio)){
                            await idClient.IdentifyStreamAsync(stream, this.serviceClient, this.ClientId, reqId).ConfigureAwait(false);
                        }
                    }); 
                }
                else
                {
                    if(audioProcessor.IsCompleted)
                    {
                        break;
                    }
                }

                await Task.Delay(DefaultDelayBetweenRequests).ConfigureAwait(false);
            }
        }

        private int GetCurrentRequestId()
        {
            return Interlocked.Increment(ref this.requestID);
        }

        /// <summary>
        /// Disposes the client
        /// </summary>
        public void Dispose()
        {
            if (!audioProcessor.IsCompleted)
            {
                // If audio processor hasn't been completed yet,
                // cancel the requesting task first.
                this.RequestingTaskCancelletionTokenSource.Cancel();
            }
            this.RequestingTask.Wait();

            idClient.Dispose();
        }

    }
}
