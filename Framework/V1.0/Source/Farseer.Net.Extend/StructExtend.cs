namespace FS.Extend
{
    /// <summary>
    ///     数字类型扩展工具
    /// </summary>
    public static class StructExtend
    {
        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this int number, bool isHaveTag = true, int len = 2)
        {
            var str = string.Empty;
            if (isHaveTag)
            {
                str = "￥";
            }
            return str + number.ToString(string.Format("n{0}", len));
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this int? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this uint number, bool isHaveTag = true, int len = 2)
        {
            var str = string.Empty;
            if (isHaveTag)
            {
                str = "￥";
            }
            return str + number.ToString(string.Format("n{0}", len));
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this uint? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this decimal number, bool isHaveTag = true, int len = 2)
        {
            var str = string.Empty;
            if (isHaveTag)
            {
                str = "￥";
            }
            return str + number.ToString(string.Format("n{0}", len));
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this decimal? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this double number, bool isHaveTag = true, int len = 2)
        {
            var str = string.Empty;
            if (isHaveTag)
            {
                str = "￥";
            }
            return str + number.ToString(string.Format("n{0}", len));
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this double? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     当NullOrEmpty，用新的字符串代替，否则用原来的。
        /// </summary>
        /// <param name="obj">要检测的值</param>
        /// <param name="newString">要替换的新字符串</param>
        public static string IsNullOrEmpty<T>(this T? obj, string newString) where T : struct
        {
            return (obj == null || obj.ToString().IsNullOrEmpty()) ? newString : obj.ToString();
        }

        /// <summary>
        ///     当不为NullOrEmpty，用新的字符串代替，否则用原来的。
        /// </summary>
        /// <param name="obj">要检测的值</param>
        /// <param name="newString">要替换的新字符串</param>
        public static string IsNotNullOrEmpty<T>(this T? obj, string newString) where T : struct
        {
            return (obj == null || obj.ToString().IsNullOrEmpty()) ? obj.ToString() : newString;
        }
    }
}