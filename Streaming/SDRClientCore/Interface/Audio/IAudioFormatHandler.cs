using Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Audio
{
    /// <summary>
    /// Handles audio formats, and parses header
    /// </summary>
    interface IAudioFormatHandler
    {
        IAudioFormat InputAudioFormat { get; }

        AudioHeaderParsingResult ParseHeader(byte[] header);
    }
}
