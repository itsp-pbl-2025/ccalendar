#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using AppCore.Interfaces;
using Domain.Entity;
using Domain.Enum;
using Newtonsoft.Json;

namespace AppCore.UseCases
{
    public class HistoryService : IService
    {
        private readonly IHistoryRepository _historyRepo;
        
        public string Name { get; }

        public HistoryService(IHistoryRepository historyRepo, string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            _historyRepo = historyRepo;
        }

        public void Setup() {}
        
        /// <summary>
        /// 文字列変換に対応した任意の値を保存、または上書きする。
        /// </summary>
        /// <param name="type">保存先のHistoryType</param>
        /// <param name="value">任意の値</param>
        /// <typeparam name="T">値からクラスが一意にならない場合に指定</typeparam>
        /// <returns>挿入がされたならtrue</returns>
        public bool UpdateHistory<T>(HistoryType type, T value)
        {
            var str = EncodeSpecified(value);
            return _historyRepo.InsertUpdate(new HistoryContainer(type, str, DateTime.UtcNow));
        }

        /// <summary>
        /// 既存の値より大きい場合に、上書き保存する。比較関数を指定可能。
        /// </summary>
        /// <param name="type">保存先のHistoryType</param>
        /// <param name="value">任意の値</param>
        /// <param name="comparer">指定したい場合は、比較関数を定義可能</param>
        /// <typeparam name="T">値からクラスが一意にならない場合に指定</typeparam>
        /// <returns>更新がされたならtrue</returns>
        public bool UpdateHistoryIfGreater<T>(HistoryType type, T value, Func<T, T, bool>? comparer = null)
        {
            if (!TryGetHistory<T>(type, out var current))
            {
                UpdateHistory(type, value);
                return true;
            }
            
            if (comparer is not null)
            {
                if (comparer.Invoke(current, value))
                {
                    UpdateHistory(type, value);
                    return true;
                }
                return false;
            }

            if (value is IComparable<T> target && target.CompareTo(current) > 0)
            {
                UpdateHistory(type, value);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 既存の値より小さい場合に、上書き保存する。比較関数を指定可能。
        /// </summary>
        /// <param name="type">保存先のHistoryType</param>
        /// <param name="value">任意の値</param>
        /// <param name="comparer">指定したい場合は、比較関数を定義可能</param>
        /// <typeparam name="T">値からクラスが一意にならない場合に指定</typeparam>
        /// <returns>更新がされたならtrue</returns>
        public bool UpdateHistoryIfLesser<T>(HistoryType type, T value, Func<T, T, bool>? comparer = null)
        {
            if (!TryGetHistory<T>(type, out var current))
            {
                UpdateHistory(type, value);
                return true;
            }
            
            if (comparer is not null)
            {
                if (comparer.Invoke(current, value))
                {
                    UpdateHistory(type, value);
                    return true;
                }
                return false;
            }

            if (value is IComparable<T> target && target.CompareTo(current) < 0)
            {
                UpdateHistory(type, value);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// 指定したTypeのデータが存在するかどうかを返す。
        /// </summary>
        /// <param name="type">確認するHistoryType</param>
        /// <returns>データがあるかどうか</returns>
        public bool ContainsHistory(HistoryType type)
        {
            var r = _historyRepo.Get(type);
            return r != null;
        }
        
        /// <summary>
        /// 指定したTypeのデータが存在するならtrueを返し、第二引数に値を格納する。
        /// </summary>
        /// <param name="type">確認するHistoryType</param>
        /// <param name="result">値を返す変数</param>
        /// <typeparam name="T">第二引数で型が定まっていない場合に指定</typeparam>
        /// <returns>データがあるかどうか</returns>
        public bool TryGetHistory<T>(HistoryType type, out T result)
        {
            var r = _historyRepo.Get(type);
            if (r == null)
            {
                result = default!;
                return false;
            }

            try
            {
                result = DecodeSpecified<T>(r.Data);
                return true;
            }
            catch (Exception)
            {
                result = default!;
                return false;
            }
        }

        /// <summary>
        /// 指定したTypeのデータが存在するなら値を、しないならデフォルト値を返す。
        /// </summary>
        /// <param name="type">取りに行くHistoryType</param>
        /// <typeparam name="T">データを解釈する型を指定</typeparam>
        /// <returns>格納された値か、存在しない場合は型のデフォルト値</returns>
        public T? GetHistoryOrDefault<T>(HistoryType type)
        {
            var r = _historyRepo.Get(type);
            if (r == null)
            {
                return default;
            }
            
            try
            {
                return DecodeSpecified<T>(r.Data);
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// 格納されているデータをそのまま返す。無ければnullが返される。
        /// </summary>
        /// <param name="type">取りに行くHistoryType</param>
        /// <returns>生データ</returns>
        public HistoryContainer? GetRawHistory(HistoryType type)
        {
            var r = _historyRepo.Get(type);
            return r;
        }

        /// <summary>
        /// 一致するTypeの情報を削除する。
        /// </summary>
        /// <param name="type">削除するHistoryType</param>
        /// <returns>削除したかどうか</returns>
        public bool RemoveHistory(HistoryType type)
        {
            return _historyRepo.Remove(type);
        }

        /// <summary>
        /// 特定の型を指定の形式で文字列へ変換する場合は記述。DecodeSpecifiedに逆関数を定義しなければならない。
        /// 指定がない場合はJsonHelperでデシリアライズされる。
        /// </summary>
        /// <param name="value">文字列に変換する値</param>
        /// <typeparam name="T">型の指定</typeparam>
        /// <returns>変換後の文字列</returns>
        private static string EncodeSpecified<T>(T value)
        {
            switch (value)
            {
                case int intValue:
                    return Convert.ToBase64String(BitConverter.GetBytes(intValue));
                case uint uintValue:
                    return Convert.ToBase64String(BitConverter.GetBytes(uintValue));
                case long longValue:
                    return Convert.ToBase64String(BitConverter.GetBytes(longValue));
                case ulong ulongValue:
                    return Convert.ToBase64String(BitConverter.GetBytes(ulongValue));
                case float floatValue:
                    return Convert.ToBase64String(BitConverter.GetBytes(floatValue));
                case double doubleValue:
                    return Convert.ToBase64String(BitConverter.GetBytes(doubleValue));
                case byte byteValue:
                    return Convert.ToBase64String(BitConverter.GetBytes(byteValue));
                case sbyte sbyteValue:
                    return Convert.ToBase64String(BitConverter.GetBytes(sbyteValue));
                case bool boolValue:
                    return boolValue ? "t" : "f";
                case string stringValue:
                    return stringValue;
                case CCDateTime dtValue:
                    return $"{dtValue.Year.Value}-{dtValue.Month.Value:d2}-{dtValue.Day.Value:d2}T{dtValue.Hour.Value:d2}:{dtValue.Minute.Value:d2}:{dtValue.Second.Value:d2}";
                case CCDateOnly doValue:
                    return $"{doValue.Year.Value}-{doValue.Month.Value:d2}-{doValue.Day.Value:d2}";
                case CCTimeOnly toValue:
                    return $"{toValue.Hour.Value:d2}:{toValue.Minute.Value:d2}:{toValue.Second.Value:d2}";
            }
            
            return EncodeDefault(value);
        }
        
        private static string EncodeDefault<T>(T value)
        {
            if (value is string s) return s;
            if (value is null) return "";

            var t = typeof(T);
            if (t.IsPrimitive || t.IsEnum || t == typeof(decimal))
                return Convert.ToString(value, CultureInfo.InvariantCulture)!;

            return JsonConvert.SerializeObject(value);
        }
        
        /// <summary>
        /// 文字列を指定の形式で特定の型へ変換する場合は記述。EncodeSpecifiedに逆関数を定義しなければならない。
        /// 指定がない場合はJsonHelperでシリアライズされる。
        /// </summary>
        /// <param name="str">特定の型に変換する文字列</param>
        /// <typeparam name="T">型の指定</typeparam>
        /// <returns>変換後の値</returns>
        private static T DecodeSpecified<T>(string str)
        {
            var type = typeof(T);

            if (type == typeof(int))
                return (T)(object)BitConverter.ToInt32(Convert.FromBase64String(str), 0);
            if (type == typeof(uint))
                return (T)(object)BitConverter.ToUInt32(Convert.FromBase64String(str), 0);
            if (type == typeof(long))
                return (T)(object)BitConverter.ToInt64(Convert.FromBase64String(str), 0);
            if (type == typeof(ulong))
                return (T)(object)BitConverter.ToUInt64(Convert.FromBase64String(str), 0);
            if (type == typeof(float))
                return (T)(object)BitConverter.ToSingle(Convert.FromBase64String(str), 0);
            if (type == typeof(double))
                return (T)(object)BitConverter.ToDouble(Convert.FromBase64String(str), 0);
            if (type == typeof(byte))
                return (T)(object)Convert.FromBase64String(str)[0];
            if (type == typeof(sbyte))
                return (T)(object)(sbyte)Convert.FromBase64String(str)[0];
            if (type == typeof(bool))
                return (T)(object)(str == "t");
            if (type == typeof(string))
                return (T)(object)str;
            if (type == typeof(CCDateTime))
            {
                var regex = new Regex(@"^(\d+)-(\d\d)-(\d\d)T(\d\d):(\d\d):(\d\d)$");
                var match = regex.Match(str).Groups;
                return (T)(object)new CCDateTime(
                    int.Parse(match[1].Value), int.Parse(match[2].Value), int.Parse(match[3].Value),
                    int.Parse(match[4].Value), int.Parse(match[5].Value),  int.Parse(match[6].Value));
            }
            if (type == typeof(CCDateOnly))
            {
                var regex = new Regex(@"^(\d+)-(\d\d)-(\d\d)$");
                var match = regex.Match(str).Groups;
                return (T)(object)new CCDateOnly(int.Parse(match[1].Value), int.Parse(match[2].Value), int.Parse(match[3].Value));
            }
            if (type == typeof(CCTimeOnly))
            {
                var regex = new Regex(@"^(\d\d):(\d\d):(\d\d)$");
                var match = regex.Match(str).Groups;
                return (T)(object)new CCTimeOnly(int.Parse(match[1].Value), int.Parse(match[2].Value), int.Parse(match[3].Value));
            }
            
            return DecodeDefault<T>(str);
        }

        private static T DecodeDefault<T>(string str)
        {
            var t = typeof(T);
            if (t == typeof(string) || string.IsNullOrEmpty(str)) return (T)(object)str;

            if (t.IsPrimitive || t.IsEnum || t == typeof(decimal))
            {
                var val = Convert.ChangeType(str, t, CultureInfo.InvariantCulture)!;
                return (T)val;
            }

            return JsonConvert.DeserializeObject<T>(str) ?? throw new JsonSerializationException($"object {nameof(T)} could not be deserialized.");
        }
        
        public void Dispose()
        {
        }
    }
}