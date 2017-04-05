// <copyright file="AudioFormat.cs" company="Microsoft">
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
    using System.Text;

    /// <summary>
    /// Audio codec and container format
    /// </summary>
    public class AudioFormat
    {
        /// <summary>
        /// Initializes a new instance of the AudioFormat class.
        /// </summary>
        /// <param name="encoding">Audio encoding</param>
        /// <param name="channelsNumber">Channels number</param>
        /// <param name="sampleRate">Sample rate</param>
        /// <param name="bitsPerSample">Bits per sample</param>
        /// <param name="audioContainer">Type of audio container, either RAW or WAV</param>
        public AudioFormat(AudioEncoding encoding, int channelsNumber, int sampleRate, int bitsPerSample, AudioContainer audioContainer)
        {
            this.ValidateAudioFormat(channelsNumber, sampleRate, bitsPerSample);

            this.Encoding = encoding;
            this.ChannelsNumber = channelsNumber;
            this.SampleRate = sampleRate;
            this.BitsPerSample = bitsPerSample;
            this.Container = audioContainer;           
        }

        /// <summary>
        /// Gets or sets audio encoding
        /// </summary>
        public AudioEncoding Encoding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets channels number
        /// </summary>
        public int ChannelsNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets sample rate
        /// </summary>
        public int SampleRate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets bits per sample
        /// </summary>
        public int BitsPerSample
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets audio container
        /// </summary>
        public AudioContainer Container
        {
            get;
            set;
        }

        /// <summary>
        /// Compares the input format of this audio
        /// </summary>
        /// <param name="obj">Input format to be compared</param>
        /// <returns>True if similar</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }                

            AudioFormat format = obj as AudioFormat;

            return format != null &&
                this.Encoding.Equals(format.Encoding) &&
                this.ChannelsNumber == format.ChannelsNumber &&
                this.SampleRate == format.SampleRate &&
                this.BitsPerSample == format.BitsPerSample &&
                this.Container.Equals(format.Container);
        }

        /// <summary>
        /// Returns description of audio format
        /// </summary>
        /// <returns>A string representation for the object's fields</returns>
        public override string ToString()
        {
            return "Container: " + this.Container.ContainerType.ToString() + ", Encoding: " + this.Encoding.ToString() + ", Rate: " + this.SampleRate + ", Sample Format: " + this.BitsPerSample + ", Channels: " + this.ChannelsNumber;
        }

        private void ValidateAudioFormat(int channelsNumber, int sampleRate, int bitsPerSample)
        {
            if (channelsNumber <= 0)
            {
                throw new ArgumentException("Channels number must be a positive number.");
            }

            if (sampleRate <= 0)
            {
                throw new ArgumentException("Sample rate must be a positive number.");
            }

            if (bitsPerSample <= 0)
            {
                throw new ArgumentException("Bits per sample must be a positive number.");
            }
        }
    }
}
