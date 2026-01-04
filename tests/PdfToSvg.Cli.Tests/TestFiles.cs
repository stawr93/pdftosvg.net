// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfToSvg.Cli.Tests
{
    internal static class TestFiles
    {
        private const string ProtectedTestFilesDirName = "Protected";
        private const string OwnTestFilesDirName = "Own";
        private const string InputDirName = "input";
        private const string ExternalFontsDirName = "external-fonts";
        private const string ExpectedDirName = "expected";

        private const string TestFilesDirName = "TestFiles";

#if NET45
        private const string ActualDirName = "actual-cli-net45";
#elif NET10_0
        private const string ActualDirName = "actual-cli-net10-nativeaot";
#endif

        static TestFiles()
        {
            var directory = TestContext.CurrentContext.WorkDirectory;

            for (var i = 0; i < 8 && !string.IsNullOrEmpty(directory); i++)
            {
                var potentialTestFileDirectory = Path.Combine(directory, TestFilesDirName);
                if (Directory.Exists(potentialTestFileDirectory))
                {
                    RootPath = directory;
                    break;
                }

                directory = Path.GetDirectoryName(directory);
            }

            if (RootPath == null)
            {
                throw new DirectoryNotFoundException("Could not find test files directory.");
            }
        }

        public static string RootPath { get; }

        public static string TestFilesPath => Path.Combine(RootPath, TestFilesDirName);

        public static string ProtectedInputDirectory => Path.Combine(TestFilesPath, ProtectedTestFilesDirName);

        public static string InputDirectory => Path.Combine(TestFilesPath, OwnTestFilesDirName, InputDirName);

        public static string ExternalFontsDirectory => Path.Combine(TestFilesPath, OwnTestFilesDirName, ExternalFontsDirName);

        public static string ExpectedDirectory => Path.Combine(TestFilesPath, OwnTestFilesDirName, ExpectedDirName);

        public static string OutputDirectory => Path.Combine(TestFilesPath, OwnTestFilesDirName, ActualDirName);
    }
}
