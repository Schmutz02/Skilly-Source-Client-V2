using System;
using System.Threading.Tasks;
using Models;

namespace Networking.WebRequestHandlers
{
    public class LogInRequestHandler
    {
        private readonly string _username;
        private readonly string _password;
        private readonly bool _rememberUsername;

        public event Action<string> OnError;
    
        public LogInRequestHandler(string username, string password, bool rememberUsername)
        {
            _username = username;
            _password = password;
            _rememberUsername = rememberUsername;
        }

        public async Task SendRequestAsync()
        {
            var result = await WebRequestSender.SendLogInRequest(_username, _password);
            
            await OnLogInRequestComplete(result);
        }

        private async Task OnLogInRequestComplete(WebRequestResult result)
        {
            if (result.Success)
            {
                Account.Login(_username, _rememberUsername);

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