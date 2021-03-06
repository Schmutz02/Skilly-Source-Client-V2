using System;
using System.Net.Http;
using Networking.Http.Requests;

namespace Networking.Http
{
    public static class WebRequestSender
    {
        private const string ERROR_PREFIX = "<Error";
    
        private static readonly HttpClient HttpClient = new HttpClient();
    
        private static Action<string, bool> _currentCallback;
    
        public static async void SendRequest(WebRequest request)
        {
            _currentCallback = request.Callback;
        
            var content = new FormUrlEncodedContent(request.Content);
            var response = await HttpClient.PostAsync(request.GetUrl(), content);

            HandleHttpResponse(response);
        }

        private static async void HandleHttpResponse(HttpResponseMessage response)
        {
            var textResponse = await response.Content.ReadAsStringAsync();
            var isSuccessResponse = !textResponse.StartsWith(ERROR_PREFIX);

            InvokeCallback(textResponse, isSuccessResponse);
        }

        private static void InvokeCallback(string textResponse, bool isSuccess)
        {
            _currentCallback.Invoke(textResponse, isSuccess);
        }
    }
}