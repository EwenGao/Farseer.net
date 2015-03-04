using System;
using System.Web.UI.WebControls;
using FS.Extend;
using FS.Utils.Common;

namespace FS.UI
{
    /// <summary>
    ///     上传文件组件
    /// </summary>
    public sealed partial class UpLoadFile : CompositeControl
    {
        /// <summary>
        ///     事件Key
        /// </summary>
        private static readonly object EventUpLoadKey = new object();

        /// <summary>
        ///     事件Key
        /// </summary>
        private static readonly object EventUpLoadCompleteKey = new object();

        /// <summary>
        ///     上传文件事件
        /// </summary>
        public event EventHandler OnUpLoadFile
        {
            add { Events.AddHandler(EventUpLoadKey, value); }
            remove { Events.RemoveHandler(EventUpLoadKey, value); }
        }

        /// <summary>
        ///     上传文件成功事件
        /// </summary>
        public event EventHandler OnUpLoadFileComplete
        {
            add { Events.AddHandler(EventUpLoadCompleteKey, value); }
            remove { Events.RemoveHandler(EventUpLoadCompleteKey, value); }
        }

        /// <summary>
        ///     上传事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpLoad(object sender, EventArgs e)
        {
            //获取已注册的事件
            var uploadHandler = (EventHandler) Events[EventUpLoadKey];

            //外部注册事件时调用
            if (uploadHandler != null)
            {
                //执行外部事件
                uploadHandler(this, e);
            }
            else
            {
                if (IsShowImage && string.IsNullOrEmpty(UploadDomain))
                {
                    lblMessage.Text = "开启图片预览时，请填写图片根";
                    return;
                }
                if (string.IsNullOrEmpty(UploadPath))
                {
                    lblMessage.Text = "传入服务器的物理路径不能为空";
                    return;
                }

                if (chkIsZipped.Checked && pnlZipped.Visible)
                {
                    if (txtZippedHeight.Text.ConvertType(0) < 1)
                    {
                        lblMessage.Text = "压缩图片高度必须为数字类型，且不能小于0";
                        return;
                    }
                    if (txtZippedWidth.Text.ConvertType(0) < 1)
                    {
                        lblMessage.Text = "压缩图片宽度必须为数字类型，且不能小于0";
                        return;
                    }
                }
                var uploadFile = new Utils.Web.UpLoadFile();
                var result = uploadFile.Upload(file.PostedFile, UploadPath + UploadDirectory, SaveType, UpLoadFileTypeList);

                lblMessage.Text = result.ErrorMessage;

                if (string.IsNullOrEmpty(lblMessage.Text))
                {
                    SavePath = result.FilePath + result.FileName;

                    //压缩图片
                    if (chkIsZipped.Checked && pnlZipped.Visible)
                    {
                        Thumbnail.MakeThumbnail(SavePath, SavePath, txtZippedWidth.Text.ConvertType(0), txtZippedHeight.Text.ConvertType(0), Thumbnail.ThumbnailType.Max, 100);
                    }

                    SavePath = SavePath.ClearString(Files.ConvertPath(UploadPath));


                    //获取已注册的事件
                    var upLoadCompleteHandler = (EventHandler) Events[EventUpLoadCompleteKey];
                    if (upLoadCompleteHandler != null)
                    {
                        //执行外部事件
                        upLoadCompleteHandler(this, e);
                    }
                }
            }
        }

        /// <summary>
        ///     冒泡事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            var handled = false;
            if (e is CommandEventArgs)
            {
                var ce = (CommandEventArgs) e;
                if (ce.CommandName == "OnUpLoad")
                {
                    OnUpLoad(source, EventArgs.Empty);
                    handled = true;
                }
                else if (ce.CommandName == "btnReturn")
                {
                    lblMessage.Text = "";
                    handled = true;
                }
            }
            return handled;
        }
    }
}