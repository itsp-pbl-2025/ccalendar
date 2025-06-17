using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace WebRequest
{
    public class RequestHandler
    {
        private const string ErrorNoInternet = "No Internet";
        private const string ErrorUnknown = "Unknown Error";

        public DateTime LastOnline { get; private set; }
        public int DelayMillisecond { get; private set; }

        private string _baseUrl;

        public RequestHandler(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        
        private static bool CheckStatus()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) return false;

            return true;
        }

        public void SetBaseUrl(string url)
        {
            _baseUrl = url;
        }

        private async UniTask<UnityWebRequestException> SendRequestWrapper(UnityWebRequest req)
        {
            var before = DateTime.Now;
            try
            {
                await req.SendWebRequest();
                
                LastOnline = DateTime.Now;
                DelayMillisecond = (int)(LastOnline.Ticks - before.Ticks) / 10000;
            }
            catch (UnityWebRequestException e)
            {
                DelayMillisecond = -1;
                return e;
            }

            return null;
        }
        
        public async UniTask<(bool err, int code, string result)> Get(string url)
        {
            if (!CheckStatus())
            {
                return (true, 0, ErrorNoInternet);
            }
            
            using var req = UnityWebRequest.Get($"{_baseUrl}{url}");

            var e = await SendRequestWrapper(req);
            if (e != null)
            {
                return (true, (int)e.ResponseCode, e.Message);
            }
            
            switch (req.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    return (true, 0, req.error);
                case UnityWebRequest.Result.ProtocolError:
                    return (true, 0, req.error);
                case UnityWebRequest.Result.Success:
                    return (false, (int)req.responseCode, req.downloadHandler.text);
                default:
                    return (true, 0, ErrorUnknown);
            }
        }
        
        public async UniTask<(bool err, int code, string result)> Post(string url, string json)
        {
            if (!CheckStatus())
            {
                return (true, 0, ErrorNoInternet);
            }

            using var req = new UnityWebRequest($"{_baseUrl}{url}", "POST");
            
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            
            var e = await SendRequestWrapper(req);
            if (e != null)
            {
                return (true, (int)e.ResponseCode, e.Message);
            }
            
            switch (req.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    return (true, 0, req.error);
                case UnityWebRequest.Result.ProtocolError:
                    return (true, 0, req.error);
                case UnityWebRequest.Result.Success:
                    return (false, (int)req.responseCode, req.downloadHandler.text);
                default:
                    return (true, 0, ErrorUnknown);
            }
        }
        
        public async UniTask<(bool err, int code, string result)> Delete(string url)
        {
            if (!CheckStatus())
            {
                return (true, 0, ErrorNoInternet);
            }

            using var req = UnityWebRequest.Delete($"{_baseUrl}{url}");

            var e = await SendRequestWrapper(req);
            if (e != null)
            {
                return (true, (int)e.ResponseCode, e.Message);
            }
            
            switch (req.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    return (true, 0, req.error);
                case UnityWebRequest.Result.ProtocolError:
                    return (true, 0, req.error);
                case UnityWebRequest.Result.Success:
                    return (false, (int)req.responseCode, req.downloadHandler.text);
                default:
                    return (true, 0, ErrorUnknown);
            }
        }
        
        public async UniTask<(bool err, int code, string result)> Patch(string url, string json)
        {
            if (!CheckStatus())
            {
                return (true, 0, ErrorNoInternet);
            }

            using var req = new UnityWebRequest($"{_baseUrl}{url}", "PATCH");
            
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            
            var e = await SendRequestWrapper(req);
            if (e != null)
            {
                return (true, (int)e.ResponseCode, e.Message);
            }
            
            switch (req.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    return (true, 0, req.error);
                case UnityWebRequest.Result.ProtocolError:
                    return (true, 0, req.error);
                case UnityWebRequest.Result.Success:
                    return (false, (int)req.responseCode, req.downloadHandler.text);
                default:
                    return (true, 0, ErrorUnknown);
            }
        }
    }
}