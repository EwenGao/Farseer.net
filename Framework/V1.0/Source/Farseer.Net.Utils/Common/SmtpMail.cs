using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using FS.Configs;
using FS.Extend;
using FS.Utils.Web;

namespace FS.Utils.Common
{
    /// <summary>
    ///     E-Mail工具
    /// </summary>
    public abstract class SmtpMail
    {
        /// <summary>
        ///     将字符串编码为Base64字符串
        /// </summary>
        private static string Base64Encode(string str)
        {
            var barray = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(barray);
        }

        /// <summary>
        ///     发送电子邮件
        /// </summary>
        /// <param name="mail">Email配置</param>
        /// <param name="lstAddress">收件人地址</param>
        /// <param name="subject">邮件的标题</param>
        /// <param name="body">邮件的正文</param>
        /// <param name="fileName">邮件附件路径名</param>
        public static bool Send(List<string> lstAddress, string subject, string body, EmailInfo mail = null,
                                string fileName = "")
        {
            if (mail == null)
            {
                mail = 0;
            }
            if (lstAddress.Count > mail.RecipientMaxNum)
            {
                new Terminator().Throw("收件人地址数量不能超过" + mail.RecipientMaxNum);
                return false;
            }

            var ObjSmtpClient = new SmtpClient(mail.SmtpServer, mail.SmtpPort)
                                    {
                                        DeliveryMethod =
                                            SmtpDeliveryMethod.Network,
                                        Credentials =
                                            new NetworkCredential(
                                            mail.LoginName, mail.LoginPwd)
                                    };

            var ObjMailMessage = new MailMessage {From = new MailAddress(mail.SendMail, mail.SendUserName)};
            foreach (var item in lstAddress)
            {
                ObjMailMessage.To.Add(new MailAddress(item));
            }

            ObjMailMessage.Subject = subject; //发送邮件主题
            ObjMailMessage.Body = body; //发送邮件内容
            ObjMailMessage.BodyEncoding = Encoding.Default; //发送邮件正文编码
            ObjMailMessage.IsBodyHtml = true; //设置为HTML格式
            ObjMailMessage.Priority = MailPriority.High; //优先级
            if (!fileName.IsNullOrEmpty()) ObjMailMessage.Attachments.Add(new Attachment(fileName)); //邮件的附件

            ObjSmtpClient.Send(ObjMailMessage);
            return true;
        }
    }
}