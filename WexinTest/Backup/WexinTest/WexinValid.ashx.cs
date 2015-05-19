using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WexinTest
{
    /// <summary>
    /// $codebehindclassname$ 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WexinValid : IHttpHandler
    {

        public readonly string Token = "kelly";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var signature = context.Request["signature"];
            var timestamp = context.Request["timestamp"];
            var nonce = context.Request["nonce"];
            var echostr = context.Request["echostr"];

            //if (Common.valid(signature, timestamp, nonce, echostr))
            //    context.Response.Write(echostr);
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
