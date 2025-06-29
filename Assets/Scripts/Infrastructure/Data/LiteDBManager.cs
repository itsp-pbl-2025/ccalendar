#nullable enable
using System;
using System.IO;
using System.Text.RegularExpressions;
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
        public LiteDBManager(string path)
        {
            var key = SecureStringStorage.KeyStorage.Load();
            if (key == null)
            {
                key = SecureStringStorage.PasswordGenerator.Generate();
                SecureStringStorage.KeyStorage.Save(key);
            }

            var conn = new ConnectionString { Filename = path, Password = key };
            try
            {
                DB = new LiteDatabase(conn);
            }
            catch (LiteException e)
            {
                File.Delete(path);
                File.Delete(Regex.Replace(path, @"(?=\.[^\\/]+$)", "-log"));

                DB = new LiteDatabase(conn);
                Console.Error.WriteLine($"initialization failed, DB recreated. Error: {e.Message}");
            }
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