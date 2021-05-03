using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using RtspViewer.Extensions;

namespace RtspViewer.Configuration
{
    public class StreamConfiguration
    {
        public string Address { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public static void Save(string filePath, StreamConfiguration config)
        {

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            using (var sw = new StreamWriter(fs))
            {
                var json = JsonConvert.SerializeObject(config);
                var encryptor = new AesManaged().SetMachineSecret();
                var encryptedText = encryptor.EncryptText(json);

                sw.Write(encryptedText);
            }
        }

        public static bool TryLoad(string filePath, out StreamConfiguration config)
        {
            if (!File.Exists(filePath))
            {
                config = null;
                return false;
            }

            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open))
                using (var sr = new StreamReader(fs))
                {
                    var encryptedText = sr.ReadToEnd();
                    var encryptor = new AesManaged().SetMachineSecret();
                    var json = encryptor.DecryptText(encryptedText);
                    config = JsonConvert.DeserializeObject<StreamConfiguration>(json);
                }
            }
            catch
            {
                config = null;
                return false;
            }

            return true;
        }
    }
}
