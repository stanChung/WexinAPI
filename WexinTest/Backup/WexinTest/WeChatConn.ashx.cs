using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Text;

namespace WexinTest
{
    /// <summary>
    /// $codebehindclassname$ 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WeChatConn : IHttpHandler
    {

        //public readonly string Token = "kelly";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            if (context.Request.HttpMethod.ToUpper().Equals("POST"))
            {
                var replyMsg = Common.ReciveGeneralMsg(context, "你好！！我們將儘快處理您的問題。");
                context.Response.ContentType = "application/x-www-form-urlencoded";
                context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");

                context.Response.Write(replyMsg);
                context.Response.End();
            }
            else
                Common.valid(context);




        }








        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
