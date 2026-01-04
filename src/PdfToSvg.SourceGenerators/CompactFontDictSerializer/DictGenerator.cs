// Copyright (c) PdfToSvg.NET contributors.
// https://github.com/dmester/pdftosvg.net
// Licensed under the MIT License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PdfToSvg.SourceGenerators.CompactFontDictSerializer
{
    [Generator]
    public class DictGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classes = context.SyntaxProvider
                .CreateSyntaxProvider(
                    (syntaxNode, _) => 
                        syntaxNode is ClassDeclarationSyntax classDeclaration &&
                        classDeclaration.HasAttribute("CompactFontDictAttribute") &&
                        classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)),
                    (context, _) =>
                        context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol)
                .Where(symbol => symbol is not null);

            var compilationAndClasses = context.CompilationProvider.Combine(classes.Collect());

            context.RegisterSourceOutput(
                compilationAndClasses,
                (spc, source) => Generate(spc, source.Left, source.Right!));
        }

        private static List<DictProperty> GetProperties(INamedTypeSymbol classSymbol, INamedTypeSymbol operatorAttribute)
        {
            return classSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .SelectMany(property => property
                    .GetAttributes()
                    .Where(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, operatorAttribute))
                    .Select(attribute =>
                    {
                        var args = attribute.ConstructorArguments;
                        if (args.Length < 1)
                        {
                            return null;
                        }

                        var value1 = attribute.ConstructorArguments[0].Value;
                        if (!(value1 is int value))
                        {
                            return null;
                        }

                        if (args.Length > 1)
                        {
                            var value2 = attribute.ConstructorArguments[1].Value;
                            if (value2 is int iValue2)
                            {
                                value = (value << 8) | iValue2;
                            }
                        }

                        var order = attribute.GetNamedArgumentValue("Order") as int? ?? 0;
                        var isNullableValueType = property.Type.IsNullableValueType();

                        return new DictProperty
                        {
                            Name = property.Name,
                            Type = property.Type.GetNonNullableType(),
                            IsNullable = property.Type.IsReferenceType || isNullableValueType,
                            IsNullableValueType = isNullableValueType,
                            Location = property.Locations.FirstOrDefault(),
                            Operator = value,
                            Order = order,
                        };
                    }))
                .WhereNotNull()

                .OrderBy(p => p.Order)
                .ThenBy(p => p.Operator)

                .ToList();
        }

        private static void Generate(SourceProductionContext context, Compilation compilation, ImmutableArray<INamedTypeSymbol> classSymbols)
        {
            var operatorAttribute = compilation.GetTypeByMetadataNameOrThrow("PdfToSvg.Fonts.CompactFonts.CompactFontDictOperatorAttribute");

            foreach (var classSymbol in classSymbols)
            {
                var writer = new SourceWriter();

                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using PdfToSvg.Drawing;");
                writer.WriteLine("using PdfToSvg.DocumentModel;");
                writer.WriteLine("using PdfToSvg.Fonts.CompactFonts;");

                writer.WriteLine("using InputDictData = System.Collections.Generic.Dictionary<int, double[]>;");
                writer.WriteLine("using OutputDictData = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, double[]>>;");
                writer.WriteLine();

                writer.WriteLine($"namespace {classSymbol.ContainingNamespace}");
                using (writer.BeginBlock())
                {
                    writer.WriteLine($"{classSymbol.DeclaredAccessibility.ToDisplayString()} partial class {classSymbol.Name}");
                    using (writer.BeginBlock())
                    {
                        var properties = GetProperties(classSymbol, operatorAttribute);

                        GenerateSerialize(context, writer, classSymbol, properties);
                        GenerateDeserialize(context, writer, classSymbol, properties);
                    }
                }
                writer.WriteLine();

                context.AddSource(classSymbol.Name + ".g.cs", SourceText.From(writer.ToString(), Encoding.UTF8));
            }
        }

        private static void GenerateSerialize(SourceProductionContext context, SourceWriter writer, INamedTypeSymbol classSymbol, List<DictProperty> properties)
        {
            writer.WriteLine($"public OutputDictData Serialize(CompactFontStringTable strings, bool readOnlyStrings)");
            using (writer.BeginBlock())
            {
                writer.WriteLine($"var target = new OutputDictData();");
                writer.WriteLine($"var defaultValues = new {classSymbol.Name}();");
                writer.WriteLine();

                foreach (var property in properties)
                {
                    string inclusionCondition;
                    string? value = null;

                    var nonNullableSourceValue = "this." + property.Name;
                    if (property.IsNullableValueType)
                    {
                        nonNullableSourceValue += ".GetValueOrDefault()";
                    }

                    if (property.Type is IArrayTypeSymbol arrayType)
                    {
                        // Array value
                        inclusionCondition = $"this.{property.Name} is not null && !CompactFontDictSerializationUtils.AreEqual(this.{property.Name}, defaultValues.{property.Name})";

                        switch (arrayType.ElementType.Name)
                        {
                            case "Int32":
                                value = $"CompactFontDictSerializationUtils.ConvertIntArrayToDoubleArray({nonNullableSourceValue})";
                                break;

                            case "Double":
                                value = nonNullableSourceValue;
                                break;
                        }
                    }
                    else
                    {
                        // Scalar value
                        inclusionCondition =
                            (property.IsNullable
                                ? $"this.{property.Name} != null && "
                                : $"") +
                            $"this.{property.Name} != defaultValues.{property.Name}";

                        switch (property.Type.Name)
                        {
                            case "Int32":
                            case "Double":
                                value = nonNullableSourceValue;
                                break;

                            case "String":
                                value = $"readOnlyStrings ? strings.Lookup({nonNullableSourceValue}) : strings.AddOrLookup({nonNullableSourceValue})";
                                break;

                            case "Boolean":
                                value = $"{nonNullableSourceValue} ? 1d : 0d";
                                break;
                        }

                        if (value is not null)
                        {
                            value = $"new double[] {{ {value} }}";
                        }
                    }

                    if (value is null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            id: "PTS0002",
                            category: "Generator",
                            message: $"Property '{classSymbol.Name}.{property.Name}' is decorated with [CompactFontDictOperation] but has a not supported scalar type. Only int, double, string, bool, int[] and double[] are supported. Found '{property.Type.Name}'",
                            DiagnosticSeverity.Error,
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true,
                            warningLevel: 0,
                            location: property.Location));
                        continue;
                    }

                    writer.WriteLine($"if ({inclusionCondition})");
                    using (writer.BeginBlock())
                    {
                        writer.WriteLine($"target.Add(new KeyValuePair<int, double[]>(0x{property.Operator:x4}, {value}));");
                    }

                    writer.WriteLine();
                }

                writer.WriteLine($"return target;");
            }

            writer.WriteLine();
        }

        private static void GenerateDeserialize(SourceProductionContext context, SourceWriter writer, INamedTypeSymbol classSymbol, List<DictProperty> properties)
        {
            writer.WriteLine($"public void Deserialize(");
            writer.WriteLine($"    InputDictData dictData,");
            writer.WriteLine($"    CompactFontStringTable strings)");
            using (writer.BeginBlock())
            {
                foreach (var property in properties)
                {
                    var varName = "prop_" + property.Name;

                    string? value = null;
                    var nullValue = "default";

                    if (property.Type is IArrayTypeSymbol arrayType)
                    {
                        switch (arrayType.ElementType.Name)
                        {
                            case "Int32":
                                value = $"CompactFontDictSerializationUtils.ConvertDoubleArrayToIntArray({varName})";
                                break;

                            case "Double":
                                value = varName;
                                break;

                            default:
                                throw new Exception("No generated value accessor. This should have been captured in the serializer generator. Type: " + property.Type);
                        }

                        writer.WriteLine($"if (dictData.TryGetValue(0x{property.Operator:x4}, out var {varName}))");
                        using (writer.BeginBlock())
                        {
                            writer.WriteLine($"this.{property.Name} = {value};");
                        }

                        writer.WriteLine();
                    }
                    else
                    {
                        switch (property.Type.Name)
                        {
                            case "Int32":
                                value = $"(int){varName}[0]";
                                break;

                            case "String":
                                value = $"strings.Lookup((int){varName}[0])";
                                break;

                            case "Double":
                                value = $"{varName}[0]";
                                if (!property.IsNullable)
                                {
                                    nullValue = "double.NaN";
                                }
                                break;

                            case "Boolean":
                                value = $"{varName}[0] != 0d";
                                break;

                            default:
                                throw new Exception("No generated value accessor. This should have been captured in the serializer generator. Type: " + property.Type);
                        }

                        writer.WriteLine($"if (dictData.TryGetValue(0x{property.Operator:x4}, out var {varName}))");
                        using (writer.BeginBlock())
                        {
                            writer.WriteLine($"if ({varName}.Length > 0) this.{property.Name} = {value};");
                            writer.WriteLine($"else this.{property.Name} = {nullValue};");
                        }

                        writer.WriteLine();
                    }
                }
            }

            writer.WriteLine();
        }
    }
}
