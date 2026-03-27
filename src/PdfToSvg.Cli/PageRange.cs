// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PdfToSvg.Cli
{
    internal class PageRange
    {
        public PageRange(int from, int to)
        {
            From = from;
            To = to;
        }

        public int From { get; }
        public int To { get; }

        public static bool TryParse(string input, out IList<PageRange> result)
        {
            result = new List<PageRange>();

            foreach (var part in input.Split(','))
            {
                var trimmed = part.Trim();
                if (trimmed.Length > 0)
                {
                    var rangeParts = trimmed.Split([".."], count: 2, StringSplitOptions.None);

                    if (rangeParts.Length == 0)
                    {
                        return false;
                    }

                    // FROM
                    var from = -1;
                    if (rangeParts[0].Length > 0)
                    {
                        if (!int.TryParse(rangeParts[0], NumberStyles.None, CultureInfo.InvariantCulture, out from))
                        {
                            return false;
                        }
                    }

                    if (rangeParts.Length == 1)
                    {
                        if (from < 0)
                        {
                            return false;
                        }
                        else
                        {
                            result.Add(new PageRange(from, from));
                        }
                    }
                    else
                    {
                        // TO
                        var to = -1;
                        if (rangeParts[1].Length > 0)
                        {
                            if (!int.TryParse(rangeParts[1], NumberStyles.None, CultureInfo.InvariantCulture, out to))
                            {
                                return false;
                            }
                        }

                        if (from < 0 && to < 0)
                        {
                            return false;
                        }
                        else
                        {
                            result.Add(new PageRange(from, to));
                        }
                    }
                }
            }

            return result.Count > 0;
        }

        public override string ToString()
        {
            var format =
                From < 0 ? "..{1}" :
                To < 0 ? "{0}.." :
                From == To ? "{0}" :
                "{0}..{1}";

            return string.Format(CultureInfo.InvariantCulture, format, From, To);
        }
    }
}
