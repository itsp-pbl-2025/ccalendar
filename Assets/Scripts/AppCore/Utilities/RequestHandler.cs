using System.Net;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using Domain.Api;

namespace AppCore.Utilities
{
    public class RequestHandler
    {
        public class Result
        {
            public Result(HttpResponseMessage response)
            {
                IsSuccess = response.IsSuccessStatusCode;
                StatusCode = response.StatusCode;
            }
            
            public bool IsSuccess { get; }
            public HttpStatusCode StatusCode { get; }
        }

        public class Result<T>
        {
            public Result(HttpResponseMessage response, string responseBody)
            {
                IsSuccess = response.IsSuccessStatusCode;
                StatusCode = response.StatusCode;

                if (IsSuccess)
                {
                    if (typeof(T).IsArray)
                    {
                        Data = JsonHelper.FromJson<DataOf<T>>($"{{\"data\":{responseBody}}}").data;
                    }
                    else
                    {
                        Data = JsonHelper.FromJson<T>(responseBody);
                    }
                }
                else
                {
                    Data = default;
                }
            }
            
            public bool IsSuccess { get; }
            public HttpStatusCode StatusCode { get; }
            public T Data { get; }
        }
        
        private readonly HttpClient _httpClient = new();
        
        private string _baseUrl;

        public RequestHandler(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async UniTask<Result<T>> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(_baseUrl + url);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return new Result<T>(response, responseBody);

            }
            return new Result<T>(response, "");
        }

        public async UniTask<Result<T1>> PostAsync<T1, T2>(string url, T2 postData)
        {
            var jsonData = JsonHelper.ToJson(postData);
            var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(_baseUrl + url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return new Result<T1>(response, responseBody);

            }
            return new Result<T1>(response, "");
        }

        public async UniTask PostAsync<T>(string url, T postData)
        {
            var jsonData = JsonHelper.ToJson(postData);
            var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(_baseUrl + url, content);
        }
    }
}