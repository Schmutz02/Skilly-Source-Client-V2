using System;
using System.Collections.Generic;

namespace Networking.Http.Requests
{
    public abstract class WebRequest
    {
        public Action<string, bool> Callback { get; }
        public KeyValuePair<string, string>[] Content { get; }

        private string _request;

        protected WebRequest(string request, Action<string, bool> callback, KeyValuePair<string, string>[] content = null)
        {
            _request = request;
            Callback = callback;
            Content = content ?? new KeyValuePair<string, string>[0];
        }

        public string GetUrl()
        {
            if (_request[0] != '/')
            {
                _request = "/" + _request;
            }

            return Settings.ServerURL + _request;
        }
    }
}
