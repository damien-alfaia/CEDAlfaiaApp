using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CEDAlfaiaApp.Core
{
    public static class StringExtensions
    {
        /// <summary>
        /// Hashe un mot de passe
        /// </summary>
        /// <param name="pPassword"></param>
        /// <returns></returns>
        public static byte[] ToPasswordHash (this string pPassword)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1data = sha1.ComputeHash(Encoding.ASCII.GetBytes(pPassword));
            return sha1data;
        }
    }
}
