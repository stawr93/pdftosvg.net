// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using PdfToSvg;
using PdfToSvg.DocumentModel;
using PdfToSvg.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToSvg.Tests.Drawing
{
    [OperationTarget]
    internal partial class Target
    {
        public int AllTypesCalls;
        public int Overloaded1Calls;
        public int Overloaded2Calls;
        public int ThreadingSyncCalls;
        public int ThreadingAsyncCalls = 0;
        public int OptionalCalls;
        public int ParamsCalls;
        public int VariadicCalls;
        public int EmptyVariadicCalls;
        public int EmptyParamsCalls;

        [Operation("AllTypes")]
        private void AllTypes(
            string str1,
            string str2,
            PdfName name,
            PdfDictionary dictionary,
            PdfRef reference,
            double dbl1,
            double dbl2,
            float flt1,
            float flt2,
            int int1,
            int int2,
            object[] objs,
            int[] ints,
            bool boolean)
        {
            AllTypesCalls++;
            Assert.AreEqual("abc", str1);
            Assert.AreEqual("def", str2);

            Assert.AreEqual("name", name.Value);
            Assert.IsNotNull(dictionary);

            Assert.AreEqual(142, reference.Id.ObjectNumber);
            Assert.AreEqual(42.0, dbl1);
            Assert.AreEqual(42, dbl2);
            Assert.AreEqual(42.0f, flt1);
            Assert.AreEqual(42, flt2);
            Assert.AreEqual(77, int1);
            Assert.AreEqual(88, int2);
            Assert.AreEqual(new object[] { 1, "2", 3 }, objs);
            Assert.AreEqual(new int[] { 1, 2, 3 }, ints);
            Assert.AreEqual(true, boolean);
        }

        [Operation("Params")]
        private void Params(string str, params float[] values)
        {
            ParamsCalls++;
            Assert.AreEqual("abc1", str);
            Assert.AreEqual(new[] { 2f, 42f, 349f }, values);
        }

        [Operation("Variadic")]
        private void Variadic([VariadicParam] float[] floats, [VariadicParam] string[] strings)
        {
            VariadicCalls++;
            Assert.AreEqual(new[] { 2f, 42f, 349f }, floats);
            Assert.AreEqual(new[] { "abc" }, strings);
        }

        [Operation("EmptyVariadic")]
        private void EmptyVariadic([VariadicParam] float[] floats, [VariadicParam] string[] strings)
        {
            EmptyVariadicCalls++;
            Assert.AreEqual(new float[] { }, floats);
            Assert.AreEqual(new[] { "abc" }, strings);
        }

        [Operation("EmptyParams")]
        private void EmptyParams(string str = null, params float[] values)
        {
            EmptyParamsCalls++;
            Assert.AreEqual(null, str);
            Assert.AreEqual(new float[0], values);
        }

        [Operation("Overloaded")]
        private void Overloaded1(
            PdfName name)
        {
            Overloaded1Calls++;
            Assert.AreEqual("abc1", name.Value);
        }

        [Operation("Overloaded")]
        private void Overloaded2(
            string name)
        {
            Overloaded2Calls++;
            Assert.AreEqual("abc2", name);
        }

        [Operation("Threading")]
        private void ThreadingSync()
        {
            ThreadingSyncCalls++;
        }

        [Operation("Optional")]
        private void Optional(
            string name = "abc",
            int number = 42,
            float fnumber = 42.1f)
        {
            OptionalCalls++;
            Assert.AreEqual("def", name);
            Assert.AreEqual(42, number);
            Assert.AreEqual(fnumber, 42.1f);
        }

        public bool Dispatch(string name, object[] args)
        {
            return Proxy.Invoke(this, name, args);
        }

#if !NET40
        [Operation("Threading")]
        private Task ThreadingAsync()
        {
            ThreadingAsyncCalls++;

            var tsc = new TaskCompletionSource<bool>();
            tsc.SetResult(true);
            return tsc.Task;
        }

        public Task<bool> DispatchAsync(string name, object[] args)
        {
            return Proxy.InvokeAsync(this, name, args);
        }
#endif
    }

    public class OperationDispatcherTests
    {
        [Test]
        public void TypeConversions()
        {
            var target = new Target();

            Assert.IsTrue(target.Dispatch("AllTypes", new object[] {
                "abc",
                "def",
                PdfName.Create("name"),
                new PdfDictionary(),
                new PdfRef(142, 0),
                42.0,
                42,
                42.0,
                42,
                77,
                88.0,
                new object[] { 1, "2", 3 },
                new object[] { 1, 2, 3 },
                true
            }));
            Assert.AreEqual(1, target.AllTypesCalls, nameof(target.AllTypesCalls));
        }

        [Test]
        public void Overloaded()
        {
            var target = new Target();

            Assert.IsTrue(target.Dispatch("Overloaded", new object[] { PdfName.Create("abc1") }));
            Assert.AreEqual(1, target.Overloaded1Calls, nameof(target.Overloaded1Calls));

            Assert.IsTrue(target.Dispatch("Overloaded", new object[] { "abc2" }));
            Assert.AreEqual(1, target.Overloaded2Calls, nameof(target.Overloaded2Calls));
        }

        [Test]
        public void Optional()
        {
            var target = new Target();

            Assert.IsTrue(target.Dispatch("Optional", new object[] { "def" }));
            Assert.AreEqual(1, target.OptionalCalls, nameof(target.OptionalCalls));
        }

        [Test]
        public void Params()
        {
            var target = new Target();

            Assert.IsTrue(target.Dispatch("Params", new object[] { "abc1", 2, 42.0, 349 }));
            Assert.AreEqual(1, target.ParamsCalls, nameof(target.ParamsCalls));

            Assert.IsTrue(target.Dispatch("EmptyParams", new object[0]));
            Assert.AreEqual(1, target.EmptyParamsCalls, nameof(target.EmptyParamsCalls));
        }

        [Test]
        public void Variadic()
        {
            var target = new Target();

            Assert.IsTrue(target.Dispatch("Variadic", new object[] { 2, 42.0, 349, "abc" }));
            Assert.AreEqual(1, target.VariadicCalls, nameof(target.VariadicCalls));

            Assert.IsTrue(target.Dispatch("EmptyVariadic", new object[] { "abc" }));
            Assert.AreEqual(1, target.EmptyVariadicCalls, nameof(target.EmptyVariadicCalls));
        }

        [Test]
        public void Threading_Sync()
        {
            var target = new Target();

            Assert.IsTrue(target.Dispatch("Threading", new object[0]));
            Assert.AreEqual(1, target.ThreadingSyncCalls, nameof(target.ThreadingSyncCalls));
            Assert.AreEqual(0, target.ThreadingAsyncCalls, nameof(target.ThreadingAsyncCalls));
        }

#if !NET40
        [Test]
        public async Task Threading_Async()
        {
            var target = new Target();

            Assert.IsTrue(await target.DispatchAsync("Threading", new object[0]));
            Assert.AreEqual(0, target.ThreadingSyncCalls, nameof(target.ThreadingSyncCalls));
            Assert.AreEqual(1, target.ThreadingAsyncCalls, nameof(target.ThreadingAsyncCalls));
        }
#endif
    }
}
