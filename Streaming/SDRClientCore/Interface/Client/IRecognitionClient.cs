
namespace Microsoft.ATLC.SDR.ClientCore.Interface.Client
{
    using Microsoft.ProjectOxford.SpeakerRecognition;
    using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    using Audio;

    /// <summary>
    /// Speaker Diarization and Recognition Client
    /// </summary>
    public interface IRecognitionClient : IDisposable
    {
        /// <summary>
        /// Windows size in seconds
        /// </summary>
        int WindowSize
        {
            get; set;
        }

        /// <summary>
        /// Step size in seconds
        /// </summary>
        int StepSize
        {
            get; set;
        }

        /// <summary>
        /// Speaker ids for identification
        /// </summary>
        Guid[] SpeakerIds
        {
            get; set;
        }

        /// <summary>
        /// Id associated with all requests related to this client
        /// </summary>
        Guid ClientId
        {
            get; set;
        }

        /// <summary>
        /// Recognition audio format
        /// </summary>
        IAudioFormat AudioFormat
        {
            get; set;
        }

        /// <summary>
        /// Streams audio to recognition service
        /// </summary>
        /// <param name="audioBytes">Audio bytes to be sent for recognition</param>
        Task StreamAudioAsync(byte[] audioBytes);

        /// <summary>
        /// Sends a signal to the recognition client that the audio is stream ended
        /// </summary>
        Task EndStreamAudioAsync();
    }
}