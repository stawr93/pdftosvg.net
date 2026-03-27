// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfToSvg.Tests.Shims
{
    public class MemoryStreamExtensionsTests
    {
#if NETFRAMEWORK
        [Test]
        public void TryGetBuffer_Slice_Start()
        {
            var memoryStream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6 }, 0, 3, writable: false, publiclyVisible: true);
            Assert.IsFalse(memoryStream.TryGetBuffer(out var buffer));
        }

        [Test]
        public void TryGetBuffer_Slice_End()
        {
            var memoryStream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6 }, 3, 3, writable: false, publiclyVisible: true);
            Assert.IsFalse(memoryStream.TryGetBuffer(out var buffer));
        }
#endif

        [Test]
        public void TryGetBuffer_Authorized()
        {
            var memoryStream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6 }, 0, 6, writable: false, publiclyVisible: true);
            memoryStream.Position = 2;

            Assert.IsTrue(memoryStream.TryGetBuffer(out var buffer));

            Assert.AreEqual(0, buffer.Offset);
            Assert.AreEqual(6, buffer.Count);
            Assert.AreEqual(2, memoryStream.Position);
        }

        [Test]
        public void TryGetBuffer_Unauthorized()
        {
            var memoryStream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6 }, 0, 6, writable: false, publiclyVisible: false);
            memoryStream.Position = 2;

            Assert.IsFalse(memoryStream.TryGetBuffer(out var _));
        }
    }
}
