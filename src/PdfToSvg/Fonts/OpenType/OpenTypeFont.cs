// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using PdfToSvg.Common;
using PdfToSvg.Fonts.CompactFonts;
using PdfToSvg.Fonts.OpenType.Conversion;
using PdfToSvg.Fonts.OpenType.Tables;
using PdfToSvg.Fonts.OpenType.Utils;
using PdfToSvg.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PdfToSvg.Fonts.OpenType
{
    /// <summary>
    /// Parses .ttf and .otf files.
    /// </summary>
    /// <remarks>
    /// Specification:
    /// https://docs.microsoft.com/en-us/typography/opentype/spec/otff
    /// </remarks>
    [DebuggerDisplay("{Names.FullFontName}")]
    internal class OpenTypeFont
    {
        private readonly List<IBaseTable> tables;
        private IList<OpenTypeCMap>? cmaps;

        public OpenTypeFont()
        {
            tables = new List<IBaseTable>();
            Names = new OpenTypeNames(tables);
        }

        public ICollection<IBaseTable> Tables => tables;

        public int NumGlyphs => tables.Get<MaxpTable>()?.NumGlyphs ?? 0;

        public OpenTypeNames Names { get; }

        public IList<OpenTypeCMap> CMaps
        {
            get
            {
                if (cmaps == null)
                {
                    foreach (var table in tables)
                    {
                        if (table is CMapTable cmap)
                        {
                            cmaps = cmap.EncodingRecords
                                .Select(encoding => OpenTypeCMapDecoder.GetCMap(encoding))
                                .WhereNotNull()
                                .ToList();
                        }
                    }
                }

                return cmaps ?? ArrayUtils.Empty<OpenTypeCMap>();
            }
        }

        public static OpenTypeFont Parse(byte[] data)
        {
            var directory = TableDirectory.Read(data);

            var font = new OpenTypeFont();
            font.tables.AddRange(directory.Tables);
            return font;
        }

        /// <summary>
        /// Parses only the 'name' table from an OpenType font.
        /// </summary>
        public static OpenTypeNames ParseNames(Stream stream, CancellationToken cancellationToken = default)
        {
            var directory = TableDirectory.Read(stream, tag => tag == "name", cancellationToken);
            return new OpenTypeNames(directory.Tables);
        }

#if HAVE_ASYNC
        /// <summary>
        /// Parses only the 'name' table from an OpenType font.
        /// </summary>
        public static async Task<OpenTypeNames> ParseNamesAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var directory = await TableDirectory.ReadAsync(stream, tag => tag == "name", cancellationToken).ConfigureAwait(false);
            return new OpenTypeNames(directory.Tables);
        }
#endif

        public byte[] ToByteArray()
        {
            var writer = new OpenTypeWriter();

            var directory = new TableDirectory { Tables = tables.ToArray() };
            directory.Write(writer);

            return writer.ToArray();
        }
    }
}
