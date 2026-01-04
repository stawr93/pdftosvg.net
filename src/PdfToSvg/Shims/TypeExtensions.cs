// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace System.Reflection
{
    internal static class TypeExtensions
    {
#if NET40
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#endif
    }
}
