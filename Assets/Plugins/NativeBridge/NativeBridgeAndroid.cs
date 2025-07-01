using Unity.Notifications.Android;
using UnityEngine;
using System;

namespace NativeBridge
{
    public class NativeBridgeAndroid: INative
    {
     # if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass _unityPlayer;
        private static AndroidJavaObject _currentActivity;
        private static AndroidJavaObject _vibrator;

        public static AndroidJavaClass UnityPlayer
        {
            get
            {
                if (_unityPlayer == null)
                {
                    _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                }
                return _unityPlayer;
            }
        }

        public static AndroidJavaObject CurrentActivity
        {
            get
            {
                if (_currentActivity == null)
                {
                    _currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
                return _currentActivity;
            }
        }

        public static AndroidJavaObject Vibrator
        {
            get
            {
                if (_vibrator == null)
                {
                    _vibrator = CurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                }
                return _vibrator;
            }
        }
     # else
     		public static AndroidJavaClass UnityPlayer;
     		public static AndroidJavaObject CurrentActivity;
     		public static AndroidJavaObject Vibrator;
     # endif
        private static AudioSource _audioSource;
        readonly string _channelId = "default_channel";
        public NativeBridgeAndroid()
        {  
            // Androidの通知チャネルを登録
            var channel = new AndroidNotificationChannel
            {
                Id = _channelId,
                Name = "Default Channel",
                Importance = Importance.Default,
                Description = "Default channel for notifications"
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }
        /// <summary>
        /// Android端末で通知をスケジュールするための実装。
        /// </summary>
        public int ScheduleNotification(string title, string message, DateTime datetime)
        {
            var notification = new AndroidNotification
            {
                Title = title,
                Text = message,
                Number = 0,
                FireTime = datetime
            };

            return AndroidNotificationCenter.SendNotification(notification, _channelId);
        }

        public void RemoveNotification(int notificationId)
        {
            AndroidNotificationCenter.CancelNotification(notificationId);
        }

        public void Vibrate(long duration)
        {
            Vibrator.Call("vibrate", duration);
        }

        public void PlaySound(string soundName)
        {
            if (_audioSource == null)
            {
                GameObject audioObject = new GameObject("NativeAudioSource");
                UnityEngine.Object.DontDestroyOnLoad(audioObject); // シーン跨いでも残すなら
                _audioSource = audioObject.AddComponent<AudioSource>();
            }

            AudioClip clip = Resources.Load<AudioClip>(soundName);
            if (clip != null)
            {
                _audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"AudioClip '{soundName}' not found in Resources folder.");
            }
        }

        public bool IsVibrationSupported()
        {
            return SystemInfo.supportsVibration;
        }
        
        public string GetName()
        {
            return "NativeBridgeAndroid";
        }
    }
}