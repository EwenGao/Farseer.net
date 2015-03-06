using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.VO.Members;
using EmitMapper;

namespace Demo.BusinessRule.Members
{
    public class UserBR
    {
        ///// <summary>
        /////     返回用户列表
        ///// </summary>
        ///// <param name="pageSize">显示数量</param>
        ///// <param name="pageIndex">索引页</param>
        ///// <param name="recordCount">记录总数</param>
        //public static List<UserListVO> GetList(int pageSize, int pageIndex, out int recordCount)
        //{
        //    var lst = DB.UserBO.ToList(out recordCount, pageSize, pageIndex);

        //    // var bean = UserBO.Data;

        //    // 获取数据
        //    //  var lst = bean.ToList(out recordCount, pageSize, pageIndex);

        //    return ObjectMapperManager.DefaultInstance.GetMapper<List<UserPO>, List<UserListVO>>().Map(lst);
        //}
    }
}
