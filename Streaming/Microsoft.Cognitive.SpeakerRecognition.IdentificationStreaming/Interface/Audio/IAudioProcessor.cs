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

namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Audio
{
    using System.Threading.Tasks;

    /// <summary>
    /// An audio processor that handles streaming by means of the sliding window,
    /// It processes input stream, and prepares the corresponding waves according to the sliding window parameters.
    /// </summary>
    internal interface IAudioProcessor
    {
        /// <summary>
        /// A boolean to indicate whether processing is over or not.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Stores input bytes into buffer for processing.
        /// </summary>
        /// <param name="bytesToSend">The byte array to send</param>
        Task AppendAsync(byte[] bytesToSend);

        /// <summary>
        /// Stores input bytes into buffer for processing.
        /// </summary>
        /// <param name="buffer">The byte array to send from</param>
        /// <param name="offset">The index at which to start index</param>
        /// <param name="numberOfBytes">The number of bytes to be sent</param>
        Task AppendAsync(byte[] buffer, int offset, int numberOfBytes);

        /// <summary>
        /// Gets the next window to be sent.
        /// </summary>
        /// <returns>A byte array containing the wave to be sent</returns>
        Task<byte[]> GetNextRequestAsync();

        /// <summary>
        /// Signals the processor to stop processing data
        /// </summary>
        Task CompleteAsync();
    }
}
