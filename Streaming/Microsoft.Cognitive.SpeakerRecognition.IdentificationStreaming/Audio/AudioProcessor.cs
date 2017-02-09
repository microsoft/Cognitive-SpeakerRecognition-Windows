
namespace Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming.Audio
{
    // System Namespace
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    // Microsoft.Cognitive.SpeakerRecognition.IdentificationStreaming Namespace
    using Interface.Audio;

    /// <summary>
    /// An audio processor that handles streaming by means of the sliding window,
    /// It processes input stream, and prepares the corresponding waves according to the sliding window parameters.
    /// </summary>
    internal class AudioProcessor : IAudioProcessor
    {
        private Queue<byte[]> secondsQueue;
        private ConcurrentQueue<byte[]> wavesQueue;

        private byte[] lastSecond;
        private int lastSecondIndex;

        private bool headerFound;
        private byte[] headerBuffer;
        private int headerBufferIndex;

        private readonly int MaxWaveHeaderSize;

        private int stepSize = 0;
        private int windowSize = 0;
        private int secondsBuffered = 0;

        private int numberOfBytesPerSecond;

        private IAudioFormatHandler audioFormatHandler;
        
        /// <summary>
        /// A boolean to indicate whether processing is over or not.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// A constructor to create an AudioProcessor object given a specified window size, step size and an audio format handler
        /// </summary>
        /// <param name="windowSize">The number of seconds to be included in each request</param>
        /// <param name="stepSize">The number of seconds between every request</param>
        /// <param name="audioFormatHandler">A helper handler to process the input stream, and verify it's type</param>
        public AudioProcessor(int windowSize, int stepSize, IAudioFormatHandler audioFormatHandler)
        {

            if (windowSize <= 0)
            {
                throw new ArgumentException("Window size must be a positive integer", nameof(windowSize));
            }

            if(stepSize <= 0)
            {
                throw new ArgumentException("Step size must be a positive integer", nameof(stepSize));
            }

            if(audioFormatHandler == null)
            {
                throw new ArgumentNullException(nameof(audioFormatHandler));
            }

            this.windowSize = windowSize;
            this.stepSize = stepSize;
            this.audioFormatHandler = audioFormatHandler;

            this.secondsQueue = new Queue<byte[]>();
            this.wavesQueue = new ConcurrentQueue<byte[]>();

            this.lastSecond = null;
            this.lastSecondIndex = 0;

            this.MaxWaveHeaderSize = audioFormatHandler.InputAudioFormat.Container.MaxHeaderSize;

            this.headerFound = false;
            this.headerBuffer = new byte[MaxWaveHeaderSize];
            this.headerBufferIndex = 0;
        }

        /// <summary>
        /// Stores input bytes into buffer for processing.
        /// </summary>
        /// <param name="bytesToSend">The byte array to send</param>
        public async Task AppendAsync(byte[] bytesToSend)
        {
            if(bytesToSend == null)
            {
                throw new ArgumentNullException(nameof(bytesToSend));
            }

            await AppendAsync(bytesToSend, 0, bytesToSend.Length).ConfigureAwait(false);
        }

        /// <summary>
        /// Stores input bytes into buffer for processing.
        /// </summary>
        /// <param name="buffer">The byte array to send from</param>
        /// <param name="offset">The index at which to start index</param>
        /// <param name="numberOfBytes">The number of bytes to be sent</param>
        public async Task AppendAsync(byte[] buffer, int offset, int numberOfBytes)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentException("Offset to start sending from must be a positive integer", nameof(offset));
            }

            if (numberOfBytes < 0)
            {
                throw new ArgumentException("Number of bytes to send must be a positive integer", nameof(numberOfBytes));
            }            

            if (offset + numberOfBytes > buffer.Length)
            {
                throw new ArgumentException("There aren't enough bytes to send");
            }

