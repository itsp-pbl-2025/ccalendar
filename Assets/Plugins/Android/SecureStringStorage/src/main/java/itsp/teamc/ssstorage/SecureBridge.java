package istp.teamc.ssstorage;

import android.content.Context;
import android.content.SharedPreferences;
import android.security.keystore.KeyGenParameterSpec;
import android.security.keystore.KeyProperties;

import java.nio.charset.StandardCharsets;
import java.security.KeyStore;

import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;
import javax.crypto.spec.GCMParameterSpec;
import android.util.Base64;

public class SecureBridge {

    private static final String ANDROID_KEYSTORE = "AndroidKeyStore";
    private static final String KEY_ALIAS        = "LiteDB_AES_Key";
    private static final String PREF_NAME        = "litedb_secure_prefs";
    private static final String IV_POSTFIX       = "_iv";

    private final SharedPreferences prefs;

    public SecureBridge(Context ctx) {
        prefs = ctx.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        ensureKey();
    }

    /* ---------------- public API ---------------- */

    public boolean save(String key, String value) {
        try {
            Cipher cipher = Cipher.getInstance("AES/GCM/NoPadding");
            cipher.init(Cipher.ENCRYPT_MODE, getSecretKey());
            byte[] iv  = cipher.getIV();
            byte[] enc = cipher.doFinal(value.getBytes(StandardCharsets.UTF_8));

            prefs.edit()
                 .putString(key, Base64.encodeToString(enc, Base64.NO_WRAP))
                 .putString(key + IV_POSTFIX, Base64.encodeToString(iv, Base64.NO_WRAP))
                 .apply();
            return true;
        } catch (Exception e) { e.printStackTrace(); return false; }
    }

    public String load(String key) {
        try {
            String encB64 = prefs.getString(key, null);
            String ivB64  = prefs.getString(key + IV_POSTFIX, null);
            if (encB64 == null || ivB64 == null) return null;

            byte[] enc = Base64.decode(encB64, Base64.NO_WRAP);
            byte[] iv  = Base64.decode(ivB64, Base64.NO_WRAP);

            Cipher cipher = Cipher.getInstance("AES/GCM/NoPadding");
            cipher.init(Cipher.DECRYPT_MODE, getSecretKey(), new GCMParameterSpec(128, iv));
            byte[] dec = cipher.doFinal(enc);
            return new String(dec, StandardCharsets.UTF_8);
        } catch (Exception e) { e.printStackTrace(); return null; }
    }

    public void delete(String key) {
        prefs.edit()
             .remove(key)
             .remove(key + IV_POSTFIX)
             .apply();
    }

    /* ---------------- KeyStore helpers ---------------- */

    private void ensureKey() {
        try {
            KeyStore ks = KeyStore.getInstance(ANDROID_KEYSTORE);
            ks.load(null);
            if (!ks.containsAlias(KEY_ALIAS)) {
                KeyGenParameterSpec spec = new KeyGenParameterSpec.Builder(
                        KEY_ALIAS,
                        KeyProperties.PURPOSE_ENCRYPT | KeyProperties.PURPOSE_DECRYPT)
                        .setBlockModes(KeyProperties.BLOCK_MODE_GCM)
                        .setEncryptionPaddings(KeyProperties.ENCRYPTION_PADDING_NONE)
                        .setRandomizedEncryptionRequired(true)
                        .build();
                KeyGenerator kg = KeyGenerator.getInstance(
                        KeyProperties.KEY_ALGORITHM_AES, ANDROID_KEYSTORE);
                kg.init(spec);
                kg.generateKey();
            }
        } catch (Exception e) { e.printStackTrace(); }
    }

    private SecretKey getSecretKey() throws Exception {
        KeyStore ks = KeyStore.getInstance(ANDROID_KEYSTORE);
        ks.load(null);
        return ((SecretKey) ks.getKey(KEY_ALIAS, null));
    }
}
