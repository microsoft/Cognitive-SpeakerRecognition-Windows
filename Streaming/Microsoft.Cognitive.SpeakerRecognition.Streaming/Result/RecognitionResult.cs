// <copyright file="RecognitionResult.cs" company="Microsoft">
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

namespace  Microsoft.Cognitive.SpeakerRecognition.Streaming.Result
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ProjectOxford.SpeakerRecognition;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;

    /// <summary>
    /// Recognition result which includes the ID of the client initiated the request, the ID of the request and the identification result for the request
    /// </summary>
    public class RecognitionResult
    {
        /// <summary>
        /// Initializes a new instance of the RecognitionResult class incase of a successful recognition
        /// </summary>
        /// <param name="result">Operation result</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="requestId">Request ID</param>
        public RecognitionResult(Identification result, Guid clientId, int requestId)
        {
            this.Value = result;
            this.ClientId = clientId;
            this.RequestId = requestId;

            this.Succeeded = true;
        }

        /// <summary>
        /// Initializes a new instance of the RecognitionResult class incase of a failed recognition
        /// </summary>
        /// <param name="status">Flag that Indicates whether the request has succeeded or not</param>
        /// <param name="failureMsg">Failure message in case of a failure</param>
        /// <param name="requestId">Request ID</param>
        public RecognitionResult(bool status, string failureMsg, int requestId)
        {
            this.Succeeded = status;
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
        /// Client ID
        /// </summary>
        public Guid ClientId
        {
            get; set;
        }

        /// <summary>
        /// Request ID which gets incremented with each request
        /// </summary>
        public int RequestId
        {
            get; set;
        }

        /// <summary>
        /// Flag that Indicates whether the request has succeeded or not
        /// </summary>
        public bool Succeeded
        {
            get; set;
        }

        /// <summary>
        /// Gets and Sets failure message in case of a failure
        /// </summary>
        public string FailureMsg
        {
            get; set;
        }
    }
}
