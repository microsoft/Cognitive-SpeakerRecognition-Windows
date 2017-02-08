
namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Client
{
    using Microsoft.ProjectOxford.SpeakerRecognition;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;

    // System Namespace
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Threading;

    // Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming Namespace
    using Interface.Client;
    using Interface.Result;
    using Interface.Audio;
    using Audio;

    /// <summary>
    /// Speaker Diarization and Recognition Client
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
        /// <param name="windowSize">Windows size in seconds</param>
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
        /// Windows size in seconds
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
        public async Task StreamAudioAsync(byte[] audioBytes)
        {
            await audioProcessor.AppendAsync(audioBytes).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a signal to the recognition client that the audio is stream ended
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
