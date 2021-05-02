using System;
using System.Security.Cryptography;
using System.IO;

namespace RtspViewer.Extensions
{
    public static class CryptographyExtensions
    {
        public static string EncryptText(this SymmetricAlgorithm aesAlgorithm, string text)
        {
            ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV);

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(text);
                }

                var encryptedBytes = ms.ToArray();
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        public static string DecryptText(this SymmetricAlgorithm aesAlgorithm, string encryptedText)
        {
            ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor(aesAlgorithm.Key, aesAlgorithm.IV);

            var encryptedBytes = Convert.FromBase64String(encryptedText);

            using (var ms = new MemoryStream(encryptedBytes))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
