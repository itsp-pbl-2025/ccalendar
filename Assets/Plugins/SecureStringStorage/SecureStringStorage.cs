using System;
using System.Text;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using System.Runtime.InteropServices;

internal static class Dpapi
{
    [DllImport("crypt32.dll", SetLastError = true)]
    private static extern bool CryptProtectData(
        ref DATA_BLOB pDataIn, string szDescription,
        IntPtr pOptionalEntropy, IntPtr pvReserved,
        IntPtr pPromptStruct, int dwFlags,
        ref DATA_BLOB pDataOut);

    [DllImport("crypt32.dll", SetLastError = true)]
    private static extern bool CryptUnprotectData(
        ref DATA_BLOB pDataIn, IntPtr ppszDescription,
        IntPtr pOptionalEntropy, IntPtr pvReserved,
        IntPtr pPromptStruct, int dwFlags,
        ref DATA_BLOB pDataOut);

    [StructLayout(LayoutKind.Sequential)]
    private struct DATA_BLOB { public int cbData; public IntPtr pbData; }

    public static byte[] Protect(byte[] data)
    {
        var inBlob  = ToBlob(data);
        var outBlob = new DATA_BLOB();
        if (!CryptProtectData(ref inBlob, null, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero, 0, ref outBlob))
            throw new System.ComponentModel.Win32Exception();
        return FromBlob(outBlob);
    }

    public static byte[] Unprotect(byte[] data)
    {
        var inBlob  = ToBlob(data);
        var outBlob = new DATA_BLOB();
        if (!CryptUnprotectData(ref inBlob, IntPtr.Zero, IntPtr.Zero,
                    IntPtr.Zero, IntPtr.Zero, 0, ref outBlob))
                throw new System.ComponentModel.Win32Exception();
        return FromBlob(outBlob);
    }

    /* helpers */
    private static DATA_BLOB ToBlob(byte[] d)
    {
        var blob = new DATA_BLOB { cbData = d.Length };
        blob.pbData = Marshal.AllocHGlobal(d.Length);
        Marshal.Copy(d, 0, blob.pbData, d.Length);
        return blob;
    }
    private static byte[] FromBlob(DATA_BLOB b)
    {
        var d = new byte[b.cbData];
        Marshal.Copy(b.pbData, d, 0, b.cbData);
        Marshal.FreeHGlobal(b.pbData);
        return d;
    }
}
#endif

namespace SecureStringStorage
{
    public static class KeyStorage
    {
        // 共有キー名（Keychain/KeyStore エントリー名）
        private const string Alias = "com.slack.itsp-pbl-2025.litedb_password";

        public static bool Save(string plainText)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(plainText);
                byte[] enc  = Dpapi.Protect(data);
                PlayerPrefs.SetString(Alias, Convert.ToBase64String(enc)); // <- DPAPI なので安全
                PlayerPrefs.Save();
                return true;
            }
            catch (Exception e) { Debug.LogError(e); return false; }

#elif UNITY_IOS && !UNITY_EDITOR
        return _sp_keychain_save(Alias, plainText);

#elif UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var ctx = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var bridge = new AndroidJavaObject("itsp.teamc.ssstorage.SecureBridge", ctx);
            return bridge.Call<bool>("save", Alias, plainText);
        }
        catch (Exception e) { Debug.LogError(e); return false; }

#else
        PlayerPrefs.SetString(Alias, plainText); PlayerPrefs.Save();
        return true;
#endif
        }

        public static string Load()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            if (!PlayerPrefs.HasKey(Alias)) return null;
            try
            {
                byte[] enc  = Convert.FromBase64String(PlayerPrefs.GetString(Alias));
                byte[] data = Dpapi.Unprotect(enc);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception e) { Debug.LogError(e); return null; }

#elif UNITY_IOS && !UNITY_EDITOR
        return _sp_keychain_load(Alias);

#elif UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var ctx = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var bridge = new AndroidJavaObject("itsp.teamc.ssstorage.SecureBridge", ctx);
            return bridge.Call<string>("load", Alias);
        }
        catch (Exception e) { Debug.LogError(e); return null; }

#else
        return PlayerPrefs.GetString(Alias, null);
#endif
        }

        public static void Delete()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            PlayerPrefs.DeleteKey(Alias);

#elif UNITY_IOS && !UNITY_EDITOR
        _sp_keychain_delete(Alias);

#elif UNITY_ANDROID && !UNITY_EDITOR
        using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        using var ctx = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        using var bridge = new AndroidJavaObject("itsp.teamc.ssstorage.SecureBridge", ctx);
        bridge.Call("delete", Alias);

#else
        PlayerPrefs.DeleteKey(Alias);
#endif
        }
        
#if UNITY_IOS && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern bool   _sp_keychain_save  (string key, string val);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string _sp_keychain_load  (string key);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void   _sp_keychain_delete(string key);
#endif
    }
}
