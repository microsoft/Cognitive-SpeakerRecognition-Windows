// <copyright file="AudioContainer.cs" company="Microsoft">
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

namespace  Microsoft.Cognitive.SpeakerRecognition.Streaming.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Types of audio containers
    /// </summary>
    public enum AudioContainerType
    {
        /// <summary>
        /// Audio with no header
        /// </summary>
        RAW,

        /// <summary>
        /// WAV audio
        /// </summary>
        WAV
    }

    /// <summary>
    /// Audio container which supports two types of containers: RAW and WAV
    /// </summary>
    public class AudioContainer
    {
        private int maxHeaderSize;

        /// <summary>
        /// Initializes a new instance of the AudioContainer class.
        /// </summary>
        /// <param name="type">Audio container type</param>
        public AudioContainer(AudioContainerType type)
        {
            this.ContainerType = type;

            if (type.Equals(AudioContainerType.WAV))
            {
                this.maxHeaderSize = 5000;
            }                
            else if (type.Equals(AudioContainerType.RAW))
            {
                this.maxHeaderSize = 0;
            }                
        }

        /// <summary>
        /// Gets or sets audio container type
        /// </summary>
        public AudioContainerType ContainerType
        {
            get;
            set;
        }

        internal int MaxHeaderSize
        {
            get
            {
                return this.maxHeaderSize;
            }            
        }

        public override bool Equals(object obj)
        {
            AudioContainer container = obj as AudioContainer;
            if (this.ContainerType.Equals(container.ContainerType))
            {
                return true;
            }
                
            return false;
        }
    }
}
