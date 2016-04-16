using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;

namespace Stump.Core.Cryptography
{
    public static class Cryptography
    {
        public static string GetMD5Hash(string input)
        {
            MD5 mD = MD5.Create();
            byte[] array = mD.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            byte[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                byte b = array2[i];
                stringBuilder.Append(b.ToString("x2", CultureInfo.CurrentCulture));
            }
            return stringBuilder.ToString();
        }

        public static string GetFileMD5Hash(string fileName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            MD5 mD = MD5.Create();
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                byte[] array = mD.ComputeHash(fileStream);
                for (int i = 0; i < array.Length; i++)
                {
                    byte b = array[i];
                    stringBuilder.Append(b.ToString("x2").ToLower());
                }
            }
            return stringBuilder.ToString();
        }

        public static string GetFileMD5HashBase64(string fileName)
        {
            string result;
            using (MD5 mD = MD5.Create())
            {
                result = Convert.ToBase64String(mD.ComputeHash(File.ReadAllBytes(fileName)));
            }
            return result;
        }

        public static bool VerifyMD5Hash(string chaine, string hash)
        {
            string mD5Hash = Cryptography.GetMD5Hash(chaine);
            StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
            return ordinalIgnoreCase.Compare(mD5Hash, hash) == 0;
        }

        public static string EncryptRSA(string encryptValue, RSAParameters parameters)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            rSACryptoServiceProvider.ImportParameters(parameters);
            byte[] bytes = Encoding.UTF8.GetBytes(encryptValue);
            byte[] inArray = rSACryptoServiceProvider.Encrypt(bytes, false);
            return Convert.ToBase64String(inArray);
        }

        public static string DecryptRSA(byte[] encryptedValue, RSAParameters parameters)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            rSACryptoServiceProvider.ImportParameters(parameters);
            return Encoding.UTF8.GetString(rSACryptoServiceProvider.Decrypt(encryptedValue, false));
        }

        public static byte[] EncryptAES(byte[] data, byte[] key)
        {
            var iv = key.Take(16).ToArray();
            try
            {
                using (var rijndaelManaged = new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC })
                {
                    ICryptoTransform crypto = rijndaelManaged.CreateEncryptor();

                    return crypto.TransformFinalBlock(data, 0, data.Length);
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
    }
}