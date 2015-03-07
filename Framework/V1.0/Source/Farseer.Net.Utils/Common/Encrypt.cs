using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using FS.Extend;

namespace FS.Utils.Common
{
    /// <summary>
    ///     加密工具
    /// </summary>
    public abstract class Encrypt
    {
        /// <summary>
        ///     MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="isReverse">是否加密后反转字符串</param>
        /// <param name="ToUpper">是否加密后转为大写</param>
        /// <param name="count">加密次数</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str, bool ToUpper = false, bool isReverse = false, int count = 1)
        {
            if (count <= 0) { return str; }

            var b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            var md5 = string.Empty;
            for (var i = 0; i < b.Length; i++)
            {
                md5 += b[i].ToString("x").PadLeft(2, '0');
            }

            if (isReverse) { md5 = Reverse(md5); }
            if (ToUpper) { md5 = md5.ToUpper(); }

            return MD5(ToUpper ? md5.ToUpper() : md5.ToLower(), ToUpper, isReverse, --count);
        }

        /// <summary>
        ///     SHA256函数
        /// </summary>
        /// ///
        /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            var SHA256Data = Encoding.UTF8.GetBytes(str);
            var Sha256 = new SHA256Managed();
            var Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result); //返回长度为44字节的字符串
        }

        /// <summary>
        ///     反转字符串
        /// </summary>
        /// <param name="input">要反转字符串</param>
        /// <returns></returns>
        public static string Reverse(string input)
        {
            var chars = input.ToUpper().ToCharArray();
            var length = chars.Length;
            for (var index = 0; index < length / 2; index++)
            {
                var c = chars[index];
                chars[index] = chars[length - 1 - index];
                chars[length - 1 - index] = c;
            }
            return new String(chars);
        }

        /// <summary>
        ///     加密
        /// </summary>
        public class AES
        {
            //默认密钥向量
            private static readonly byte[] Keys =
                {
                    0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F,
                    0x77, 0x6D, 0x61, 0x6E, 0x3F
                };

            /// <summary>
            ///     AES加密字符串
            /// </summary>
            /// <param name="encryptString">待加密的字符串</param>
            /// <param name="encryptKey">加密密钥,要求为8位</param>
            /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
            public static string Encode(string encryptString, string encryptKey)
            {
                encryptKey = encryptKey.SubString(0, 32);
                encryptKey = encryptKey.PadRight(32, ' ');

                var rijndaelProvider = new RijndaelManaged
                                           {
                                               Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32)),
                                               IV = Keys
                                           };
                var rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

                var inputData = Encoding.UTF8.GetBytes(encryptString);
                var encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Convert.ToBase64String(encryptedData);
            }

            /// <summary>
            ///     AES解密字符串
            /// </summary>
            /// <param name="decryptString">待解密的字符串</param>
            /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
            /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
            public static string Decode(string decryptString, string decryptKey)
            {
                try
                {
                    decryptKey = decryptKey.SubString(0, 32);
                    decryptKey = decryptKey.PadRight(32, ' ');

                    var rijndaelProvider = new RijndaelManaged { Key = Encoding.UTF8.GetBytes(decryptKey), IV = Keys };
                    var rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

                    var inputData = Convert.FromBase64String(decryptString);
                    var decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                    return Encoding.UTF8.GetString(decryptedData);
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        ///     加密
        /// </summary>
        public class DES
        {
            //默认密钥向量
            private static readonly byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

            /// <summary>
            ///     DES加密字符串
            /// </summary>
            /// <param name="encryptString">待加密的字符串</param>
            /// <param name="encryptKey">加密密钥,要求为8位</param>
            /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
            public static string Encode(string encryptString, string encryptKey)
            {
                encryptKey = encryptKey.SubString(0, 8);
                encryptKey = encryptKey.PadRight(8, ' ');
                var rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                var rgbIV = Keys;
                var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                var dCSP = new DESCryptoServiceProvider();
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }

            /// <summary>
            ///     DES解密字符串
            /// </summary>
            /// <param name="decryptString">待解密的字符串</param>
            /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
            /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
            public static string Decode(string decryptString, string decryptKey)
            {
                try
                {
                    decryptKey = decryptKey.SubString(0, 8);
                    decryptKey = decryptKey.PadRight(8, ' ');
                    var rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                    var rgbIV = Keys;
                    var inputByteArray = Convert.FromBase64String(decryptString);
                    var DCSP = new DESCryptoServiceProvider();

                    var mStream = new MemoryStream();
                    var cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                    cStream.Write(inputByteArray, 0, inputByteArray.Length);
                    cStream.FlushFinalBlock();
                    return Encoding.UTF8.GetString(mStream.ToArray());
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        public class ZipWrapper
        {
            /// <summary>
            /// 压缩
            /// </summary>
            /// <param name="str">字符串</param>
            public static string Compress(string str)
            {
                //因输入的字符串不是Base64所以转换为Base64,因为HTTP如果不传递Base64则会发生http 400错误
                return Convert.ToBase64String(Compress(Convert.FromBase64String(Convert.ToBase64String(Encoding.Default.GetBytes(str)))));
            }

            /// <summary>
            /// 解压
            /// </summary>
            /// <param name="str">字符串</param>
            public static string Decompress(string str)
            {
                return Encoding.Default.GetString(Decompress(Convert.FromBase64String(str)));
            }

            /// <summary>
            /// 压缩
            /// </summary>
            /// <param name="bytes">字节组</param>
            public static byte[] Compress(byte[] bytes)
            {
                using (var ms = new MemoryStream())
                {
                    var Compress = new GZipStream(ms, CompressionMode.Compress);
                    Compress.Write(bytes, 0, bytes.Length);
                    Compress.Close();
                    return ms.ToArray();
                }
            }

            /// <summary>
            /// 压缩
            /// </summary>
            /// <param name="bytes">字节组</param>
            public static byte[] Decompress(Byte[] bytes)
            {
                using (var tempMs = new MemoryStream())
                {
                    using (var ms = new MemoryStream(bytes))
                    {
                        var Decompress = new GZipStream(ms, CompressionMode.Decompress);
                        Decompress.CopyTo(tempMs);
                        Decompress.Close();
                        return tempMs.ToArray();
                    }
                }
            }
        }
    }
}