﻿// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using PdfToSvg.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToSvg.Tests.IO
{
    public class StreamExtensionsTests
    {
        [Test]
        public void ReadAll()
        {
            var stream = new ByteByByteMemoryStream(1, 2, 3, 4, 5, 6, 7, 8, 9);

            var buffer = new byte[5];

            Assert.AreEqual(5, stream.ReadAll(buffer, 0, buffer.Length));
            Assert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, buffer);

            Assert.AreEqual(1, stream.ReadAll(buffer, 0, 1));
            Assert.AreEqual(new byte[] { 6, 2, 3, 4, 5 }, buffer);

            Assert.AreEqual(3, stream.ReadAll(buffer, 0, buffer.Length));
            Assert.AreEqual(new byte[] { 7, 8, 9, 4, 5 }, buffer);

            Assert.AreEqual(0, stream.ReadAll(buffer, 0, buffer.Length));
            Assert.AreEqual(new byte[] { 7, 8, 9, 4, 5 }, buffer);

            Assert.AreEqual(0, stream.ReadAll(buffer, 0, buffer.Length));
            Assert.AreEqual(new byte[] { 7, 8, 9, 4, 5 }, buffer);
        }

        [Test]
        public async Task ReadAllAsync()
        {
            var stream = new ByteByByteMemoryStream(1, 2, 3, 4, 5, 6, 7, 8, 9);

            var buffer = new byte[5];

            Assert.AreEqual(5, await stream.ReadAllAsync(buffer, 0, buffer.Length));
            Assert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, buffer);

            Assert.AreEqual(1, await stream.ReadAllAsync(buffer, 0, 1));
            Assert.AreEqual(new byte[] { 6, 2, 3, 4, 5 }, buffer);

            Assert.AreEqual(3, await stream.ReadAllAsync(buffer, 0, buffer.Length));
            Assert.AreEqual(new byte[] { 7, 8, 9, 4, 5 }, buffer);

            Assert.AreEqual(0, await stream.ReadAllAsync(buffer, 0, buffer.Length));
            Assert.AreEqual(new byte[] { 7, 8, 9, 4, 5 }, buffer);

            Assert.AreEqual(0, await stream.ReadAllAsync(buffer, 0, buffer.Length));
            Assert.AreEqual(new byte[] { 7, 8, 9, 4, 5 }, buffer);
        }

        [Test]
        public void Skip()
        {
            var data = new byte[10000];

            var random = new Random(1);
            random.NextBytes(data);

            var stream = new NoSeekMemoryStream(data);

            stream.Skip(1);
            Assert.AreEqual(data[1], stream.ReadByte());
            Assert.AreEqual(data[2], stream.ReadByte());

            stream.Skip(1500);
            Assert.AreEqual(data[1503], stream.ReadByte());
            Assert.AreEqual(data[1504], stream.ReadByte());

            stream.Skip(9000);
            Assert.AreEqual(-1, stream.ReadByte());
        }

        [Test]
        public void WriteBigEndian()
        {
            var stream = new MemoryStream();

            stream.WriteBigEndian(0xfedca123u);

            Assert.AreEqual(new byte[] { 0xfe, 0xdc, 0xa1, 0x23 }, stream.ToArray());
        }
    }
}
