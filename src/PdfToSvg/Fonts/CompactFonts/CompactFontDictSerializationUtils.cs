// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfToSvg.Fonts.CompactFonts
{
    internal static class CompactFontDictSerializationUtils
    {
        public static int[] ConvertDoubleArrayToIntArray(double[] source)
        {
            var result = new int[source.Length];

            for (var i = 0; i < source.Length; i++)
            {
                result[i] = (int)source[i];
            }

            return result;
        }

        public static double[] ConvertIntArrayToDoubleArray(int[] source)
        {
            var result = new double[source.Length];

            for (var i = 0; i < source.Length; i++)
            {
                result[i] = source[i];
            }

            return result;
        }

        public static bool AreEqual<T>(T[] a, T[] b)
        {
            if (a == null) return b == null;
            if (b == null) return false;
            if (a.Length != b.Length) return false;

            var comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < a.Length; i++)
            {
                if (!comparer.Equals(a[i], b[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
