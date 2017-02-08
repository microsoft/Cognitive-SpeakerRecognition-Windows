using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Audio
{
    internal class AudioHeaderParsingResult
    {
        public int DataChunckStart
        {
            get; set;
        }

        public int NumberofBytesPerSecond
        {
            get; set;
        }
    }
}
