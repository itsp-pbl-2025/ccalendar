using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

namespace Presentation.Utilities
{
    public class EventDispatcher
    {
        #region GlobalEvent

        private class GlobalEventEntry
        {
            private uint _indexer = 0;
            private readonly Dictionary<uint, GlobalEventListener> _listeners = new();

            public void AddListener(GlobalEventListener listener)
            {
                _listeners.Add(listener.Id, listener);
            }

            public bool RemoveListener(GlobalEventListener listener)
            {
                return _listeners.Remove(listener.Id);
            }

            public IEnumerable<GlobalEventListener> GetListeners()
            {
                return _listeners.Values;
            }
            
            public uint GetNextIndex() => ++_indexer;
        }
        
        public class GlobalEventListener : IDisposable
        {
            public GlobalEvent GlobalEvent { get; }
            public uint Id { get; }
            private readonly Action<string> _onEventCallback;
            private readonly Func<GlobalEventListener, bool> _onDisposeCallback;
            

            private bool _disposed;

            public GlobalEventListener(GlobalEvent globalEvent, uint id, Action<string> onEventCallback,
                Func<GlobalEventListener, bool> onDisposeCallback)
            {
                GlobalEvent = globalEvent;
                Id = id;
                _onEventCallback = onEventCallback;
                _onDisposeCallback = onDisposeCallback;
            }

            public void ReceiveEvent(string str)
            {
                _onEventCallback?.Invoke(str);                
            }
        
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (_disposed) return;
                _disposed = true;

                _onDisposeCallback?.Invoke(this);
            }

            ~GlobalEventListener() => Dispose(false);
        }
        
        private readonly Dictionary<GlobalEvent, GlobalEventEntry> _globalEventListeners = new();

        private GlobalEventListener AddUnsafeGlobalEventListener(GlobalEvent ev, Action<string> onEventCallback)
        {
            if (!_globalEventListeners.TryGetValue(ev, out var entry))
            {
                entry = new GlobalEventEntry();
                _globalEventListeners.Add(ev, entry);
            }
            
            var listener = new GlobalEventListener(ev, entry.GetNextIndex(), onEventCallback, RemoveGlobalEventListener);
            entry.AddListener(listener);
            
            return listener;
        }

        public GlobalEventListener AddGlobalEventListener(GameObject obj, GlobalEvent ev, Action<string> onEventCallback)
        {
            var listener = AddUnsafeGlobalEventListener(ev, onEventCallback);
            listener.AddTo(obj);
            return listener;
        }

        public GlobalEventListener AddGlobalEventListener(Component cmp, GlobalEvent ev, Action<string> onEventCallback)
        {
            var listener = AddUnsafeGlobalEventListener(ev, onEventCallback);
            listener.AddTo(cmp);
            return listener;
        }
        
        public GlobalEventListener AddGlobalEventListener(CompositeDisposable cd, GlobalEvent ev, Action<string> onEventCallback)
        {
            var listener = AddUnsafeGlobalEventListener(ev, onEventCallback);
            listener.AddTo(cd);
            return listener;
        }

        public bool RemoveGlobalEventListener(GlobalEventListener gel)
        {
            return _globalEventListeners.TryGetValue(gel.GlobalEvent, out var entry) && entry.RemoveListener(gel);
        }

        public void SendGlobalEvent(GlobalEvent ev, string str = "")
        {
            if (!_globalEventListeners.TryGetValue(ev, out var entry)) return;

            foreach (var listener in entry.GetListeners().ToList())
            {
                listener.ReceiveEvent(str);
            }
        }

        #endregion
    }
}