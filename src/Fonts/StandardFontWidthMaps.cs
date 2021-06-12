﻿// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using PdfToSvg.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfToSvg.Fonts
{
    internal static class StandardFontWidthMaps
    {
        private const double WidthMultiplier = 0.001;

        // The width maps are encoded as pairs of unicode character codes, followed by its width.
        // The maps are generated from Adobe "Font Metrics for PDF Core 14 Fonts", found here:
        // https://web.archive.org/web/20200328170616/https://www.adobe.com/devnet/font.html
        // http://download.macromedia.com/pub/developer/opentype/tech-notes/Core14_AFMs.zip

        // Copyright of the font metrics:
        //
        // This file and the 14 PostScript(R) AFM files it accompanies may be used, copied, and distributed for any
        // purpose and without charge, with or without modification, provided that all copyright notices are retained;
        // that the AFM files are not distributed without this file; that all modifications to this file or any of the
        // AFM files are prominently noted in the modified file(s); and that this paragraph is not modified. Adobe
        // Systems has no responsibility or obligation to support the use of the AFM files.
        //
        // End of copyright

        private static readonly ushort[] helveticaBold = new ushort[]
        {
            32, 278, 33, 333, 34, 474, 35, 556, 36, 556, 37, 889, 38, 722, 8217, 278,
            40, 333, 41, 333, 42, 389, 43, 584, 44, 278, 45, 333, 46, 278, 47, 278,
            48, 556, 49, 556, 50, 556, 51, 556, 52, 556, 53, 556, 54, 556, 55, 556,
            56, 556, 57, 556, 58, 333, 59, 333, 60, 584, 61, 584, 62, 584, 63, 611,
            64, 975, 65, 722, 66, 722, 67, 722, 68, 722, 69, 667, 70, 611, 71, 778,
            72, 722, 73, 278, 74, 556, 75, 722, 76, 611, 77, 833, 78, 722, 79, 778,
            80, 667, 81, 778, 82, 722, 83, 667, 84, 611, 85, 722, 86, 667, 87, 944,
            88, 667, 89, 667, 90, 611, 91, 333, 92, 278, 93, 333, 94, 584, 95, 556,
            8216, 278, 97, 556, 98, 611, 99, 556, 100, 611, 101, 556, 102, 333, 103, 611,
            104, 611, 105, 278, 106, 278, 107, 556, 108, 278, 109, 889, 110, 611, 111, 611,
            112, 611, 113, 611, 114, 389, 115, 556, 116, 333, 117, 611, 118, 556, 119, 778,
            120, 556, 121, 556, 122, 500, 123, 389, 124, 280, 125, 389, 126, 584, 161, 333,
            162, 556, 163, 556, 8260, 167, 165, 556, 402, 556, 167, 556, 164, 556, 39, 238,
            8220, 500, 171, 556, 8249, 333, 8250, 333, 64257, 611, 64258, 611, 8211, 556, 8224, 556,
            8225, 556, 183, 278, 182, 556, 8226, 350, 8218, 278, 8222, 500, 8221, 500, 187, 556,
            8230, 1000, 8240, 1000, 191, 611, 96, 333, 180, 333, 710, 333, 732, 333, 175, 333,
            728, 333, 729, 333, 168, 333, 730, 333, 184, 333, 733, 333, 731, 333, 711, 333,
            8212, 1000, 198, 1000, 170, 370, 321, 611, 216, 778, 338, 1000, 186, 365, 230, 889,
            305, 278, 322, 278, 248, 611, 339, 944, 223, 611, 207, 278, 233, 556, 259, 556,
            369, 611, 283, 556, 376, 667, 247, 584, 221, 667, 194, 722, 225, 556, 219, 722,
            253, 556, 537, 556, 234, 556, 366, 722, 220, 722, 261, 556, 218, 722, 371, 611,
            203, 667, 272, 722, 63171, 250, 169, 737, 274, 667, 269, 556, 229, 556, 325, 722,
            314, 278, 224, 556, 354, 611, 262, 722, 227, 556, 278, 667, 353, 556, 351, 556,
            237, 278, 9674, 494, 344, 722, 290, 778, 251, 611, 226, 556, 256, 722, 345, 389,
            231, 556, 379, 611, 222, 667, 332, 778, 340, 722, 346, 667, 271, 743, 362, 722,
            367, 611, 179, 333, 210, 778, 192, 722, 258, 722, 215, 584, 250, 611, 356, 611,
            8706, 494, 255, 556, 323, 722, 238, 278, 202, 667, 228, 556, 235, 556, 263, 556,
            324, 611, 363, 611, 327, 722, 205, 278, 177, 584, 166, 280, 174, 737, 286, 778,
            304, 278, 8721, 600, 200, 667, 341, 389, 333, 611, 377, 611, 381, 611, 8805, 549,
            208, 722, 199, 722, 316, 278, 357, 389, 281, 556, 370, 722, 193, 722, 196, 722,
            232, 556, 378, 500, 303, 278, 211, 778, 243, 611, 257, 556, 347, 556, 239, 278,
            212, 778, 217, 722, 8710, 612, 254, 611, 178, 333, 214, 778, 181, 611, 236, 278,
            337, 611, 280, 667, 273, 611, 190, 834, 350, 667, 318, 400, 310, 722, 313, 611,
            8482, 1000, 279, 556, 204, 278, 298, 278, 317, 611, 189, 834, 8804, 549, 244, 611,
            241, 611, 368, 722, 201, 667, 275, 556, 287, 611, 188, 834, 352, 667, 536, 667,
            336, 778, 176, 400, 242, 611, 268, 722, 249, 611, 8730, 549, 270, 722, 343, 389,
            209, 722, 245, 611, 342, 722, 315, 611, 195, 722, 260, 722, 197, 722, 213, 778,
            380, 500, 282, 667, 302, 278, 311, 556, 8722, 584, 206, 278, 328, 611, 355, 333,
            172, 584, 246, 611, 252, 611, 8800, 549, 291, 611, 240, 611, 382, 500, 326, 611,
            185, 333, 299, 278, 8364, 556,
        };

        private static readonly ushort[] helveticaBoldOblique = new ushort[]
        {
            32, 278, 33, 333, 34, 474, 35, 556, 36, 556, 37, 889, 38, 722, 8217, 278,
            40, 333, 41, 333, 42, 389, 43, 584, 44, 278, 45, 333, 46, 278, 47, 278,
            48, 556, 49, 556, 50, 556, 51, 556, 52, 556, 53, 556, 54, 556, 55, 556,
            56, 556, 57, 556, 58, 333, 59, 333, 60, 584, 61, 584, 62, 584, 63, 611,
            64, 975, 65, 722, 66, 722, 67, 722, 68, 722, 69, 667, 70, 611, 71, 778,
            72, 722, 73, 278, 74, 556, 75, 722, 76, 611, 77, 833, 78, 722, 79, 778,
            80, 667, 81, 778, 82, 722, 83, 667, 84, 611, 85, 722, 86, 667, 87, 944,
            88, 667, 89, 667, 90, 611, 91, 333, 92, 278, 93, 333, 94, 584, 95, 556,
            8216, 278, 97, 556, 98, 611, 99, 556, 100, 611, 101, 556, 102, 333, 103, 611,
            104, 611, 105, 278, 106, 278, 107, 556, 108, 278, 109, 889, 110, 611, 111, 611,
            112, 611, 113, 611, 114, 389, 115, 556, 116, 333, 117, 611, 118, 556, 119, 778,
            120, 556, 121, 556, 122, 500, 123, 389, 124, 280, 125, 389, 126, 584, 161, 333,
            162, 556, 163, 556, 8260, 167, 165, 556, 402, 556, 167, 556, 164, 556, 39, 238,
            8220, 500, 171, 556, 8249, 333, 8250, 333, 64257, 611, 64258, 611, 8211, 556, 8224, 556,
            8225, 556, 183, 278, 182, 556, 8226, 350, 8218, 278, 8222, 500, 8221, 500, 187, 556,
            8230, 1000, 8240, 1000, 191, 611, 96, 333, 180, 333, 710, 333, 732, 333, 175, 333,
            728, 333, 729, 333, 168, 333, 730, 333, 184, 333, 733, 333, 731, 333, 711, 333,
            8212, 1000, 198, 1000, 170, 370, 321, 611, 216, 778, 338, 1000, 186, 365, 230, 889,
            305, 278, 322, 278, 248, 611, 339, 944, 223, 611, 207, 278, 233, 556, 259, 556,
            369, 611, 283, 556, 376, 667, 247, 584, 221, 667, 194, 722, 225, 556, 219, 722,
            253, 556, 537, 556, 234, 556, 366, 722, 220, 722, 261, 556, 218, 722, 371, 611,
            203, 667, 272, 722, 63171, 250, 169, 737, 274, 667, 269, 556, 229, 556, 325, 722,
            314, 278, 224, 556, 354, 611, 262, 722, 227, 556, 278, 667, 353, 556, 351, 556,
            237, 278, 9674, 494, 344, 722, 290, 778, 251, 611, 226, 556, 256, 722, 345, 389,
            231, 556, 379, 611, 222, 667, 332, 778, 340, 722, 346, 667, 271, 743, 362, 722,
            367, 611, 179, 333, 210, 778, 192, 722, 258, 722, 215, 584, 250, 611, 356, 611,
            8706, 494, 255, 556, 323, 722, 238, 278, 202, 667, 228, 556, 235, 556, 263, 556,
            324, 611, 363, 611, 327, 722, 205, 278, 177, 584, 166, 280, 174, 737, 286, 778,
            304, 278, 8721, 600, 200, 667, 341, 389, 333, 611, 377, 611, 381, 611, 8805, 549,
            208, 722, 199, 722, 316, 278, 357, 389, 281, 556, 370, 722, 193, 722, 196, 722,
            232, 556, 378, 500, 303, 278, 211, 778, 243, 611, 257, 556, 347, 556, 239, 278,
            212, 778, 217, 722, 8710, 612, 254, 611, 178, 333, 214, 778, 181, 611, 236, 278,
            337, 611, 280, 667, 273, 611, 190, 834, 350, 667, 318, 400, 310, 722, 313, 611,
            8482, 1000, 279, 556, 204, 278, 298, 278, 317, 611, 189, 834, 8804, 549, 244, 611,
            241, 611, 368, 722, 201, 667, 275, 556, 287, 611, 188, 834, 352, 667, 536, 667,
            336, 778, 176, 400, 242, 611, 268, 722, 249, 611, 8730, 549, 270, 722, 343, 389,
            209, 722, 245, 611, 342, 722, 315, 611, 195, 722, 260, 722, 197, 722, 213, 778,
            380, 500, 282, 667, 302, 278, 311, 556, 8722, 584, 206, 278, 328, 611, 355, 333,
            172, 584, 246, 611, 252, 611, 8800, 549, 291, 611, 240, 611, 382, 500, 326, 611,
            185, 333, 299, 278, 8364, 556,
        };

        private static readonly ushort[] helveticaOblique = new ushort[]
        {
            32, 278, 33, 278, 34, 355, 35, 556, 36, 556, 37, 889, 38, 667, 8217, 222,
            40, 333, 41, 333, 42, 389, 43, 584, 44, 278, 45, 333, 46, 278, 47, 278,
            48, 556, 49, 556, 50, 556, 51, 556, 52, 556, 53, 556, 54, 556, 55, 556,
            56, 556, 57, 556, 58, 278, 59, 278, 60, 584, 61, 584, 62, 584, 63, 556,
            64, 1015, 65, 667, 66, 667, 67, 722, 68, 722, 69, 667, 70, 611, 71, 778,
            72, 722, 73, 278, 74, 500, 75, 667, 76, 556, 77, 833, 78, 722, 79, 778,
            80, 667, 81, 778, 82, 722, 83, 667, 84, 611, 85, 722, 86, 667, 87, 944,
            88, 667, 89, 667, 90, 611, 91, 278, 92, 278, 93, 278, 94, 469, 95, 556,
            8216, 222, 97, 556, 98, 556, 99, 500, 100, 556, 101, 556, 102, 278, 103, 556,
            104, 556, 105, 222, 106, 222, 107, 500, 108, 222, 109, 833, 110, 556, 111, 556,
            112, 556, 113, 556, 114, 333, 115, 500, 116, 278, 117, 556, 118, 500, 119, 722,
            120, 500, 121, 500, 122, 500, 123, 334, 124, 260, 125, 334, 126, 584, 161, 333,
            162, 556, 163, 556, 8260, 167, 165, 556, 402, 556, 167, 556, 164, 556, 39, 191,
            8220, 333, 171, 556, 8249, 333, 8250, 333, 64257, 500, 64258, 500, 8211, 556, 8224, 556,
            8225, 556, 183, 278, 182, 537, 8226, 350, 8218, 222, 8222, 333, 8221, 333, 187, 556,
            8230, 1000, 8240, 1000, 191, 611, 96, 333, 180, 333, 710, 333, 732, 333, 175, 333,
            728, 333, 729, 333, 168, 333, 730, 333, 184, 333, 733, 333, 731, 333, 711, 333,
            8212, 1000, 198, 1000, 170, 370, 321, 556, 216, 778, 338, 1000, 186, 365, 230, 889,
            305, 278, 322, 222, 248, 611, 339, 944, 223, 611, 207, 278, 233, 556, 259, 556,
            369, 556, 283, 556, 376, 667, 247, 584, 221, 667, 194, 667, 225, 556, 219, 722,
            253, 500, 537, 500, 234, 556, 366, 722, 220, 722, 261, 556, 218, 722, 371, 556,
            203, 667, 272, 722, 63171, 250, 169, 737, 274, 667, 269, 500, 229, 556, 325, 722,
            314, 222, 224, 556, 354, 611, 262, 722, 227, 556, 278, 667, 353, 500, 351, 500,
            237, 278, 9674, 471, 344, 722, 290, 778, 251, 556, 226, 556, 256, 667, 345, 333,
            231, 500, 379, 611, 222, 667, 332, 778, 340, 722, 346, 667, 271, 643, 362, 722,
            367, 556, 179, 333, 210, 778, 192, 667, 258, 667, 215, 584, 250, 556, 356, 611,
            8706, 476, 255, 500, 323, 722, 238, 278, 202, 667, 228, 556, 235, 556, 263, 500,
            324, 556, 363, 556, 327, 722, 205, 278, 177, 584, 166, 260, 174, 737, 286, 778,
            304, 278, 8721, 600, 200, 667, 341, 333, 333, 556, 377, 611, 381, 611, 8805, 549,
            208, 722, 199, 722, 316, 222, 357, 317, 281, 556, 370, 722, 193, 667, 196, 667,
            232, 556, 378, 500, 303, 222, 211, 778, 243, 556, 257, 556, 347, 500, 239, 278,
            212, 778, 217, 722, 8710, 612, 254, 556, 178, 333, 214, 778, 181, 556, 236, 278,
            337, 556, 280, 667, 273, 556, 190, 834, 350, 667, 318, 299, 310, 667, 313, 556,
            8482, 1000, 279, 556, 204, 278, 298, 278, 317, 556, 189, 834, 8804, 549, 244, 556,
            241, 556, 368, 722, 201, 667, 275, 556, 287, 556, 188, 834, 352, 667, 536, 667,
            336, 778, 176, 400, 242, 556, 268, 722, 249, 556, 8730, 453, 270, 722, 343, 333,
            209, 722, 245, 556, 342, 722, 315, 556, 195, 667, 260, 667, 197, 667, 213, 778,
            380, 500, 282, 667, 302, 278, 311, 500, 8722, 584, 206, 278, 328, 556, 355, 278,
            172, 584, 246, 556, 252, 556, 8800, 549, 291, 556, 240, 556, 382, 500, 326, 556,
            185, 333, 299, 278, 8364, 556,
        };

        private static readonly ushort[] helvetica = new ushort[]
        {
            32, 278, 33, 278, 34, 355, 35, 556, 36, 556, 37, 889, 38, 667, 8217, 222,
            40, 333, 41, 333, 42, 389, 43, 584, 44, 278, 45, 333, 46, 278, 47, 278,
            48, 556, 49, 556, 50, 556, 51, 556, 52, 556, 53, 556, 54, 556, 55, 556,
            56, 556, 57, 556, 58, 278, 59, 278, 60, 584, 61, 584, 62, 584, 63, 556,
            64, 1015, 65, 667, 66, 667, 67, 722, 68, 722, 69, 667, 70, 611, 71, 778,
            72, 722, 73, 278, 74, 500, 75, 667, 76, 556, 77, 833, 78, 722, 79, 778,
            80, 667, 81, 778, 82, 722, 83, 667, 84, 611, 85, 722, 86, 667, 87, 944,
            88, 667, 89, 667, 90, 611, 91, 278, 92, 278, 93, 278, 94, 469, 95, 556,
            8216, 222, 97, 556, 98, 556, 99, 500, 100, 556, 101, 556, 102, 278, 103, 556,
            104, 556, 105, 222, 106, 222, 107, 500, 108, 222, 109, 833, 110, 556, 111, 556,
            112, 556, 113, 556, 114, 333, 115, 500, 116, 278, 117, 556, 118, 500, 119, 722,
            120, 500, 121, 500, 122, 500, 123, 334, 124, 260, 125, 334, 126, 584, 161, 333,
            162, 556, 163, 556, 8260, 167, 165, 556, 402, 556, 167, 556, 164, 556, 39, 191,
            8220, 333, 171, 556, 8249, 333, 8250, 333, 64257, 500, 64258, 500, 8211, 556, 8224, 556,
            8225, 556, 183, 278, 182, 537, 8226, 350, 8218, 222, 8222, 333, 8221, 333, 187, 556,
            8230, 1000, 8240, 1000, 191, 611, 96, 333, 180, 333, 710, 333, 732, 333, 175, 333,
            728, 333, 729, 333, 168, 333, 730, 333, 184, 333, 733, 333, 731, 333, 711, 333,
            8212, 1000, 198, 1000, 170, 370, 321, 556, 216, 778, 338, 1000, 186, 365, 230, 889,
            305, 278, 322, 222, 248, 611, 339, 944, 223, 611, 207, 278, 233, 556, 259, 556,
            369, 556, 283, 556, 376, 667, 247, 584, 221, 667, 194, 667, 225, 556, 219, 722,
            253, 500, 537, 500, 234, 556, 366, 722, 220, 722, 261, 556, 218, 722, 371, 556,
            203, 667, 272, 722, 63171, 250, 169, 737, 274, 667, 269, 500, 229, 556, 325, 722,
            314, 222, 224, 556, 354, 611, 262, 722, 227, 556, 278, 667, 353, 500, 351, 500,
            237, 278, 9674, 471, 344, 722, 290, 778, 251, 556, 226, 556, 256, 667, 345, 333,
            231, 500, 379, 611, 222, 667, 332, 778, 340, 722, 346, 667, 271, 643, 362, 722,
            367, 556, 179, 333, 210, 778, 192, 667, 258, 667, 215, 584, 250, 556, 356, 611,
            8706, 476, 255, 500, 323, 722, 238, 278, 202, 667, 228, 556, 235, 556, 263, 500,
            324, 556, 363, 556, 327, 722, 205, 278, 177, 584, 166, 260, 174, 737, 286, 778,
            304, 278, 8721, 600, 200, 667, 341, 333, 333, 556, 377, 611, 381, 611, 8805, 549,
            208, 722, 199, 722, 316, 222, 357, 317, 281, 556, 370, 722, 193, 667, 196, 667,
            232, 556, 378, 500, 303, 222, 211, 778, 243, 556, 257, 556, 347, 500, 239, 278,
            212, 778, 217, 722, 8710, 612, 254, 556, 178, 333, 214, 778, 181, 556, 236, 278,
            337, 556, 280, 667, 273, 556, 190, 834, 350, 667, 318, 299, 310, 667, 313, 556,
            8482, 1000, 279, 556, 204, 278, 298, 278, 317, 556, 189, 834, 8804, 549, 244, 556,
            241, 556, 368, 722, 201, 667, 275, 556, 287, 556, 188, 834, 352, 667, 536, 667,
            336, 778, 176, 400, 242, 556, 268, 722, 249, 556, 8730, 453, 270, 722, 343, 333,
            209, 722, 245, 556, 342, 722, 315, 556, 195, 667, 260, 667, 197, 667, 213, 778,
            380, 500, 282, 667, 302, 278, 311, 500, 8722, 584, 206, 278, 328, 556, 355, 278,
            172, 584, 246, 556, 252, 556, 8800, 549, 291, 556, 240, 556, 382, 500, 326, 556,
            185, 333, 299, 278, 8364, 556,
        };

        private static readonly ushort[] symbol = new ushort[]
        {
            32, 250, 33, 333, 8704, 713, 35, 500, 8707, 549, 37, 833, 38, 778, 8715, 439,
            40, 333, 41, 333, 8727, 500, 43, 549, 44, 250, 8722, 549, 46, 250, 47, 278,
            48, 500, 49, 500, 50, 500, 51, 500, 52, 500, 53, 500, 54, 500, 55, 500,
            56, 500, 57, 500, 58, 278, 59, 278, 60, 549, 61, 549, 62, 549, 63, 444,
            8773, 549, 913, 722, 914, 667, 935, 722, 8710, 612, 917, 611, 934, 763, 915, 603,
            919, 722, 921, 333, 977, 631, 922, 722, 923, 686, 924, 889, 925, 722, 927, 722,
            928, 768, 920, 741, 929, 556, 931, 592, 932, 611, 933, 690, 962, 439, 8486, 768,
            926, 645, 936, 795, 918, 611, 91, 333, 8756, 863, 93, 333, 8869, 658, 95, 500,
            63717, 500, 945, 631, 946, 549, 967, 549, 948, 494, 949, 439, 966, 521, 947, 411,
            951, 603, 953, 329, 981, 603, 954, 549, 955, 549, 181, 576, 957, 521, 959, 549,
            960, 549, 952, 521, 961, 549, 963, 603, 964, 439, 965, 576, 982, 713, 969, 686,
            958, 493, 968, 686, 950, 494, 123, 480, 124, 200, 125, 480, 8764, 549, 8364, 750,
            978, 620, 8242, 247, 8804, 549, 8260, 167, 8734, 713, 402, 500, 9827, 753, 9830, 753,
            9829, 753, 9824, 753, 8596, 1042, 8592, 987, 8593, 603, 8594, 987, 8595, 603, 176, 400,
            177, 549, 8243, 411, 8805, 549, 215, 549, 8733, 713, 8706, 494, 8226, 460, 247, 549,
            8800, 549, 8801, 549, 8776, 549, 8230, 1000, 63718, 603, 63719, 1000, 8629, 658, 8501, 823,
            8465, 686, 8476, 795, 8472, 987, 8855, 768, 8853, 768, 8709, 823, 8745, 768, 8746, 768,
            8835, 713, 8839, 713, 8836, 713, 8834, 713, 8838, 713, 8712, 713, 8713, 713, 8736, 768,
            8711, 713, 63194, 790, 63193, 790, 63195, 890, 8719, 823, 8730, 549, 8901, 250, 172, 713,
            8743, 603, 8744, 603, 8660, 1042, 8656, 987, 8657, 603, 8658, 987, 8659, 603, 9674, 494,
            9001, 329, 63720, 790, 63721, 790, 63722, 786, 8721, 713, 63723, 384, 63724, 384, 63725, 384,
            63726, 384, 63727, 384, 63728, 384, 63729, 494, 63730, 494, 63731, 494, 63732, 494, 9002, 329,
            8747, 274, 8992, 686, 63733, 686, 8993, 686, 63734, 384, 63735, 384, 63736, 384, 63737, 384,
            63738, 384, 63739, 384, 63740, 494, 63741, 494, 63742, 494, 63743, 790,
        };

        private static readonly ushort[] timesBold = new ushort[]
        {
            32, 250, 33, 333, 34, 555, 35, 500, 36, 500, 37, 1000, 38, 833, 8217, 333,
            40, 333, 41, 333, 42, 500, 43, 570, 44, 250, 45, 333, 46, 250, 47, 278,
            48, 500, 49, 500, 50, 500, 51, 500, 52, 500, 53, 500, 54, 500, 55, 500,
            56, 500, 57, 500, 58, 333, 59, 333, 60, 570, 61, 570, 62, 570, 63, 500,
            64, 930, 65, 722, 66, 667, 67, 722, 68, 722, 69, 667, 70, 611, 71, 778,
            72, 778, 73, 389, 74, 500, 75, 778, 76, 667, 77, 944, 78, 722, 79, 778,
            80, 611, 81, 778, 82, 722, 83, 556, 84, 667, 85, 722, 86, 722, 87, 1000,
            88, 722, 89, 722, 90, 667, 91, 333, 92, 278, 93, 333, 94, 581, 95, 500,
            8216, 333, 97, 500, 98, 556, 99, 444, 100, 556, 101, 444, 102, 333, 103, 500,
            104, 556, 105, 278, 106, 333, 107, 556, 108, 278, 109, 833, 110, 556, 111, 500,
            112, 556, 113, 556, 114, 444, 115, 389, 116, 333, 117, 556, 118, 500, 119, 722,
            120, 500, 121, 500, 122, 444, 123, 394, 124, 220, 125, 394, 126, 520, 161, 333,
            162, 500, 163, 500, 8260, 167, 165, 500, 402, 500, 167, 500, 164, 500, 39, 278,
            8220, 500, 171, 500, 8249, 333, 8250, 333, 64257, 556, 64258, 556, 8211, 500, 8224, 500,
            8225, 500, 183, 250, 182, 540, 8226, 350, 8218, 333, 8222, 500, 8221, 500, 187, 500,
            8230, 1000, 8240, 1000, 191, 500, 96, 333, 180, 333, 710, 333, 732, 333, 175, 333,
            728, 333, 729, 333, 168, 333, 730, 333, 184, 333, 733, 333, 731, 333, 711, 333,
            8212, 1000, 198, 1000, 170, 300, 321, 667, 216, 778, 338, 1000, 186, 330, 230, 722,
            305, 278, 322, 278, 248, 500, 339, 722, 223, 556, 207, 389, 233, 444, 259, 500,
            369, 556, 283, 444, 376, 722, 247, 570, 221, 722, 194, 722, 225, 500, 219, 722,
            253, 500, 537, 389, 234, 444, 366, 722, 220, 722, 261, 500, 218, 722, 371, 556,
            203, 667, 272, 722, 63171, 250, 169, 747, 274, 667, 269, 444, 229, 500, 325, 722,
            314, 278, 224, 500, 354, 667, 262, 722, 227, 500, 278, 667, 353, 389, 351, 389,
            237, 278, 9674, 494, 344, 722, 290, 778, 251, 556, 226, 500, 256, 722, 345, 444,
            231, 444, 379, 667, 222, 611, 332, 778, 340, 722, 346, 556, 271, 672, 362, 722,
            367, 556, 179, 300, 210, 778, 192, 722, 258, 722, 215, 570, 250, 556, 356, 667,
            8706, 494, 255, 500, 323, 722, 238, 278, 202, 667, 228, 500, 235, 444, 263, 444,
            324, 556, 363, 556, 327, 722, 205, 389, 177, 570, 166, 220, 174, 747, 286, 778,
            304, 389, 8721, 600, 200, 667, 341, 444, 333, 500, 377, 667, 381, 667, 8805, 549,
            208, 722, 199, 722, 316, 278, 357, 416, 281, 444, 370, 722, 193, 722, 196, 722,
            232, 444, 378, 444, 303, 278, 211, 778, 243, 500, 257, 500, 347, 389, 239, 278,
            212, 778, 217, 722, 8710, 612, 254, 556, 178, 300, 214, 778, 181, 556, 236, 278,
            337, 500, 280, 667, 273, 556, 190, 750, 350, 556, 318, 394, 310, 778, 313, 667,
            8482, 1000, 279, 444, 204, 389, 298, 389, 317, 667, 189, 750, 8804, 549, 244, 500,
            241, 556, 368, 722, 201, 667, 275, 444, 287, 500, 188, 750, 352, 556, 536, 556,
            336, 778, 176, 400, 242, 500, 268, 722, 249, 556, 8730, 549, 270, 722, 343, 444,
            209, 722, 245, 500, 342, 722, 315, 667, 195, 722, 260, 722, 197, 722, 213, 778,
            380, 444, 282, 667, 302, 389, 311, 556, 8722, 570, 206, 389, 328, 556, 355, 333,
            172, 570, 246, 500, 252, 556, 8800, 549, 291, 500, 240, 500, 382, 444, 326, 556,
            185, 300, 299, 278, 8364, 500,
        };

        private static readonly ushort[] timesBoldItalic = new ushort[]
        {
            32, 250, 33, 389, 34, 555, 35, 500, 36, 500, 37, 833, 38, 778, 8217, 333,
            40, 333, 41, 333, 42, 500, 43, 570, 44, 250, 45, 333, 46, 250, 47, 278,
            48, 500, 49, 500, 50, 500, 51, 500, 52, 500, 53, 500, 54, 500, 55, 500,
            56, 500, 57, 500, 58, 333, 59, 333, 60, 570, 61, 570, 62, 570, 63, 500,
            64, 832, 65, 667, 66, 667, 67, 667, 68, 722, 69, 667, 70, 667, 71, 722,
            72, 778, 73, 389, 74, 500, 75, 667, 76, 611, 77, 889, 78, 722, 79, 722,
            80, 611, 81, 722, 82, 667, 83, 556, 84, 611, 85, 722, 86, 667, 87, 889,
            88, 667, 89, 611, 90, 611, 91, 333, 92, 278, 93, 333, 94, 570, 95, 500,
            8216, 333, 97, 500, 98, 500, 99, 444, 100, 500, 101, 444, 102, 333, 103, 500,
            104, 556, 105, 278, 106, 278, 107, 500, 108, 278, 109, 778, 110, 556, 111, 500,
            112, 500, 113, 500, 114, 389, 115, 389, 116, 278, 117, 556, 118, 444, 119, 667,
            120, 500, 121, 444, 122, 389, 123, 348, 124, 220, 125, 348, 126, 570, 161, 389,
            162, 500, 163, 500, 8260, 167, 165, 500, 402, 500, 167, 500, 164, 500, 39, 278,
            8220, 500, 171, 500, 8249, 333, 8250, 333, 64257, 556, 64258, 556, 8211, 500, 8224, 500,
            8225, 500, 183, 250, 182, 500, 8226, 350, 8218, 333, 8222, 500, 8221, 500, 187, 500,
            8230, 1000, 8240, 1000, 191, 500, 96, 333, 180, 333, 710, 333, 732, 333, 175, 333,
            728, 333, 729, 333, 168, 333, 730, 333, 184, 333, 733, 333, 731, 333, 711, 333,
            8212, 1000, 198, 944, 170, 266, 321, 611, 216, 722, 338, 944, 186, 300, 230, 722,
            305, 278, 322, 278, 248, 500, 339, 722, 223, 500, 207, 389, 233, 444, 259, 500,
            369, 556, 283, 444, 376, 611, 247, 570, 221, 611, 194, 667, 225, 500, 219, 722,
            253, 444, 537, 389, 234, 444, 366, 722, 220, 722, 261, 500, 218, 722, 371, 556,
            203, 667, 272, 722, 63171, 250, 169, 747, 274, 667, 269, 444, 229, 500, 325, 722,
            314, 278, 224, 500, 354, 611, 262, 667, 227, 500, 278, 667, 353, 389, 351, 389,
            237, 278, 9674, 494, 344, 667, 290, 722, 251, 556, 226, 500, 256, 667, 345, 389,
            231, 444, 379, 611, 222, 611, 332, 722, 340, 667, 346, 556, 271, 608, 362, 722,
            367, 556, 179, 300, 210, 722, 192, 667, 258, 667, 215, 570, 250, 556, 356, 611,
            8706, 494, 255, 444, 323, 722, 238, 278, 202, 667, 228, 500, 235, 444, 263, 444,
            324, 556, 363, 556, 327, 722, 205, 389, 177, 570, 166, 220, 174, 747, 286, 722,
            304, 389, 8721, 600, 200, 667, 341, 389, 333, 500, 377, 611, 381, 611, 8805, 549,
            208, 722, 199, 667, 316, 278, 357, 366, 281, 444, 370, 722, 193, 667, 196, 667,
            232, 444, 378, 389, 303, 278, 211, 722, 243, 500, 257, 500, 347, 389, 239, 278,
            212, 722, 217, 722, 8710, 612, 254, 500, 178, 300, 214, 722, 181, 576, 236, 278,
            337, 500, 280, 667, 273, 500, 190, 750, 350, 556, 318, 382, 310, 667, 313, 611,
            8482, 1000, 279, 444, 204, 389, 298, 389, 317, 611, 189, 750, 8804, 549, 244, 500,
            241, 556, 368, 722, 201, 667, 275, 444, 287, 500, 188, 750, 352, 556, 536, 556,
            336, 722, 176, 400, 242, 500, 268, 667, 249, 556, 8730, 549, 270, 722, 343, 389,
            209, 722, 245, 500, 342, 667, 315, 611, 195, 667, 260, 667, 197, 667, 213, 722,
            380, 389, 282, 667, 302, 389, 311, 500, 8722, 606, 206, 389, 328, 556, 355, 278,
            172, 606, 246, 500, 252, 556, 8800, 549, 291, 500, 240, 500, 382, 389, 326, 556,
            185, 300, 299, 278, 8364, 500,
        };

        private static readonly ushort[] timesItalic = new ushort[]
        {
            32, 250, 33, 333, 34, 420, 35, 500, 36, 500, 37, 833, 38, 778, 8217, 333,
            40, 333, 41, 333, 42, 500, 43, 675, 44, 250, 45, 333, 46, 250, 47, 278,
            48, 500, 49, 500, 50, 500, 51, 500, 52, 500, 53, 500, 54, 500, 55, 500,
            56, 500, 57, 500, 58, 333, 59, 333, 60, 675, 61, 675, 62, 675, 63, 500,
            64, 920, 65, 611, 66, 611, 67, 667, 68, 722, 69, 611, 70, 611, 71, 722,
            72, 722, 73, 333, 74, 444, 75, 667, 76, 556, 77, 833, 78, 667, 79, 722,
            80, 611, 81, 722, 82, 611, 83, 500, 84, 556, 85, 722, 86, 611, 87, 833,
            88, 611, 89, 556, 90, 556, 91, 389, 92, 278, 93, 389, 94, 422, 95, 500,
            8216, 333, 97, 500, 98, 500, 99, 444, 100, 500, 101, 444, 102, 278, 103, 500,
            104, 500, 105, 278, 106, 278, 107, 444, 108, 278, 109, 722, 110, 500, 111, 500,
            112, 500, 113, 500, 114, 389, 115, 389, 116, 278, 117, 500, 118, 444, 119, 667,
            120, 444, 121, 444, 122, 389, 123, 400, 124, 275, 125, 400, 126, 541, 161, 389,
            162, 500, 163, 500, 8260, 167, 165, 500, 402, 500, 167, 500, 164, 500, 39, 214,
            8220, 556, 171, 500, 8249, 333, 8250, 333, 64257, 500, 64258, 500, 8211, 500, 8224, 500,
            8225, 500, 183, 250, 182, 523, 8226, 350, 8218, 333, 8222, 556, 8221, 556, 187, 500,
            8230, 889, 8240, 1000, 191, 500, 96, 333, 180, 333, 710, 333, 732, 333, 175, 333,
            728, 333, 729, 333, 168, 333, 730, 333, 184, 333, 733, 333, 731, 333, 711, 333,
            8212, 889, 198, 889, 170, 276, 321, 556, 216, 722, 338, 944, 186, 310, 230, 667,
            305, 278, 322, 278, 248, 500, 339, 667, 223, 500, 207, 333, 233, 444, 259, 500,
            369, 500, 283, 444, 376, 556, 247, 675, 221, 556, 194, 611, 225, 500, 219, 722,
            253, 444, 537, 389, 234, 444, 366, 722, 220, 722, 261, 500, 218, 722, 371, 500,
            203, 611, 272, 722, 63171, 250, 169, 760, 274, 611, 269, 444, 229, 500, 325, 667,
            314, 278, 224, 500, 354, 556, 262, 667, 227, 500, 278, 611, 353, 389, 351, 389,
            237, 278, 9674, 471, 344, 611, 290, 722, 251, 500, 226, 500, 256, 611, 345, 389,
            231, 444, 379, 556, 222, 611, 332, 722, 340, 611, 346, 500, 271, 544, 362, 722,
            367, 500, 179, 300, 210, 722, 192, 611, 258, 611, 215, 675, 250, 500, 356, 556,
            8706, 476, 255, 444, 323, 667, 238, 278, 202, 611, 228, 500, 235, 444, 263, 444,
            324, 500, 363, 500, 327, 667, 205, 333, 177, 675, 166, 275, 174, 760, 286, 722,
            304, 333, 8721, 600, 200, 611, 341, 389, 333, 500, 377, 556, 381, 556, 8805, 549,
            208, 722, 199, 667, 316, 278, 357, 300, 281, 444, 370, 722, 193, 611, 196, 611,
            232, 444, 378, 389, 303, 278, 211, 722, 243, 500, 257, 500, 347, 389, 239, 278,
            212, 722, 217, 722, 8710, 612, 254, 500, 178, 300, 214, 722, 181, 500, 236, 278,
            337, 500, 280, 611, 273, 500, 190, 750, 350, 500, 318, 300, 310, 667, 313, 556,
            8482, 980, 279, 444, 204, 333, 298, 333, 317, 611, 189, 750, 8804, 549, 244, 500,
            241, 500, 368, 722, 201, 611, 275, 444, 287, 500, 188, 750, 352, 500, 536, 500,
            336, 722, 176, 400, 242, 500, 268, 667, 249, 500, 8730, 453, 270, 722, 343, 389,
            209, 667, 245, 500, 342, 611, 315, 556, 195, 611, 260, 611, 197, 611, 213, 722,
            380, 389, 282, 611, 302, 333, 311, 444, 8722, 675, 206, 333, 328, 500, 355, 278,
            172, 675, 246, 500, 252, 500, 8800, 549, 291, 500, 240, 500, 382, 389, 326, 500,
            185, 300, 299, 278, 8364, 500,
        };

        private static readonly ushort[] timesRoman = new ushort[]
        {
            32, 250, 33, 333, 34, 408, 35, 500, 36, 500, 37, 833, 38, 778, 8217, 333,
            40, 333, 41, 333, 42, 500, 43, 564, 44, 250, 45, 333, 46, 250, 47, 278,
            48, 500, 49, 500, 50, 500, 51, 500, 52, 500, 53, 500, 54, 500, 55, 500,
            56, 500, 57, 500, 58, 278, 59, 278, 60, 564, 61, 564, 62, 564, 63, 444,
            64, 921, 65, 722, 66, 667, 67, 667, 68, 722, 69, 611, 70, 556, 71, 722,
            72, 722, 73, 333, 74, 389, 75, 722, 76, 611, 77, 889, 78, 722, 79, 722,
            80, 556, 81, 722, 82, 667, 83, 556, 84, 611, 85, 722, 86, 722, 87, 944,
            88, 722, 89, 722, 90, 611, 91, 333, 92, 278, 93, 333, 94, 469, 95, 500,
            8216, 333, 97, 444, 98, 500, 99, 444, 100, 500, 101, 444, 102, 333, 103, 500,
            104, 500, 105, 278, 106, 278, 107, 500, 108, 278, 109, 778, 110, 500, 111, 500,
            112, 500, 113, 500, 114, 333, 115, 389, 116, 278, 117, 500, 118, 500, 119, 722,
            120, 500, 121, 500, 122, 444, 123, 480, 124, 200, 125, 480, 126, 541, 161, 333,
            162, 500, 163, 500, 8260, 167, 165, 500, 402, 500, 167, 500, 164, 500, 39, 180,
            8220, 444, 171, 500, 8249, 333, 8250, 333, 64257, 556, 64258, 556, 8211, 500, 8224, 500,
            8225, 500, 183, 250, 182, 453, 8226, 350, 8218, 333, 8222, 444, 8221, 444, 187, 500,
            8230, 1000, 8240, 1000, 191, 444, 96, 333, 180, 333, 710, 333, 732, 333, 175, 333,
            728, 333, 729, 333, 168, 333, 730, 333, 184, 333, 733, 333, 731, 333, 711, 333,
            8212, 1000, 198, 889, 170, 276, 321, 611, 216, 722, 338, 889, 186, 310, 230, 667,
            305, 278, 322, 278, 248, 500, 339, 722, 223, 500, 207, 333, 233, 444, 259, 444,
            369, 500, 283, 444, 376, 722, 247, 564, 221, 722, 194, 722, 225, 444, 219, 722,
            253, 500, 537, 389, 234, 444, 366, 722, 220, 722, 261, 444, 218, 722, 371, 500,
            203, 611, 272, 722, 63171, 250, 169, 760, 274, 611, 269, 444, 229, 444, 325, 722,
            314, 278, 224, 444, 354, 611, 262, 667, 227, 444, 278, 611, 353, 389, 351, 389,
            237, 278, 9674, 471, 344, 667, 290, 722, 251, 500, 226, 444, 256, 722, 345, 333,
            231, 444, 379, 611, 222, 556, 332, 722, 340, 667, 346, 556, 271, 588, 362, 722,
            367, 500, 179, 300, 210, 722, 192, 722, 258, 722, 215, 564, 250, 500, 356, 611,
            8706, 476, 255, 500, 323, 722, 238, 278, 202, 611, 228, 444, 235, 444, 263, 444,
            324, 500, 363, 500, 327, 722, 205, 333, 177, 564, 166, 200, 174, 760, 286, 722,
            304, 333, 8721, 600, 200, 611, 341, 333, 333, 500, 377, 611, 381, 611, 8805, 549,
            208, 722, 199, 667, 316, 278, 357, 326, 281, 444, 370, 722, 193, 722, 196, 722,
            232, 444, 378, 444, 303, 278, 211, 722, 243, 500, 257, 444, 347, 389, 239, 278,
            212, 722, 217, 722, 8710, 612, 254, 500, 178, 300, 214, 722, 181, 500, 236, 278,
            337, 500, 280, 611, 273, 500, 190, 750, 350, 556, 318, 344, 310, 722, 313, 611,
            8482, 980, 279, 444, 204, 333, 298, 333, 317, 611, 189, 750, 8804, 549, 244, 500,
            241, 500, 368, 722, 201, 611, 275, 444, 287, 500, 188, 750, 352, 556, 536, 556,
            336, 722, 176, 400, 242, 500, 268, 667, 249, 500, 8730, 453, 270, 722, 343, 333,
            209, 722, 245, 500, 342, 667, 315, 611, 195, 722, 260, 722, 197, 722, 213, 722,
            380, 444, 282, 611, 302, 333, 311, 500, 8722, 564, 206, 333, 328, 500, 355, 278,
            172, 564, 246, 500, 252, 500, 8800, 549, 291, 500, 240, 500, 382, 444, 326, 500,
            185, 300, 299, 278, 8364, 500,
        };

        private static readonly Dictionary<PdfName, ushort[]> variableWidth = new Dictionary<PdfName, ushort[]>
        {
            { StandardFonts.TimesRoman, timesRoman },
            { StandardFonts.TimesItalic, timesItalic },
            { StandardFonts.TimesBold, timesBold },
            { StandardFonts.TimesBoldItalic, timesBoldItalic },

            { StandardFonts.Helvetica, helvetica },
            { StandardFonts.HelveticaOblique, helveticaOblique },
            { StandardFonts.HelveticaBold, helveticaBold },
            { StandardFonts.HelveticaBoldOblique, helveticaBoldOblique },

            { StandardFonts.Symbol, symbol },
        };

        private static readonly Dictionary<PdfName, double> monoWidth = new Dictionary<PdfName, double>
        {
            { StandardFonts.Courier, 600 },
            { StandardFonts.CourierOblique, 600 },
            { StandardFonts.CourierBold, 600 },
            { StandardFonts.CourierBoldOblique, 600 },
        };

        public static WidthMap? GetWidths(PdfName fontName)
        {
            fontName = StandardFonts.TranslateAlternativeNames(fontName);

            if (monoWidth.TryGetValue(fontName, out var charWidth))
            {
                return new MonospaceWidthMap(charWidth * WidthMultiplier);
            }

            if (variableWidth.TryGetValue(fontName, out var widthData))
            {
                return new UnicodeWidthMap(widthData, WidthMultiplier);
            }

            return null;
        }
    }
}
