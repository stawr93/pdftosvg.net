﻿// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PdfToSvg.IO
{
    internal class InputFile : IDisposable
    {
        private const int DefaultBufferSize = 4096;
        private const int OpenTimeout = 60000;
        private Stream? baseStream;
        private SemaphoreSlim? readSemaphore = new SemaphoreSlim(1, 1);
        private readonly bool leaveOpen;

        public InputFile(Stream baseStream, bool leaveOpen)
        {
            this.baseStream = baseStream;
            this.leaveOpen = leaveOpen;
        }

        public InputFile(string path, bool useAsync)
        {
            this.baseStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync);
        }

#if HAVE_ASYNC
        public Task<BufferedReader> CreateExclusiveReaderAsync(CancellationToken cancellationToken)
        {
            return CreateExclusiveReaderAsync(DefaultBufferSize, cancellationToken);
        }

        public async Task<BufferedReader> CreateExclusiveReaderAsync(int bufferSize, CancellationToken cancellationToken)
        {
            if (baseStream == null || readSemaphore == null) throw new ObjectDisposedException(nameof(InputFile));

            if (!await readSemaphore.WaitAsync(OpenTimeout, cancellationToken).ConfigureAwait(false))
            {
                throw NewTimeoutException();
            }

            BufferedStreamReader reader;
            var succeeded = false;

            try
            {
                reader = new BufferedStreamReader(baseStream, () => readSemaphore?.Release(), bufferSize);
                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    readSemaphore.Release();
                }
            }

            return reader;
        }
#endif

        public BufferedReader CreateExclusiveReader(CancellationToken cancellationToken)
        {
            return CreateExclusiveReader(DefaultBufferSize, cancellationToken);
        }

        public BufferedReader CreateExclusiveReader(int bufferSize, CancellationToken cancellationToken)
        {
            if (baseStream == null || readSemaphore == null) throw new ObjectDisposedException(nameof(InputFile));

            if (!readSemaphore.Wait(OpenTimeout, cancellationToken))
            {
                throw NewTimeoutException();
            }

            BufferedStreamReader reader;
            var succeeded = false;

            try
            {
                reader = new BufferedStreamReader(baseStream, () => readSemaphore?.Release(), bufferSize);
                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    readSemaphore.Release();
                }
            }

            return reader;
        }

#if HAVE_ASYNC
        public Task<BufferedReader> CreateExclusiveSliceReaderAsync(long offset, long length, CancellationToken cancellationToken)
        {
            return CreateExclusiveSliceReaderAsync(offset, length, DefaultBufferSize, cancellationToken);
        }

        public async Task<BufferedReader> CreateExclusiveSliceReaderAsync(long offset, long length, int bufferSize, CancellationToken cancellationToken)
        {
            if (baseStream == null || readSemaphore == null) throw new ObjectDisposedException(nameof(InputFile));

            if (!await readSemaphore.WaitAsync(OpenTimeout, cancellationToken).ConfigureAwait(false))
            {
                throw NewTimeoutException();
            }

            BufferedStreamReader reader;
            var succeeded = false;

            try
            {
                baseStream.Position = offset;
                reader = new BufferedStreamReader(baseStream, offset, length, () => readSemaphore?.Release(), bufferSize);
                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    readSemaphore.Release();
                }
            }

            return reader;
        }
#endif

        public BufferedReader CreateExclusiveSliceReader(long offset, long length, CancellationToken cancellationToken)
        {
            return CreateExclusiveSliceReader(offset, length, DefaultBufferSize, cancellationToken);
        }

        public BufferedReader CreateExclusiveSliceReader(long offset, long length, int bufferSize, CancellationToken cancellationToken)
        {
            if (baseStream == null || readSemaphore == null) throw new ObjectDisposedException(nameof(InputFile));

            if (!readSemaphore.Wait(OpenTimeout, cancellationToken))
            {
                throw NewTimeoutException();
            }

            BufferedStreamReader reader;
            var succeeded = false;

            try
            {
                baseStream.Position = offset;
                reader = new BufferedStreamReader(baseStream, offset, length, () => readSemaphore?.Release(), bufferSize);
                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    readSemaphore.Release();
                }
            }

            return reader;
        }

        private static Exception NewTimeoutException()
        {
            return new TimeoutException("Failed to lock the input file within a reasonable time.");
        }

        public void Dispose()
        {
            readSemaphore?.Dispose();

            if (!leaveOpen)
            {
                baseStream?.Dispose();
            }

            readSemaphore = null;
            baseStream = null;
        }
    }
}
