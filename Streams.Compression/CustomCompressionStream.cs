using System;
using System.Collections.Generic;
using System.IO;


namespace Streams.Compression {
    public class CustomCompressionStream : Stream {
        private readonly Stream _baseStream;
        private readonly bool _read;
        private readonly IEnumerator<byte> _reader;

        public CustomCompressionStream(Stream baseStream, bool read) {
            _read = read; // Используйте этот флаг, чтобы понимать:
            // ваш стрим открыт в режиме чтения или в режиме записи.
            // Не нужно поддерживать и чтение и запись одновременно.
            _baseStream = baseStream;
            if (_read)
                _reader = ReadAndUnCompress().GetEnumerator();
        }

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin) {
            if (!_read)
                throw new NotSupportedException();
            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            if (!_read)
                throw new NotSupportedException();
            _baseStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            int result = 0;
            for (int i = 0; i < count; ++i) {
                if (_reader.MoveNext()) {
                    buffer[i + offset] = _reader.Current;
                    ++result;
                }
                else {
                    break;
                }
            }

            return result;
        }

        private IEnumerable<byte> ReadAndUnCompress() {
            byte[] buffer = new byte[1024];
            int bytesCount = 1;
            int offset = 0;
            while (true) {
                Array.Clear(buffer, offset, 1024 - offset);
                var count = _baseStream.Read(buffer, offset, 1024 - offset);

                if (count == 0 && offset != 0)
                    throw new InvalidOperationException();

                if (count == 0)
                    yield break;

                count += offset;

                var length = count % 2 == 0 ? count : count - 1;
                for (int i = 0; i < length; ++i) {
                    if (i % 2 == 0) {
                        bytesCount = buffer[i];
                    }
                    else {
                        for (int j = 0; j < bytesCount; ++j)
                            yield return buffer[i];
                    }
                }

                if (count % 2 == 0)
                    offset = 0;
                else {
                    offset = 1;
                    buffer[0] = buffer[count - 1];
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (count == 0)
                return;

            var resultBytes = new List<byte>(count);
            var currentByte = buffer[offset];
            var currentByteCount = 0;

            for (int i = 0; i < count; ++i) {
                if (buffer[offset + i] == currentByte && currentByteCount < 255) {
                    ++currentByteCount;
                    if (i + 1 == count) {
                        resultBytes.Add((byte) currentByteCount);
                        resultBytes.Add(currentByte);
                    }
                }
                else {
                    resultBytes.Add((byte) currentByteCount);
                    resultBytes.Add(currentByte);
                    currentByte = buffer[offset + i];
                    currentByteCount = 1;
                }
            }

            _baseStream.Write(resultBytes.ToArray(), 0, resultBytes.Count);
        }

        public override bool CanRead => _read;
        public override bool CanSeek => false;
        public override bool CanWrite => !_read;

        public override long Length {
            get {
                if (!_read)
                    throw new NotSupportedException();
                return _baseStream.Length;
            }
        }

        public override long Position {
            get {
                if (!_read)
                    throw new NotSupportedException();
                return _baseStream.Position;
            }
            set {
                if (!_read)
                    throw new NotSupportedException();
                _baseStream.Position = value;
            }
        }
    }
}