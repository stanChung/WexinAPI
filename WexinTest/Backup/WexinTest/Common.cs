using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WexinTest
{
    public class Common
    {
        public static readonly string Token = "kelly";

        static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// 解收普通消息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static WexinGeneralMessage ReciveGeneralMsg(HttpContext context)
        {
            var msgRecive = new WexinGeneralMessage();
            var xmlContent = string.Empty;

            try
            {
                var msgStream = context.Request.InputStream;
                var buffer = new byte[msgStream.Length];
                msgStream.Read(buffer, 0, buffer.Length);
                xmlContent = Encoding.UTF8.GetString(buffer);


                var xDoc = new XmlDocument();
                xDoc.LoadXml(xmlContent);
                msgRecive.ToUserName = xDoc.SelectSingleNode("xml/ToUserName").InnerText;
                msgRecive.FromUserName = xDoc.SelectSingleNode("xml/FromUserName").InnerText;
                msgRecive.CreateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds((double)long.Parse(xDoc.SelectSingleNode("xml/CreateTime").InnerText));
                msgRecive.MsgType = xDoc.SelectSingleNode("xml/MsgType").InnerText;
                msgRecive.Content = xDoc.SelectSingleNode("xml/Content").InnerText;
                msgRecive.MsgId = xDoc.SelectSingleNode("xml/MsgId").InnerText;
                msgRecive.ReplyStatus = "0";
                DataAccess.InsertTextMessage(msgRecive);

                logger.InfoFormat(
                    "General message recive success：{0}" + Environment.NewLine +
                    "XML Content：{1}" + Environment.NewLine,
                    DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), xmlContent);
                logger.InfoFormat("----------------------------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                logger.InfoFormat(
                    "General message recive faild：{0}" + Environment.NewLine +
                    "Error message：{1}" + Environment.NewLine,
                    DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), ex.Message);
                logger.InfoFormat("----------------------------------------------------------------------------------------------");
            }

            return msgRecive;
        }

        /// <summary>
        /// 接收並回覆普通消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="replyMsg">回應消息內容</param>
        /// <returns></returns>
        public static string ReciveGeneralMsg(HttpContext context, string replyMsg)
        {
            var msgRecive = ReciveGeneralMsg(context);
            var msgResponse = new WexinGeneralMessage();

            msgResponse.ToUserName = msgRecive.FromUserName;
            msgResponse.FromUserName = msgRecive.ToUserName;
            msgResponse.CreateTime = msgRecive.CreateTime;
            msgResponse.MsgType = msgRecive.MsgType;
            msgResponse.Content = replyMsg;

            return msgResponse.ToXmlMessage;

        }

        /// <summary>
        /// 查詢使用者基本資料
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static WexinUserProfile RetriveUser(tokenData accessToken, string openId)
        {


            var url = string.Format(@"https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_TW", accessToken.access_token, openId);
            var request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            var response = (HttpWebResponse)request.GetResponse();
            var text = string.Empty;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            WexinUserProfile user = JsonConvert.DeserializeObject<WexinUserProfile>(text);

            return user;
        }

        /// <summary>
        /// 取得微信的基礎接口存取tpken
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static tokenData ApplyNewAccessTokenFromWexin(string appid, string secret)
        {

            var url = string.Format(@"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);
            var request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            var response = (HttpWebResponse)request.GetResponse();
            var text = string.Empty;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }
            tokenData tb = JsonConvert.DeserializeObject<tokenData>(text);
            tb.expir_datetime = DateTime.Now.AddSeconds(7200);


            return tb;
        }


        /// <summary>
        /// 微信驗證
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool valid(HttpContext context)
        {

            var result = false;
            try
            {
                var signature = context.Request["signature"];
                var timestamp = context.Request["timestamp"];
                var nonce = context.Request["nonce"];
                var echostr = context.Request["echostr"];

                if (Common.valid(signature, timestamp, nonce, echostr))
                {
                    result = true;
                    context.Response.Write(echostr);
                    context.Response.End();
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        /// <summary>
        /// 發送客服文本消息給指定的用戶帳號
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="lstContent"></param>
        public static void SendCustomMessage(string msgId,string openId, string content)
        {

            var jobj = new JObject(
                new JProperty("touser", openId),
                new JProperty("msgtype", "text"),
                new JProperty("text", new JObject(new JProperty("content", content)))
                );





            var url = string.Format(@"https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}", DataAccess.GetAccessToken().access_token);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                var json = JsonConvert.SerializeObject(jobj, Newtonsoft.Json.Formatting.Indented);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            DataAccess.SetGeneralMsgReplyStatus(msgId, "1");
        }





        /// <summary>
        /// 微信驗證
        /// </summary>
        /// <param name="signature">簽章</param>
        /// <param name="timestamp">時間戳記</param>
        /// <param name="nonce">隨機數字</param>
        /// <param name="echostr">回應給微信的隨機字串</param>
        /// <returns></returns>
        private static bool valid(string signature, string timestamp, string nonce, string echostr)
        {


            var result = false;

            logger.Info("Start Validation：" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            logger.InfoFormat(string.Format("signature={0}" + Environment.NewLine + "timestamp={1}" + Environment.NewLine + "nonce={2}" + Environment.NewLine + "echostr={3}" + Environment.NewLine, signature, timestamp, nonce, echostr));

            result = CheckSignature(signature, timestamp, nonce) && !string.IsNullOrEmpty(echostr);

            logger.Info("Validation result：" + result + Environment.NewLine);
            logger.InfoFormat("----------------------------------------------------------------------------------------------");
            return result;
        }


        /// <summary>
        /// 驗證微信簽名
        /// </summary>
        /// <returns></returns>
        private static bool CheckSignature(string signature, string timestamp, string nonce)
        {
            string[] ArrTmp = { Token, timestamp, nonce };
            Array.Sort(ArrTmp); //字典排序
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();

            logger.InfoFormat("SHA1:{0}" + Environment.NewLine, tmpStr);


            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public class tokenData
        {
            public string access_token { get; set; }
            public string expires_in { get; set; }
            public DateTime expir_datetime { get; set; }
        }



    }



}


