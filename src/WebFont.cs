﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToSvg
{
    public class WebFont : Font
    {
        public WebFont(string fontFamily, string? woffUrl = null, string? woff2Url = null, string? trueTypeUrl = null)
        {
            FontFamily = fontFamily;

            WoffUrl = woffUrl;
            Woff2Url = woff2Url;
            TrueTypeUrl = trueTypeUrl;
        }

        public override string FontFamily { get; }

        public string? WoffUrl { get; }

        public string? Woff2Url { get; }

        public string? TrueTypeUrl { get; }
    }
}