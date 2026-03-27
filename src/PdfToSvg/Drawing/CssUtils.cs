// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using PdfToSvg.Encodings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfToSvg.Drawing
{
    internal static class CssUtils
    {
        public static string EncodeString(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            // Spec:
            // https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Values/string
            // https://drafts.csswg.org/css-values/#strings
            // https://www.w3.org/TR/css-syntax/#consume-string-token

            var result = new StringBuilder(s.Length);
            result.Append('"');
            
            for (var i = 0; i < s.Length; )
            {
                var ch = s[i];

                if (ch == '"')
                {
                    result.Append("\\\"");
                    i++;
                }
                else if (ch == '\\')
                {
                    result.Append("\\\\");
                    i++;
                }
                else if (ch >= '!' && ch <= '}' || ch == ' ')
                {
                    result.Append(ch);
                    i++;
                }
                else
                {
                    // Escape character

                    // CSS does not use UTF-16, so we need to decode code points
                    var codepoint = Utf16Encoding.DecodeCodePoint(s, i, out var codePointLength);

                    result.Append('\\');
                    result.Append(codepoint.ToString("x"));

                    i += codePointLength;

                    if (i < s.Length)
                    {
                        ch = s[i];

                        // If the next character is a whitespace, it will be consumed by the escaped character.
                        // https://www.w3.org/TR/css-syntax/#consume-escaped-code-point
                        //
                        // Also, if it is a hex digit, it can be misinterpreted as part of the escaped character.
                        // 
                        // To prevent this, terminate the escaped character with a space

                        if (// Whitespace character (\r, \n and \t ignored since they will also be escaped)
                            ch == ' ' ||

                            // Hex digits
                            ch >= 'a' && ch <= 'f' || ch >= 'A' && ch <= 'F' || ch >= '0' && ch <= '9')
                        {
                            result.Append(' ');
                        }
                    }
                }
            }

            result.Append('"');
            return result.ToString();
        }
    }
}
