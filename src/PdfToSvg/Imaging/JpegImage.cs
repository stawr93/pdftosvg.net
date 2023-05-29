﻿// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using PdfToSvg.ColorSpaces;
using PdfToSvg.DocumentModel;
using PdfToSvg.Imaging.Jpeg;
using PdfToSvg.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PdfToSvg.Imaging
{
    internal class JpegImage : Image
    {
        private readonly PdfStream imageDictionaryStream;
        private readonly ColorSpace colorSpace;

        public JpegImage(PdfDictionary imageDictionary, ColorSpace colorSpace) : base("image/jpeg")
        {
            if (imageDictionary.Stream == null)
            {
                throw new ArgumentException("There was no data stream attached to the image dictionary.", nameof(imageDictionary));
            }

            this.imageDictionaryStream = imageDictionary.Stream;
            this.colorSpace = colorSpace;
        }

        private byte[] Convert(byte[] sourceJpegData)
        {
            if (colorSpace is DeviceRgbColorSpace ||
                colorSpace is DeviceGrayColorSpace)
            {
                return sourceJpegData;
            }

            var decoder = new JpegDecoder();
            decoder.ReadMetadata(sourceJpegData, 0, sourceJpegData.Length);

            var sourceColorSpace = decoder.ColorSpace;

            if (sourceColorSpace == JpegColorSpace.YCbCr ||
                sourceColorSpace == JpegColorSpace.Gray)
            {
                return sourceJpegData;
            }

            if (!decoder.IsSupported)
            {
                // Possible cause: progressive JPEG.
                // Just return it and hope for the best.
                return sourceJpegData;
            }

            var encoder = new JpegEncoder();

            encoder.Width = decoder.Width;
            encoder.Height = decoder.Height;
            encoder.ColorSpace = JpegColorSpace.YCbCr;
            encoder.Quality = 90;

            encoder.WriteMetadata();

            foreach (var scan in decoder.ReadImageData())
            {
                var length = scan.Length;

                if (sourceColorSpace == JpegColorSpace.Ycck)
                {
                    length = JpegColorSpaceTransform.YcckToYcc(scan, 0, length);
                }
                else if (sourceColorSpace == JpegColorSpace.Cmyk)
                {
                    length = JpegColorSpaceTransform.CmykToYcc(scan, 0, length);
                }
                else
                {
                    throw new PdfException("Unexpected state. Color space should have been either YCCK or CMYK but was " + sourceColorSpace + ".");
                }

                encoder.WriteImageData(scan, 0, length);
            }

            encoder.WriteEndImage();

            return encoder.ToByteArray();
        }

        private Stream GetStream(CancellationToken cancellationToken)
        {
            Stream? resultStream = null;

            var filters = imageDictionaryStream.Filters;
            var encodedStream = imageDictionaryStream.Open(cancellationToken);

            try
            {
                resultStream = filters.Take(filters.Count - 1).Decode(encodedStream);
            }
            finally
            {
                if (resultStream == null)
                {
                    encodedStream.Dispose();
                }
            }

            return resultStream;
        }

        public override byte[] GetContent(CancellationToken cancellationToken)
        {
            var memoryStream = new MemoryStream();

            using (var jpegStream = GetStream(cancellationToken))
            {
                jpegStream.CopyTo(memoryStream, cancellationToken);
            }

            return Convert(memoryStream.ToArray());
        }

#if HAVE_ASYNC
        public override async Task<byte[]> GetContentAsync(CancellationToken cancellationToken)
        {
            var memoryStream = new MemoryStream();

            using (var jpegStream = GetStream(cancellationToken))
            {
                await jpegStream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
            }

            return Convert(memoryStream.ToArray());
        }
#endif
    }
}
