// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using PdfToSvg.Common;
using PdfToSvg.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfToSvg.Tests.Parsing
{
    public class ObjectScannerTests
    {

        [TestCase("\nstrea\r stream", "stream", 8)]

        [TestCase("\nstream", "stream", 1)]
        [TestCase("\n stream", "stream", 2)]
        [TestCase("\n stream ", "stream", 2)]

        [TestCase("\nendstream", "endstream", 1)]
        [TestCase("\n endstream", "endstream", 2)]
        [TestCase("\n endstream ", "endstream", 2)]

        [TestCase("\nendobj", "endobj", 1)]
        [TestCase("\n endobj", "endobj", 2)]
        [TestCase("\n endobj ", "endobj", 2)]

        [TestCase("\ntrailer << >>", "trailer", 1)]
        [TestCase("\n trailer<<>>", "trailer", 2)]
        [TestCase("\n trailer\n<< >>", "trailer", 2)]

        [TestCase("\n1  2  obj", "obj 1 2", 1)]
        [TestCase("\n1 2 obj", "obj 1 2", 1)]
        [TestCase("\n  1 2 obj", "obj 1 2", 3)]
        [TestCase("\n  1 2 obj  ", "obj 1 2", 3)]
        [TestCase("\n12345678 987654321 obj  ", "obj 12345678 987654321", 1)]
        public void ScanStream_Success(string haystack, string expectedToken, long expectedPosition)
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(haystack));

            var result = ObjectScanner.ScanStream(stream, default);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedToken, result[0].ToString());
            Assert.AreEqual(expectedPosition, result[0].Position);
        }

        [TestCase("")]
        [TestCase("\n")]
        [TestCase("\n1")]
        [TestCase("\n1 1")]
        [TestCase("\n1 1 ")]
        [TestCase("\n1x1 obj")]
        [TestCase("\nstrea")]
        [TestCase("\nstrea\n")]
        [TestCase("\nstreax")]
        [TestCase("\nstreaM")]
        [TestCase("\n9147483647 9147483647 obj  ")]
        [TestCase("\n91474836470 91474836470 obj  ")]
        public void ScanStream_Fail(string haystack)
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(haystack));

            var result = ObjectScanner.ScanStream(stream, default);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ScanStream_StreamLongerThanBufferSize()
        {
            var count = 10000;
            const string Fragment = "\n stream ";
            var content = new StringBuilder();

            for (var i = 0; i < count; i++)
            {
                content.Append(Fragment);
            }

            var stream = new MemoryStream(Encoding.ASCII.GetBytes(content.ToString()));

            var result = ObjectScanner
                .ScanStream(stream, default)
                .GroupBy(x => x.Position)
                .Select(x => x.First())
                .OrderBy(x => x.Position)
                .ToList();

            Assert.AreEqual(count, result.Count);

            for (var i = 0; i < count; i++)
            {
                Assert.AreEqual(ObjectScanner.ScanToken.Stream, result[i].Token);
                Assert.AreEqual(i * Fragment.Length + 2, result[i].Position);
            }
        }
    }
}
