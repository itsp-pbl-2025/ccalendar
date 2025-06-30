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
        private const string MessageExceptionSourceSequenceEmpty = "Source Sequence is empty";
        
        #region Vector2

        public static Vector2 Average<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2> source)
            where TEnumerator : struct, IValueEnumerator<Vector2>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                throw new InvalidOperationException(MessageExceptionSourceSequenceEmpty);
            }

            double sumX = e.Current.x;
            double sumY = e.Current.y;
            long count = 1;
            while (e.MoveNext())
            {
                var current = e.Current;
                sumX += current.x;
                sumY += current.y;
                count++;
            }

            return new Vector2((float)(sumX / count), (float)(sumY / count));
        }

        public static Vector2 Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2> source)
            where TEnumerator : struct, IValueEnumerator<Vector2>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                return Vector2.zero;
            }

            double sumX = e.Current.x;
            double sumY = e.Current.y;
            while (e.MoveNext())
            {
                var current = e.Current;
                sumX += current.x;
                sumY += current.y;
            }

            return new Vector2((float)sumX, (float)sumY);
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
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                throw new InvalidOperationException(MessageExceptionSourceSequenceEmpty);
            }

            long sumX = e.Current.x;
            long sumY = e.Current.y;
            long count = 1;
            while (e.MoveNext())
            {
                var current = e.Current;
                sumX = checked(sumX + current.x);
                sumY = checked(sumY + current.y);
                count++;
            }

            return new Vector2Int((int)(sumX / count), (int)(sumY / count));
        }

        public static Vector2Int Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector2Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector2Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                return Vector2Int.zero;
            }

            long sumX = e.Current.x;
            long sumY = e.Current.y;
            while (e.MoveNext())
            {
                var current = e.Current;
                sumX = checked(sumX + current.x);
                sumY = checked(sumY + current.y);
            }

            return new Vector2Int((int)sumX, (int)sumY);
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
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                throw new InvalidOperationException(MessageExceptionSourceSequenceEmpty);
            }

            double sumX = e.Current.x;
            double sumY = e.Current.y;
            double sumZ = e.Current.z;
            long count = 1;
            while (e.MoveNext())
            {
                var current = e.Current;
                sumX += current.x;
                sumY += current.y;
                sumZ += current.z;
                count++;
            }

            return new Vector3((float)(sumX / count), (float)(sumY / count), (float)(sumZ / count));
        }

        public static Vector3 Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3> source)
            where TEnumerator : struct, IValueEnumerator<Vector3>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                return Vector3.zero;
            }

            double sumX = e.Current.x;
            double sumY = e.Current.y;
            double sumZ = e.Current.z;
            while (e.MoveNext())
            {
                var current = e.Current;
                sumX += current.x;
                sumY += current.y;
                sumZ += current.z;
            }

            return new Vector3((float)sumX, (float)sumY, (float)sumZ);
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
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                throw new InvalidOperationException(MessageExceptionSourceSequenceEmpty);
            }

            long sumX = e.Current.x;
            long sumY = e.Current.y;
            long sumZ = e.Current.z;
            long count = 1;
            while (e.MoveNext())
            {
                var current = e.Current;
                sumX = checked(sumX + current.x);
                sumY = checked(sumY + current.y);
                sumZ = checked(sumZ + current.z);
                count++;
            }

            return new Vector3Int((int)(sumX / count), (int)(sumY / count), (int)(sumZ / count));
        }

        public static Vector3Int Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector3Int> source)
            where TEnumerator : struct, IValueEnumerator<Vector3Int>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                return Vector3Int.zero;
            }

            long sumX = e.Current.x;
            long sumY = e.Current.y;
            long sumZ = e.Current.z;
            while (e.MoveNext())
            {
                var current = e.Current;
                sumX = checked(sumX + current.x);
                sumY = checked(sumY + current.y);
                sumZ = checked(sumZ + current.z);
            }

            return new Vector3Int((int)sumX, (int)sumY, (int)sumZ);
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
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                throw new InvalidOperationException(MessageExceptionSourceSequenceEmpty);
            }

            double sumX = e.Current.x;
            double sumY = e.Current.y;
            double sumZ = e.Current.z;
            double sumW = e.Current.w;
            long count = 1;
            while (e.MoveNext())
            {
                var current = e.Current;
                sumX += current.x;
                sumY += current.y;
                sumZ += current.z;
                sumW += current.w;
                count++;
            }

            return new Vector4((float)(sumX / count), (float)(sumY / count), (float)(sumZ / count), (float)(sumW / count));
        }

        public static Vector4 Sum<TEnumerator>(this ValueEnumerable<TEnumerator, Vector4> source)
            where TEnumerator : struct, IValueEnumerator<Vector4>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
            {
                return Vector4.zero;
            }

            double sumX = e.Current.x;
            double sumY = e.Current.y;
            double sumZ = e.Current.z;
            double sumW = e.Current.w;
            while (e.MoveNext())
            {
                var current = e.Current;
                sumX += current.x;
                sumY += current.y;
                sumZ += current.z;
                sumW += current.w;
            }

            return new Vector4((float)sumX, (float)sumY, (float)sumZ, (float)sumW);
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