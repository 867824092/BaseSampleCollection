using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;

namespace SourceGenConsole; 

public class RoslynSample {
    public static void Run() {
        {
                var tree = CSharpSyntaxTree.ParseText(@"class Test{ 
static void Main()=> Console.WriteLine(""Hello"");
}");
                SyntaxNode root = tree.GetRoot();
                Console.WriteLine(root.GetType().Name);
                Console.WriteLine("*****************");
                var cds = root.ChildNodes().Single() as ClassDeclarationSyntax;
                foreach (var member in cds!.Members) {
                    Console.WriteLine(member.ToString());
                }
                Console.WriteLine("*****************");
                // 关键字标记
                foreach (var token in root.DescendantTokens()) {
                    //Console.WriteLine($"{token.Kind(),-30} {token.Text}");
                    Console.WriteLine($"{token.Kind(),-30} {token.ToFullString()}");
                }

                Console.WriteLine("*****************");
                // var ourMethod = root.DescendantNodes()
                //     .First(m => m.Kind() == SyntaxKind.MethodDeclaration);
                // Console.WriteLine(ourMethod.ToString());
                var ourMethod = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();
                Console.WriteLine(ourMethod.ToString());
            }

            {
                var treeText = CSharpSyntaxTree.ParseText ("using System.Text;");
                var root = treeText.GetRoot();
                foreach (var token in  root.DescendantTokens()) {
                    Console.WriteLine($"{token.Kind(),-30} {token.ToFullString()}");
                }
            }
            {
                QualifiedNameSyntax qualifiedNameSyntax = SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("System"),
                    SyntaxFactory.IdentifierName("Text"));
                UsingDirectiveSyntax usingDirectiveSyntax = SyntaxFactory.UsingDirective(qualifiedNameSyntax);
                Console.WriteLine(usingDirectiveSyntax.ToFullString()); // using System.Text;

                usingDirectiveSyntax =
                    usingDirectiveSyntax.WithUsingKeyword(
                        usingDirectiveSyntax.UsingKeyword.WithTrailingTrivia(SyntaxFactory.Whitespace(" ")));
                Console.WriteLine(usingDirectiveSyntax.ToFullString()); // using System.Text;
                Console.WriteLine("               ");
                var existingTree = CSharpSyntaxTree.ParseText("class Program {}");
                var existingUnit = existingTree.GetRoot() as CompilationUnitSyntax;
                var unitWithUsing = existingUnit.AddUsings(usingDirectiveSyntax);
                var treeWithUsing = CSharpSyntaxTree.Create(unitWithUsing.NormalizeWhitespace());
                Console.WriteLine(treeWithUsing.ToString());
                
                Console.WriteLine("               ");
                // 从头开始创建
                var unit = SyntaxFactory.CompilationUnit()
                    .AddUsings(usingDirectiveSyntax.WithTrailingTrivia(SyntaxFactory.EndOfLine("\r\n\r\n")));
                unit = unit.AddMembers(SyntaxFactory.ClassDeclaration("Program"));
                var tree = CSharpSyntaxTree.Create(unit.NormalizeWhitespace());
                Console.WriteLine(tree.ToString());
            }
    }

    public static void CSharpSyntaxRewriter() {
        var tree = CSharpSyntaxTree.ParseText(@"class Program {
static void Main(){ Test(); }
static void Test(){  }
}");
        var rewriter = new MyRewriter();
        var newRoot = rewriter.Visit(tree.GetRoot());
        Console.WriteLine(newRoot.ToFullString());
    }

    public static void CompilationRun() {
        var compilation = CSharpCompilation.Create("test");
        compilation = compilation.WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        var tree = CSharpSyntaxTree.ParseText(@"class Program 
{
	static void Main() => System.Console.WriteLine (""Hello"");
}");
        compilation = compilation.AddSyntaxTrees(tree);
        string trustedAssemblies = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        var trustedAssemblyPaths = trustedAssemblies.Split (Path.PathSeparator);
        var references = 
            trustedAssemblyPaths.Select (path => MetadataReference.CreateFromFile (path));
        compilation = compilation.AddReferences(references);
        
        // compilation = CSharpCompilation
        //     .Create ("test")
        //     .WithOptions (new CSharpCompilationOptions (OutputKind.ConsoleApplication))
        //     .AddSyntaxTrees (tree)
        //     .AddReferences (references);

        foreach (var diagnostic in compilation.GetDiagnostics()) {
            Console.WriteLine(diagnostic.ToString());
        }

//         EmitResult result = compilation.Emit("test.dll");
//         Console.WriteLine(result.Success);
//         File.WriteAllText ("test.runtimeconfig.json", @$"{{
// 	""runtimeOptions"": {{
// 		""tfm"": ""net{Environment.Version.Major}.{Environment.Version.Minor}"",
// 		""framework"": {{
// 			""name"": ""Microsoft.NETCore.App"",
// 			""version"": ""{Environment.Version.Major}.{Environment.Version.Minor}.{Environment.Version.Build}""
// 		}}
// 	}}
// }}");
        EmitResult emitResult = compilation.Emit("test.exe");
        Console.WriteLine(string.Join("\r\n",emitResult.Diagnostics.Select(u=>u.GetMessage())));
    }

    public static void Symbol() {
        var tree = CSharpSyntaxTree.ParseText (@"class Program 
{
	static void Main() => System.Console.WriteLine (123);
}");

        var references = ((string)AppContext.GetData ("TRUSTED_PLATFORM_ASSEMBLIES"))
            .Split (Path.PathSeparator)
            .Select (path => MetadataReference.CreateFromFile (path));

        var compilation = CSharpCompilation.Create ("test")
            .AddReferences (references)
            .AddSyntaxTrees (tree);

        SemanticModel model = compilation.GetSemanticModel (tree);

        var writeLineNode = tree.GetRoot().DescendantTokens().Single (
            t => t.Text == "WriteLine").Parent;

        ISymbol symbol = model.GetSymbolInfo (writeLineNode).Symbol;

        Console.WriteLine (symbol.Name);                   // WriteLine
        Console.WriteLine (symbol.Kind);                   // Method
        Console.WriteLine (symbol.IsStatic);               // True
        Console.WriteLine (symbol.ContainingType.Name);    // Console

        var method = (IMethodSymbol)symbol;
        Console.WriteLine (method.ReturnType.ToString());  // void

        Console.WriteLine (symbol.Language);                // C#

        var location = symbol.Locations.First();
        Console.WriteLine (location.Kind);                       // MetadataFile
        Console.WriteLine (location.SourceTree == null);         // True
        Console.WriteLine (location.SourceSpan);                 // [0..0)

        Console.WriteLine(string.Join(" ",symbol.ContainingType.GetMembers ("WriteLine").OfType<IMethodSymbol>()
            .Select (m => m.ToString())));
    }
}

public class MyRewriter : CSharpSyntaxRewriter {
    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node) {
        return node.WithIdentifier(SyntaxFactory.Identifier(node.Identifier.LeadingTrivia,
            node.Identifier.Text.ToUpperInvariant(),
            node.Identifier.TrailingTrivia));
    }
}