// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using PdfToSvg.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfToSvg.Tests.Drawing
{
    public class CssUtilsTests
    {
        [TestCase("", "\"\"")]
        [TestCase("Value1\tValue2\tValue3", "\"Value1\\9Value2\\9Value3\"")]
        [TestCase("This string contains 'single' quotation marks", "\"This string contains 'single' quotation marks\"")]
        [TestCase("This string contains \"double\" quotation marks", "\"This string contains \\\"double\\\" quotation marks\"")]
        [TestCase("This string contains \\ a backslash", "\"This string contains \\\\ a backslash\"")]
        [TestCase("This is a string\r\nspanning multiple\nlines", "\"This is a string\\d\\aspanning multiple\\alines\"")]
        [TestCase("Go to \u21E8\tthe right!", "\"Go to \\21e8\\9the right!\"")]
        [TestCase("Time for \uD83C\uDF7F popcorn!", "\"Time for \\1f37f  popcorn!\"")]
        [TestCase("Sum of zero: \u22110", "\"Sum of zero: \\2211 0\"")]
        public void EncodeString(string input, string expectedOutput)
        {
            var actualOutput = CssUtils.EncodeString(input);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void EncodeString_Null()
        {
            Assert.Throws<ArgumentNullException>(() => CssUtils.EncodeString(null));
        }
    }
}
