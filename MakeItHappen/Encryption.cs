using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MakeItHappen
{

    public static class Encryption
    {
         
        public static string GetRandomNumber(int length)
        {
            var rg = new RNGCryptoServiceProvider();
            var number = new byte[length];
            rg.GetBytes(number);
            return Convert.ToBase64String(number);



        }
        public enum HashMethod
        {
            SHA256
        }
           
        public static byte[] Encrypt(byte[] bytes)
        {
            var aes = new AesCryptoServiceProvider();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Convert.FromBase64String(System.Configuration.ConfigurationManager.AppSettings["key"]);
            aes.IV = Convert.FromBase64String(System.Configuration.ConfigurationManager.AppSettings["IV"]);
            using (var ms = new MemoryStream())
            {
                var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();
                return (ms.ToArray());
            }

        }
        public static byte[] Encrypt(string data)
        {
            return Encrypt(Encoding.UTF8.GetBytes(data));


        }

        public static byte[] Decrypt(byte[] data)
        {
            var aes = new AesCryptoServiceProvider();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Convert.FromBase64String(System.Configuration.ConfigurationManager.AppSettings["key"]);
            aes.IV = Convert.FromBase64String(System.Configuration.ConfigurationManager.AppSettings["IV"]);
            using (var ms = new MemoryStream())
            {
                var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
                 
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }

        }
    }
}
