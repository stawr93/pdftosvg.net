// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfToSvg.SourceGenerators.OperationProxy
{
    internal class ProxyMethod
    {
        public required bool IsAsync;
        public required string Operator;
        public required string ProxyMethodName;
        public required List<IMethodSymbol> TargetMethods;
    }
}
