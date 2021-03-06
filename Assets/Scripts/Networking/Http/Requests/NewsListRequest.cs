using System;

namespace Networking.Http.Requests
{
    public class NewsListRequest : WebRequest
    {
        public NewsListRequest(Action<string, bool> callback) :
            base("news/list", callback)
        {
        }
    }
}