using FS.Core.Model;
using FS.ORM;
using System.ComponentModel.DataAnnotations;

namespace FS.Model.Web
{
    /// <summary>
    /// 频道
    /// </summary>
    [DB(Name = "Web_ChlDB")]
    public class ChlDB : BaseCateModel<ChlDB> { }
}
