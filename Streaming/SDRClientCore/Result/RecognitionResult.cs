using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ATLC.SDR.ClientCore.Interface.Result;

namespace Microsoft.ATLC.SDR.ClientCore.Result
{
    /// <summary>
    /// Identification result
    /// </summary>
    public class RecognitionResult : IRecognitionResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">Operation result</param>
        /// <param name="clientId">Client id</param>
        /// <param name="requestId">Request id</param>
        public RecognitionResult(Identification result, Guid clientId, int requestId)
        {
            this.Value = result;
            this.ClientId = clientId;
            this.RequestId = requestId;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="failed">Flag that states if the request has failed or not</param>
        /// <param name="failureMsg">Failure message in case of a failure</param>
        /// <param name="requestId">Request id</param>
        public RecognitionResult(bool failed, string failureMsg, int requestId)
        {
            this.Failed = failed;
            this.FailureMsg = failureMsg;
            this.RequestId = requestId;
        }

        /// <summary>
        /// Operation result
        /// </summary>
        public Identification Value
        {
            get; set;
        }

        /// <summary>
        /// Client id
        /// </summary>
        public Guid ClientId
        {
            get; set;
        }

        /// <summary>
        /// Request id which gets incremented with each request
        /// </summary>
        public int RequestId
        {
            get; set;
        }

        /// <summary>
        /// Flag that states if the request has failed or not
        /// </summary>
        public bool Failed
        {
            get; set;
        }

        /// <summary>
        /// Failure message in case of a failure
        /// </summary>
        public string FailureMsg
        {
            get; set;
        }
    }
}
