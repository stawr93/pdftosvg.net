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
using System.Linq;
using System.Text;

namespace PdfToSvg.SourceGenerators.OperationProxy
{
    [Generator]
    public class OperatorGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classes = context.SyntaxProvider
                .CreateSyntaxProvider(
                    (syntaxNode, _) =>
                        syntaxNode is ClassDeclarationSyntax classDeclaration &&
                        classDeclaration.HasAttribute("OperationTargetAttribute") &&
                        classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)),
                    (context, _) =>
                        context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol)
                .Where(symbol => symbol is not null);

            var compilationAndClasses = context.CompilationProvider.Combine(classes.Collect());

            context.RegisterSourceOutput(
                compilationAndClasses,
                (spc, source) => Generate(spc, source.Left, source.Right!));
        }

        private static List<ProxyMethod> GetProxyMethods(SourceProductionContext context, Compilation compilation, INamedTypeSymbol classSymbol)
        {
            var taskClass = compilation.GetTypeByMetadataNameOrThrow("System.Threading.Tasks.Task");
            var operationAttribute = compilation.GetTypeByMetadataNameOrThrow("PdfToSvg.Drawing.OperationAttribute");

            return classSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => method.HasAttribute(operationAttribute))
                .SelectMany(method =>
                {
                    bool isAsync;

                    if (method.ReturnsVoid)
                    {
                        isAsync = false;
                    }
                    else if (SymbolEqualityComparer.Default.Equals(method.ReturnType, taskClass))
                    {
                        isAsync = true;
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            id: "PTS0001",
                            category: "Generator",
                            message: $"Method '{method.Name}' is decorated with [Operation] but does not have one of the allowed return types Task or void",
                            DiagnosticSeverity.Error,
                            DiagnosticSeverity.Error,
                            isEnabledByDefault: true,
                            warningLevel: 0,
                            location: method.Locations.FirstOrDefault()));
                        isAsync = false;
                    }

                    var operators = method.GetAttributes()
                        .Where(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, operationAttribute))
                        .Select(a => a.ConstructorArguments[0].Value)
                        .OfType<string>();

                    return operators
                        .Select(op => new
                        {
                            Operator = op,
                            TargetMethod = method,
                            IsAsync = isAsync,
                        });
                })
                .GroupBy(method => (method.IsAsync, method.Operator))
                .Select(group => new ProxyMethod
                {
                    Operator = group.Key.Operator,
                    IsAsync = group.Key.IsAsync,
                    ProxyMethodName = GetProxyMethodName(group.Key.Operator, group.Key.IsAsync),
                    TargetMethods = group
                        .Select(m => m.TargetMethod)
                        .ToList(),
                })
                .ToList();
        }

        private static void Generate(SourceProductionContext context, Compilation compilation, ImmutableArray<INamedTypeSymbol> classSymbols)
        {
            var variadicAttribute = compilation.GetTypeByMetadataNameOrThrow("PdfToSvg.Drawing.VariadicParamAttribute");

            foreach (var classSymbol in classSymbols)
            {
                var proxyMethods = GetProxyMethods(context, compilation, classSymbol);

                if (proxyMethods.Count == 0)
                {
                    continue;
                }

                var writer = new SourceWriter();

                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using PdfToSvg.Drawing;");
                writer.WriteLine("using PdfToSvg.DocumentModel;");
                writer.WriteLine();

                writer.WriteLine($"namespace {classSymbol.ContainingNamespace}");
                using (writer.BeginBlock())
                {
                    writer.WriteLine($"{classSymbol.DeclaredAccessibility.ToDisplayString()} partial class {classSymbol.Name}");
                    using (writer.BeginBlock())
                    {
                        writer.WriteLine($"private static class Proxy");
                        using (writer.BeginBlock())
                        {
                            var hasAsyncMethods = proxyMethods.Any(pm => pm.IsAsync);

                            // Indexes
                            GenerateIndex(writer, classSymbol, proxyMethods, isAsync: false);

                            if (hasAsyncMethods)
                            {
                                GenerateIndex(writer, classSymbol, proxyMethods, isAsync: true);
                            }

                            // Entry methods
                            GenerateEntryMethod(writer, classSymbol, proxyMethods, isAsync: false);

                            if (hasAsyncMethods)
                            {
                                GenerateEntryMethod(writer, classSymbol, proxyMethods, isAsync: true);
                            }

                            // Proxy methods
                            foreach (var proxyMethod in proxyMethods)
                            {
                                GenerateProxyMethod(writer, classSymbol, proxyMethod, variadicAttribute);
                            }
                        }
                    }
                }
                writer.WriteLine();

                context.AddSource(classSymbol.Name + ".OperationProxies.g.cs", SourceText.From(writer.ToString(), Encoding.UTF8));
            }
        }

        private static void GenerateIndex(SourceWriter writer, INamedTypeSymbol classSymbol, IEnumerable<ProxyMethod> proxyMethods, bool isAsync)
        {
            if (isAsync)
            {
                writer.WriteLine($"private static Dictionary<string, Func<{classSymbol.Name}, object[], System.Threading.Tasks.Task<bool>>> asyncHandlers = new(StringComparer.Ordinal)");
            }
            else
            {
                writer.WriteLine($"private static Dictionary<string, Func<{classSymbol.Name}, object[], bool>> syncHandlers = new(StringComparer.Ordinal)");
            }

            using (writer.BeginBlock(suffix: ";"))
            {
                foreach (var proxyMethod in proxyMethods)
                {
                    if (proxyMethod.IsAsync == isAsync)
                    {
                        var op = Utils.LiteralExpression(proxyMethod.Operator).ToFullString();
                        writer.WriteLine($"{{ {op}, {proxyMethod.ProxyMethodName} }},");
                    }
                }
            }

            writer.WriteLine();
        }

        private static void GenerateEntryMethod(SourceWriter writer, INamedTypeSymbol classSymbol, IEnumerable<ProxyMethod> proxyMethods, bool isAsync)
        {
            var awaitKeyword = isAsync ? "await " : "";
            var configureAwait = isAsync ? ".ConfigureAwait(false)" : "";

            if (isAsync)
            {
                writer.WriteLine($"public static async System.Threading.Tasks.Task<bool> InvokeAsync({classSymbol.Name} instance, string name, object[] args)");
            }
            else
            {
                writer.WriteLine($"public static bool Invoke({classSymbol.Name} instance, string name, object[] args)");
            }

            using (writer.BeginBlock())
            {
                if (isAsync)
                {
                    writer.WriteLine($"if (asyncHandlers.TryGetValue(name, out var asyncHandler) &&");
                    writer.WriteLine($"    await asyncHandler(instance, args){configureAwait})");
                    using (writer.BeginBlock())
                    {
                        writer.WriteLine("return true;");
                    }
                }

                writer.WriteLine($"if (syncHandlers.TryGetValue(name, out var syncHandler) &&");
                writer.WriteLine($"    syncHandler(instance, args))");
                using (writer.BeginBlock())
                {
                    writer.WriteLine("return true;");
                }

                writer.WriteLine("return false;");
            }
            writer.WriteLine();
        }

        private static void GenerateProxyMethod(SourceWriter writer, INamedTypeSymbol classSymbol, ProxyMethod proxyMethod, INamedTypeSymbol variadicAttribute)
        {
            var awaitKeyword = proxyMethod.IsAsync ? "await " : "";
            var configureAwait = proxyMethod.IsAsync ? ".ConfigureAwait(false)" : "";

            if (proxyMethod.IsAsync)
            {
                writer.WriteLine($"private static async System.Threading.Tasks.Task<bool> {proxyMethod.ProxyMethodName}({classSymbol.Name} instance, object[] args)");
            }
            else
            {
                writer.WriteLine($"private static bool {proxyMethod.ProxyMethodName}({classSymbol.Name} instance, object[] args)");
            }

            using (writer.BeginBlock())
            {
                var hasConditionalTargets = false;

                foreach (var targetMethod in proxyMethod.TargetMethods.OrderByDescending(tm => tm.Parameters.Length))
                {
                    if (targetMethod.Parameters.Length == 0)
                    {
                        // No parameters
                        writer.WriteLine($"{awaitKeyword}instance.{targetMethod.Name}(){configureAwait};");
                        writer.WriteLine("return true;");
                    }
                    else
                    {
                        // Try to cast parameters
                        var indexVarScope = proxyMethod.TargetMethods.Count > 1 ? writer.BeginBlock() : null;
                        writer.WriteLine("var index = 0;");

                        var argumentNames = new List<string>();
                        var argumentCasts = new List<string>();

                        foreach (var parameter in targetMethod.Parameters)
                        {
                            var argumentName = "arg" + parameter.Ordinal;
                            argumentNames.Add(argumentName);

                            if (parameter.IsParams || parameter.HasAttribute(variadicAttribute))
                            {
                                // Variadic parameter
                                var elementType = ((IArrayTypeSymbol)parameter.Type).ElementType;
                                var tryCast = GetTryCastMethod(elementType);
                                argumentCasts.Add($"OperationArgument.TryCastVariadicArgument<{elementType.ToDisplayString()}>(args, ref index, {tryCast}, out var {argumentName})");
                            }
                            else if (parameter.IsOptional)
                            {
                                // Optional parameter
                                var tryCast = GetTryCastMethod(parameter.Type);
                                var nonNullableType = parameter.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
                                var defaultValue = parameter.HasExplicitDefaultValue
                                    ? Utils.LiteralExpression(parameter.ExplicitDefaultValue).ToFullString()
                                    : "default";

                                argumentCasts.Add($"OperationArgument.TryCastOptionalArgument<{nonNullableType.ToDisplayString()}>(args, ref index, {defaultValue ?? "default"}, {tryCast}, out var {argumentName})");
                            }
                            else
                            {
                                // Scalar parameter
                                var tryCast = GetTryCastMethod(parameter.Type);
                                argumentCasts.Add($"{tryCast}(args, ref index, out var {argumentName})");
                            }
                        }

                        // Cast arguments
                        for (var i = 0; i < argumentCasts.Count; i++)
                        {
                            var prefix = i == 0 ? "if (" : "    ";
                            var suffix = i == argumentCasts.Count - 1 ? ")" : " &&";
                            writer.WriteLine(prefix + argumentCasts[i] + suffix);
                        }

                        // Call target method
                        using (writer.BeginBlock())
                        {
                            var callArgs = string.Join(", ", argumentNames);
                            writer.WriteLine($"{awaitKeyword}instance.{targetMethod.Name}({callArgs}){configureAwait};");
                            writer.WriteLine("return true;");
                        }

                        indexVarScope?.Dispose();
                        hasConditionalTargets = true;
                    }
                }

                if (hasConditionalTargets)
                {
                    writer.WriteLine("return false;");
                }
            }

            writer.WriteLine();
        }

        private static string GetProxyMethodName(string value, bool isAsync)
        {
            return "Op_" + 
                string.Concat(value
                    .Select(ch =>
                        char.IsLetterOrDigit(ch) ? ch.ToString() :
                        ch == '"' ? "DQuot" :
                        ch == '\'' ? "SQuot" :
                        ch == '*' ? "Star" :
                        "_"
                    )) +
                (isAsync ? "Async" : "");
        }

        private static string? GetTryCastMethod(ITypeSymbol type)
        {
            var name = type.Name;

            switch (name)
            {
                case "Single":
                    return "OperationArgument.TryCastFloatArgument";

                case "Int32":
                    return "OperationArgument.TryCastIntArgument";

                case "Double":
                    return "OperationArgument.TryCastDoubleArgument";
            }

            if (type is IArrayTypeSymbol arrayType)
            {
                var elementCast = GetTryCastMethod(arrayType.ElementType);

                var nonNullableElement = arrayType.ElementType.IsReferenceType
                    ? arrayType.ElementType.WithNullableAnnotation(NullableAnnotation.NotAnnotated)
                    : arrayType.ElementType;

                return $"OperationArgument.TryCastArrayArgument<{nonNullableElement.ToDisplayString()}>({elementCast})";
            }

            return $"OperationArgument.TryCastArgument<{type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString()}>";
        }
    }
}
