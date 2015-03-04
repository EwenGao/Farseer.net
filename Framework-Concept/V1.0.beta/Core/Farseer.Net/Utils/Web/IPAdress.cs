using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FS.Extend;
using FS.Utils.Common;

namespace FS.Utils.Web
{
    /// <summary>
    ///     纯真数据库操作类
    ///     2009.5.25 YM
    /// </summary>
    public class IPAdress
    {
        /// <summary>
        ///     IP数据库的位置
        /// </summary>
        private static string file;

        private static byte[] data;
        private static long firstStartIpOffset, lastStartIpOffset, ipCount;

        private static readonly Regex regex =
            new Regex(@"(((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))\.){3}((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))");

        /// <summary>
        ///     IP数据库的位置
        /// </summary>
        public static string File
        {
            get
            {
                if (file.IsNullOrEmpty())
                {
                    file = Files.GetRootPath() + "App_Data/QQWry.Dat";
                }
                return file;
            }
            set { file = value; }
        }


        /// <summary>
        ///     获取IP地址位置
        /// </summary>
        /// <param name="ip">IP</param>
        public static string GetAddress(string ip = "")
        {
            if (ip.IsNullOrEmpty())
            {
                ip = Req.GetIP();
            }
            var loc = GetLocation(ip);
            return loc.Area + loc.Address;
        }

        /// <summary>
        ///     获取IP地址位置
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="province">返回省份</param>
        /// <param name="city">返回城市</param>
        public static string GetAddress(string ip, out string province, out string city)
        {
            var loc = GetLocation(ip);
            province = loc.Province;
            city = loc.City;
            return loc.Area + loc.Address;
        }

        /// <summary>
        ///     获取IP地址位置
        /// </summary>
        /// <param name="province">返回省份</param>
        /// <param name="city">返回城市</param>
        public static string GetAddress(out string province, out string city)
        {
            return GetAddress(Req.GetIP(), out province, out city);
        }

