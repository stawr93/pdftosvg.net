// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using PdfToSvg.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PdfToSvg.Drawing
{
    internal static class OperationArgument
    {
        public delegate bool TryCastArgumentFunc<T>(object?[] args, ref int cursor, out T result);

        public static bool TryCastVariadicArgument<T>(object?[] args, ref int cursor, TryCastArgumentFunc<T> tryCastValue, out T[] matched)
        {
            matched = new T[args.Length];
            var resultIndex = 0;

            while (cursor < args.Length)
            {
                if (tryCastValue(args, ref cursor, out var r))
                {
                    matched[resultIndex++] = r;
                }
                else
                {
                    break;
                }
            }

            Array.Resize(ref matched, resultIndex);
            return true;
        }

        public static TryCastArgumentFunc<T[]> TryCastArrayArgument<T>(TryCastArgumentFunc<T> tryCastValue)
        {
            return (object?[] arr, ref int cursor, out T[] result) =>
            {
                if (cursor < arr.Length && arr[cursor] is object?[] sourceArray)
                {
                    var inputIndex = 0;
                    var resultIndex = 0;

                    result = new T[sourceArray.Length];

                    while (resultIndex < result.Length && tryCastValue(sourceArray, ref inputIndex, out var castedItem))
                    {
                        result[resultIndex++] = castedItem;
                    }

                    if (resultIndex == result.Length)
                    {
                        cursor++;
                        return true;
                    }
                }
                
                result = ArrayUtils.Empty<T>();
                return false;
            };
        }

        [MethodImpl(MethodInliningOptions.AggressiveInlining)]
        public static bool TryCastFloatArgument(object?[] arr, ref int cursor, out float result)
        {
            if (cursor < arr.Length)
            {
                var value = arr[cursor];

                if (value is double)
                {
                    result = (float)(double)value;
                    cursor++;
                    return true;
                }
                else if (value is int)
                {
                    result = (int)value;
                    cursor++;
                    return true;
                }
            }
            
            result = 0;
            return false;
        }

        [MethodImpl(MethodInliningOptions.AggressiveInlining)]
        public static bool TryCastIntArgument(object?[] arr, ref int cursor, out int result)
        {
            if (cursor < arr.Length)
            {
                var value = arr[cursor];

                if (value is int)
                {
                    result = (int)value;
                    cursor++;
                    return true;
                }
                else if (value is double)
                {
                    result = (int)(double)value;
                    cursor++;
                    return true;
                }
            }

            result = 0;
            return false;
        }

        [MethodImpl(MethodInliningOptions.AggressiveInlining)]
        public static bool TryCastDoubleArgument(object?[] arr, ref int cursor, out double result)
        {
            if (cursor < arr.Length)
            {
                var value = arr[cursor];
                if (value is double)
                {
                    result = (double)value;
                    cursor++;
                    return true;
                }
                else if (value is int)
                {
                    result = (int)value;
                    cursor++;
                    return true;
                }
            }

            result = 0;
            return false;
        }

        public static bool TryCastArgument<T>(object?[] arr, ref int cursor, out T result)
        {
            if (cursor < arr.Length)
            {
                var value = arr[cursor];
                if (value is T)
                {
                    result = (T)value;
                    cursor++;
                    return true;
                }
            }

            result = default!;
            return false;
        }

        public static bool TryCastOptionalArgument<T>(object?[] arr, ref int cursor, T defaultValue, TryCastArgumentFunc<T> tryCastValue, out T result)
        {
            if (cursor < arr.Length)
            {
                if (tryCastValue(arr, ref cursor, out result))
                {
                    return true;
                }
                else
                {
                    result = defaultValue;
                    return false;
                }
            }

            result = defaultValue;
            return true;
        }
    }
}
