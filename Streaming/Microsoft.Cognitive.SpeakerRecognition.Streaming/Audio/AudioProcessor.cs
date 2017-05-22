// <copyright file="AudioProcessor.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
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

namespace  Microsoft.Cognitive.SpeakerRecognition.Streaming.Audio
{
    // System Namespace
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;    

    /// <summary>
    /// An audio processor that handles streaming by means of the sliding window,
    /// It processes input stream, and prepares the corresponding waves according to the sliding window parameters.
    /// </summary>
    internal class AudioProcessor
    {
        private readonly int maxWaveHeaderSize;

        private Queue<byte[]> secondsQueue;
        private ConcurrentQueue<byte[]> wavesQueue;

        private byte[] lastSecond;
        private int lastSecondIndex;

        private bool headerFound;
        private byte[] headerBuffer;
        private int headerBufferIndex;

        private int stepSize = 0;
        private int windowSize = 0;
        private int secondsBuffered = 0;

        private int numberOfBytesPerSecond;

        private AudioFormatHandler audioFormatHandler;
        
        /// <summary>
        /// Initializes a new instance of the AudioProcessor class given a specified window size, step size and an audio format handler
        /// </summary>
        /// <param name="windowSize">The number of seconds to be included in each request</param>
        /// <param name="stepSize">The number of seconds between every request</param>
        /// <param name="audioFormatHandler">A helper handler to process the input stream, and verify its type</param>
        public AudioProcessor(int windowSize, int stepSize, AudioFormatHandler audioFormatHandler)
        {
            if (windowSize <= 0)
            {
                throw new ArgumentException("Window size must be a positive integer", nameof(windowSize));
            }

            if (stepSize <= 0)
            {
                throw new ArgumentException("Step size must be a positive integer", nameof(stepSize));
            }

            if (audioFormatHandler == null)
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

            this.maxWaveHeaderSize = audioFormatHandler.InputAudioFormat.Container.MaxHeaderSize;

            this.headerFound = false;
            this.headerBuffer = new byte[this.maxWaveHeaderSize];
            this.headerBufferIndex = 0;
        }

        /// <summary>
        /// Gets a boolean to indicate whether processing is complete or not.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Stores input bytes into buffer for processing.
        /// </summary>
        /// <param name="bytesToSend">The byte array to send</param>
        public async Task AppendAsync(byte[] bytesToSend)
        {
            if (bytesToSend == null)
            {
                throw new ArgumentNullException(nameof(bytesToSend));
            }

            await this.AppendAsync(bytesToSend, 0, bytesToSend.Length).ConfigureAwait(false);
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
                throw new ArgumentException("Offset to start sending from must be a non-negative integer", nameof(offset));
            }

            if (numberOfBytes < 0)
            {
                throw new ArgumentException("Number of bytes to send must be a non-negative integer", nameof(numberOfBytes));
            }            

            if (offset + numberOfBytes > buffer.Length)
            {
                throw new ArgumentException("There aren't enough bytes to send");
            }

            if (!this.headerFound)
            {
                await this.ProcessHeader(buffer, offset, numberOfBytes).ConfigureAwait(false);
            }
            else
            {
                await this.AppendToQueue(buffer, offset, numberOfBytes).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the next window to be sent.
        /// </summary>
        /// <returns>A byte array containing the wave to be sent</returns>
        public Task<byte[]> GetNextRequestAsync()
        {
            byte[] audio;
            if (!this.wavesQueue.TryDequeue(out audio))
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
                await this.PrepareRequestAsync().ConfigureAwait(false);
            }
        }

        private async Task AppendToQueue(byte[] bytesToSend, int offset, int numberOfBytes)
        {
            do
            {
                int numberOfBytesToSend = Math.Min(numberOfBytes, this.numberOfBytesPerSecond - this.lastSecondIndex);
                Array.Copy(bytesToSend, offset, this.lastSecond, this.lastSecondIndex, numberOfBytesToSend);

                offset += numberOfBytesToSend;
                numberOfBytes -= numberOfBytesToSend;
                this.lastSecondIndex += numberOfBytesToSend;

                if (this.lastSecondIndex == this.numberOfBytesPerSecond)
                {
                    var second = (byte[])this.lastSecond.Clone();

                    this.secondsQueue.Enqueue(second);
                    if (this.secondsQueue.Count > this.windowSize)
                    {
                        this.secondsQueue.Dequeue();
                    }

                    this.secondsBuffered = (this.secondsBuffered + 1) % this.stepSize;
                    if (this.secondsBuffered == 0)
                    {
                        await this.PrepareRequestAsync().ConfigureAwait(false);
                    }

                    this.lastSecondIndex = 0;
                }
            }
            while (numberOfBytes > 0);
        }

        private async Task ProcessHeader(byte[] bytesToSend, int offset, int numberOfBytes)
        {
            int numberOfBytesToSend = Math.Min(numberOfBytes, this.maxWaveHeaderSize - this.headerBufferIndex);
            Array.Copy(bytesToSend, offset, this.headerBuffer, this.headerBufferIndex, numberOfBytesToSend);

            offset += numberOfBytesToSend;
            numberOfBytes -= numberOfBytesToSend;
            this.headerBufferIndex += numberOfBytesToSend;

            if (this.headerBufferIndex == this.maxWaveHeaderSize)
            {
                var result = this.audioFormatHandler.ParseHeader(this.headerBuffer);

                this.numberOfBytesPerSecond = result.NumberofBytesPerSecond;
                if (this.numberOfBytesPerSecond <= 0)
                {
                    throw new InvalidDataException("The input audio's number of bytes per second must be a positive integer");
                }

                this.lastSecond = new byte[this.numberOfBytesPerSecond];

                this.headerFound = true;
                int headerSize = result.DataChunckStart;

                await this.AppendAsync(this.headerBuffer, headerSize, this.maxWaveHeaderSize - headerSize).ConfigureAwait(false);
                await this.AppendAsync(bytesToSend, offset, numberOfBytes).ConfigureAwait(false);
            }
        }

        private Task PrepareRequestAsync()
        {
            var audioWave = this.GenerateWaveFile();
            this.wavesQueue.Enqueue(audioWave);
            return Task.FromResult(0);
        }

        private byte[] GenerateWaveFile()
        {
            const int BitDepth = 16;
            const int SampleRate = 16000;
            int totalSampleCount = SampleRate * this.secondsQueue.Count;            

            using (var stream = new MemoryStream())
            {
                stream.Position = 0;
                stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
                stream.Write(BitConverter.GetBytes(((BitDepth / 8) * totalSampleCount) + 36), 0, 4);
                stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);
                stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);
                stream.Write(BitConverter.GetBytes(16), 0, 4);
                stream.Write(BitConverter.GetBytes((ushort)1), 0, 2);
                stream.Write(BitConverter.GetBytes(1), 0, 2);
                stream.Write(BitConverter.GetBytes(SampleRate), 0, 4);
                stream.Write(BitConverter.GetBytes(SampleRate * (BitDepth / 8)), 0, 4);
                stream.Write(BitConverter.GetBytes((ushort)(BitDepth / 8)), 0, 2);
                stream.Write(BitConverter.GetBytes(BitDepth), 0, 2);
                stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
                stream.Write(BitConverter.GetBytes((BitDepth / 8) * totalSampleCount), 0, 4);

                foreach (var wave in this.secondsQueue)
                {
                    stream.Write(wave, 0, wave.Length);
                }

                return stream.ToArray();
            }
        }
    }
}