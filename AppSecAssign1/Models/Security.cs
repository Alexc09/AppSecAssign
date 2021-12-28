using AlexAppSecAssign.Models;
using AppSecAssign1.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace AppSecAssign1.Models
{
    public static class Security
    {
        public static string Hash(string Text)
        {
            return BC.HashPassword(Text, workFactor: 11);
        }

        public static String GenerateRandomString(int length)
        {
            const string src = "abcdefghijklmnopqrstuvwxyz0123456789";
            var sb = new StringBuilder();
            Random RNG = new Random();
            for (var i = 0; i < length; i++)
            {
                var c = src[RNG.Next(0, src.Length)];
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            var byteArr = new byte[length];
            RandomNumberGenerator.Fill(byteArr);
            return byteArr;

        }

        private static string GenerateEncodedRandomString(int length)
        {
            var b64 = Convert.ToBase64String(GenerateRandomBytes(length));
            return b64;
        }

        public static (string, byte[], byte[]) Encrypt(string text)
        {
            // Convert plaintext to bytes
            byte[] plainText = Encoding.UTF8.GetBytes(text);

            Aes cipher = Aes.Create();
            ICryptoTransform encryptTransform = cipher.CreateEncryptor();
            byte[] cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            string cipherString = Convert.ToBase64String(cipherText);

            return (cipherString, cipher.Key, cipher.IV);
        }

        public static string Decrypt(string cipherString, byte[] Key, byte[] IV)
        {
            byte[] cipherText = Convert.FromBase64String(cipherString);

            Aes cipher = Aes.Create();
            cipher.Key = Key;
            cipher.IV = IV;
            ICryptoTransform decryptTransform = cipher.CreateDecryptor();

            byte[] decryptedText = decryptTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);
            string decryptedString = Encoding.UTF8.GetString(decryptedText);
            return decryptedString;
        }
    }
}
