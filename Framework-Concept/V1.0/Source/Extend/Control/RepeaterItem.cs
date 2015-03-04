using System.Web.UI;
using System.Web.UI.WebControls;

namespace FS.Extend
{
    /// <summary>
    ///     控件扩展工具
    /// </summary>
    public static partial class ControlExtend
    {
        /// <summary>
        ///     获取父类RepeaterItem
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="Container">RepeaterItem</param>
        /// <returns></returns>
        public static T Parent<T>(this RepeaterItem Container)
        {
            return (T) ((RepeaterItem) (Container.NamingContainer).NamingContainer).DataItem;
        }

        /// <summary>
        /// Container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Container"></param>
        /// <returns></returns>
        public static T Item<T>(this RepeaterItem Container)
        {
            return (T) (Container).DataItem;
        }

        /// <summary>
        ///  获取数据绑定上下文数据项。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="page">页面</param>
        public static T GetDataItem<T>(this Page page)
        {
            return (T)page.GetDataItem();
        }
    }
}