using System;
using System.Text;

namespace FS.Utils.Common
{
    /// <summary>
    ///     对StringBuilder再封装
    /// </summary>
    public class StrPlus
    {
        // Fields
        private StringBuilder str = new StringBuilder();

        /// <summary>
        ///     给定类型，返回DbExecutor
        /// </summary>
        /// <param name="type">实体类型</param>
        public static implicit operator string(StrPlus sp)
        {
            return sp.Value;
        }

        /// <summary>
        ///     给定类型，返回DbExecutor
        /// </summary>
        /// <param name="type">实体类型</param>
        public static implicit operator StrPlus(string s)
        {
            return new StrPlus(s);
        }

        public StrPlus(string strValue) { str = new StringBuilder(strValue); }
        public StrPlus() { }

        public char this[int index]
        {
            get { return str[index]; }
            set { str[index] = value; }
        }

        /// <summary>
        ///     返回所有字符串
        /// </summary>
        public string Value
        {
            get { return str.ToString(); }
        }

        /// <summary>
        ///     添加字符串
        /// </summary>
        public void Append(string Text, int SpaceNum = 0)
        {
            str.Append(Space(SpaceNum) + Text);
        }

        /// <summary>
        ///     添加字符串
        /// </summary>
        public void AppendLine(string Text = "", int SpaceNum = 0)
        {
            Append(Text, SpaceNum);
            Append("\r\n");
        }

        /// <summary>
        ///     添加字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void AppendFormat(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                format = string.Format(format, args);
            }
            Append(format);
        }

        /// <summary>
        ///     添加字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void AppendFormatLine(string format, params object[] args)
        {
            AppendFormat(format, args);
            AppendLine();
        }

        /// <summary>
        ///     添加字符串
        /// </summary>
        /// <param name="SpaceNum"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void AppendFormatLine(string format, int SpaceNum = 0, params object[] args)
        {
            AppendFormat(format, SpaceNum, args);
            AppendLine();
        }

        /// <summary>
        ///     删除指定最后的字符串
        /// </summary>
        public string DelLastChar(string strChar)
        {
            var s = this.str.ToString();
            var length = s.LastIndexOf(strChar);
            if (length > 0)
            {
                this.str = new StringBuilder();
                this.str.Append(s.Substring(0, length));
            }
            return this.str.ToString();
        }

        /// <summary>
        ///     移出指定数量的字符串
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="Num"></param>
        public void Remove(int Start, int Num)
        {
            str.Remove(Start, Num);
        }

        /// <summary>
        ///     制表符
        /// </summary>
        public string Space(int SpaceNum)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < SpaceNum; i++)
            {
                builder.Append("\t");
            }
            return builder.ToString();
        }

        /// <summary>
        ///     转换为String类型
        /// </summary>
        public override string ToString()
        {
            return str.ToString();
        }

        /// <summary>
        ///     清除所有字符串
        /// </summary>
        public void Clear()
        {
            str = new StringBuilder();
        }

        /// <summary>
        /// 从此实例检索子字符串。子字符串从指定的字符位置开始且具有指定的长度。
        /// </summary>
        /// <param name="startIndex">此实例中子字符串的起始字符位置（从零开始）。</param>
        /// <param name="length">子字符串中的字符数。</param>
        public string Substring(int startIndex, int length = 0)
        {
            if (length == 0) { return str.ToString().Substring(startIndex); }
            return str.ToString().Substring(startIndex, length);
        }

        /// <summary>
        /// 获取当前 System.String 对象中的字符数。
        /// </summary>
        public int Length { get { return str.ToString().Length; } }

        /// <summary>
        /// 将对象的字符串表示形式插入到此实例中的指定字符位置。
        /// </summary>
        /// <param name="index">此实例中开始插入的位置。</param>
        /// <param name="value">要插入的对象或 null。</param>
        public StrPlus Insert<T>(int index, T value)
        {
            str.Insert(index, value);
            return this;
        }

        /// <summary>
        /// 重载+运算符，实现字符串相加的方式
        /// </summary>
        /// <param name="a">字符串</param>
        /// <param name="b">本体</param>
        public static StrPlus operator +(string a, StrPlus b)
        {
            return b.Insert(0, a);
        }

        /// <summary>
        /// 重载+运算符，实现字符串相加的方式
        /// </summary>
        /// <param name="a">本体</param>
        /// <param name="b">字符串</param>
        public static StrPlus operator +(StrPlus b, string a)
        {
            b.Append(a);
            return b;
        }

        /// <summary>
        /// 返回一个新字符串，其中当前实例中出现的所有指定字符串都替换为另一个指定的字符串。
        /// </summary>
        /// <param name="oldValue">要被替换的字符串。</param>
        /// <param name="newValue">要替换出现的所有 oldValue 的字符串。</param>
        public StrPlus Replace(string oldValue, string newValue)
        {
            str = str.Replace(oldValue, newValue);
            return this;
        }

        /// <summary>
        ///     分隔字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="splitString">分隔符号</param>
        /// <returns></returns>
        public string[] Split(string splitString = ",")
        {
            return str.ToString().Split(new string[1] { splitString }, StringSplitOptions.None);
        }

        /// <summary>
        /// 报告指定字符串在此实例中的第一个匹配项的索引。
        /// </summary>
        public int IndexOf(string value)
        {
            return str.ToString().IndexOf(value);
        }

        /// <summary>
        /// 报告指定字符串在此实例中的第一个匹配项的索引。
        /// </summary>
        public int IndexOf(char value)
        {
            return str.ToString().IndexOf(value);
        }
    }
}