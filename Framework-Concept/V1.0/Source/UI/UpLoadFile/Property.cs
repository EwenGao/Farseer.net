using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using FS.Configs;
using FS.Utils.Common;
using FS.Utils.Web;

namespace FS.UI
{
    /// <summary>
    ///     上传文件组件
    /// </summary>
    public sealed partial class UpLoadFile : CompositeControl
    {
        /// <summary>
        ///     初始化
        /// </summary>
        public UpLoadFile()
        {
            UploadDomain = Req.GetDomain() + "/";
            UploadPath = Files.GetMapPath("/");
            UploadDirectory = GeneralConfigs.ConfigInfo.UploadDirectory;
            EnsureChildControls();
        }

        /// <summary>
        ///     文件上传后，保存的路径
        /// </summary>
        public TextBox txtSavePath { get; set; }

        /// <summary>
        ///     文件上传控件
        /// </summary>
        public FileUpload file { get; set; }

        /// <summary>
        ///     图片显示控件
        /// </summary>
        public Image img { get; set; }

        /// <summary>
        ///     上传按扭
        /// </summary>
        public Button btnUpLoad { get; set; }

        /// <summary>
        ///     提示信息
        /// </summary>
        public Label lblMessage { get; set; }

        /// <summary>
        ///     是否启用压缩
        /// </summary>
        [
            Category("UpLoadFile"),
            DefaultValue("false"),
            Description("是否启用压缩"),
            Browsable(true)
        ]
        public bool IsShowZipped
        {
            get { return pnlZipped.Visible; }
            set { pnlZipped.Visible = value; }
        }

        private CheckBox chkIsZipped { get; set; }

        /// <summary>
        ///     压缩宽度
        /// </summary>
        public TextBox txtZippedWidth { get; set; }

        private Label lblZippedWidth { get; set; }
        private Label lblZippedWidthUnit { get; set; }


        /// <summary>
        ///     压缩高度
        /// </summary>
        public TextBox txtZippedHeight { get; set; }

        private Label lblZippedHeight { get; set; }
        private Label lblZippedHeightUnit { get; set; }

        private Panel pnlZipped { get; set; }

        /// <summary>
        ///     保存到数据库的路径
        /// </summary>
        [
            Category("UpLoadFile"),
            DefaultValue(""),
            Description("保存到数据库的路径"),
            Browsable(true)
        ]
        public string SavePath
        {
            get { return txtSavePath.Text; }
            set { txtSavePath.Text = value; }
        }

        /// <summary>
        ///     图片显示根
        /// </summary>
        [
            Category("UpLoadFile"),
            DefaultValue(""),
            Description("图片显示根"),
            Browsable(true)
        ]
        public string UploadDomain { get; set; }

        /// <summary>
        ///     传入服务器的根目录
        /// </summary>
        [
            Category("UpLoadFile"),
            DefaultValue(""),
            Description("传入服务器的根目录"),
            Browsable(true)
        ]
        public string UploadPath { get; set; }

        /// <summary>
        ///     存入数据库的文件夹
        /// </summary>
        [
            Category("UpLoadFile"),
            DefaultValue(""),
            Description("存入数据库的文件夹"),
            Browsable(true)
        ]
        public string UploadDirectory { get; set; }

        /// <summary>
        ///     是否显示图片预览
        /// </summary>
        [
            Category("UpLoadFile"),
            DefaultValue(""),
            Description("是否显示图片预览"),
            Browsable(true)
        ]
        public bool IsShowImage
        {
            get { return img.Visible; }
            set { img.Visible = value; }
        }

        /// <summary>
        ///     是否显示路径文本框
        /// </summary>
        [
            Category("UpLoadFile"),
            DefaultValue(""),
            Description("是否显示路径文本框"),
            Browsable(true)
        ]
        public bool IsShowFilePath
        {
            get { return txtSavePath.Visible; }
            set { txtSavePath.Visible = value; }
        }

        /// <summary>
        ///     保存方式
        /// </summary>
        [
            Category("UpLoadFile"),
            DefaultValue(""),
            Description("保存方式"),
            Browsable(true)
        ]
        public Utils.Web.UpLoadFile.SaveType SaveType { get; set; }

        /// <summary>
        ///     上传的文件权限
        /// </summary>
        [
            Category("UpLoadFile"),
            DefaultValue(""),
            Description("上传的文件权限"),
            Browsable(true)
        ]
        public List<Utils.Web.UpLoadFile.stuUpLoadFileType> UpLoadFileTypeList { get; set; }
    }
}