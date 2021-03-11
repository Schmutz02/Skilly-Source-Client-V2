using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Models;

namespace Networking
{
    public static class WebRequestSender
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        private static async Task<WebRequestResult> SendRequestAsync(string request, FormUrlEncodedContent content)
        {
            try
            {
                var response = await HttpClient.PostAsync(Settings.ServerURL + request, content);
                return await HandleHttpResponse(response);
            }
            catch (Exception)
            {
                var errorXml = XElement.Parse("<Error>Unable to contact server</Error>");
                return new WebRequestResult(errorXml, false);
            }
        }
        
        private static async Task<WebRequestResult> HandleHttpResponse(HttpResponseMessage response)
        {
            var textResponse = await response.Content.ReadAsStringAsync();
            var xml = XElement.Parse(textResponse);
            var isSuccessResponse = xml.Name != "Error";

            return new WebRequestResult(xml, isSuccessResponse);
        }

        public static async Task<WebRequestResult> SendLogInRequest(string username, string password)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            return await SendRequestAsync("/account/verify", content);
        }

        public static async Task<WebRequestResult> SendCharListRequest(string username, string password)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });
            
            return await SendRequestAsync("/char/list", content);
        }
    }

    public readonly struct WebRequestResult
    {
        public readonly XElement Response;
        public readonly bool Success;

        public WebRequestResult(XElement response, bool success)
        {
            Response = response;
            Success = success;
        }
    }
}