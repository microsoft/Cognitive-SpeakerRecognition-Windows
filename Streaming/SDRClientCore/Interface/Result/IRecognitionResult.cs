using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ATLC.SDR.ClientCore.Interface.Result
{
    /// <summary>
    /// Identification result
    /// </summary>
    public interface IRecognitionResult
    {
        /// <summary>
        /// Operation result
        /// </summary>
        Identification Value
        {
            get; set;
        }

        /// <summary>
        /// Client id
        /// </summary>
        Guid ClientId
        {
            get; set;
        }

        /// <summary>
        /// Request id which gets incremented with each request
        /// </summary>
        int RequestId
        {
            get; set;
        }

        /// <summary>
        /// States if the request has failed or not
        /// </summary>
        bool Failed
        {
            get; set;
        }

        /// <summary>
        /// Failure message in case of a failure
        /// </summary>
        string FailureMsg
        {
            get; set;
        }
    }
}
