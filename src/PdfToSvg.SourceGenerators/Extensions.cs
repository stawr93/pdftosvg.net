// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfToSvg.SourceGenerators
{
    internal static class Extensions
    {
        public static ITypeSymbol GetNonNullableType(this ITypeSymbol source)
        {
            if (source.IsReferenceType)
            {
                return source.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            }

            if (source is INamedTypeSymbol namedSymbol &&
                source.IsValueType &&
                source.ContainingNamespace.Name == "System" &&
                source.Name == "Nullable")
            {
                return namedSymbol.TypeArguments[0];
            }

            return source;
        }

        public static bool IsNullableValueType(this ITypeSymbol source)
        {
            return
                source is INamedTypeSymbol namedSymbol &&
                source.IsValueType &&
                source.ContainingNamespace.Name == "System" &&
                source.Name == "Nullable";
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
        {
            return source.Where(x => x != null)!;
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct
        {
            return source.Where(x => x.HasValue).Select(x => x!.Value);
        }

        public static object? GetNamedArgumentValue(this AttributeData attribute, string key)
        {
            foreach (var namedArg in attribute.NamedArguments)
            {
                if (namedArg.Key == key)
                {
                    return namedArg.Value.Value;
                }
            }

            return null;
        }

        public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol)
        {
            return symbol
                .GetAttributes()
                .Select(attr => attr.AttributeClass)
                .Contains(attributeSymbol, SymbolEqualityComparer.Default);
        }

        public static bool HasAttribute(this MemberDeclarationSyntax declaration, string name)
        {
            const string AttributeSuffix = "Attribute";

            var withoutSuffix = name;

            if (withoutSuffix.EndsWith(AttributeSuffix, StringComparison.Ordinal))
            {
                withoutSuffix = withoutSuffix.Substring(0, withoutSuffix.Length - AttributeSuffix.Length);
            }

            var withSuffix = withoutSuffix + AttributeSuffix;

            foreach (var attributeList in declaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var thisName = attribute.Name.ToString();

                    if (thisName == withoutSuffix ||
                        thisName == withSuffix)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static INamedTypeSymbol GetTypeByMetadataNameOrThrow(this Compilation compilation, string fullyQualifiedMetadataName)
        {
            var symbol = compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
            return symbol ?? throw new InvalidOperationException($"Cannot find type '{fullyQualifiedMetadataName}' in compilation.");
        }

        public static string ToDisplayString(this Accessibility value)
        {
            return value switch
            {
                Accessibility.Public => "public",
                Accessibility.Internal => "internal",
                Accessibility.Protected => "protected",
                Accessibility.ProtectedOrInternal => "protected internal",
                Accessibility.ProtectedAndInternal => "protected internal",
                _ => "private"
            };
        }
    }
}
