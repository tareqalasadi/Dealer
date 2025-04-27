using System.Security.Cryptography;
using System.Text;

namespace Dealer.Helper
{
    public static class EncryptionHelper
    {
        private static readonly string key = "123456789*ghaith"; // 16 chars for AES-128

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(16));
            aes.Key = keyBytes;
            aes.IV = keyBytes;

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        public static string Decrypt(string encryptedText)
        {
            encryptedText = encryptedText
                .Replace("-", "+")
                .Replace("_", "/");
            switch (encryptedText.Length % 4)
            {
                case 2: encryptedText += "=="; break;
                case 3: encryptedText += "="; break;
            }

            using var aes = Aes.Create();
            var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(16));
            aes.Key = keyBytes;
            aes.IV = keyBytes;

            using var decryptor = aes.CreateDecryptor();
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }

}
