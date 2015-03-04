using System.Collections.Generic;
using System.Management;

namespace FS.Utils.Common
{
    /// <summary>
    /// 机器硬件信息
    /// </summary>
    public class Mac
    {
        /// <summary>
        /// 获取CPU码
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCpu()
        {
            var lst = new List<string>();
            var mc = new ManagementClass("Win32_Processor");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                lst.Add(mo.Properties["ProcessorId"].Value.ToString());
            }
            return lst;
        }

        /// <summary>
        /// 获取硬盘码
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHD()
        {
            var lst = new List<string>();
            var mc = new ManagementClass("Win32_DiskDrive");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                lst.Add(mo.Properties["Model"].Value.ToString());
            }
            return lst;
        }

        /// <summary>
        /// 获取网卡码
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMac()
        {
            var lst = new List<string>();
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                if ((bool)mo["IPEnabled"]) { lst.Add(mo["MacAddress"].ToString()); }
                mo.Dispose();
            }
            return lst;
        }
    }
}
