using System;
using UnityEngine;
using ZLinq;

namespace Presentation.Utilities
{
    /// <summary>
    /// ZLinqがサポートしていない形式の変換を行うための拡張クラス
    /// </summary>
    public static class ZLinqExtensions
    {
        #region Common

        private static TSource Average<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TSource, TSource> sumFunc, Func<TSource, int, TSource> divFunc)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                throw new InvalidOperationException("Source sequence is empty.");
            }
            
            var sum = e.Current;
            var count = 1;
            while (e.MoveNext())
            {
                sum = sumFunc(sum, e.Current);
                count++;
            }

            return divFunc(sum, count);
        }
        
        #endregion
        
        #region Vector2

        public static Vector2 Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2> source)
            where TEnumerator : struct, IValueEnumerator<Vector2>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Average((v1, v2) => v1 + v2, (v, i) => v / i);
        }

        public static Vector2 Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2> source)
            where TEnumerator : struct, IValueEnumerator<Vector2>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate((v1, v2) => v1 + v2);
        }

        public static Vector2 Min<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2> source)
            where TEnumerator : struct, IValueEnumerator<Vector2>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate(Vector2.Min);
        }

        public static Vector2 Max<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2> source)
            where TEnumerator : struct, IValueEnumerator<Vector2>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate(Vector2.Max);
        }

        #endregion
        
        #region Vector2Int

        public static Vector2Int Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector2Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Average((v1, v2) => v1 + v2, (v, i) => v / i);
        }

        public static Vector2Int Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector2Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate((v1, v2) => v1 + v2);
        }

        public static Vector2Int Min<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector2Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate(Vector2Int.Min);
        }

        public static Vector2Int Max<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector2Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate(Vector2Int.Max);
        }

        #endregion
        
        #region Vector3

        public static Vector3 Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3> source)
            where TEnumerator : struct, IValueEnumerator<Vector3>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Average((v1, v2) => v1 + v2, (v, i) => v / i);
        }

        public static Vector3 Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3> source)
            where TEnumerator : struct, IValueEnumerator<Vector3>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate((v1, v2) => v1 + v2);
        }

        public static Vector3 Min<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3> source)
            where TEnumerator : struct, IValueEnumerator<Vector3>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate(Vector3.Min);
        }

        public static Vector3 Max<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3> source)
            where TEnumerator : struct, IValueEnumerator<Vector3>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate(Vector3.Max);
        }
        
        #endregion
        
        #region Vector3Int

        public static Vector3Int Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector3Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Average((v1, v2) => v1 + v2, (v, i) => v / i);
        }

        public static Vector3Int Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector3Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate((v1, v2) => v1 + v2);
        }

        public static Vector3Int Min<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector3Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate(Vector3Int.Min);
        }

        public static Vector3Int Max<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector3Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate(Vector3Int.Max);
        }
        
        #endregion
        
        #region Vector4

        public static Vector4 Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector4> source)
            where TEnumerator : struct, IValueEnumerator<Vector4>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Average((v1, v2) => v1 + v2, (v, i) => v / i);
        }

        public static Vector4 Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector4> source)
            where TEnumerator : struct, IValueEnumerator<Vector4>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate((v1, v2) => v1 + v2);
        }

        public static Vector4 Min<TEnumerator>(this ValueEnumerable<TEnumerator, Vector4> source)
            where TEnumerator : struct, IValueEnumerator<Vector4>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate(Vector4.Min);
        }

        public static Vector4 Max<TEnumerator>(this ValueEnumerable<TEnumerator, Vector4> source)
            where TEnumerator : struct, IValueEnumerator<Vector4>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            return source.Aggregate(Vector4.Max);
        }
        
        #endregion
    }
}