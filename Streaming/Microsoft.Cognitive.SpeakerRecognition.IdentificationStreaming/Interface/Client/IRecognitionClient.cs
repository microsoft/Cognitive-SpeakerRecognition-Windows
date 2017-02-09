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

namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Client
{
    using Microsoft.ProjectOxford.SpeakerRecognition;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;    
    using Audio;

    /// <summary>
    /// Speaker Identification-Streaming and Recognition Client
    /// </summary>
    public interface IRecognitionClient : IDisposable
    {
        /// <summary>
        /// Number of seconds sent per request
        /// </summary>
        int WindowSize
        {
            get; set;
        }

        /// <summary>
        /// Step size in seconds
        /// </summary>
        int StepSize
        {
            get; set;
        }

        /// <summary>
        /// Speaker ids for identification
        /// </summary>
        Guid[] SpeakerIds
        {
            get; set;
        }

        /// <summary>
        /// Id associated with all requests related to this client
        /// </summary>
        Guid ClientId
        {
            get; set;
        }

        /// <summary>
        /// Recognition audio format
        /// </summary>
        IAudioFormat AudioFormat
        {
            get; set;
        }

        /// <summary>
        /// Streams audio to recognition service
        /// </summary>
        /// <param name="audioBytes">Audio bytes to be sent for recognition</param>
        /// <param name="offset">The position in the audio from where the stream should begin</param>
        /// <param name="length">The length of audio that should be streamed starting from the offset position</param>
        Task StreamAudioAsync(byte[] audioBytes, int offset, int length);

        /// <summary>
        /// Sends a signal to the recognition client that the audio is stream ended
        /// </summary>
        Task EndStreamAudioAsync();
    }
}