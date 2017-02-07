using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ATLC.SDR.ClientCore.Audio
{
    /// <summary>
    /// Audio container
    /// </summary>
    public class AudioContainer
    {
        /// <summary>
        /// Creates new audio container object
        /// </summary>
        /// <param name="type">Audio container type</param>
        public AudioContainer(AudioContainerType type)
        {
            ContainerType = type;

            if (type.Equals(AudioContainerType.WAV))
                this.maxHeaderSize = 5000;
            else if (type.Equals(AudioContainerType.RAW))
                this.maxHeaderSize = 0;
        }

        /// <summary>
        /// Audio container type
        /// </summary>
        public AudioContainerType ContainerType
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            AudioContainer container = obj as AudioContainer;
            if (ContainerType.Equals(container.ContainerType))
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        int maxHeaderSize;

        internal int MaxHeaderSize
        {
            get
            {
                return maxHeaderSize;
            }            
        }
    }

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
}
