using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Streams.Compression
{
    public class CustomCompressionStream_should
    {
        private static readonly Random random = new Random();

        [Test]
        public void DoNotReadInConstructor()
        {
            var baseStream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
            var writer = new CustomCompressionStream(baseStream, false);
            Assert.NotNull(writer);
            Assert.AreEqual(0, baseStream.Position);
        }

        [Test]
        public void CanNotReadForWriter()
        {
            var baseStream = new MemoryStream();
            var writer = new CustomCompressionStream(baseStream, false);
            Assert.AreEqual(false, writer.CanRead);
            Assert.AreEqual(true, writer.CanWrite);
        }

        [Test]
        public void CanReadForReader()
        {
            var baseStream = new MemoryStream();
            var reader = new CustomCompressionStream(baseStream, true);
            Assert.AreEqual(true, reader.CanRead);
            Assert.AreEqual(false, reader.CanWrite);
        }

        [Test]
        public void CheckNotSupportedException()
        {
            var baseStream = new MemoryStream();
            var writer = new CustomCompressionStream(baseStream, false);
            Assert.Throws<NotSupportedException>(() => writer.SetLength(0));
            long x;
            Assert.Throws<NotSupportedException>(() => x = writer.Length);
            Assert.Throws<NotSupportedException>(() => writer.Position++);
            Assert.AreEqual(false, writer.CanSeek);
            Assert.Throws<NotSupportedException>(() => writer.Seek(0, SeekOrigin.Current));
        }

        [Test]
        public void CanFlush()
        {
            using (new CustomCompressionStream(new MemoryStream(), false))
            {
            }
        }

        [Test]
        public void WriteCompressData()
        {
            var baseStream = new MemoryStream();
            var data = GenerateSomeInputData();
            using (var writer = new CustomCompressionStream(baseStream, false))
                writer.Write(data.ToArray(), 0, data.Length);

            var ratio = (double)baseStream.Position / data.Length;
            Assert.Less(ratio, 0.95);
            Assert.Pass($"Compress ratio is {ratio:0.#%}");
        }

        [Test]
        public void WriteByPiecesIsEqualToFullWrite()
        {
            var data = new List<byte>();
            var baseStream1 = new MemoryStream();
            using (var writer1 = new CustomCompressionStream(baseStream1, false))
            {
                for (int partIndex = 0; partIndex < 10; ++partIndex)
                {
                    var part = GenerateSomeInputData(10, 1);
                    part[0] = 0;
                    writer1.Write(part, 0, part.Length);
                    data.AddRange(part);
                }
            }

            var baseStream2 = new MemoryStream();
            using (var writer2 = new CustomCompressionStream(baseStream2, false))
                writer2.Write(data.ToArray(), 0, data.Count);
            CollectionAssert.AreEqual(baseStream1.ToArray(), baseStream2.ToArray());
        }

        [Test]
        public void ReadTheSameDataWasWritten()
        {
            var baseStream = new MemoryStream();
            var data = GenerateSomeInputData();
            using (var writer = new CustomCompressionStream(baseStream, false))
                writer.Write(data, 0, data.Length);
            baseStream.Position = 0;
            var reader = new CustomCompressionStream(baseStream, true);
            CollectionAssert.AreEqual(data, ReadAll(reader));
        }

        [Test]
        public void ReadNothingFromEmptyStream()
        {
            var baseStream = new MemoryStream();
            var data = new byte[0];
            using (var writer = new CustomCompressionStream(baseStream, false))
                writer.Write(data, 0, data.Length);

            baseStream.Position = 0;
            var reader = new CustomCompressionStream(baseStream, true);
            CollectionAssert.AreEqual(data, ReadAll(reader));
        }

        [Test]
        public void ReadByPieces()
        {
            var baseStream = new MemoryStream();
            var data = GenerateSomeInputData();
            using (var writer = new CustomCompressionStream(baseStream, false))
                writer.Write(data, 0, data.Length);

            var reader = new CustomCompressionStream(baseStream, true);
            baseStream.Position = 0;

            var readData = new List<byte>();
            var piece = new byte[5];
            while (true)
            {
                int cnt = reader.Read(piece, 0, piece.Length);
                if (cnt == 0) break;
                readData.AddRange(piece.Take(cnt));
            }
            CollectionAssert.AreEqual(data, readData);
        }

        [Test]
        public void WriteLongSequenceOfOneSymbol()
        {
            var baseStream = new MemoryStream();
            var data = new List<byte>();
            for (int i = 0; i < 256; i++)
            {
                var bt = (byte)i;
                for (int j = 0; j < 254 + 3 * i; j++)
                    data.Add(bt);
            }

            using (var writer = new CustomCompressionStream(baseStream, false))
                writer.Write(data.ToArray(), 0, data.Count);

            baseStream.Position = 0;
            var reader = new CustomCompressionStream(baseStream, true);
            CollectionAssert.AreEqual(data, ReadAll(reader));
        }

        [Test]
        public void WriteWithOffset()
        {
            var baseStream = new MemoryStream();
            var data = new List<byte> { 1, 2, 2, 3, 3, 3 };
            using (var writer = new CustomCompressionStream(baseStream, false))
                writer.Write(data.ToArray(), 1, data.Count - 2);
            baseStream.Position = 0;
            var reader = new CustomCompressionStream(baseStream, true);
            CollectionAssert.AreEqual(new[] { 2, 2, 3, 3 }, ReadAll(reader));
        }

        [Test]
        public void ReadWithOffset()
        {
            var baseStream = new MemoryStream();
            var data = new List<byte> { 1, 2, 2, 3, 3, 3 };
            using (var writer = new CustomCompressionStream(baseStream, false))
                writer.Write(data.ToArray(), 0, data.Count);
            baseStream.Position = 0;
            var reader = new CustomCompressionStream(baseStream, true);
            var buffer = new byte[data.Count];
            var readCount = buffer.Length - 2;
            int cnt = reader.Read(buffer, 1, readCount);
            Assert.AreEqual(cnt, readCount);
            Assert.AreEqual(0, buffer[0]);
            Assert.AreEqual(0, buffer[buffer.Length - 1]);
            CollectionAssert.AreEqual(
                data.Take(readCount),
                buffer.Skip(1).Take(readCount));
        }

        [Test]
        public void ReadIsLazy()
        {
            var baseStream = new InfinityStream();
            var reader = new CustomCompressionStream(baseStream, true);
            var buffer = new byte[1000];
            int cnt = reader.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(cnt, buffer.Length);
            CollectionAssert.AreEqual(Enumerable.Repeat(1, 1000), buffer);
        }

        [Test]
        public void ReadLessBytesThanRequested()
        {
            var baseStream = new MemoryStream(WriteAll(new byte[] { 1, 1, 1, 1 }));
            var reader = new CustomCompressionStream(baseStream, true);
            var buffer = new byte[10];
            int cnt = reader.Read(buffer, 0, 10);
            Assert.Less(cnt, 10);
        }

        [Test]
        public void BaseStreamReturnsLessThanRequestedOnRead()
        {
            var baseStream = new PartialStream(1, WriteAll(new byte[] { 1, 1, 1, 1, 1 }));
            var reader = new CustomCompressionStream(baseStream, true);
            var buffer = new byte[5];
            var count = reader.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(buffer.Length, count);
            CollectionAssert.AreEqual(Enumerable.Repeat(1, 5), buffer);
        }

        [Test]
        public void FailOnOddLengthOfBaseStream()
        {
            var baseStream = new MemoryStream(WriteAll(new byte[] { 1, 2, 3, 5, 6 }).Take(3).ToArray());
            var reader = new CustomCompressionStream(baseStream, true);
            var buffer = new byte[1000];
            Assert.Throws<InvalidOperationException>(() => reader.Read(buffer, 0, buffer.Length));
        }

        [Test]
        [Repeat(10)]
        public void RandomizedWriteReadTest()
        {
            var baseStream = new MemoryStream();
            var data = GenerateSomeInputData();
            using (var writer = new CustomCompressionStream(baseStream, false))
                writer.Write(data, 0, data.Length);

            var ratio = (double)baseStream.Position / data.Length;
            Assert.Less(ratio, 0.95);

            baseStream.Position = 0;
            var reader = new CustomCompressionStream(baseStream, true);
            var readData = new List<byte>();
            var buffer = new byte[10];
            while (true)
            {
                int cnt = reader.Read(buffer, 0, buffer.Length);
                if (cnt == 0) break;
                readData.AddRange(buffer.Take(cnt));
            }
            CollectionAssert.AreEqual(data, readData);
        }

        private byte[] ReadAll(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        private byte[] WriteAll(byte[] input)
        {
            var baseStream = new MemoryStream();
            using (var compressor = new CustomCompressionStream(baseStream, false))
                compressor.Write(input, 0, input.Length);
            return baseStream.ToArray();
        }

        private static byte[] GenerateSomeInputData(int size = 100, int minValue = 0)
        {
            var data = new List<byte>();
            while (size > 0)
            {
                var bt = (byte)random.Next(minValue, 255);
                for (int j = 0; j < 2 + random.Next(2); j++)
                {
                    data.Add(bt);
                    size--;
                }
            }

            return data.ToArray();
        }
    }
}
