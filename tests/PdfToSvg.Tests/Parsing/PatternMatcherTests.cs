// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using PdfToSvg.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PdfToSvg.Tests.Parsing
{
    public class PatternMatcherTests
    {
        [TestCase("   hi", ' ', 0, 5, true, 3)]
        [TestCase("   hi", ' ', 1, 5, true, 3)]
        [TestCase("   hi", ' ', 1, 1, true, 1)]
        [TestCase("hi", ' ', 1, 1, false, 0)]
        [TestCase("hi", ' ', 0, 1, true, 0)]
        [TestCase("", ' ', 1, 1, false, 0)]
        [TestCase("", ' ', 0, 1, true, 0)]
        public void SkipChar(string input, char ch, int min, int max, bool expectedSuccess, int expectedCursor)
        {
            var matcher = new PatternMatcher(input);

            var actualSuccess = matcher.SkipChar(ch, min, max);

            Assert.AreEqual(expectedSuccess, actualSuccess);
            Assert.AreEqual(expectedCursor, matcher.Cursor);
        }
        
        [TestCase("   hi", "\t ", 10, 3)]
        [TestCase("   hi", "\t ", 2, 2)]
        [TestCase(" \t hi", "\t ", 2, 2)]
        [TestCase("hi", "\t ", 2, 0)]
        [TestCase("", "\t ", 2, 0)]
        public void SkipChars(string input, string chars, int max, int expectedCursor)
        {
            var matcher = new PatternMatcher(input);

            matcher.SkipChars(chars, max);

            Assert.AreEqual(expectedCursor, matcher.Cursor);
        }

        [TestCase("", "hello")]
        [TestCase("hel", "hello")]
        [TestCase("hellox", "hello")]
        [TestCase("helloX", "hello")]
        [TestCase("hello1", "hello")]
        [TestCase("hellO", "hello")]
        public void ReadKeyword_Fail(string input, string keyword)
        {
            var matcher = new PatternMatcher(input);

            var success = matcher.ReadKeyword(keyword);

            Assert.AreEqual(false, success);
            Assert.AreEqual(0, matcher.Cursor);
        }

        [TestCase("hello hi", "hello")]
        [TestCase("hello ", "hello")]
        [TestCase("hello", "hello")]
        public void ReadKeyword_Success(string input, string keyword)
        {
            var matcher = new PatternMatcher(input);

            var success = matcher.ReadKeyword(keyword);

            Assert.AreEqual(true, success);
            Assert.AreEqual(keyword.Length, matcher.Cursor);
        }

        [TestCase("", "hello")]
        [TestCase("hel", "hello")]
        [TestCase("hellO", "hello")]
        public void ReadString_Fail(string input, string str)
        {
            var matcher = new PatternMatcher(input);

            var success = matcher.ReadString(str);

            Assert.AreEqual(false, success);
            Assert.AreEqual(0, matcher.Cursor);
        }

        [TestCase("hello ", "hello")]
        [TestCase("helloo", "hello")]
        [TestCase("hello", "hello")]
        public void ReadString_Success(string input, string str)
        {
            var matcher = new PatternMatcher(input);

            var success = matcher.ReadString(str);

            Assert.AreEqual(true, success);
            Assert.AreEqual(str.Length, matcher.Cursor);
        }


        [TestCase("1", 1, 1)]
        [TestCase("1 ", 1, 1)]
        [TestCase("123456789", 123456789, 9)]
        [TestCase("123456789 ", 123456789, 9)]
        public void ReadInt32_Success(string input, int expectedResult, int expectedCursor)
        {
            var matcher = new PatternMatcher(input);

            var success = matcher.ReadInt32(out var actualResult);

            Assert.AreEqual(true, success);
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedCursor, matcher.Cursor);
        }

        [TestCase("")]
        [TestCase("a ")]
        [TestCase("\0")]
        [TestCase("9147483647")]
        public void ReadInt32_Fail(string input)
        {
            var matcher = new PatternMatcher(input);

            var success = matcher.ReadInt32(out var actualResult);

            Assert.AreEqual(false, success);
            Assert.AreEqual(0, matcher.Cursor);
        }

        [TestCase("1", 1, 1)]
        [TestCase("1 ", 1, 1)]
        [TestCase("123456789123456789", 123456789123456789L, 18)]
        [TestCase("123456789 ", 123456789L, 9)]
        public void ReadInt64_Success(string input, long expectedResult, int expectedCursor)
        {
            var matcher = new PatternMatcher(input);

            var success = matcher.ReadInt64(out var actualResult);

            Assert.AreEqual(true, success);
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedCursor, matcher.Cursor);
        }

        [TestCase("")]
        [TestCase("a ")]
        [TestCase("\0")]
        [TestCase("9999999999999999999")]
        public void ReadInt64_Fail(string input)
        {
            var matcher = new PatternMatcher(input);

            var success = matcher.ReadInt64(out var actualResult);

            Assert.AreEqual(false, success);
            Assert.AreEqual(0, matcher.Cursor);
        }
    }
}
