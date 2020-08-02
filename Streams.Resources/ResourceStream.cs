using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Streams.Resources {
    public class ResourceReaderStream : Stream {
        private readonly Stream _underlyingStream;
        private readonly string _key;

        private bool _keyFound;
        private bool _streamEnded;

        private byte[] _value;
        private int _pointer;

        public ResourceReaderStream(Stream stream, string key) {
            _underlyingStream = stream;
            _key = key;
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return _underlyingStream.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            _underlyingStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            // if not key found yet: SeekValue();
            // if value is not read yet: ReadFieldValue(...)
            // else return 0;
            if (!_keyFound && !_streamEnded)
                SeekValue();
            if (!_streamEnded)
                return ReadFieldValue(buffer, offset, count);
            return 0;
        }

        private int ReadFieldValue(byte[] buffer, int offset, int count) {
            count = Math.Min(count, _value.Length - _pointer);
            for (int i = 0; i < count; ++i) {
                buffer[i + offset] = _value[i + _pointer];
            }

            _pointer += count;
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            _underlyingStream.Write(buffer, offset, count);
        }

        public override bool CanRead => _underlyingStream.CanRead;
        public override bool CanSeek => _underlyingStream.CanSeek;
        public override bool CanWrite => _underlyingStream.CanWrite;
        public override long Length => _underlyingStream.Length;

        public override long Position {
            get => _underlyingStream.Position;
            set => _underlyingStream.Position = value;
        }

        private IEnumerable<string> GetItems() {
            var itemBytesList = new List<byte>();
            while (true) {
                var buffer = new byte[Constants.BufferSize];
                var count = _underlyingStream.Read(buffer, 0, Constants.BufferSize);
                if (count == 0)
                    yield break;

                int i = 0;
                for (; i < count - 1; ++i) {
                    if (buffer[i] == 0 && buffer[i + 1] == 0) {
                        itemBytesList.Add(0);
                        ++i;
                    }
                    else if (buffer[i] == 0 && buffer[i + 1] == 1) {
                        ++i;
                        yield return Encoding.ASCII.GetString(itemBytesList.ToArray());
                        itemBytesList = new List<byte>();
                    }
                    else {
                        itemBytesList.Add(buffer[i]);
                    }
                }

                if (buffer[count - 1] != 0 && buffer[count - 1] != 1)
                    itemBytesList.Add(buffer[count - 1]);
                if (count == 1)
                    yield return Encoding.ASCII.GetString(itemBytesList.ToArray());
            }
        }

        struct KeyValue {
            public string Key;
            public string Value;
        }

        private IEnumerable<KeyValue> GetKeyValuePairs() {
            int i = 0;
            string key = "";
            foreach (var item in GetItems()) {
                if (i % 2 == 0)
                    key = item;
                else {
                    yield return new KeyValue {Key = key, Value = item};
                }

                ++i;
            }
        }

        private void SeekValue() {
            foreach (var keyValuePair in GetKeyValuePairs()) {
                if (keyValuePair.Key == _key) {
                    _keyFound = true;
                    _value = Encoding.ASCII.GetBytes(keyValuePair.Value);
                    return;
                }
            }

            _streamEnded = true;
        }

        public override void Flush() {
            // nothing to do
        }
    }
}