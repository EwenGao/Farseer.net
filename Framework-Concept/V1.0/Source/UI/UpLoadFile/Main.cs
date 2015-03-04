using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: TagPrefix("FS.UI", "Qyn")]

namespace FS.UI
{
    /// <summary>
    ///     上传文件组件
    /// </summary>
    [
        DefaultProperty("PageSize"),
        ToolboxData("<{0}:UpLoadFile runat=server />")
    ]
    public sealed partial class UpLoadFile : CompositeControl
    {
        /// <summary>
        ///     输出控件视图
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            img.ImageUrl = UploadDomain + SavePath; //显示图片

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            if (img.Visible)
            {
                img.RenderControl(writer);
            }
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            if (pnlZipped.Visible)
            {
                pnlZipped.RenderControl(writer);
            }

            txtSavePath.RenderControl(writer);

            writer.Write("<br />");
            file.RenderControl(writer);

            btnUpLoad.RenderControl(writer);

            if (!string.IsNullOrEmpty(lblMessage.Text))
            {
                lblMessage.RenderControl(writer);
            }

            writer.RenderEndTag();
        }

        /// <summary>
        ///     初始化控件
        /// </summary>
        protected override void CreateChildControls()
        {
            Controls.Clear();
            pnlZipped = new Panel { ID = "pnlZipped" };
            Controls.Add(pnlZipped);

            chkIsZipped = new CheckBox { ID = "chkIsZipped", Text = "压缩图片" };
            pnlZipped.Controls.Add(chkIsZipped);

            lblZippedWidth = new Label { ID = "lblZippedWidth", Text = "   宽度：" };
            pnlZipped.Controls.Add(lblZippedWidth);

            txtZippedWidth = new TextBox { ID = "txtZippedWidth", Text = "0", Width = 40 };
            pnlZipped.Controls.Add(txtZippedWidth);

            lblZippedWidthUnit = new Label { ID = "lblZippedWidthUnit", Text = "px " };
            pnlZipped.Controls.Add(lblZippedWidthUnit);

            lblZippedHeight = new Label { ID = "lblZippedHeight", Text = "   高度：" };
            pnlZipped.Controls.Add(lblZippedHeight);

            txtZippedHeight = new TextBox { ID = "txtZippedHeight", Text = "0", Width = 40 };
            pnlZipped.Controls.Add(txtZippedHeight);

            lblZippedHeightUnit = new Label { ID = "txtZippedHeightUnit", Text = "px " };
            pnlZipped.Controls.Add(lblZippedHeightUnit);

            txtSavePath = new TextBox { ID = "txtSavePath", Width = 250 };
            Controls.Add(txtSavePath);

            file = new FileUpload { ID = "filePath", Width = 320 };
            Controls.Add(file);

            img = new Image { ID = "imgPath" };
            Controls.Add(img);

            btnUpLoad = new Button { ID = "btnUpLoad", Text = "上传", CommandName = "OnUpLoad" };
            Controls.Add(btnUpLoad);

            lblMessage = new Label { ID = "lblMessage" };
            Controls.Add(lblMessage);

            //告诉编译器，控件已经初始化了
            ChildControlsCreated = true;
        }
    }
}