using System;
using System.Xml;
using System.IO;

namespace WexinTest
{
    /// <summary>
    /// 微信普通消息物件
    /// </summary>
    public class WexinGeneralMessage
    {
        /// <summary>
        /// 消息id
        /// </summary>
        public string MsgId { get; set; }
        /// <summary>
        /// 接收方帳號（收到的OpenID)
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 傳送方的帳號
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 消息創建時間（秒數型態）
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 消息類型
        /// </summary>
        public string MsgType { get; set; }
        /// <summary>
        /// 消息內容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 我方回應狀態
        /// <para>0=等待回應、1=已回應過、2=不需要回應</para>
        /// </summary>
        public string ReplyStatus { get; set; }

        /// <summary>
        /// 取得微信需要的訊息XML格式
        /// </summary>
        public string ToXmlMessage
        {
            get
            {
                var xDoc = new XmlDocument();
                var xRoot = xDoc.CreateElement("xml");
                var xNod = xDoc.CreateElement("ToUserName");
                xNod.AppendChild(xDoc.CreateCDataSection(ToUserName));
                xRoot.AppendChild(xNod);
                xNod = xDoc.CreateElement("FromUserName");
                xNod.AppendChild(xDoc.CreateCDataSection(FromUserName));
                xRoot.AppendChild(xNod);
                xNod = xDoc.CreateElement("CreateTime");
                xNod.InnerText = Convert.ToInt64((CreateTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString();
                xRoot.AppendChild(xNod);
                xNod = xDoc.CreateElement("MsgType");
                xNod.AppendChild(xDoc.CreateCDataSection(MsgType));
                xRoot.AppendChild(xNod);
                xNod = xDoc.CreateElement("Content");
                xNod.AppendChild(xDoc.CreateCDataSection(Content));
                xRoot.AppendChild(xNod);

                xDoc.AppendChild(xRoot);

                var tmp = string.Empty;
                using (var sw = new StringWriter())
                using (var xw = XmlWriter.Create(sw))
                {
                    xDoc.WriteTo(xw);
                    xw.Flush();
                    tmp = sw.GetStringBuilder().ToString();
                }

                return tmp;
            }
        }



 


    }
}
