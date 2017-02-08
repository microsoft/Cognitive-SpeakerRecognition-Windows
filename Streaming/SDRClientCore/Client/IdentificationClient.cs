using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ATLC.SDR.ClientCore.Interface.Client;
using Microsoft.ATLC.SDR.ClientCore.Interface.Result;
using Microsoft.ATLC.SDR.ClientCore.Result;
using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using System.Diagnostics;

namespace Microsoft.ATLC.SDR.ClientCore.Client
{
    /// <summary>
    /// Identification client
    /// </summary>
    internal class IdentificationClient : IIdentificationClient
    {
        private Guid[] speakerIds;
        private Action<IRecognitionResult> resultCallback;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="speakerIds"> Speaker ids for identification</param>
        /// <param name="callback">Value callback action consisted of identification result, request id and second sequence number</param>
        public IdentificationClient(Guid[] speakerIds, Action<IRecognitionResult> callback)
        {
            this.speakerIds = speakerIds;
            this.resultCallback = callback;
        }

        /// <summary>
        /// The endpoint used by the client to perform the identification.
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
        /// <param name="clientId">Client id</param>
        /// <param name="requestId">Request id</param>
        /// <returns></returns>
        public async Task IdentifyStreamAsync(Stream stream, SpeakerIdentificationServiceClient serviceClient, Guid clientId, int requestId)
        {
            try
            {
                OperationLocation processPollingLocation;
                processPollingLocation = await serviceClient.IdentifyAsync(stream, speakerIds, forceShortAudio: true).ConfigureAwait(false);

                IdentificationOperation identificationResponse = null;
                int numOfRetries = 2;
                TimeSpan timeBetweenRetries = TimeSpan.FromSeconds(1.0);
                while (numOfRetries > 0)
                {
                    await Task.Delay(timeBetweenRetries);
                    identificationResponse = await serviceClient.CheckIdentificationStatusAsync(processPollingLocation).ConfigureAwait(false);

                    if (identificationResponse.Status == Status.Succeeded)
                    {
                        var result = new RecognitionResult(identificationResponse.ProcessingResult, clientId, requestId);
                        resultCallback(result);
                        return;
                    }
                    else if (identificationResponse.Status == Status.Failed)
                    {
                        var failureResult = new RecognitionResult(true, identificationResponse.Message, requestId);
                        resultCallback(failureResult);
                        return;
                    }
                    numOfRetries--;
                }
                if (numOfRetries <= 0)
                {
                    var failureResult = new RecognitionResult(true, "Request timeout.", requestId);
                    resultCallback(failureResult);
                    return;
                }
            }
            catch (Exception ex)
            {
                var result = new RecognitionResult(true, ex.Message, requestId);
                resultCallback(result);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }
    }
}
