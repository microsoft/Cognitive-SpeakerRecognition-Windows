using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Client;
using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Result;
using System;
using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Audio;

namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Client
{
    /// <summary>
    /// Identification-Streaming Client Factory
    /// </summary>
    public class ClientFactory : IClientFactory
    {
        /// <summary>
        /// Creates new identification-streaming recognition client
        /// </summary>
        /// <param name="clientId">Id associated with all requests related to this client</param>
        /// <param name="speakerIds">Speaker ids for recognition</param>
        /// <param name="stepSize">Frequency of sending requests to the server in seconds. 
        /// If set to 1, the client will send a request to the server for every second received from the user</param>
        /// <param name="windowSize">Windows size in seconds</param>
        /// <param name="audioFormat">Audio format</param>
        /// <param name="resultCallBack">Value callback action consisted of identification result, client id and request id</param>
        /// <param name="serviceClient">Client used in identifying the streamed audio file</param>
        /// <returns>SDR recognition client</returns>
        public IRecognitionClient CreateRecognitionClient(Guid clientId, Guid[] speakerIds, int stepSize, int windowSize, IAudioFormat audioFormat, Action<IRecognitionResult> resultCallBack, SpeakerIdentificationServiceClient serviceClient)
        {
            if(speakerIds.Length < 1)
            {
                throw new ArgumentException("Speakers count can't be smaller than 1.");
            }            

            var recognitionClient = new RecognitionClient(clientId, speakerIds, stepSize, windowSize, audioFormat, resultCallBack, serviceClient);
            return recognitionClient;
        }

        /// <summary>
        ///  Disposes the factory
        /// </summary>
        public void Dispose()
        {
        }
    }
}
