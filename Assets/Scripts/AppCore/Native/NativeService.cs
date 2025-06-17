using AppCore.UseCases;
using Domain.Entity;

namespace AppCore.Native
{
    /// <summary>
    /// 端末のネイティブ機能を集約的に提供するサービス。
    /// 
    /// </summary>
    public class NativeService : IService {
        public string Name => "";
        private INative _native;
        public NativeService()
        {
//             _native = 
// #if __ANDROID__
//             new NativeBridgeAndroid();
// #elif __IOS__
//             new NativeBridgeIOS();
// #elif !__MOBILE__
//                 new NativeBridgePhony();
// #else
//             throw new System.NotImplementedException();
// #endif
        }
        public void Dispose(){}

        public void ScheduleNotificationWithSchedule(Schedule schedule)
        {
            
        } 
    }
}