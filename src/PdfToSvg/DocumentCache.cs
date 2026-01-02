// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using PdfToSvg.DocumentModel;
using PdfToSvg.Fonts;
using PdfToSvg.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PdfToSvg
{
    internal class DocumentCache
    {
        private readonly 
            ConditionalWeakTable<FontResolver,
                ConditionalWeakTable<FontRepository,
                    Dictionary<PdfDictionary, SharedFactory<BaseFont>>
                >> fontCache = new();

        private static readonly FontRepository emptyFontRepository = new();

        public SharedFactory<BaseFont> GetFontFactory(PdfDictionary fontDict, FontResolver fontResolver, FontRepository fontRepository, Func<SharedFactory<BaseFont>> fontFactory)
        {
            // Shared cache for all empty font repositories
            if (fontRepository.Count == 0)
            {
                fontRepository = emptyFontRepository;
            }

            lock (fontCache)
            {
                if (!fontCache.TryGetValue(fontResolver, out var repositories))
                {
                    repositories = new();
                    fontCache.Add(fontResolver, repositories);
                }

                if (!repositories.TryGetValue(fontRepository, out var factories))
                {
                    factories = new();
                    repositories.Add(fontRepository, factories);
                }

                if (!factories.TryGetValue(fontDict, out var factory))
                {
                    factory = fontFactory();
                    factories[fontDict] = factory;
                }

                return factory;
            }
        }
    }
}