        /// <summary>
        ///     查找指定IP位置
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Location GetLocation(string ip = "")
        {
            if (ip.IsNullOrEmpty())
            {
                ip = Req.GetIP();
            }

            if (data == null)
            {
                using (var fs = new FileStream(File, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                }
                var buffer = new byte[8];
                Array.Copy(data, 0, buffer, 0, 8);
                firstStartIpOffset = ((buffer[0] + (buffer[1]*0x100)) + ((buffer[2]*0x100)*0x100)) +
                                     (((buffer[3]*0x100)*0x100)*0x100);
                lastStartIpOffset = ((buffer[4] + (buffer[5]*0x100)) + ((buffer[6]*0x100)*0x100)) +
                                    (((buffer[7]*0x100)*0x100)*0x100);
                ipCount = Convert.ToInt64((((lastStartIpOffset - firstStartIpOffset))/7.0));

                if (ipCount <= 1L)
                {
                    throw new ArgumentException("ip FileDataError");
                }
            }
            if (!regex.Match(ip).Success)
            {
                throw new ArgumentException("IP格式错误");
            }

            var Location = new Location {IP = ip};
            var intIP = IpToInt(ip);
            if ((intIP >= IpToInt("127.0.0.1") && (intIP <= IpToInt("127.255.255.255"))))
            {
                Location.Area = "本机/局域网地址";
                Location.Address = "";
            }
            else
            {
                if ((((intIP >= IpToInt("0.0.0.0")) && (intIP <= IpToInt("2.255.255.255"))) ||
                     ((intIP >= IpToInt("64.0.0.0")) && (intIP <= IpToInt("126.255.255.255")))) ||
                    ((intIP >= IpToInt("58.0.0.0")) && (intIP <= IpToInt("60.255.255.255"))))
                {
                    Location.Area = "网络保留地址";
                    Location.Address = "";
                }
            }
            var right = ipCount;
            var left = 0L;
            var startIp = 0L;
            var endIpOff = 0L;
            var endIp = 0L;
            var countryFlag = 0;
            while (left < (right - 1L))
            {
                var middle = (right + left)/2L;
                startIp = GetStartIp(middle, out endIpOff);
                if (intIP == startIp)
                {
                    left = middle;
                    break;
                }
                if (intIP > startIp)
                {
                    left = middle;
                }
                else
                {
                    right = middle;
                }
            }
            startIp = GetStartIp(left, out endIpOff);
            endIp = GetEndIp(endIpOff, out countryFlag);
            if ((startIp <= intIP) && (endIp >= intIP))
            {
                string local;
                Location.Area = GetCountry(endIpOff, countryFlag, out local);
                Location.Address = local;
            }
            else
            {
                Location.Area = "未知";
                Location.Address = "";
            }
            Location.Address = Location.Address.Replace("CZ88.NET", "", RegexOptions.IgnoreCase);
            return Location;
        }

        #region 私有方法

        private static long GetStartIp(long left, out long endIpOff)
        {
            var leftOffset = firstStartIpOffset + (left*7L);
            var buffer = new byte[7];
            Array.Copy(data, leftOffset, buffer, 0, 7);
            endIpOff = (Convert.ToInt64(buffer[4].ToString()) + (Convert.ToInt64(buffer[5].ToString())*0x100L)) +
                       ((Convert.ToInt64(buffer[6].ToString())*0x100L)*0x100L);
            return ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString())*0x100L)) +
                    ((Convert.ToInt64(buffer[2].ToString())*0x100L)*0x100L)) +
                   (((Convert.ToInt64(buffer[3].ToString())*0x100L)*0x100L)*0x100L);
        }

        private static long GetEndIp(long endIpOff, out int countryFlag)
        {
            var buffer = new byte[5];
            Array.Copy(data, endIpOff, buffer, 0, 5);
            countryFlag = buffer[4];
            return ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString())*0x100L)) +
                    ((Convert.ToInt64(buffer[2].ToString())*0x100L)*0x100L)) +
                   (((Convert.ToInt64(buffer[3].ToString())*0x100L)*0x100L)*0x100L);
        }

        private static string GetCountry(long endIpOff, int countryFlag, out string local)
        {
            var country = "";
            var offset = endIpOff + 4L;
            switch (countryFlag)
            {
                case 1:
                case 2:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    offset = endIpOff + 8L;
                    local = (1 == countryFlag) ? "" : GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
                default:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    local = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
            }
            return country;
        }

        private static string GetFlagStr(ref long offset, ref int countryFlag, ref long endIpOff)
        {
            var flag = 0;
            var buffer = new byte[3];

            while (true)
            {
                //用于向前累加偏移量
                var forwardOffset = offset;
                flag = data[forwardOffset++];
                //没有重定向
                if (flag != 1 && flag != 2)
                {
                    break;
                }
                Array.Copy(data, forwardOffset, buffer, 0, 3);
                forwardOffset += 3;
                if (flag == 2)
                {
                    countryFlag = 2;
                    endIpOff = offset - 4L;
                }
                offset = (Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString())*0x100L)) +
                         ((Convert.ToInt64(buffer[2].ToString())*0x100L)*0x100L);
            }
            return offset < 12L ? "" : GetStr(ref offset);
        }

        private static string GetStr(ref long offset)
        {
            byte lowByte = 0;
            byte highByte = 0;
            var stringBuilder = new StringBuilder();
            var bytes = new byte[2];
            var encoding = Encoding.GetEncoding("GB2312");
            while (true)
            {
                lowByte = data[offset++];
                if (lowByte == 0)
                {
                    return stringBuilder.ToString();
                }
                if (lowByte > 0x7f)
                {
                    highByte = data[offset++];
                    bytes[0] = lowByte;
                    bytes[1] = highByte;
                    if (highByte == 0)
                    {
                        return stringBuilder.ToString();
                    }
                    stringBuilder.Append(encoding.GetString(bytes));
                }
                else
                {
                    stringBuilder.Append((char) lowByte);
                }
            }
        }

        private static long IpToInt(string ip)
        {
            var separator = new[] {'.'};
            if (ip.Split(separator).Length == 3)
            {
                ip = ip + ".0";
            }
            var strArray = ip.Split(separator);
            var num2 = ((long.Parse(strArray[0])*0x100L)*0x100L)*0x100L;
            var num3 = (long.Parse(strArray[1])*0x100L)*0x100L;
            var num4 = long.Parse(strArray[2])*0x100L;
            var num5 = long.Parse(strArray[3]);
            return (((num2 + num3) + num4) + num5);
        }

        private static string IntToIP(long ip_Int)
        {
            var num = ((ip_Int & 0xff000000L) >> 0x18);
            if (num < 0L)
            {
                num += 0x100L;
            }
            var num2 = (ip_Int & 0xff0000L) >> 0x10;
            if (num2 < 0L)
            {
                num2 += 0x100L;
            }
            var num3 = (ip_Int & 0xff00L) >> 8;
            if (num3 < 0L)
            {
                num3 += 0x100L;
            }
            var num4 = ip_Int & 0xffL;
            if (num4 < 0L)
            {
                num4 += 0x100L;
            }
            return (num.ToString() + "." + num2.ToString() + "." + num3.ToString() + "." + num4.ToString());
        }

        #endregion
    }

    /// <summary>
    ///     地理位置
    /// </summary>
    public class Location
    {
        /// <summary>
        ///     IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        ///     区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        ///     详细位置
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     省
        /// </summary>
        public string Province
        {
            get { return new Regex(".*省|广西|内蒙古|宁夏|新疆|西藏").Match(Area).Value; }
        }

        /// <summary>
        ///     城市
        /// </summary>
        public string City
        {
            get
            {
                return !Province.IsNullOrEmpty() ? Area.Replace(Province, "") : string.Empty;
            }
        }
    }
}