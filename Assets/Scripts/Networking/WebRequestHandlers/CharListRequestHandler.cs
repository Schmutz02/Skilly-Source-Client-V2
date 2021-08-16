using System.Threading.Tasks;
using Models;

namespace Networking.WebRequestHandlers
{
    public class CharListRequestHandler
    {
        private readonly string _username;
        private readonly string _password;

        public CharListRequestHandler(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public async Task SendRequestAsync()
        {
            var result = await WebRequestSender.SendCharListRequestAsync(_username, _password);
            while (!result.Success)
            {
                result = await WebRequestSender.SendCharListRequestAsync(_username, _password);
            }
            
            OnListRequestComplete(result);
        }

        private void OnListRequestComplete(WebRequestResult result)
        {
            if (result.Success)
            {
                Account.LoadFromCharList(result.Response);
            }
        }
    }
}