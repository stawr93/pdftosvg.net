// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using PdfToSvg.Fonts.OpenType;
using PdfToSvg.Fonts.OpenType.Enums;
using PdfToSvg.Fonts.OpenType.Tables;
using PdfToSvg.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToSvg.Tests.Fonts.OpenType
{
    public class OpenTypeFontTests
    {
        private string TestFilePath => Path.Combine(TestContext.CurrentContext.TestDirectory, "Fonts", "TestFiles", "symbol.ttf");

        [Test]
        public void Parse_Name_StringContent()
        {
            var bytes = File.ReadAllBytes(TestFilePath);
            var font = OpenTypeFont.Parse(bytes);
            var nameTable = font.Tables.Get<NameTable>();

            var windowsName = nameTable.NameRecords
                .FirstOrDefault(x => x.PlatformID == OpenTypePlatformID.Windows && x.NameID == OpenTypeNameID.FontFamily);

            var macName = nameTable.NameRecords
                .FirstOrDefault(x => x.PlatformID == OpenTypePlatformID.Macintosh && x.NameID == OpenTypeNameID.FontFamily);

            Assert.AreEqual("Untitled1", windowsName.StringContent);
            Assert.AreEqual("Untitled1", macName.StringContent);
        }

        [Test]
        public void ParseNames()
        {
            using var stream = File.OpenRead(TestFilePath);
            var names = OpenTypeFont.ParseNames(stream);

            Assert.AreEqual("Untitled1", names.FontFamily);
            Assert.AreEqual("Untitled1", names.PostScriptName);
        }

#if !NET40
        [Test]
        public async Task ParseNamesAsync()
        {
            using var stream = File.OpenRead(TestFilePath);
            var names = await OpenTypeFont.ParseNamesAsync(stream);

            Assert.AreEqual("Untitled1", names.FontFamily);
            Assert.AreEqual("Untitled1", names.PostScriptName);
        }
#endif
    }
}
