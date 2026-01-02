// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Parser = PdfToSvg.Fonts.CharStrings.CharStringParser;

namespace PdfToSvg.Fonts.CharStrings
{
    /// <summary>
    /// Holds information about a char string operator.
    /// </summary>
    internal class CharStringOperator
    {
        private readonly Action<Parser> implementation;

        public CharStringOperator(Action<Parser> implementation,
            CharStringOperatorOptions options = CharStringOperatorOptions.None)
        {
            this.implementation = implementation;
            ClearStack = options == CharStringOperatorOptions.ClearStack;
        }

        /// <summary>
        /// Specifies whether the operator is clearing the stack. Used for detecting the leading advance width in char
        /// strings.
        /// </summary>
        public bool ClearStack { get; }

        /// <summary>
        /// Invokes the operator.
        /// </summary>
        [DebuggerStepThrough]
        public void Invoke(Parser parser) => implementation.Invoke(parser);
    }
}
