using System;

namespace NativeBridge
{
    public class NativeBridgePhony : INative
    {
        /// <summary>
        /// 端末のネイティブ機能をモックするクラス。
        /// 実際の端末ではなく、テストやデバッグ用に使用される。
        /// </summary>
        public int ScheduleNotification(string title, string message, DateTime datetime)
        {
            Console.WriteLine($"[Phony] ScheduleNotification: {title}, {message}, {datetime}");
            return 42117; // four ni i D
        }

        public void RemoveNotification(int notificationId)
        {
            Console.WriteLine($"[Phony] RemoveNotification: {notificationId}");
        }

        public void Vibrate(long duration)
        {
            Console.WriteLine($"[Phony] Vibrate: {duration}ms");
        }

        public void PlaySound(string soundName)
        {
            Console.WriteLine($"[Phony] PlaySound: {soundName}");
        }

        public bool IsVibrationSupported() => false;
        
        public string GetName()
        {
            return "NativeBridgePhony";
        }
    }

}