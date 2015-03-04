using Demo.PO.Table.Members;
using FS.Core.Context;

namespace Demo.PO.Table
{
    /// <summary>
    /// 表容器
    /// </summary>
    public class TableContainer : TableContext
    {
        public TableContainer() : base(0) { }

        public static TableContainer Instance { get { return new TableContainer(); } }

        public TableSet<UserPO> User { get; set; }
        public TableSet<UserPO> User2 { get; set; }
        public TableSet<UserPO> User3 { get; set; }
    }
}