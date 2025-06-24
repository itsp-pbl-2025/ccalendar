#nullable enable
using System;
using System.IO;
using LiteDB;

namespace Infrastructure.Data
{
    public class LiteDBManager : IDisposable
    {
        public LiteDatabase DB { get; }

        private readonly MemoryStream? _mem;
        private bool _disposed = false;

        /// <summary>
        /// どこにも書き込まれない生のデータベースを作成します。
        /// </summary>
        public LiteDBManager()
        {
            const int sizeMax = 1024 * 1024; // 1 MB
            _mem = new MemoryStream(sizeMax);
            DB = new LiteDatabase(_mem);
            BsonMapper.Global.EmptyStringToNull = false;
        }

        /// <summary>
        /// 指定したパスにあるデータベースを開きます。存在しない場合、新しいデータベースを作成します。
        /// </summary>
        /// <param name="path">データベースのパス</param>
        /// <param name="key">データベースの鍵</param>
        public LiteDBManager(string path, string key)
        {
            DB = new LiteDatabase(new ConnectionString {
                Filename = path,
                Password = key,
            });
            BsonMapper.Global.EmptyStringToNull = false;
        }

        #region Destructor

        public void Dispose()
        {
            if (!_disposed)
            {
                DB.Dispose();
                _mem?.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~LiteDBManager()
        {
            Dispose();
        }

        #endregion
    }
}