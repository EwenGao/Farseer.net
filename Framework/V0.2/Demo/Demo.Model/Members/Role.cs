using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FS.Core.Model;
using FS.Mapping.Table;

namespace FS.Model.Members
{
    [DB(Name = "Members_Role")]
    public class RoleDB : BaseCacheModel<RoleDB>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Display(Name = "角色名称")]
        [StringLength(50)]
        [Required()]
        public string Caption { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [Display(Name = "角色描述")]
        [StringLength(50)]
        public string Descr { get; set; }

    }
}
