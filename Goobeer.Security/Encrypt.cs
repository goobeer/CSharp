using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Goobeer.Security
{
    public enum HashType
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }

    public enum SymmetricType
    {
        AES,
        DES,
        RC2,
        Rijndael,
        TripleDES
    }

    public static class HashEncryptHelper
    {
        public static string HashEncrypt(HashType algName, string source, string salt="", bool lowerFormate = true)
        {
            StringBuilder sb = new StringBuilder();
            using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(algName.ToString()))
            {
                byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(source + salt));

                string formate = lowerFormate ? "x2" : "X2";
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString(formate));
                }
            }
            
            return sb.ToString();
        }

        public static string HashEncrypt(HashType hashName, string source, int offset, int count, string salt="", bool lowerFormate = true)
        {
            StringBuilder sb = new StringBuilder();
            using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(hashName.ToString()))
            {
                byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(source + salt), offset, count);
                string formate = lowerFormate ? "x2" : "X2";
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString(formate));
                }
            }
            return sb.ToString();
        }

        public static string HashEncrypt(HashType hashName, Stream inputStream, bool lowerFormate = true)
        {
            StringBuilder sb = new StringBuilder();
            using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(hashName.ToString()))
            {
                byte[] data = hashAlgorithm.ComputeHash(inputStream);

                string formate = lowerFormate ? "x2" : "X2";
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString(formate));
                }
            }
            
            return sb.ToString();
        }
    }

    /// <summary>
    /// 对称 加/解 密
    /// </summary>
    public static class SymmetricEncryptHelper
    {
        public static SymmetricEncryptedData SymmetricEncrypt(SymmetricType algName,string source)
        {
            SymmetricEncryptedData encryptedData = new SymmetricEncryptedData();

            using (SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create(algName.ToString()))
            {
                encryptedData.Key = symmetricAlgorithm.Key;
                encryptedData.IV = symmetricAlgorithm.IV;

                using (ICryptoTransform transform = symmetricAlgorithm.CreateEncryptor())
                {
                    using (MemoryStream memStream=new MemoryStream())
                    {
                        using (CryptoStream crypStream=new CryptoStream(memStream,transform,CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer=new StreamWriter(crypStream))
                            {
                                writer.Write(source);
                            }
                            encryptedData.Data = memStream.ToArray();
                        }
                    }
                }
            }

            return encryptedData;
        }

        public static string SymmetricDecrypt(SymmetricType algName, SymmetricEncryptedData data)
        {
            string result = string.Empty;

            using (SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create(algName.ToString()))
            {
                using (ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor(data.Key,data.IV))
                {
                    using (MemoryStream memStream = new MemoryStream(data.Data))
                    {
                        using (CryptoStream crypStream = new CryptoStream(memStream, transform, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(crypStream))
                            {
                                result = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
