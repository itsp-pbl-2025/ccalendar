using System;
using System.Security.Cryptography;
using System.Text;

namespace SecureStringStorage
{
    public static class PasswordGenerator
    {
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits    = "0123456789";
        private const string Symbols   = "!@#$%^&*-_+=<>?";

        private static readonly string AllChars = Uppercase + Lowercase + Digits + Symbols;

        /// <summary>
        /// 暗号論的に安全なランダムパスワードを生成します。
        /// </summary>
        /// <param name="length">生成するパスワードの長さ</param>
        /// <returns>指定長のランダムパスワード</returns>
        public static string Generate(int length = 32)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

            var password = new StringBuilder(length);
            var charSet  = AllChars;
            var setLength = charSet.Length;

            // 暗号論的に強い RNG を生成
            using (var rng = RandomNumberGenerator.Create())
            {
                // 必要な乱数バイト数は、インデックス取得のたびに 4 バイト
                var buffer = new byte[4];

                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    var idx = (int)(num % setLength);
                    password.Append(charSet[idx]);
                }
            }

            return password.ToString();
        }
    }
}