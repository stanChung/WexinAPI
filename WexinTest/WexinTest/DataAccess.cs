using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace WexinTest
{
    public class DataAccess
    {
        private static string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["WMS"].ConnectionString;

        /// <summary>
        /// 新增一筆普通文本消息資料
        /// </summary>
        /// <param name="o"></param>
        public static void InsertTextMessage(WexinGeneralMessage o)
        {
            using (var conn = new SqlConnection(conStr))
            using (var cmd = new SqlCommand())
            {
                cmd.CommandText =
                    @"INSERT INTO wmstextmessage values(@MsgId,@ToUserName,@FromUserName,@CreateTime,@MsgType,@Content,@ReplyStatus)";

                cmd.Parameters.AddWithValue("@MsgId", o.MsgId);
                cmd.Parameters.AddWithValue("@ToUserName", o.ToUserName);
                cmd.Parameters.AddWithValue("@FromUserName", o.FromUserName);
                cmd.Parameters.AddWithValue("@CreateTime", DateTime.Parse(o.CreateTime.ToString("yyyy/MM/dd HH:mm:ss")));
                cmd.Parameters.AddWithValue("@MsgType", o.MsgType);
                cmd.Parameters.AddWithValue("@Content", o.Content);
                cmd.Parameters.AddWithValue("@ReplyStatus", o.ReplyStatus);

                cmd.Connection = conn;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 重置ACCESS_TOKEN
        /// </summary>
        public static void ResetAccessToken(Common.tokenData token)
        {
            using (var conn = new SqlConnection(conStr))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"DELETE wmsaccesstoken;
                                       INSERT INTO wmsaccesstoken VALUES(@access_token,@expire_datetime);";
                    cmd.Connection.Open();
                    cmd.Transaction = cmd.Connection.BeginTransaction();
                    cmd.Parameters.AddWithValue("@access_token", token.access_token);
                    cmd.Parameters.AddWithValue("@expire_datetime", token.expir_datetime);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        cmd.Transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 取得目前系統存在的access_token
        /// </summary>
        /// <returns></returns>
        public static Common.tokenData GetAccessToken()
        {
            var token = new Common.tokenData();

            using (var conn = new SqlConnection(conStr))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.Connection.Open();
                cmd.CommandText = "SELECT * FROM wmsaccesstoken";
                var sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    token.access_token = sdr["access_token"].ToString();
                    token.expir_datetime = Convert.ToDateTime(sdr["expire_datetime"]);
                }
            }


            if ((token.expir_datetime - DateTime.Now).TotalSeconds <= 0)
            {
                //如果過期了，就重新取號
                token = Common.ApplyNewAccessTokenFromWexin("wxa18aa0df297e3e28", "745d52f6538ad54efa8d723df89137e5");
                DataAccess.ResetAccessToken(token);
            }

            return token;
        }

        /// <summary>
        /// 取得待回應清單
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DataTable GetReplyList()
        {
            var token = DataAccess.GetAccessToken();
            var strSQL = @"SELECT * from wmstextmessage where ReplyStatus=0";
            var dt = new DataTable();

            using (var conn = new SqlConnection(conStr))
            using (var sda = new SqlDataAdapter(strSQL, conn))
            {
                conn.Open();
                sda.Fill(dt);

            }
            if (dt.Rows.Count > 0)
            {
                dt.Columns.Add(new DataColumn("NickName"));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["NickName"] = Common.RetriveUser(token, dt.Rows[i]["FromUserName"].ToString()).nickname;
                }
            }

            return dt;
        }

        /// <summary>
        /// 更新微信文本訊息資料的回覆狀態
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="statusId">我方回應狀態
        /// <para>0=等待回應、1=已回應過、2=不需要回應</para>
        /// </param>
        public static void SetGeneralMsgReplyStatus(string msgId, string statusId)
        {
            using(var conn=new SqlConnection(conStr))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"Update wmstextmessage set ReplyStatus=@ReplyStatus where MsgId=@MsgId";

                cmd.Parameters.AddWithValue("@ReplyStatus", statusId);
                cmd.Parameters.AddWithValue("@MsgId", msgId);

                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
