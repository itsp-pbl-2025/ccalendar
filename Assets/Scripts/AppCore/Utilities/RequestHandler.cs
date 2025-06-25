using System;
using System.Net;
using System.Net.Http;
using Cysharp.Threading.Tasks;

namespace AppCore.Utilities
{
    public class RequestHandler
    {
        public abstract class ResultBase
        {
            public Exception Error { get; protected set; }
            public HttpStatusCode StatusCode { get; protected set; }
            
            public bool IsSuccess => Error == null;

            protected ResultBase()
            {
                Error = null;
                StatusCode = HttpStatusCode.NoContent;
            }

            protected ResultBase(HttpStatusCode statusCode, Exception ex = null)
            {
                Error = ex;
                StatusCode = statusCode;
            }
        }

        public class Result : ResultBase
        {
            public Result() {}
            public Result(HttpStatusCode statusCode, Exception ex = null) : base(statusCode, ex) {}
        }

        public class Result<T> : ResultBase
        {
            public T Data { get; }
            
            public Result(HttpResponseMessage response, string responseBody)
            {
                StatusCode = response.StatusCode;
                
                
                if (!response.IsSuccessStatusCode)
                {
                    Error = new HttpRequestException($"HTTP {(int)response.StatusCode} {response.StatusCode} {responseBody}");
                    Data = default;
                    return;
                }

                try
                {
                    Data = JsonConvert.DeserializeObject<T>(responseBody);
                }
                catch (Exception ex)
                {
                    Error = ex;
                    Data = default;
                }
            }

            public Result(HttpStatusCode statusCode, Exception ex = null) : base(statusCode, ex)
            {
                Data = default;
            }
        }
        
        private static readonly HttpClient CommonHttpClient = new();
        private readonly HttpClient _httpClient;
        
        private Uri _baseUrl;

        public RequestHandler(string baseUrl)
        {
            CommonHttpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient = CommonHttpClient;
            _baseUrl = new Uri(baseUrl, UriKind.Absolute);
        }

        public RequestHandler(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = new Uri(baseUrl, UriKind.Absolute);
        }

        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = new Uri(baseUrl, UriKind.Absolute);
        }
        
        private Uri BuildUrl(string relativePath) => new Uri(_baseUrl, relativePath);

        public async UniTask<Result<TResponse>> GetAsync<TResponse>(string relativePath)
        {
            try
            {
                var response = await CommonHttpClient.GetAsync(BuildUrl(relativePath));
                var responseBody = await response.Content.ReadAsStringAsync();
                return new Result<TResponse>(response, responseBody);
            }
            catch (Exception e)
            {
                return new Result<TResponse>(HttpStatusCode.BadRequest, e);
            }
        }

        public async UniTask<Result<TResponse>> PostAsync<TResponse, TRequest>(string relativePath, TRequest postData)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(postData);
                var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            
                var response = await CommonHttpClient.PostAsync(BuildUrl(relativePath), content);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return new Result<TResponse>(response, responseBody);

                }
                return new Result<TResponse>(response, "");
            }
            catch (Exception e)
            {
                return new Result<TResponse>(HttpStatusCode.BadRequest, e);
            }
        }

        public async UniTask<Result> PostAsync<TRequest>(string relativePath, TRequest postData)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(postData);
                var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                await CommonHttpClient.PostAsync(BuildUrl(relativePath), content);
                return new Result();
            }
            catch (Exception e)
            {
                return new Result(HttpStatusCode.BadRequest, e);
            }
        }
    }
}