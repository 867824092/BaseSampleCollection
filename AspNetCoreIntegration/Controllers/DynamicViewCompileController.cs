using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using AspNetCoreIntegration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.AspNetCore.Razor;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AspNetCoreIntegration.Controllers
{
    [Route("api/dvc")]
    [ApiController]
    public class DynamicViewCompileController : ControllerBase
    {
        [HttpGet]
        public async Task<string> Get() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("@using System.Linq");
            sb.AppendLine("@using System.Collections");
            sb.AppendLine("@using System.Collections.Generic");
            sb.AppendLine("@inherits AspNetCoreIntegration.Models.RazorEngineTemplateBase");
            sb.AppendLine("Hello @Model.Name, welcome to Razor World!");
            var fileContent = sb.ToString();
            string fileName = Path.GetRandomFileName();
            RazorProjectEngine engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(@"."),
                (builder) =>
                {
                    builder.SetNamespace("Dynamic.Views");
                });
            RazorSourceDocument document = RazorSourceDocument.Create(fileContent, fileName);

            RazorCodeDocument codeDocument = engine.Process(
                document,
                null,
                new List<RazorSourceDocument>(),
                new List<TagHelperDescriptor>());

            RazorCSharpDocument razorCSharpDocument = codeDocument.GetCSharpDocument();
            var syntaxTree = CSharpSyntaxTree.ParseText(razorCSharpDocument.GeneratedCode);
            Console.WriteLine(syntaxTree.ToString());
            var compilation = CSharpCompilation.Create(
                fileName,
                new[]
                { syntaxTree },
                new[]
                { MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                  MetadataReference.CreateFromFile(typeof(System.Collections.IList).Assembly.Location),
                  MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                  MetadataReference.CreateFromFile(typeof(RazorEngineTemplateBase).Assembly.Location),
                MetadataReference.CreateFromFile(AssemblyLoadContext.Default
                      .LoadFromAssemblyName(new AssemblyName("Microsoft.CSharp")).Location),
                  MetadataReference.CreateFromFile(AssemblyLoadContext.Default
                      .LoadFromAssemblyName(new AssemblyName("System.Runtime")).Location),
                  MetadataReference.CreateFromFile(AssemblyLoadContext.Default
                      .LoadFromAssemblyName(new AssemblyName("System.Linq")).Location),
                  MetadataReference.CreateFromFile(AssemblyLoadContext.Default
                      .LoadFromAssemblyName(new AssemblyName("System.Linq.Expressions")).Location),
                  MetadataReference.CreateFromFile(AssemblyLoadContext.Default
                      .LoadFromAssemblyName(new AssemblyName("System.Collections")).Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var assemblyStream = new MemoryStream();
            var result = compilation.Emit(assemblyStream);

            if (!result.Success) {
                throw new Exception(string.Join(" , ", result.Diagnostics.ToList()));
            }

            assemblyStream.Seek(0, SeekOrigin.Begin);

            var assembly = Assembly.Load(assemblyStream.ToArray());

            var templateType = assembly.GetType("Dynamic.Views.Template");
            var instance = (RazorEngineTemplateBase)Activator.CreateInstance(templateType);
            instance.Model =new AnonymousTypeWrapper(new
            {
            Name = "Alex"
            });

            await instance.ExecuteAsync();
            return instance.Result();
        }
    }
    
    public class MemoryRazorProjectItem : RazorProjectItem
    {
        public MemoryRazorProjectItem(
            string filePath,
            string content = "Default content",
            string physicalPath = null,
            string relativePhysicalPath = null,
            string basePath = "/")
        {
            FilePath = filePath;
            PhysicalPath = physicalPath;
            RelativePhysicalPath = relativePhysicalPath;
            BasePath = basePath;
            Content = content;
        }

        public override string BasePath { get; }

        public override string FilePath { get; }

        public override string PhysicalPath { get; }

        public override string RelativePhysicalPath { get; }

        public override bool Exists => true;

        public string Content { get; set; }

        public override Stream Read()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(Content));

            return stream;
        }
    }
    
    public class _Views_Something_cshtml : RazorPage<dynamic>
    {
        public override async Task ExecuteAsync()
        {
            var output = "Getting old ain't for wimps! - Anonymous";

            WriteLiteral("/r/n<div>Quote of the Day: ");
            Write(output);
            WriteLiteral("</div>");
        }
    }
}
