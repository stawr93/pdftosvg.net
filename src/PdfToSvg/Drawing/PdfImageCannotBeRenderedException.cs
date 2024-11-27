// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

namespace PdfToSvg.Drawing;

/// <summary>
/// Represents error that occurs when an image from PDF file cannot be rendered to SVG.
/// </summary>
public sealed class PdfImageCannotBeRenderedException : PdfException
{
    public PdfImageCannotBeRenderedException() : base("An image cannot be rendered")
    {
    }
}
