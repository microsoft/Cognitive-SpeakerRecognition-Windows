
namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Interface.Audio
{
    using System.Threading.Tasks;

    /// <summary>
    /// An audio processor that handles streaming by means of the sliding window,
    /// It processes input stream, and prepares the corresponding waves according to the sliding window parameters.
    /// </summary>
    internal interface IAudioProcessor
    {
        /// <summary>
        /// A boolean to indicate whether processing is over or not.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Stores input bytes into buffer for processing.
        /// </summary>
        /// <param name="bytesToSend">The byte array to send</param>
        Task AppendAsync(byte[] bytesToSend);

        /// <summary>
        /// Stores input bytes into buffer for processing.
        /// </summary>
        /// <param name="buffer">The byte array to send from</param>
        /// <param name="offset">The index at which to start index</param>
        /// <param name="numberOfBytes">The number of bytes to be sent</param>
        Task AppendAsync(byte[] buffer, int offset, int numberOfBytes);

        /// <summary>
        /// Gets the next window to be sent.
        /// </summary>
        /// <returns>A byte array containing the wave to be sent</returns>
        Task<byte[]> GetNextRequestAsync();

        /// <summary>
        /// Signals the processor to stop processing data
        /// </summary>
        Task CompleteAsync();
    }
}
