using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;
using System.Runtime.Loader;

namespace PluginManagent {
    public class PluginAssemblyLoadContext : AssemblyLoadContext {
   

        public PluginAssemblyLoadContext(string name):base(name,true) {
            
        }

    }

    public class PluginContexts {
        private Dictionary<string, PluginAssemblyLoadContext> plugins = new Dictionary<string, PluginAssemblyLoadContext>();

        public bool IsExists(string name) => plugins.ContainsKey(name);

        public void Add(string name, PluginAssemblyLoadContext context) {
            if (!IsExists(name)) {
                plugins.Add(name, context); 
            }
        }

        public void Remove(string name) {
            if (IsExists(name)) {
                plugins[name].Unload();
                plugins.Remove(name);
            }
        }
    }
}
