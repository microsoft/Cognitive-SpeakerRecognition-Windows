// <copyright file="AudioFormatHandler.cs" company="Microsoft">
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
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles audio formats, and parses header
    /// </summary>
    internal class AudioFormatHandler
    {
        private AudioHeaderParsingResult parsingResult;

        private Encoding encoding = new ASCIIEncoding();

        private AudioFormat defaultAudioFormat;

        /// <summary>
        /// Initializes a new instance of the AudioFormatHandler class.
        /// </summary>
        /// <param name="audioFormat">Audio format</param>
        public AudioFormatHandler(AudioFormat audioFormat)
        {
            this.InputAudioFormat = audioFormat;
            this.defaultAudioFormat = new AudioFormat(AudioEncoding.PCM, 1, 16000, 16, new AudioContainer(AudioContainerType.WAV));
        }

        /// <summary>
        /// Gets input audio codec and container format
        /// </summary>
        public AudioFormat InputAudioFormat
        {
            get; private set;
        }

        /// <summary>
        /// Parses an audio file header and returns parsing results (start of data-chunk and number of bytes per second)
        /// </summary>
        /// <param name="header">Audio file header</param>
        /// <returns>Parsing results (start of data-chunk and number of bytes per second)</returns>
        public virtual AudioHeaderParsingResult ParseHeader(byte[] header)
        {
            if (header.Length < this.InputAudioFormat.Container.MaxHeaderSize)
            {
                throw new ArgumentException($"Input size is incorrect. Expected {this.InputAudioFormat.Container.MaxHeaderSize} vs Actual: {header.Length}");
            }

            this.parsingResult = new AudioHeaderParsingResult();
            if (this.InputAudioFormat.Container.MaxHeaderSize == 0)
            {
                this.parsingResult.NumberofBytesPerSecond = this.CalculateBytesPerSecond(this.defaultAudioFormat);
                this.parsingResult.DataChunckStart = 0;
                return this.parsingResult;
            }

            this.ProcessHeader(header);

            this.parsingResult.NumberofBytesPerSecond = this.CalculateBytesPerSecond(this.InputAudioFormat);
            return this.parsingResult;
        }

        private void ProcessHeader(byte[] header)
        {
            var parsedFormat = this.ParseContainerHeader(header);

            if (!this.InputAudioFormat.Equals(parsedFormat))
            {
                throw new ArgumentException($"Actual format does not match claimed format. Actual format:  {parsedFormat.ToString()} vs Claimed format: {this.InputAudioFormat.ToString()}");
            }
        }

        private AudioFormat ParseContainerHeader(byte[] header)
        {
            AudioFormat parsedFormat = null;

            using (Stream stream = new MemoryStream(header))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                if (this.InputAudioFormat.Container.ContainerType.Equals(AudioContainerType.WAV))
                {
                    string label = this.GetChunkLabel(reader, stream, 0);
                    if (string.CompareOrdinal(label, "RIFF") != 0)
                    {
                        throw new InvalidDataException("Unable to find RIFF signature in header");
                    }

                    label = this.GetChunkLabel(reader, stream, 8);
                    if (string.CompareOrdinal(label, "WAVE") != 0)
                    {
                        throw new InvalidDataException("Unable to find WAVE signature in header");
                    }

                    bool isParsed = false;
                    while (!isParsed)
                    {
                        // Safe to cast to int because the header size can't be > 5k
                        label = this.GetChunkLabel(reader, stream, (int)stream.Position);
                        int chunkSize = reader.ReadInt32();

                        switch (label)
                        {
                            case "fmt ":
                                long currentStreamPosition = stream.Position;
                                AudioEncoding encoding = AudioEncoding.None;
                                if (reader.ReadInt16() == 1)
                                {
                                    encoding = AudioEncoding.PCM;
                                }                                    

                                int channelsNumber = reader.ReadInt16();

                                int sampleRate = reader.ReadInt32();

                                // Skipping the unneeded format specs
                                stream.Position += 6;

                                int bitsPerSample = reader.ReadInt16();

                                parsedFormat = new AudioFormat(encoding, channelsNumber, sampleRate, bitsPerSample, new AudioContainer(AudioContainerType.WAV));

                                stream.Position = currentStreamPosition + chunkSize;
                                break;
                            case "data":
                                isParsed = true;
                                this.parsingResult.DataChunckStart = (int)stream.Position;
                                if (parsedFormat == null)
                                {
                                    throw new InvalidDataException("Unable to find the fmt chunk in header");
                                }

                                break;
                            default:
                                stream.Position += chunkSize;                                
                                break;
                        }
                    }
                }
                else
                {
                    throw new InvalidDataException($"Unsupported container format: {this.InputAudioFormat.Container.ContainerType.ToString()}");
                }
            }

            return parsedFormat;
        }

        private string GetChunkLabel(BinaryReader reader, Stream stream, int position)
        {
            stream.Position = position;
            byte[] lableBytes = reader.ReadBytes(4);
            return this.encoding.GetString(lableBytes, 0, lableBytes.Length);
        }

        private int CalculateBytesPerSecond(AudioFormat format)
        {
            int count = (format.BitsPerSample * format.SampleRate * format.ChannelsNumber) / 8;
            return count;
        }
    }
}