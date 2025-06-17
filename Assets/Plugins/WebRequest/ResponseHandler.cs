using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace WebRequest
{
    public class ResponseHandler<T>
    {
        public class Result
        {
            public bool error { get; }
            public int code { get; }
            public string response { get; }
            public T result { get; }

            public Result(bool err, int code, string response)
            {
                error = err;
                this.code = code;
                this.response = response;

                if (err) return;
                
                try
                {
                    if (typeof(T).IsArray)
                    {
                        result = JsonUtility.FromJson<DataOf<T>>($"{{\"data\":{response}}}").data;
                    }
                    else
                    {
                        result = JsonUtility.FromJson<T>(response);
                    }
                }
                catch (Exception)
                {
                    error = true;
                    result = default;
                }
            }
        }
        
        private readonly ReactiveProperty<bool> _state = new(false);
        private (bool err, int code, string result) _response;
        private Result _result;
        
        private Action<int, T> _successCallback;
        private Action<int, string> _failureCallback;

        public ResponseHandler(UniTask<(bool err, int code, string result)> request)
        {
            SendRequest(request).Forget();
        }

        private async UniTask SendRequest(UniTask<(bool err, int code, string result)> request)
        {
            _response = await request;
            _result = new Result(_response.err, _response.code, _response.result);

            if (_result.error)
            {
                _failureCallback?.Invoke(_response.code, _response.result);
            }
            else
            {
                _successCallback?.Invoke(_response.code, _result.result);
            }

            _state.Value = true;
        }

        public async UniTask<Result> ToUniTask()
        {
            if (!_state.Value)
            {
                await _state.Where(x => x).FirstAsync();
            }

            return _result;
        }
        
        public void SetSuccessCallback(Action<int, T> callback)
        {
            _successCallback = callback;
        }

        public void SetFailureCallback(Action<int, string> callback)
        {
            _failureCallback = callback;
        }
    }
}