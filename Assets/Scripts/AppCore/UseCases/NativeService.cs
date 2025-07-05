using AppCore.UseCases;
using Domain.Entity;
using NativeBridge;
using System;

namespace AppCore.Native
{
    /// <summary>
    /// 端末のネイティブ機能を集約的に提供するサービス。
    /// 
    /// </summary>
    public class NativeService : IService {
        public string Name { get; }
        private INative _native;
        
        public NativeService(string name = "")
        {
            Name = name != "" ? name : GetType().Name;
            Console.WriteLine("NativeService is constructed with:" + Name);
             _native = 
        #if __ANDROID__
             new NativeBridgeAndroid();
        #elif __IOS__
             new NativeBridgeIOS();
        #elif !__MOBILE__
                  new NativeBridgePhony();
        #else
             throw new System.NotImplementedException();
        #endif

             Console.WriteLine("NativeService is constructed with:" + _native.GetName());
        }

        public void Setup() {}
        
        public void Dispose(){}

        public void ScheduleNotificationWithSchedule(Schedule schedule)
        {
             // TODO: Implementation
             throw new NotImplementedException();
        } 
    }
}