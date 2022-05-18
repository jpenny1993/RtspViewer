using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using RtspViewer.Extensions;

namespace RtspViewer.Configuration
{
    public class StreamConfiguration
    {
        public string Address { get; set; }
        public ConnectionType Protocol { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public static TData Decrypt<TData>(string encryptedText)
        {
            try
            {
                var encryptor = new AesManaged().SetMachineSecret();
                var xml = encryptor.DecryptText(encryptedText);
                var serialiser = new XmlSerializer(typeof(TData));
                var reader = new StringReader(xml);
                var data = (TData)serialiser.Deserialize(reader);
                return data;
            }
            catch
            {
                return default(TData);
            }
        }

        public static string Encrypt<TData>(TData config)
        {
            var serialiser = new XmlSerializer(typeof(TData));
            var writer = new StringWriter();
            serialiser.Serialize(writer, config);
            var xml = writer.ToString();
            var encryptor = new AesManaged().SetMachineSecret();
            return encryptor.EncryptText(xml);
        }

        public static void Save<TData>(string filePath, TData config)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fs = new FileStream(filePath, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                var encryptedText = Encrypt(config);
                sw.Write(encryptedText);
            }
        }

        public static bool TryLoad<TData>(string filePath, out TData config)
        {
            if (!File.Exists(filePath))
            {
                config = default(TData);
                return false;
            }

            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open))
                using (var sr = new StreamReader(fs))
                {
                    var encryptedText = sr.ReadToEnd();
                    config = Decrypt<TData>(encryptedText);
                }
            }
            catch
            {
                config = default(TData);
            }

            return config != null;
        }
    }
}
