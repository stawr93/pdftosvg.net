// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using System;
using System.Globalization;

namespace PdfToSvg.Parsing
{
    internal struct PatternMatcher(string input, int startCursor = 0)
    {
        public int Cursor = startCursor;

        public bool EndOfInput => Cursor >= input.Length;

        public bool SkipChar(char ch, int min = 0, int max = int.MaxValue)
        {
            var peekCursor = Cursor;
            var maxCursor = peekCursor + max;

            while (peekCursor < maxCursor && peekCursor < input.Length && input[peekCursor] == ch)
            {
                peekCursor++;
            }

            if (peekCursor - Cursor < min)
            {
                return false;
            }
            else
            {
                Cursor = peekCursor;
                return true;
            }
        }

        public void SkipChars(string chars, int max)
        {
            var maxCursor = Cursor + max;

            while (Cursor < maxCursor && Cursor < input.Length && chars.IndexOf(input[Cursor]) >= 0)
            {
                Cursor++;
            }
        }

        public bool ReadKeyword(string keyword)
        {
            if (Cursor + keyword.Length <= input.Length &&
                input.IndexOf(keyword, Cursor, keyword.Length, StringComparison.Ordinal) >= 0)
            {
                var peekCursor = Cursor + keyword.Length;
                if (peekCursor < input.Length)
                {
                    var nextChar = input[peekCursor];
                    if (nextChar >= 'a' && nextChar <= 'z' ||
                        nextChar >= 'A' && nextChar <= 'Z' ||
                        nextChar >= '0' && nextChar <= '9')
                    {
                        return false;
                    }
                }

                Cursor = peekCursor;
                return true;
            }
            
            return false;
        }

        public bool ReadString(string value)
        {
            if (Cursor + value.Length <= input.Length && 
                input.IndexOf(value, Cursor, value.Length, StringComparison.Ordinal) >= 0)
            {
                Cursor += value.Length;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ReadInt32(out int result)
        {
            const int MaxLength = 10;
            var peekCursor = Cursor;
            var maxPeekCursor = peekCursor + MaxLength;

            while (peekCursor < input.Length)
            {
                if (peekCursor > maxPeekCursor)
                {
                    result = 0;
                    return false;
                }

                var ch = input[peekCursor];
                if (ch < '0' || ch > '9')
                {
                    // End of number
                    break;
                }

                peekCursor++;
            }

            if (peekCursor > Cursor)
            {
                var numberString = input.Substring(Cursor, peekCursor - Cursor);

                if (int.TryParse(numberString, NumberStyles.None, CultureInfo.InvariantCulture, out result))
                {
                    Cursor = peekCursor;
                    return true;
                }
            }
            
            result = 0;
            return false;
        }

        public bool ReadInt64(out long result)
        {
            const int MaxLength = 19;
            var peekCursor = Cursor;
            var maxPeekCursor = peekCursor + MaxLength;

            while (peekCursor < input.Length)
            {
                if (peekCursor > maxPeekCursor)
                {
                    result = 0;
                    return false;
                }

                var ch = input[peekCursor];
                if (ch < '0' || ch > '9')
                {
                    // End of number
                    break;
                }

                peekCursor++;
            }

            if (peekCursor > Cursor)
            {
                var numberString = input.Substring(Cursor, peekCursor - Cursor);

                if (long.TryParse(numberString, NumberStyles.None, CultureInfo.InvariantCulture, out result))
                {
                    Cursor = peekCursor;
                    return true;
                }
            }

            result = 0;
            return false;
        }


        public override string ToString()
        {
            var contextBefore = Math.Min(20, Cursor);
            var contextAfter = Math.Min(60, input.Length - Cursor);
            return 
                (contextBefore == 0 ? "" : "... ") +
                input.Substring(Cursor - contextBefore, contextBefore) + 
                " \u2192" + 
                input.Substring(Cursor, contextAfter) +
                (contextAfter == 0 ? "" : " ...");
        }
    }
}
