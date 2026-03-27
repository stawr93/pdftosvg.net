// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfToSvg.SourceGenerators.CompactFontDictSerializer
{
    internal class DictProperty
    {
        public required string Name;
        public required ITypeSymbol Type;
        public bool IsNullable;
        public bool IsNullableValueType;
        public Location? Location;
        public int Operator;
        public int Order;
    }
}
