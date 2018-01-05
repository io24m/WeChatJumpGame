#region copyright
// <copyright file="AESComm"  company="CIS"> 
// Copyright (c) CIS. All Right Reserved
// </copyright>
// <author>ddfm</author>
// <datecreated>2018/1/3 11:22:12</datecreated>
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Comm
{
    public class AESComm
    {
        public static CipherMode CipherMode { get; set; } = CipherMode.ECB;

        public static string Key { get; set; } = "12345678901234567890123456789012";
        public static string IV { get; set; } = "12345678901234567890123456789012";
        public static string Encrypt(string toEncrypt)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(Key);
            byte[] iv = Encoding.UTF8.GetBytes(IV);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = keyArray,
                IV = iv,
                Mode = CipherMode,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string toDecrypt)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(Key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            byte[] iv = Encoding.UTF8.GetBytes(IV);

            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = keyArray,
                IV = iv,
                Mode = CipherMode,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
