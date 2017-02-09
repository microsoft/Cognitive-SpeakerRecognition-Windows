using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Audio
{
    /// <summary>
    /// Audio codec and container format
    /// </summary>
    public interface IAudioFormat
    {
        /// <summary>
        /// Audio encoding
        /// </summary>
        AudioEncoding Encoding
        {
            get;
            set;
        }

        /// <summary>
        /// Channels number
        /// </summary>
        int ChannelsNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Sample rate
        /// </summary>
        int SampleRate
        {
            get;
            set;
        }

        /// <summary>
        /// Bits per sample
        /// </summary>
        int BitsPerSample
        {
            get;
            set;
        }

        /// <summary>
        /// Audio container
        /// </summary>
        AudioContainer Container
        {
            get;
            set;
        }

        /// <summary>
        /// Returns description of audio format
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
