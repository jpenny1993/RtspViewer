using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DeviceId;

namespace RtspViewer.Extensions
{
    internal static class CryptographyExtensions
    {
        public static string EncryptText(this SymmetricAlgorithm algorithm, string text)
        {
            ICryptoTransform encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);

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

        public static string DecryptText(this SymmetricAlgorithm algorithm, string encryptedText)
        {
            ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);

            var encryptedBytes = Convert.FromBase64String(encryptedText);

            using (var ms = new MemoryStream(encryptedBytes))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }

        public static SymmetricAlgorithm SetMachineSecret(this SymmetricAlgorithm algorithm)
        {
            // Use hardware + user data to determine Key & IV
            // That way we can reload the config without storing the keys
            algorithm.Key = GetMachineInfo(algorithm.KeySize / 8); // default is 258 bits / 32 bytes
            algorithm.IV = GetUserInfo(algorithm.BlockSize / 8);   // default is 128 bits / 16 bytes
            return algorithm;
        }

        private static byte[] GetMachineInfo(int keySize)
        {
            var deviceInfoBytes = new DeviceIdBuilder()
                .AddMacAddress()
                .AddProcessorId()
                .AddMotherboardSerialNumber()
                .ToBytes();

            var keyBytes = new byte[keySize];
            Array.Copy(deviceInfoBytes, keyBytes, keyBytes.Length);
            return keyBytes;
        }

        private static byte[] GetUserInfo(int keySize)
        {
            var userInfoBytes = new DeviceIdBuilder()
                .AddOSInstallationID()
                .AddUserName()
                .ToBytes();

            var keyBytes = new byte[keySize];
            Array.Copy(userInfoBytes, keyBytes, keyBytes.Length);
            return keyBytes;
        }

        private static byte[] ToBytes(this DeviceIdBuilder builder)
        {
            return Encoding.Default.GetBytes(builder.ToString());
        }
    }
}
