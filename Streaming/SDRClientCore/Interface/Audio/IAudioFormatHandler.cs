using Microsoft.ATLC.SDR.ClientCore.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ATLC.SDR.ClientCore.Interface.Audio
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
