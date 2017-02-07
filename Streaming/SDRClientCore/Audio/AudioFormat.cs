using Microsoft.ATLC.SDR.ClientCore.Interface.Audio;
using System.Text;
using System;

namespace Microsoft.ATLC.SDR.ClientCore.Audio
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
            Encoding = encoding;
            ChannelsNumber = channelsNumber;
            SampleRate = sampleRate;
            BitsPerSample = bitsPerSample;
            Container = audioContainer;

            ValidateAudioFormat();
        }

        private void ValidateAudioFormat()
        {
            if(ChannelsNumber < 0)
            {
                throw new ArgumentException("Channels number can't be a negative number.");
            }

            if (SampleRate < 0)
            {
                throw new ArgumentException("Sample rate can't be a negative number.");
            }

            if (BitsPerSample < 0)
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
