// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PdfToSvg.Tests
{
    public class ImageTests
    {
        private static byte[] TestContent = [1, 2, 3, 4, 5, 255];
        private MockImage image = new();
        private string testImagePath;

        [SetUp]
        public void SetUp()
        {
            testImagePath = Path.GetTempFileName();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(testImagePath))
            {
                File.Delete(testImagePath);
            }
        }

        private class MockImage : Image
        {
            public MockImage() : base("image/jpeg", ".jpg", 1, 1)
            {
            }

            public override byte[] GetContent(CancellationToken cancellationToken = default)
            {
                return TestContent;
            }

#if !NET40
            public override async Task<byte[]> GetContentAsync(CancellationToken cancellationToken = default)
            {
                await Task.Yield();
                return TestContent;
            }
#endif
        }

        [Test]
        public void Save_Path_Success()
        {
            image.Save(testImagePath);

            var actual = File.ReadAllBytes(testImagePath);
            Assert.AreEqual(TestContent, actual);
        }

        [Test]
        public void Save_Path_Cancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.Throws<OperationCanceledException>(() => image.Save(testImagePath, cts.Token));
        }

        [Test]
        public void Save_Stream_Success()
        {
            using var memoryStream = new MemoryStream();

            image.Save(memoryStream);

            var actual = memoryStream.ToArray();
            Assert.AreEqual(TestContent, actual);
        }

        [Test]
        public void Save_Stream_Cancelled()
        {
            using var memoryStream = new MemoryStream();

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.Throws<OperationCanceledException>(() => image.Save(memoryStream, cts.Token));
        }

#if !NET40
        [Test]
        public async Task SaveAsync_Path_Success()
        {
            await image.SaveAsync(testImagePath);

            var actual = File.ReadAllBytes(testImagePath);
            Assert.AreEqual(TestContent, actual);
        }

        [Test]
        public async Task SaveAsync_Path_Cancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<OperationCanceledException>(() => image.SaveAsync(testImagePath, cts.Token));
        }

        [Test]
        public async Task SaveAsync_Stream_Success()
        {
            using var memoryStream = new MemoryStream();

            await image.SaveAsync(memoryStream);

            var actual = memoryStream.ToArray();
            Assert.AreEqual(TestContent, actual);
        }

        [Test]
        public async Task SaveAsync_Stream_Cancelled()
        {
            using var memoryStream = new MemoryStream();

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<OperationCanceledException>(() => image.SaveAsync(memoryStream, cts.Token));
        }
#endif
    }
}
