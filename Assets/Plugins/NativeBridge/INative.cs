using System;

namespace NativeBridge
{
    public interface INative
    {
        /// <summary>
        /// 時間を指定して通知をスケジュールする。
        /// </summary>
        /// <param name="title">通知のタイトルを指定する。</param>
        /// <param name="message">通知のメッセージを指定する。</param>
        /// <param name="datetime">通知を表示する時間を指定する。</param>
        /// <returns>登録された通知の固有ID(int)。</returns>
        public int ScheduleNotification(string title, string message, DateTime datetime);
        
        /// <summary>
        /// 指定した通知IDの通知をキャンセルする。
        /// </summary>
        /// <param name="notificationId">キャンセルする通知のIDを指定する。
        /// </param>
        public void RemoveNotification(string notificationId);
        
        /// <summary>
        /// バイブレーションを指定した時間だけ行う。
        /// </summary>
        /// <param name="duration">バイブレーションする時間(miliseconds)。
        /// </param>
        public void Vibrate(long duration);
        
        /// <summary>
        /// 音声を再生する。
        /// </summary>
        /// <param name="soundName">再生する音声の名前を指定する。
        /// </param>
        public void PlaySound(string soundName);
        
        /// <summary>
        /// バイブレーションがサポートされているかどうかを確認する。
        /// </summary>
        /// <returns> バイブレーションがサポートされている場合はtrue、そうでない場合はfalseを返す。
        /// </returns>
        public bool IsVibrationSupported();

        public string GetName();

    }
}