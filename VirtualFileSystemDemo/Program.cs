// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Text;

//new ServiceCollection().AddSingleton<IFileProvider>(new PhysicalFileProvider(@"F:\ffmpeg-4.4.1"))
//    .AddSingleton<IFileManager, FileManager>()
//    .BuildServiceProvider()
//    .GetRequiredService<IFileManager>()
//    //.ShowStructure((layer, name) => {
//    //    Console.WriteLine($"{new string(' ', layer * 4)}{name}");
//    //})
//    .ReadAllTextAsync(@"LICENSE.txt").Wait()
//    ;


using (var fileProvider = new PhysicalFileProvider(@"F:\ffmpeg-4.4.1")) {
    string original = null;
    ChangeToken.OnChange(() => fileProvider.Watch("data.txt"), Callback);
    while (true) {
        File.WriteAllText(@"F:\ffmpeg-4.4.1\data.txt", DateTime.Now.ToString());
        Task.Delay(5 * 1000).Wait();
    }
    async void Callback() {
        var stream = fileProvider.GetFileInfo("data.txt").CreateReadStream();
        {
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string current = Encoding.Default.GetString(buffer);
            if (current != original) {
                Console.WriteLine(original = current);
            }
        }
    }
}



public interface IFileManager {
    void ShowStructure(Action<int, string> render);
    Task ReadAllTextAsync(string path);
}

public class FileManager : IFileManager {
    private readonly IFileProvider _fileProvider;
    public FileManager(IFileProvider fileProvider) {
        _fileProvider = fileProvider;
    }
   
public void ShowStructure(Action<int, string> render) {
        int indent = -1;
        Render("");
        void Render(string subPath) {
            indent++;
            foreach (var fileinfo in _fileProvider.GetDirectoryContents(subPath)) {
                render(indent,fileinfo.Name);
                if (fileinfo.IsDirectory) {
                    Render($@"{subPath}\{fileinfo.Name}".TrimStart('\\'));
                }
            }
            indent--;
        }
    }
    public async Task ReadAllTextAsync(string path) {
        byte[] buffer;
        using (var stream = _fileProvider.GetFileInfo(path).CreateReadStream()) {
            buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length);
        }
        Console.WriteLine(Encoding.Default.GetString(buffer));
    }
}
