using System.Data;

namespace FS.Extend
{
    /// <summary>
    ///     Data扩展工具
    /// </summary>
    public static class IDataReaderExtend
    {
        /// <summary>
        ///     判断IDataReader是否存在某列
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HaveName(this IDataReader reader, string name)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).IsEquals(name)) { return true; }
            }
            return false;
        }
    }
}