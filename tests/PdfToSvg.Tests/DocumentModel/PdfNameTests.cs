// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using PdfToSvg.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfToSvg.Tests.DocumentModel
{
    public class PdfNameTests
    {
        [Test]
        public void Create_KnownNameIsInterned()
        {
            var name = PdfName.Create("Root");
            Assert.AreEqual("Root", name.Value);
            Assert.AreSame(Names.Root, name);
        }

        [Test]
        public void Create_UnknownName()
        {
            var name = PdfName.Create("__MyCustomName__");
            Assert.AreEqual("__MyCustomName__", name.Value);
        }
    }
}
