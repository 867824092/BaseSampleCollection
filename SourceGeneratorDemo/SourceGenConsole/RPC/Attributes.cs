using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenConsole.RPC {
    [AttributeUsage(AttributeTargets.Interface)]
   public class ServiceAttribute : Attribute {
   }
    [AttributeUsage(AttributeTargets.Method)]
    public class EndpointAttribute : Attribute {
        public string Endpoint { get; }
        public EndpointAttribute(string endpoint)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }
    }
}
