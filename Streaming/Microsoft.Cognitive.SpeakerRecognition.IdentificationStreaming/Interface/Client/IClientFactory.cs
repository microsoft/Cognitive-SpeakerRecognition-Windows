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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Result;
using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Audio;
using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;

namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Client
{
    /// <summary>
    /// Identification-Streaming Client Factory
    /// </summary>
    public interface IClientFactory : IDisposable
    {
        /// <summary>
        /// Creates new identification-streaming recognition client
        /// </summary>
        /// <param name="clientId">Id associated with all requests related to this client</param>
        /// <param name="speakerIds">Speaker ids for recognition</param>
        /// <param name="stepSize">Frequency of sending requests to the server in seconds. 
        /// If set to 1, the client will send a request to the server for every second received from the user</param>
        /// <param name="windowSize">Number of seconds sent per request</param>
        /// <param name="audioFormat">Audio format</param>
        /// <param name="resultCallBack">Value callback action consisted of identification result, client id and request id</param>
        /// <param name="serviceClient">Client used in identifying the streamed audio file</param>
        /// <returns>Identification-Streaming and recognition client</returns>
        IRecognitionClient CreateRecognitionClient(Guid clientId, Guid[] speakerIds, int stepSize, int windowSize, IAudioFormat audioFormat, Action<IRecognitionResult> resultCallBack, SpeakerIdentificationServiceClient serviceClient);
    }
}
