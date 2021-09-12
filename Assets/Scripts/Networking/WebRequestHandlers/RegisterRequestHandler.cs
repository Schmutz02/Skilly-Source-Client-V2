using System;
using System.Threading.Tasks;
using Models;

namespace Networking.WebRequestHandlers
{
    public class RegisterRequestHandler
    {
        private readonly string _username;
        private readonly string _password;
        private readonly bool _rememberUsername;
        
        public event Action<string> OnError;
    
        public RegisterRequestHandler(string username, string password, bool rememberUsername)
        {
            _username = username;
            _password = password;
            _rememberUsername = rememberUsername;
        }
        
        public async Task SendRequestAsync()
        {
            var result = await WebRequestSender.SendRegisterRequestAsync(_username, _password);
            
            await OnRegisterRequestComplete(result);
        }
        
        private async Task OnRegisterRequestComplete(WebRequestResult result)
        {
            if (result.Success)
            {
                Account.Login(_username, _password, _rememberUsername);

                var charListRequest = new CharListRequestHandler(_username, _password);
                await charListRequest.SendRequestAsync();
            }
            else
            {
                OnError?.Invoke(result.Response.Value);
            }
        }
    }
}