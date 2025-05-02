#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Database.Model;
using LiteDB;
using UnityEngine;

namespace Common.ProgressSpace
{
    public class DataManager : IDisposable
    {
        private readonly LiteDatabase _db;
        private bool _disposed = false;
        
        public DataManager()
        {
            var key = SecureStringStorage.KeyStorage.Load();
            if (key == null)
            {
                key = SecureStringStorage.PasswordGenerator.Generate();
                SecureStringStorage.KeyStorage.Save(key);
            }
            
            var dbPath = System.IO.Path.Combine(Application.persistentDataPath, "Logger.ini");
            
            _db = new LiteDatabase(new ConnectionString {
                Filename = dbPath,
                Password = key,
            });
            
            InitLiteCollection();
        }

        #region Destructor

        public void Dispose()
        {
            if (!_disposed)
            {
                _db.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~DataManager()
        {
            Dispose();
        }

        #endregion

        private void InitLiteCollection()
        {
            _colLoginStatus = _db.GetCollection<DLoginStatus>(CollectionLoginStatus);
        }
        
        #region LoginStatus
        
        private const string CollectionLoginStatus = "DLoginStatus";
        private ILiteCollection<DLoginStatus> _colLoginStatus = null!;
        
        public void OnLoggedIn()
        {
            _colLoginStatus.Insert(new DLoginStatus
            {
                Id = 0, // auto increment
                LoginTime = DateTime.Now
            });
        }
        
        public void OnLoggedOut()
        {
            var maxId = _colLoginStatus.FindAll().ToList().Select(status => status.Id).Prepend(0).Max();
            var status = _colLoginStatus.FindById(maxId);
            
            if (status == null) return;
            status.LogoutTime = DateTime.Now;
            _colLoginStatus.Update(status);
        }

        public DLoginStatus? FindPreviousLoginStatus()
        {
            var maxId = _colLoginStatus.FindAll().ToList().Select(status => status.Id).Prepend(0).Max();
            return _colLoginStatus.FindById(maxId - 1);
        }
        
        public DLoginStatus? FindCurrentLoginStatus()
        {
            var maxId = _colLoginStatus.FindAll().ToList().Select(status => status.Id).Prepend(0).Max();
            return _colLoginStatus.FindById(maxId);
        }

        public List<DLoginStatus> GetAllLoginStatus()
        {
            return _colLoginStatus.FindAll().ToList();
        }

        #endregion
    }
}