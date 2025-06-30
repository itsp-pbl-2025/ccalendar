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
        #region Vector2

        public static Vector2 Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2> source)
            where TEnumerator : struct, IValueEnumerator<Vector2>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var sum = Vector2.zero;
            var count = 0;
            
            foreach (var v in source)
            {
                sum += v;
                count++;
            }

            return count > 0 ? sum / count : Vector2.zero;
        }

        public static Vector2 Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2> source)
            where TEnumerator : struct, IValueEnumerator<Vector2>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var sum = Vector2.zero;
            
            foreach (var v in source)
            {
                sum += v;
            }

            return sum;
        }

        public static Vector2 Min<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2> source)
            where TEnumerator : struct, IValueEnumerator<Vector2>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var min = Vector2.one * float.MaxValue;
            
            foreach (var v in source)
            {
                min = Vector2.Min(min, v);
            }

            return min;
        }

        public static Vector2 Max<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2> source)
            where TEnumerator : struct, IValueEnumerator<Vector2>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var max = Vector2.one * float.MinValue;
            
            foreach (var v in source)
            {
                max = Vector2.Max(max, v);
            }

            return max;
        }

        #endregion
        
        #region Vector2Int

        public static Vector2Int Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector2Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var sum = Vector2Int.zero;
            var count = 0;
            
            foreach (var v in source)
            {
                sum += v;
                count++;
            }

            return count > 0 ? sum / count : Vector2Int.zero;
        }

        public static Vector2Int Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector2Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var sum = Vector2Int.zero;
            
            foreach (var v in source)
            {
                sum += v;
            }

            return sum;
        }

        public static Vector2Int Min<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector2Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var min = Vector2Int.one * int.MaxValue;
            
            foreach (var v in source)
            {
                min = Vector2Int.Min(min, v);
            }

            return min;
        }

        public static Vector2Int Max<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector2Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var max = Vector2Int.one * int.MinValue;
            
            foreach (var v in source)
            {
                max = Vector2Int.Max(max, v);
            }

            return max;
        }

        #endregion
        
        #region Vector3

        public static Vector3 Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3> source)
            where TEnumerator : struct, IValueEnumerator<Vector3>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var sum = Vector3.zero;
            var count = 0;
            
            foreach (var v in source)
            {
                sum += v;
                count++;
            }

            return count > 0 ? sum / count : Vector3.zero;
        }

        public static Vector3 Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3> source)
            where TEnumerator : struct, IValueEnumerator<Vector3>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var sum = Vector3.zero;
            
            foreach (var v in source)
            {
                sum += v;
            }

            return sum;
        }

        public static Vector3 Min<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3> source)
            where TEnumerator : struct, IValueEnumerator<Vector3>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var min = Vector3.one * float.MaxValue;
            
            foreach (var v in source)
            {
                min = Vector3.Min(min, v);
            }

            return min;
        }

        public static Vector3 Max<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3> source)
            where TEnumerator : struct, IValueEnumerator<Vector3>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var max = Vector3.one * float.MinValue;
            
            foreach (var v in source)
            {
                max = Vector3.Max(max, v);
            }

            return max;
        }
        
        #endregion
        
        #region Vector3Int

        public static Vector3Int Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector3Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var sum = Vector3Int.zero;
            var count = 0;
            
            foreach (var v in source)
            {
                sum += v;
                count++;
            }

            return count > 0 ? sum / count : Vector3Int.zero;
        }

        public static Vector3Int Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector3Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var sum = Vector3Int.zero;
            
            foreach (var v in source)
            {
                sum += v;
            }

            return sum;
        }

        public static Vector3Int Min<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector3Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var min = Vector3Int.one * int.MaxValue;
            
            foreach (var v in source)
            {
                min = Vector3Int.Min(min, v);
            }

            return min;
        }

        public static Vector3Int Max<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector3Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var max = Vector3Int.one * int.MinValue;
            
            foreach (var v in source)
            {
                max = Vector3Int.Max(max, v);
            }

            return max;
        }
        
        #endregion
        
        #region Vector4

        public static Vector4 Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector4> source)
            where TEnumerator : struct, IValueEnumerator<Vector4>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var sum = Vector4.zero;
            var count = 0;
            
            foreach (var v in source)
            {
                sum += v;
                count++;
            }

            return count > 0 ? sum / count : Vector4.zero;
        }

        public static Vector4 Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector4> source)
            where TEnumerator : struct, IValueEnumerator<Vector4>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var sum = Vector4.zero;
            
            foreach (var v in source)
            {
                sum += v;
            }

            return sum;
        }

        public static Vector4 Min<TEnumerator>(this ValueEnumerable<TEnumerator, Vector4> source)
            where TEnumerator : struct, IValueEnumerator<Vector4>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var min = Vector4.one * float.MaxValue;
            
            foreach (var v in source)
            {
                min = Vector4.Min(min, v);
            }

            return min;
        }

        public static Vector4 Max<TEnumerator>(this ValueEnumerable<TEnumerator, Vector4> source)
            where TEnumerator : struct, IValueEnumerator<Vector4>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            var max = Vector4.one * float.MinValue;
            
            foreach (var v in source)
            {
                max = Vector4.Max(max, v);
            }

            return max;
        }
        
        #endregion
    }
}