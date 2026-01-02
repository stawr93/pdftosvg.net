// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using PdfToSvg.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfToSvg.Cli.Tests
{
    [Parallelizable(ParallelScope.Children)]
    public class ConversionTests
    {
        private static string GetNativeExePath()
        {
            var assemblyPath = typeof(ConversionTests).Assembly.Location;
            var binDir = Path.GetDirectoryName(assemblyPath);
            if (binDir == null)
            {
                throw new Exception("Native EXE path not found");
            }

            return Path.Combine(binDir, "pdftosvg.exe");
        }

        private static string GetInputFilePath(string fileName)
        {
            return Path.Combine(TestFiles.InputDirectory, fileName);
        }

        private static string GetExpectedFilePath(string fileName)
        {
            return Path.Combine(TestFiles.ExpectedDirectory, Path.ChangeExtension(fileName, ".svg"));
        }

        private static string GetActualFilePath(string fileName)
        {
            return Path.Combine(TestFiles.OutputDirectory, Path.ChangeExtension(fileName, ".svg"));
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private int RunCommand(string fileName, string arguments)
        {
            static void RedirectOutput(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                {
                    Console.WriteLine(e.Data);
                }
            }

            using var process = new Process();

            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += RedirectOutput;
            process.OutputDataReceived += RedirectOutput;

            Console.WriteLine("Command line:");
            Console.WriteLine(process.StartInfo.Arguments);
            Console.WriteLine();

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            Console.WriteLine();
            Console.WriteLine("Command exited with code " + process.ExitCode);
            Console.WriteLine();

            return process.ExitCode;
        }

        private void Convert(string pdfName, string expectedSvgName, string conversionOptions)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                Assert.Inconclusive("Not supported platform");
                return;
            }

            var expectedSvgPath = GetExpectedFilePath(expectedSvgName);
            var actualSvgPath = GetActualFilePath(expectedSvgName);
            var outputPath = Path.ChangeExtension(actualSvgPath, null) + ".svg";
            var outputPathPage1 = Path.ChangeExtension(actualSvgPath, null) + "-1.svg";
            var pdfPath = GetInputFilePath(pdfName);

            Directory.CreateDirectory(Path.GetDirectoryName(actualSvgPath));
            DeleteIfExists(outputPath);
            DeleteIfExists(outputPathPage1);

            var exitCode = RunCommand(
                fileName: GetNativeExePath(),
                arguments: $"\"{pdfPath}\" \"{actualSvgPath}\" --pages 1 {conversionOptions}"
                );
            Assert.AreEqual(0, exitCode, "Process exit code");

            var expected = File.Exists(expectedSvgPath)
                ? PngTestUtils.RecompressPngsInSvg(File.ReadAllText(expectedSvgPath, Encoding.UTF8))
                : null;

            var actual = File.ReadAllText(outputPathPage1, Encoding.UTF8);
            File.Delete(outputPathPage1);

            actual = PngTestUtils.RecompressPngsInSvg(actual);
            File.WriteAllText(outputPath, actual, Encoding.UTF8);
            
            Assert.AreEqual(expected, actual);
        }

        [TestCaseSource(nameof(TestCases))]
        public void Convert(string fileName)
        {
            Convert(fileName, fileName, "--include-fonts=none");
        }

        [TestCaseSource(nameof(FontTestCases))]
        public void ConvertEmbedded(string fileName)
        {
            Convert(fileName, "embedded-" + fileName, "--include-fonts=embed-opentype --external-fonts-dir \"" + TestFiles.ExternalFontsDirectory + "\"");
        }

        [Test]
        public void ExcludeHiddenText()
        {
            Convert("text-rendering-mode.pdf", "text-rendering-mode-without-hidden-text.svg", "--include-fonts=none --include-hidden-text=false");
        }

        [Test]
        public void ExcludeAnnotations()
        {
            Convert("annotation-markup.pdf", "annotation-markup-notexported.svg", "--include-fonts=none --include-annotations=false");
        }

        public static List<TestCaseData> TestCases
        {
            get
            {
                return Directory
                    .EnumerateFiles(Path.Combine(TestFiles.InputDirectory), "*.pdf")
                    .Select(path => new TestCaseData(Path.GetFileName(path)))
                    .ToList();
            }
        }

        public static List<TestCaseData> FontTestCases
        {
            get
            {
                return Directory
                    .EnumerateFiles(Path.Combine(TestFiles.InputDirectory), "fonts-*.pdf")
                    .Select(path => new TestCaseData(Path.GetFileName(path)))
                    .ToList();
            }
        }

        public static List<TestCaseData> ExternalFontTestCases
        {
            get
            {
                return Directory
                    .EnumerateFiles(Path.Combine(TestFiles.InputDirectory), "fonts-external-*.pdf")
                    .Select(path => new TestCaseData(Path.GetFileName(path)))
                    .ToList();
            }
        }
    }
}
