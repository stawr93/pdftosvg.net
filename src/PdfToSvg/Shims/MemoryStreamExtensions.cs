// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#if NETFRAMEWORK && !NET46_OR_GREATER
namespace PdfToSvg
{
    internal static class MemoryStreamExtensions
    {
        public static bool TryGetBuffer(this MemoryStream stream, out ArraySegment<byte> buffer)
        {
            try
            {
                var internalBuffer = stream.GetBuffer();

                // On .NET 4.5 and earlier, there is no public accessor to the offset and count passed to the
                // MemoryStream constructor. However, we can detect if only a slice of an array was passed to the 
                // constructor and reject the call, to prevent returning an invalid segment.
                if (internalBuffer.Length != stream.Capacity)
                {
                    buffer = default;
                    return false;
                }

                buffer = new ArraySegment<byte>(internalBuffer, offset: 0, count: (int)stream.Length);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                buffer = default;
                return false;
            }
        }
    }
}
#endif
