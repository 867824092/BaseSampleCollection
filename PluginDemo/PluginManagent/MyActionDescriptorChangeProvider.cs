using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace PluginManagent {
    public class MyActionDescriptorChangeProvider : IActionDescriptorChangeProvider {
        public static MyActionDescriptorChangeProvider Instance { get; } = new MyActionDescriptorChangeProvider();

        private CancellationTokenSource _tokenSource = new();

        public IChangeToken GetChangeToken() => new CancellationChangeToken(_tokenSource.Token);

        public void Change() {
            var current = Interlocked.Exchange(ref _tokenSource, new CancellationTokenSource());
            current.Cancel();
        }
    }
}
