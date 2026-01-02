// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfToSvg.Tests.Shims
{
    public class ArraySegmentExtensionsTests
    {
        [Test]
        public void ToArray()
        {
            var segment = new ArraySegment<int>(new[] { 1, 2, 3, 4, 5 }, 1, 3);
            var array = segment.ToArray();
            Assert.AreEqual(new[] { 2, 3, 4 }, array);
        }
    }
}
