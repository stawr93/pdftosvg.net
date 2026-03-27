// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfToSvg.SourceGenerators
{
    internal static class Utils
    {
        public static LiteralExpressionSyntax LiteralExpression(object? value)
        {
            if (value is null)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            }

            SyntaxToken numericToken;

            switch (value)
            {
                case bool b:
                    return SyntaxFactory.LiteralExpression(b ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression);

                case string s:
                    return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(s));

                case char c:
                    return SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, SyntaxFactory.Literal(c));

                case short sh:
                    numericToken = SyntaxFactory.Literal(sh);
                    break;

                case ushort ush:
                    numericToken = SyntaxFactory.Literal(ush);
                    break;
                case int i:
                    numericToken = SyntaxFactory.Literal(i);
                    break;
                case uint ui:
                    numericToken = SyntaxFactory.Literal(ui);
                    break;
                case long l:
                    numericToken = SyntaxFactory.Literal(l);
                    break;
                case ulong ul:
                    numericToken = SyntaxFactory.Literal(ul);
                    break;
                case float f:
                    numericToken = SyntaxFactory.Literal(f);
                    break;
                case double d:
                    numericToken = SyntaxFactory.Literal(d);
                    break;
                case decimal m:
                    numericToken = SyntaxFactory.Literal(m);
                    break;

                default:
                    throw new ArgumentException("Unsupported type");
            }

            return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, numericToken);
        }
    }
}
