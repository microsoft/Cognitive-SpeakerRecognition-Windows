using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;

namespace Microsoft.ATLC.SDR.ClientCore.Interface.Client
{
    /// <summary>
    /// Identification client
    /// </summary>
    interface IIdentificationClient : IDisposable
    {
        /// <summary>
        /// Identify a stream of audio
        /// </summary>
        /// <param name="stream">Audio buffer to be recognized</param>
        /// <param name="serviceClient">Client used in identifying the streamed audio wave</param>
        /// <param name="clientId">Client id</param>
        /// <param name="requestId">Request id</param>
        /// <returns></returns>
        Task IdentifyStreamAsync(Stream stream, SpeakerIdentificationServiceClient serviceClient, Guid clientId, int requestId);
    }
}
