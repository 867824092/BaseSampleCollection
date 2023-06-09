using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerator {
    [Generator]
    public class ServerSourceGenerator : ISourceGenerator {
        public void Initialize(GeneratorInitializationContext context) {
            //throw new System.NotImplementedException();
            context.RegisterForSyntaxNotifications(()=> new InterfaceReceiver());
        }
        public void Execute(GeneratorExecutionContext context) {
            if (context.SyntaxReceiver is InterfaceReceiver receiver) {
                foreach (InterfaceDeclarationSyntax interfaceDeclaration in receiver.Interfaces) {
                    foreach (IMethodSymbol methodSymbol in GetInterfaceMethods(context, interfaceDeclaration)) {

                    }
                }
            }
        }
        private IEnumerable<IMethodSymbol> GetInterfaceMethods(GeneratorExecutionContext context, InterfaceDeclarationSyntax interfaceDeclaration) {
            SemanticModel semanticModel = context.Compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(interfaceDeclaration) is INamedTypeSymbol interfaceSymbol)
                yield break;

            INamedTypeSymbol myInterfaceSymbol = semanticModel.Compilation.GetTypeByMetadataName("IMyInterface");
            if (myInterfaceSymbol == null)
                yield break;

            // foreach (ISymbol member in interfaceSymbol.GetMembers()) {
            //     if (member is IMethodSymbol methodSymbol && methodSymbol.ContainingType.AllInterfaces.Contains(myInterfaceSymbol)) {
            //         yield return methodSymbol;
            //     }
            // }
        }
    }
    [Generator]
    public class InterfaceReceiver : ISyntaxReceiver {
        public List<InterfaceDeclarationSyntax> Interfaces { get; } = new List<InterfaceDeclarationSyntax>();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is not InterfaceDeclarationSyntax interfaceDeclarationSyntax)
                return;
            //判断是否是自定义的接口
            if (interfaceDeclarationSyntax.Identifier.Text.StartsWith("I")) {

                var attributes = interfaceDeclarationSyntax.AttributeLists.SelectMany(x => x.Attributes);

                bool hasSpecialAttribute = attributes.Any(a =>
                    a.Name.ToString().Equals("RPCService", StringComparison.OrdinalIgnoreCase) &&
                    a.Name?.GetFirstToken().ValueText != null);
                if (hasSpecialAttribute) {
                    Interfaces.Add(interfaceDeclarationSyntax);
                }
            }
        }
    }

    [Generator]
    public class ServerIncrementalGenerator : IIncrementalGenerator {
        public const string ServiceAttributeMetadataName = "SourceGenConsole.RPC.ServiceAttribute";
        public const string EndpointAttributeMetadataName = "SourceGenConsole.RPC.EndpointAttribute";
        public void Initialize(IncrementalGeneratorInitializationContext context) {
            IncrementalValuesProvider<InterfaceDeclarationSyntax> classDeclarations = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    ServiceAttributeMetadataName,
                    (node, _) => node is InterfaceDeclarationSyntax,
                    (context, _) => context.TargetNode as InterfaceDeclarationSyntax)
                .Where(static m => m is not null);

            IncrementalValueProvider<(Compilation, ImmutableArray<InterfaceDeclarationSyntax>)> compilationAndClasses =
                context.CompilationProvider.Combine(classDeclarations.Collect());


            context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static void Execute(Compilation compilation, ImmutableArray<InterfaceDeclarationSyntax> classes, SourceProductionContext context) {
            if (classes.IsDefaultOrEmpty) {
                // nothing to do yet
                return;
            }
            IEnumerable<InterfaceDeclarationSyntax> distinctClasses = classes.Distinct();
            INamedTypeSymbol endpointAttributeSymbol = compilation.GetTypeByMetadataName(EndpointAttributeMetadataName);
            foreach (InterfaceDeclarationSyntax interfaceDeclaration in distinctClasses) {
                StringBuilder stringBuilder = new StringBuilder();
                string classBaseName = interfaceDeclaration.Identifier.Text.AsSpan(1).ToString() + "Base";
                var semanticModel = compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);
                //查找符号下的所有using
                var usings = interfaceDeclaration.SyntaxTree.GetCompilationUnitRoot().Usings;
                foreach (UsingDirectiveSyntax usingDirective in usings) {
                    stringBuilder.AppendLine(string.Format("using {0};",usingDirective.Name));
                }
                //查找符号下的命名空间
                var namespaceDeclaration = interfaceDeclaration.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                stringBuilder.AppendLine($"namespace {namespaceDeclaration.Name} {{");
                stringBuilder.AppendLine($"  public abstract partial class {classBaseName} : {interfaceDeclaration.Identifier.Text}");
                stringBuilder.AppendLine("   {");
                var methods = interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>();
                foreach (MethodDeclarationSyntax methodDeclaration in methods) {
                    var methodSemanticModel = compilation.GetSemanticModel(methodDeclaration.SyntaxTree);
                    ISymbol methodSymbol = methodSemanticModel.GetDeclaredSymbol(methodDeclaration);
                    if(methodSymbol.GetAttributes().Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, endpointAttributeSymbol))) {

                        stringBuilder.AppendLine("public abstract ");
                    }

                }
                stringBuilder.AppendLine("   }");
                stringBuilder.AppendLine("}");
                context.AddSource($"{classBaseName}.g.s", SourceText.From(stringBuilder.ToString(), Encoding.UTF8));
            }
        }
    }

}