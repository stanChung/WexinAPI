using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace WexinTest
{
    /// <summary>
    /// $codebehindclassname$ 的摘要描述
    /// </summary>
    public class GeneralMsg : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {


            Common.ReciveGeneralMsg(context);
            
            context.Response.ContentType = "text/plain";
            context.Response.Write("測試成功!!!");
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
