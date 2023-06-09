using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenConsole.RPC {
    [Service]
    public interface ILoginService {
        [Endpoint("login")]
        Task Login(string username, string password);
    }

    public abstract class LoginService : ILoginService {
        public Task Login(string username, string password) {
            throw new NotImplementedException();
        }
    }
}
