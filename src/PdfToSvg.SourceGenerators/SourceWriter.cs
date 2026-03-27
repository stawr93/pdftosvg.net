// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace PdfToSvg.SourceGenerators
{
    internal class SourceWriter
    {
        private class Block(SourceWriter writer, string suffix) : IDisposable
        {
            public void Dispose()
            {
                writer.indentation--;
                writer.WriteLine("}" + suffix);
            }
        }

        private StringBuilder sb = new StringBuilder();
        private int indentation = 0;

        public IDisposable BeginBlock(string suffix = "")
        {
            WriteLine("{");
            indentation++;
            return new Block(this, suffix);
        }

        public void WriteLine(string line = "")
        {
            sb.Append(new string(' ', indentation * 4));
            sb.AppendLine(line);
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
