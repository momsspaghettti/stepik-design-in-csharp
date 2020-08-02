using System;
using System.IO;

namespace Streams.Compression
{
    public class PartialStream : Stream
    {
        private readonly int partSize;
        private int index;
        private readonly byte[] content;

        public PartialStream(int partSize, byte[] content)
        {
            this.partSize = partSize;
            this.content = content;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = Math.Min(partSize, count);
            for (int i = offset; i < offset + count; ++i)
                buffer[i] = content[index++ % content.Length];
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead { get; }
        public override bool CanSeek { get; }
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }
    }
}