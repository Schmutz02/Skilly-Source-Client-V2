using System;
using System.Collections.Generic;

public class NewsListRequest : WebRequest
{
    public NewsListRequest(Action<string, bool> callback) :
        base("news/list", callback)
    {
    }
}