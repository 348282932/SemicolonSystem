using System;
using System.Security.Cryptography;
using System.Text;

namespace SemicolonSystem.Common
{
    public static class Encryption
    {
        /// <summary>
        /// MD5加密方法
        /// </summary>
        /// <param name="str">传入一个字符串</param>
        /// <returns></returns>
        public static string MD5(string str, string key)
        {
            str = string.Format("{0}{1}{2}", key, str, key);

            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", "");
        }
    }
}
