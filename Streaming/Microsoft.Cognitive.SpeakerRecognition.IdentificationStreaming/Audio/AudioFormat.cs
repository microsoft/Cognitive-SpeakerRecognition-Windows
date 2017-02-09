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

using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Audio;
using System.Text;
using System;

namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Audio
{
    /// <summary>
    /// Audio codec and container format
    /// </summary>
    public class AudioFormat : IAudioFormat
    {
        /// <summary>
        /// Constructs new audio format object
        /// </summary>
        /// <param name="encoding">Audio encoding</param>
        /// <param name="channelsNumber">Channels number</param>
        /// <param name="sampleRate">Sample rate</param>
        /// <param name="bitsPerSample">Bits per sample</param>
        /// <param name="audioContainer">Type of audio container, either RAW or WAV</param>
        public AudioFormat(AudioEncoding encoding, int channelsNumber, int sampleRate, int bitsPerSample, AudioContainer audioContainer)
        {
            ValidateAudioFormat(channelsNumber, sampleRate, bitsPerSample);

            Encoding = encoding;
            ChannelsNumber = channelsNumber;
            SampleRate = sampleRate;
            BitsPerSample = bitsPerSample;
            Container = audioContainer;           
        }

        private void ValidateAudioFormat(int channelsNumber, int sampleRate, int bitsPerSample)
        {
            if(channelsNumber <= 0)
            {
                throw new ArgumentException("Channels number must be a positive number.");
            }

            if (sampleRate < 0)
            {
                throw new ArgumentException("Sample rate can't be a negative number.");
            }

            if (bitsPerSample < 0)
            {
                throw new ArgumentException("Bits per sample can't be a negative number.");
            }
        }

        /// <summary>
        /// Audio encoding
        /// </summary>
        public AudioEncoding Encoding
        {
            get;
            set;
        }

        /// <summary>
        /// Channels number
        /// </summary>
        public int ChannelsNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Sample rate
        /// </summary>
        public int SampleRate
        {
            get;
            set;
        }

        /// <summary>
        /// Bits per sample
        /// </summary>
        public int BitsPerSample
        {
            get;
            set;
        }

        /// <summary>
        /// Compares the input format of this format
        /// </summary>
        /// <param name="obj">Input format to be compared</param>
        /// <returns>True if similar</returns>
        public override bool Equals(object obj)
        {
            AudioFormat format = obj as AudioFormat;
            if (
                Encoding.Equals(format.Encoding) &&
                ChannelsNumber == format.ChannelsNumber &&
                SampleRate == format.SampleRate &&
                BitsPerSample == format.BitsPerSample &&
                Container.Equals(format.Container))
                return true;
            return false;
        }

        /// <summary>
        /// Returns hash code of object
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        /// <summary>
        /// Audio container
        /// </summary>
        public AudioContainer Container
        {
            get;
            set;
        }

        /// <summary>
        /// Returns description of audio format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Container: " + Container.ContainerType.ToString() + ", Encoding: " + Encoding.ToString() + ", Rate: " + SampleRate + ", Sample Format: " + BitsPerSample + ", Channels: " + ChannelsNumber;
        }
    }
}
