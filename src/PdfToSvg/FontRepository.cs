// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using PdfToSvg.Common;
using PdfToSvg.Fonts;
using PdfToSvg.Fonts.OpenType;
using PdfToSvg.Fonts.OpenType.Enums;
using PdfToSvg.Fonts.OpenType.Tables;
using PdfToSvg.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PdfToSvg
{
    /// <summary>
    /// Contains fonts to be used if a PDF document refers to a non-embedded font.
    /// </summary>
    /// <remarks>
    /// <note type="warning">
    ///     Before embedding external fonts in your SVG, carefully read the font license to ensure the font may be
    ///     redistributed as a web font. Fonts bundled with Windows may not be used in this way. See
    ///     <see href="https://learn.microsoft.com/en-us/typography/fonts/font-faq">Microsoft Font
    ///     redistribution FAQ</see>.
    /// </note>
    /// <para>
    ///     Fonts in PDF documents are used for multiple purposes:
    /// </para>
    /// <list type="bullet">
    ///     <item>
    ///         To define the appearance of text when it is rendered on the screen
    ///     </item>
    ///     <item>
    ///         To decode text to Unicode, to make it searchable and selectable
    ///     </item>
    /// </list>
    /// <para>
    ///     Most PDF documents have all fonts embedded. If not, a PDF viewer is typically checking if the font is
    ///     installed in the operating system. If a font still cannot be found, text will not have the correct
    ///     appearance, and will in some cases show up garbled or as a series of question marks, <c>������</c>.
    /// </para>
    /// <para>
    ///     PdfToSvg.NET will check its <see cref="FontRepository"/> for any missing font. When integrating the library,
    ///     you must register fonts manually in the <see cref="FontRepository"/> if you expect converted documents to
    ///     refer to non-embedded fonts.
    /// </para>
    /// <para>
    ///     The are two reasons why PdfToSvg.NET does not automatically use system fonts as many PDF viewers do:
    /// </para>
    /// <list type="bullet">
    /// <item>
    ///     <strong>For licensing reasons</strong>
    ///     <para>
    ///         Unlike in PDF viewers, the resulting SVG from PdfToSvg.NET will typically not be shown on the screen on
    ///         the machine creating it, meaning fonts must be distributed as embedded web fonts to a client.
    ///         Creators of installed system fonts might not allow redistribution.
    ///     </para>
    /// </item>
    /// <item>
    ///     <strong>To ensure deterministic conversions</strong>
    ///     <para>
    ///         The developer might test their solution successfully on their Windows machine, where the default
    ///         Microsoft fonts are available. When deploying the solution to a Unix like environment, text shows up
    ///         garbled or as a series of question marks, <c>������</c>, since the same system fonts are not available.
    ///     </para>
    /// </item>
    /// </list>
    /// <para>
    ///     If you need to provide non-embedded fonts, the recommended approach is to register a directory
    ///     containing the missing fonts and mark them as embeddable, if you are allowed to do so.
    ///     If you don't own the rights to embed the fonts, use <see cref="SystemFonts">FontRepository.SystemFonts</see>,
    ///     which will ensure text is extracted properly without embedding the fonts in the output SVG. If you use 
    ///     <see cref="SystemFonts">FontRepository.SystemFonts</see>, be aware the result might differ from machine to
    ///     machine.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    ///     The following code registers all system fonts as fallback if a PDF document refers to a font not
    ///     embedded in the document. The system fonts will only be used for supporting text extraction and will not
    ///     be embedded in the output SVG.
    /// </para>
    /// <code language="cs" title="Register system fonts">
    /// using (var document = PdfDocument.Open(input))
    /// {
    ///     var options = new SvgConversionOptions
    ///     {
    ///         FontRepository = FontRepository.SystemFonts,
    ///     };
    /// 
    ///     var pageIndex = 0;
    /// 
    ///     foreach (var page in document.Pages)
    ///     {
    ///         var svgFileName = Path.GetFileNameWithoutExtension(input) + "-" + pageIndex++ + ".svg";
    ///         page.SaveAsSvg(svgFileName, options);
    ///     }
    /// }
    /// </code>
    /// <para>
    ///     The following code registers a font directory to be checked if a PDF document refers to a font not embedded
    ///     in the document.
    /// </para>
    /// <code language="cs" title="Register a font directory">
    /// using (var document = PdfDocument.Open(input))
    /// {
    ///     var options = new SvgConversionOptions();
    ///     options.FontRepository.AddDirectory(@"D:\Fonts", allowEmbedding: true);
    /// 
    ///     var pageIndex = 0;
    /// 
    ///     foreach (var page in document.Pages)
    ///     {
    ///         var svgFileName = Path.GetFileNameWithoutExtension(input) + "-" + pageIndex++ + ".svg";
    ///         page.SaveAsSvg(svgFileName, options);
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class FontRepository
    {
        private static FontRepository? systemFonts;

        private readonly List<Registration> registrations = new();
        private Dictionary<string, CachedFont>? fontsByPostScriptName;

        private struct Registration
        {
            public string Path;
            public bool IsDirectory;
            public bool AllowEmbedding;

            public override string ToString()
            {
                return (IsDirectory ? "Directory: " : "Font: ") + Path;
            }
        }

        [DebuggerDisplay("{DebugView,nq}")]
        private struct CachedFont
        {
            public string PostScriptName;
            public string Path;
            public bool AllowEmbedding;

            public string DebugView => PostScriptName + " (" + Path + ")";
        }

        /// <summary>
        /// Gets a read-only font repository containing the fonts installed in the operating system fonts directory.
        /// The fonts will only be used during conversion and will not be embedded in the output SVG.
        /// </summary>
        /// <example>
        /// <para>
        ///     The following code registers all system fonts as fallback if a PDF document refers to a font not
        ///     embedded in the document. The system fonts will only be used for supporting text extraction and will not
        ///     be embedded in the output SVG.
        /// </para>
        /// <code language="cs">
        /// using (var document = PdfDocument.Open(input))
        /// {
        ///     var options = new SvgConversionOptions
        ///     {
        ///         FontRepository = FontRepository.SystemFonts,
        ///     };
        /// 
        ///     var pageIndex = 0;
        /// 
        ///     foreach (var page in document.Pages)
        ///     {
        ///         var svgFileName = Path.GetFileNameWithoutExtension(input) + "-" + pageIndex++ + ".svg";
        ///         page.SaveAsSvg(svgFileName, options);
        ///     }
        /// }
        /// </code>
        /// </example>
        public static FontRepository SystemFonts
        {
            get
            {
                if (systemFonts == null)
                {
                    var repository = new FontRepository();

                    try
                    {
                        repository.AddSystemFonts();
                    }
                    catch
                    {
                        // Leave the repository empty if we could not read the system directory
                    }

                    repository.IsReadOnly = true;

                    Interlocked.CompareExchange(ref systemFonts, value: repository, comparand: null);
                }

                return systemFonts;
            }
        }

        /// <summary>
        /// Determines whether this repository is readonly.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets the number of fonts in this repository.
        /// </summary>
        public int Count
        {
            get
            {
                lock (registrations)
                {
                    var cache = GetCache();
                    return cache.Count;
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private CachedFont[] Items
        {
            get
            {
                lock (registrations)
                {
                    return GetCache().Values.ToArray();
                }
            }
        }

        /// <summary>
        /// Registers a font that can be used as fallback if it is not embedded in the PDF document.
        /// </summary>
        /// <param name="path">
        ///     Path to the font file. OpenType (*.otf) and TrueType (*.ttf) fonts are supported.
        /// </param>
        /// <param name="allowEmbedding">
        ///     If <c>true</c>, the font can be embedded in the output SVG.
        ///     If <c>false</c>, the font is used for text extraction during conversion but not embedded in the output SVG.
        /// </param>
        /// <remarks>
        /// <note type="warning">
        ///     If setting <paramref name="allowEmbedding"/> to <c>true</c>, carefully read the font license to ensure
        ///     the font may be redistributed as a web font. Fonts bundled with Windows may not be used in this way. See
        ///     <see href="https://learn.microsoft.com/en-us/typography/fonts/font-faq">Microsoft Font
        ///     redistribution FAQ</see>.
        /// </note>
        /// <para>
        ///     Fonts from a font repository will not be embedded in the output SVG if:
        /// </para>
        /// <list type="bullet">
        ///     <item>
        ///         The font originates from <see cref="SystemFonts">FontRepository.SystemFonts</see>
        ///     </item>
        ///     <item>
        ///         The font was added by <see cref="AddSystemFonts"/>
        ///     </item>
        ///     <item>
        ///         The <paramref name="allowEmbedding"/> parameter is <c>false</c>
        ///     </item>
        ///     <item>
        ///         The font usage permission is "Restricted License embedding"
        ///     </item>
        ///     <item>
        ///         The font only allows bitmap embedding
        ///     </item>
        /// </list>
        /// <para>
        ///     If the font is not used for embedding, it is still used for mapping text to Unicode, preventing it from
        ///     showing up garbled or as a series of question marks, <c>������</c>.
        /// </para>
        /// </remarks>
        /// <inheritdoc cref="EnsureWritable" path="/exception"/>
        /// <exception cref="FileNotFoundException">A font file was not found at <paramref name="path"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> was an empty string or an invalid path.</exception>
        /// <exception cref="SecurityException">The caller does not have required permissions to access the file.</exception>
        /// <exception cref="PathTooLongException"><paramref name="path"/> exceeded the system maximum path length.</exception>
        /// <example>
        /// <para>
        ///     The following code registers the font "CustomSans.ttf" to be used as fallback if a PDF document refers
        ///     to it without having it embedded in the document.
        /// </para>
        /// <code language="cs">
        /// using (var document = PdfDocument.Open(input))
        /// {
        ///     var options = new SvgConversionOptions();
        ///     options.FontRepository.AddFont(@"D:\CustomSans.ttf");
        /// 
        ///     var pageIndex = 0;
        /// 
        ///     foreach (var page in document.Pages)
        ///     {
        ///         var svgFileName = Path.GetFileNameWithoutExtension(input) + "-" + pageIndex++ + ".svg";
        ///         page.SaveAsSvg(svgFileName, options);
        ///     }
        /// }
        /// </code>
        /// </example>
        public void AddFont(string path, bool allowEmbedding = false)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (path.Length == 0)
            {
                throw new ArgumentException("Path cannot be an empty string.", nameof(path));
            }

            EnsureWritable();

            try
            {
                path = Path.GetFullPath(path);
            }
            catch (NotSupportedException ex)
            {
                throw new ArgumentException(ex.Message);
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Font not found.", fileName: path);
            }

            lock (registrations)
            {
                registrations.Add(new Registration
                {
                    IsDirectory = false,
                    Path = path,
                    AllowEmbedding = allowEmbedding,
                });
                ClearCache();
            }
        }

        /// <summary>
        /// Registers all fonts in a specified directory, to be used as fallback candidates if a font is not embedded in
        /// a PDF document.
        /// </summary>
        /// <param name="path">
        ///     Path to the directory containing the font files. OpenType (*.otf) and TrueType (*.ttf) fonts are supported.
        ///     The method does not recurse into subdirectories.
        /// </param>
        /// <param name="allowEmbedding">
        ///     If <c>true</c>, the fonts can be embedded in the output SVG.
        ///     If <c>false</c>, the fonts are used for text extraction during conversion but not embedded in the output SVG.
        /// </param>
        /// <inheritdoc cref="AddFont(string, bool)" path="/remarks"/>
        /// <inheritdoc cref="EnsureWritable" path="/exception"/>
        /// <exception cref="DirectoryNotFoundException">The directory was not found.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> was <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> was an empty string or an invalid path.</exception>
        /// <exception cref="SecurityException">The caller does not have required permissions to access the directory.</exception>
        /// <exception cref="PathTooLongException"><paramref name="path"/> exceeded the system maximum path length.</exception>
        /// <example>
        /// <para>
        ///     The following code registers the directory "D:\PDF fonts\" as containing fonts used as fallback if a PDF
        ///     document refers to them without having them embedded in the document. They are also allowed to be
        ///     embedded in the output SVG.
        /// </para>
        /// <code language="cs">
        /// using (var document = PdfDocument.Open(input))
        /// {
        ///     var options = new SvgConversionOptions();
        ///     options.FontRepository.AddDirectory(@"D:\PDF fonts\", allowEmbedding: true);
        /// 
        ///     var pageIndex = 0;
        /// 
        ///     foreach (var page in document.Pages)
        ///     {
        ///         var svgFileName = Path.GetFileNameWithoutExtension(input) + "-" + pageIndex++ + ".svg";
        ///         page.SaveAsSvg(svgFileName, options);
        ///     }
        /// }
        /// </code>
        /// </example>
        public void AddDirectory(string path, bool allowEmbedding = false)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (path.Length == 0)
            {
                throw new ArgumentException("Path cannot be an empty string.", nameof(path));
            }

            EnsureWritable();

            try
            {
                path = Path.GetFullPath(path);
            }
            catch (NotSupportedException ex)
            {
                throw new ArgumentException(ex.Message);
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Font directory not found.");
            }

            lock (registrations)
            {
                registrations.Add(new Registration
                {
                    IsDirectory = true,
                    Path = path,
                    AllowEmbedding = allowEmbedding,
                });
                ClearCache();
            }
        }

        /// <summary>
        /// Registers all fonts installed in the operating system. The fonts will only be used during conversion and
        /// will not be embedded in the output SVG.
        /// </summary>
        /// <inheritdoc cref="EnsureWritable" path="/exception"/>
        /// <inheritdoc cref="GetSystemFontsFolders" path="/exception"/>
        /// <example>
        /// <para>
        ///     The following code registers all system fonts as fallback if a PDF document refers to a font not
        ///     embedded in the document.
        /// </para>
        /// <code language="cs">
        /// using (var document = PdfDocument.Open(input))
        /// {
        ///     var options = new SvgConversionOptions();
        ///     options.FontRepository.AddSystemFonts();
        /// 
        ///     var pageIndex = 0;
        /// 
        ///     foreach (var page in document.Pages)
        ///     {
        ///         var svgFileName = Path.GetFileNameWithoutExtension(input) + "-" + pageIndex++ + ".svg";
        ///         page.SaveAsSvg(svgFileName, options);
        ///     }
        /// }
        /// </code>
        /// </example>
        public void AddSystemFonts()
        {
            EnsureWritable();

            var fontFolders = GetSystemFontsFolders();

            lock (registrations)
            {
                foreach (var path in fontFolders)
                {
                    registrations.Add(new Registration
                    {
                        IsDirectory = true,
                        Path = path,

                        // We open up for potential accidental copyright infringements if we by default enable embedding
                        // of system fonts in output SVGs. Fonts bundled with Windows are not allowed to be distributed
                        // as web fonts. There can also be other installed fonts not allowed for this purpose.
                        // However, only using the system fonts for decoding text should be ok.
                        AllowEmbedding = false,
                    });
                }

                ClearCache();
            }
        }

        /// <summary>
        /// Clears all fonts in this repository.
        /// </summary>
        /// <inheritdoc cref="EnsureWritable" path="/exception"/>
        public void Clear()
        {
            EnsureWritable();

            lock (registrations)
            {
                registrations.Clear();
                ClearCache();
            }
        }

        /// <exception cref="InvalidOperationException">The font repository is read-only.</exception>
        private void EnsureWritable()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("This " + nameof(FontRepository) + " is read-only.");
            }
        }

        private Dictionary<string, CachedFont> GetCache()
        {
            var cache = this.fontsByPostScriptName;

            if (cache == null)
            {
                cache = new Dictionary<string, CachedFont>(StringComparer.OrdinalIgnoreCase);
                this.fontsByPostScriptName = cache;
            }

            void AddFileToCache(string path, bool allowEmbedding)
            {
                // ISO-32000-2:2020 Section 9.5
                // The spec says details regarding font naming, substitution and glyph selection are
                // implementation-dependent. This was confirmed by testing how other PDF readers handle external
                // fonts.
                //
                // BaseFont should be the PostScript name of the font. We will use this as indentifer for looking up
                // external fonts. Note that the BaseFont name can be prepended with a six character prefix and a plus
                // sign if the font is subsetted. However, this should not be applicable for external fonts.
                //
                using var stream = File.OpenRead(path);
                var names = OpenTypeFont.ParseNames(stream);

                var postScriptName = names.PostScriptName;
                if (postScriptName != null)
                {
                    cache[postScriptName] = new CachedFont
                    {
                        Path = path,
                        PostScriptName = postScriptName,
                        AllowEmbedding = allowEmbedding,
                    };
                }
            }

            foreach (var registration in registrations)
            {
                if (registration.IsDirectory)
                {
                    // Directory
                    try
                    {
                        foreach (var path in Directory.EnumerateFiles(registration.Path))
                        {
                            var extension = Path.GetExtension(path)?.ToLowerInvariant();
                            if (extension == ".ttf" || extension == ".otf")
                            {
                                try
                                {
                                    AddFileToCache(path, registration.AllowEmbedding);
                                }
                                catch (Exception ex)
                                {
                                    // Ignore font
                                    Log.WriteLine("Font " + registration.Path + " could not be cached. " + ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ignore directory
                        Log.WriteLine("Directory " + registration.Path + " could not be cached. " + ex);
                    }
                }
                else
                {
                    // File
                    try
                    {
                        AddFileToCache(registration.Path, registration.AllowEmbedding);
                    }
                    catch (Exception ex)
                    {
                        // Ignore font
                        Log.WriteLine("Font " + registration.Path + " could not be cached. " + ex);
                    }
                }
            }

            return cache;
        }

        /// <exception cref="SecurityException">The caller does not have permission to get the path to the system fonts directory.</exception>
        /// <exception cref="PlatformNotSupportedException">The current platform is not supported.</exception>
        private static string[] GetSystemFontsFolders()
        {
            var pathCandidates = ArrayUtils.Empty<string>();

#if NETSTANDARD1_6
            var systemRoot = Environment.GetEnvironmentVariable("SystemRoot");
            var localAppData = Environment.GetEnvironmentVariable("LocalAppData");
            var isWindows = !string.IsNullOrEmpty(systemRoot);

            if (isWindows)
            {
                pathCandidates = [
                    Path.Combine(systemRoot, "Fonts"),
                    Path.Combine(localAppData, "Microsoft", "Windows", "Fonts"),
                ];
            }
#else
            var isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;

            if (isWindows)
            {
                var systemRoot = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                pathCandidates = [
                    Path.Combine(systemRoot, "Fonts"),
                    Path.Combine(localAppData, "Microsoft", "Windows", "Fonts"),
                ];
            }
            else
            {
                pathCandidates = [
                    Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                ];
            }
#endif

            return pathCandidates
                .Where(path => !string.IsNullOrEmpty(path) && Directory.Exists(path))
                .ToArray();
        }

        private void ClearCache()
        {
            fontsByPostScriptName = null;
        }

        private static OpenTypeFont ParseFont(byte[] content, bool allowEmbedding)
        {
            var font = OpenTypeFont.Parse(content);

            if (!allowEmbedding)
            {
                var os2 = font.Tables.Get<OS2Table>();
                
                // OS2 table is required in standalone fonts, so it should be safe to reject the file entirely if the
                // table is missing
                if (os2 == null)
                {
                    throw new FontException("Missing OS2 table in font");
                }

                os2.UsagePermissions = UsagePermission.Restricted;
            }

            return font;
        }

        internal OpenTypeFont? GetFont(string postScriptName)
        {
            CachedFont cachedFont;

            lock (registrations)
            {
                var cache = GetCache();
                
                if (!cache.TryGetValue(postScriptName, out cachedFont))
                {
                    return null;
                }
            }

            var content = File.ReadAllBytes(cachedFont.Path);
            return ParseFont(content, cachedFont.AllowEmbedding);
        }

#if HAVE_ASYNC
        internal async Task<OpenTypeFont?> GetFontAsync(string postScriptName)
        {
            CachedFont cachedFont;

            lock (registrations)
            {
                var cache = GetCache();

                if (!cache.TryGetValue(postScriptName, out cachedFont))
                {
                    return null;
                }
            }

            using var stream = File.OpenRead(cachedFont.Path);

            var content = new byte[stream.Length];
            await stream.ReadAllAsync(content, 0, content.Length).ConfigureAwait(false);
            return ParseFont(content, cachedFont.AllowEmbedding);
        }
#endif
    }
}