            if (!headerFound)
            {
                await ProcessHeader(buffer, offset, numberOfBytes).ConfigureAwait(false);
            }
            else
            {
                await AppendToQueue(buffer, offset, numberOfBytes).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the next window to be sent.
        /// </summary>
        /// <returns>A byte array containing the wave to be sent</returns>
        public Task<byte[]> GetNextRequestAsync()
        {
            byte[] audio;
            if (!wavesQueue.TryDequeue(out audio))
            {
                return Task.FromResult<byte[]>(null);
            }
            return Task.FromResult(audio);
        }

        /// <summary>
        /// Signals the processor to stop processing data
        /// </summary>
        public async Task CompleteAsync()
        {
            this.IsCompleted = true;
            if (this.secondsBuffered > 0)
            {
                await PrepareRequestAsync().ConfigureAwait(false);
            }
        }

        private async Task AppendToQueue(byte[] bytesToSend, int offset, int numberOfBytes)
        {
            do
            {
                int numberOfBytesToSend = Math.Min(numberOfBytes, numberOfBytesPerSecond - lastSecondIndex);
                Array.Copy(bytesToSend, offset, lastSecond, lastSecondIndex, numberOfBytesToSend);

                offset += numberOfBytesToSend;
                numberOfBytes -= numberOfBytesToSend;
                lastSecondIndex += numberOfBytesToSend;

                if (lastSecondIndex == numberOfBytesPerSecond)
                {
                    var second = (byte[])lastSecond.Clone();

                    secondsQueue.Enqueue(second);
                    if (secondsQueue.Count > this.windowSize)
                    {
                        secondsQueue.Dequeue();
                    }

                    secondsBuffered = (secondsBuffered + 1) % this.stepSize;
                    if (secondsBuffered == 0)
                    {
                        await PrepareRequestAsync().ConfigureAwait(false);
                    }

                    lastSecondIndex = 0;
                }
            } while (numberOfBytes > 0);
        }

        private async Task ProcessHeader(byte[] bytesToSend, int offset, int numberOfBytes)
        {
            int numberOfBytesToSend = Math.Min(numberOfBytes, MaxWaveHeaderSize - headerBufferIndex);
            Array.Copy(bytesToSend, offset, headerBuffer, headerBufferIndex, numberOfBytesToSend);

            offset += numberOfBytesToSend;
            numberOfBytes -= numberOfBytesToSend;
            headerBufferIndex += numberOfBytesToSend;

            if (headerBufferIndex == MaxWaveHeaderSize)
            {
                var result = audioFormatHandler.ParseHeader(headerBuffer);

                this.numberOfBytesPerSecond = result.NumberofBytesPerSecond;
                if(this.numberOfBytesPerSecond <= 0)
                {
                    throw new InvalidDataException("The input audio's number of bytes per second must be a positive integer");
                }

                this.lastSecond = new byte[numberOfBytesPerSecond];

                headerFound = true;
                int headerSize = result.DataChunckStart;

                await AppendAsync(headerBuffer, headerSize, MaxWaveHeaderSize - headerSize).ConfigureAwait(false);
                await AppendAsync(bytesToSend, offset, numberOfBytes).ConfigureAwait(false);
            }
        }

        private Task PrepareRequestAsync()
        {
            var audioWave = GenerateWaveFile();
            wavesQueue.Enqueue(audioWave);
            return Task.FromResult(0);
        }

        private byte[] GenerateWaveFile()
        {
            const int bitDepth = 16;
            int totalSampleCount = 16000* secondsQueue.Count;
            const int sampleRate = 16000;

            using (var stream = new MemoryStream())
            {
                stream.Position = 0;
                stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
                stream.Write(BitConverter.GetBytes(((bitDepth / 8) * totalSampleCount) + 36), 0, 4);
                stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);
                stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);
                stream.Write(BitConverter.GetBytes(16), 0, 4);
                stream.Write(BitConverter.GetBytes((ushort)1), 0, 2);
                stream.Write(BitConverter.GetBytes(1), 0, 2);
                stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);
                stream.Write(BitConverter.GetBytes(sampleRate * (bitDepth / 8)), 0, 4);
                stream.Write(BitConverter.GetBytes((ushort)(bitDepth / 8)), 0, 2);
                stream.Write(BitConverter.GetBytes(bitDepth), 0, 2);
                stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
                stream.Write(BitConverter.GetBytes((bitDepth / 8) * totalSampleCount), 0, 4);

                foreach (var wave in secondsQueue)
                {
                    stream.Write(wave, 0, wave.Length);
                }

                return stream.ToArray();
            }
        }

    }
}