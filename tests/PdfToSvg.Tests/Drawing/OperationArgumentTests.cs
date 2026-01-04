// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using PdfToSvg.DocumentModel;
using PdfToSvg.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfToSvg.Tests.Drawing
{
    [Parallelizable(ParallelScope.Children)]
    public class OperationArgumentTests
    {
        #region TryCastVariadicArgument

        [Test]
        public void TryCastVariadicArgument_Success()
        {
            var args = new object[] { 1, 2, 3, "hello" };
            var cursor = 1;

            Assert.IsTrue(OperationArgument.TryCastVariadicArgument<int>(args, ref cursor, OperationArgument.TryCastIntArgument, out var result));
            Assert.AreEqual(new int[] { 2, 3 }, result);
            Assert.AreEqual(3, cursor);
        }

        [Test]
        public void TryCastVariadicArgument_NoValid()
        {
            var args = new object[] { "hello" };
            var cursor = 0;

            Assert.IsTrue(OperationArgument.TryCastVariadicArgument<int>(args, ref cursor, OperationArgument.TryCastIntArgument, out var result));
            Assert.AreEqual(new int[] { }, result);
            Assert.AreEqual(0, cursor);
        }

        [Test]
        public void TryCastVariadicArgument_Last()
        {
            var args = new object[] { };
            var cursor = 0;

            Assert.IsFalse(OperationArgument.TryCastArrayArgument<int>(OperationArgument.TryCastIntArgument)(args, ref cursor, out var result));
            Assert.AreEqual(new int[] { }, result);
            Assert.AreEqual(0, cursor);
        }

        #endregion

        #region TryCastArrayArgument

        [Test]
        public void TryCastArrayArgument_Success()
        {
            var args = new object[] { 0, new object[] { 1, 2, 3 }, 4 };
            var cursor = 1;

            Assert.IsTrue(OperationArgument.TryCastArrayArgument<int>(OperationArgument.TryCastIntArgument)(args, ref cursor, out var result));
            Assert.AreEqual(new int[] { 1, 2, 3 }, result);
            Assert.AreEqual(2, cursor);
        }

        [Test]
        public void TryCastArrayArgument_NotSpecified()
        {
            var args = new object[] { 1, 2, 3 };
            var cursor = 4;

            Assert.IsFalse(OperationArgument.TryCastArrayArgument<int>(OperationArgument.TryCastIntArgument)(args, ref cursor, out var result));
            Assert.AreEqual(4, cursor);
        }

        [Test]
        public void TryCastArrayArgument_Invalid()
        {
            var args = new object[] { "hello" };
            var cursor = 0;

            Assert.IsFalse(OperationArgument.TryCastArrayArgument<int>(OperationArgument.TryCastIntArgument)(args, ref cursor, out var result));
            Assert.AreEqual(0, cursor);
        }

        [Test]
        public void TryCastArrayArgument_InvalidElement()
        {
            var args = new object[] { 0, new object[] { 1, 2, "hello" }, 4 };
            var cursor = 0;

            Assert.IsFalse(OperationArgument.TryCastArrayArgument<int>(OperationArgument.TryCastIntArgument)(args, ref cursor, out var result));
            Assert.AreEqual(0, cursor);
        }

        #endregion

        #region TryCastIntArgument

        [TestCase(42, 42f)]
        [TestCase(42.2d, 42.2f)]
        public void TryCastFloatArgument_Success(object value, float expectedResult)
        {
            var args = new object[] { value };
            var cursor = 0;

            Assert.IsTrue(OperationArgument.TryCastFloatArgument(args, ref cursor, out var result));
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(1, cursor);
        }

        [Test]
        public void TryCastFloatArgument_NotSpecified()
        {
            var args = new object[] { 1, 2, 3 };
            var cursor = 4;

            Assert.IsFalse(OperationArgument.TryCastFloatArgument(args, ref cursor, out var result));
            Assert.AreEqual(4, cursor);
        }

        [Test]
        public void TryCastFloatArgument_Invalid()
        {
            var args = new object[] { "hello" };
            var cursor = 0;

            Assert.IsFalse(OperationArgument.TryCastFloatArgument(args, ref cursor, out var result));
            Assert.AreEqual(0, cursor);
        }

        #endregion

        #region TryCastIntArgument

        [TestCase(42, 42)]
        [TestCase(42.2d, 42)]
        public void TryCastIntArgument_Success(object value, int expectedResult)
        {
            var args = new object[] { value };
            var cursor = 0;

            Assert.IsTrue(OperationArgument.TryCastIntArgument(args, ref cursor, out var result));
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(1, cursor);
        }

        [Test]
        public void TryCastIntArgument_NotSpecified()
        {
            var args = new object[] { 1, 2, 3 };
            var cursor = 4;

            Assert.IsFalse(OperationArgument.TryCastIntArgument(args, ref cursor, out var result));
            Assert.AreEqual(4, cursor);
        }

        [Test]
        public void TryCastIntArgument_Invalid()
        {
            var args = new object[] { "hello" };
            var cursor = 0;

            Assert.IsFalse(OperationArgument.TryCastIntArgument(args, ref cursor, out var result));
            Assert.AreEqual(0, cursor);
        }

        #endregion

        #region TryCastDoubleArgument

        [TestCase(42, 42d)]
        [TestCase(42.2d, 42.2d)]
        public void TryCastDoubleArgument_Success(object value, double expectedResult)
        {
            var args = new object[] { value };
            var cursor = 0;

            Assert.IsTrue(OperationArgument.TryCastDoubleArgument(args, ref cursor, out var result));
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(1, cursor);
        }

        [Test]
        public void TryCastDoubleArgument_NotSpecified()
        {
            var args = new object[] { 1, 2, 3 };
            var cursor = 4;

            Assert.IsFalse(OperationArgument.TryCastDoubleArgument(args, ref cursor, out var result));
            Assert.AreEqual(4, cursor);
        }

        [Test]
        public void TryCastDoubleArgument_Invalid()
        {
            var args = new object[] { "hello" };
            var cursor = 0;

            Assert.IsFalse(OperationArgument.TryCastDoubleArgument(args, ref cursor, out var result));
            Assert.AreEqual(0, cursor);
        }

        #endregion

        #region TryCastArgument

        [Test]
        public void TryCastArgument_Success()
        {
            var args = new object[] { "a" };
            var cursor = 0;

            Assert.IsTrue(OperationArgument.TryCastArgument<string>(args, ref cursor, out var result));
            Assert.AreEqual("a", result);
            Assert.AreEqual(1, cursor);
        }

        [Test]
        public void TryCastArgument_NotSpecified()
        {
            var args = new object[] { 1, 2, 3 };
            var cursor = 4;

            Assert.IsFalse(OperationArgument.TryCastArgument<PdfName>(args, ref cursor, out var result));
            Assert.AreEqual(4, cursor);
        }

        [Test]
        public void TryCastArgument_Invalid()
        {
            var args = new object[] { "hello" };
            var cursor = 0;

            Assert.IsFalse(OperationArgument.TryCastArgument<PdfName>(args, ref cursor, out var result));
            Assert.AreEqual(0, cursor);
        }

        #endregion

        #region TryCastOptionalArgument

        [Test]
        public void TryCastOptionalArgument_Success()
        {
            var args = new object[] { 1, 2, 3 };
            var cursor = 0;

            Assert.IsTrue(OperationArgument.TryCastOptionalArgument(args, ref cursor, 42, OperationArgument.TryCastIntArgument, out var result));
            Assert.AreEqual(1, result);
            Assert.AreEqual(1, cursor);
        }

        [Test]
        public void TryCastOptionalArgument_Last()
        {
            var args = new object[] { 1, 2, 3 };
            var cursor = 2;

            Assert.IsTrue(OperationArgument.TryCastOptionalArgument(args, ref cursor, 42, OperationArgument.TryCastIntArgument, out var result));
            Assert.AreEqual(3, result);
            Assert.AreEqual(3, cursor);
        }

        [Test]
        public void TryCastOptionalArgument_NotSpecified()
        {
            var args = new object[] { 1, 2, 3 };
            var cursor = 4;

            Assert.IsTrue(OperationArgument.TryCastOptionalArgument(args, ref cursor, 42, OperationArgument.TryCastIntArgument, out var result));
            Assert.AreEqual(42, result);
            Assert.AreEqual(4, cursor);
        }

        [Test]
        public void TryCastOptionalArgument_Invalid()
        {
            var args = new object[] { "hello" };
            var cursor = 0;

            Assert.IsFalse(OperationArgument.TryCastOptionalArgument(args, ref cursor, 42, OperationArgument.TryCastIntArgument, out var result));
            Assert.AreEqual(0, cursor);
        }

        #endregion
    }
}
