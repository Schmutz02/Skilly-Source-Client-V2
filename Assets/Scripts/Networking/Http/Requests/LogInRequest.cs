using System;
using System.Collections.Generic;

namespace Networking.Http.Requests
{
    public class LogInRequest : WebRequest
    {
        public LogInRequest(Action<string, bool> callback, string username, string password) :
            base("/account/verify", callback, new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            })
        {
        }
    }
}