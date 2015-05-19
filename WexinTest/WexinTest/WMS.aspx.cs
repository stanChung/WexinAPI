using System;
using System.Data;
using System.Web.UI.WebControls;

namespace WexinTest
{
    public partial class WMS : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                bindReplyList();

            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (ddlMsg.SelectedValue == "")
            {
                //ClientScript.RegisterStartupScript(Page.GetType(), "alertmsg", "<script>alert('請選取一則訊息!!!');</script>");
                return;
            }
            if (txtReply.Text.Length == 0)
            {
                //ClientScript.RegisterStartupScript(Page.GetType(), "alertmsg", "<script>alert('請輸入回覆訊息!!!');</script>");
                return;
            }

            var content = txtReply.Text;
            var msgId = ddlMsg.SelectedValue.Split(new char[] { '#' })[0];
            var openId = ddlMsg.SelectedValue.Split(new char[] { '#' })[1];
            Common.SendCustomMessage(msgId, openId, content);

            bindReplyList();

            //ClientScript.RegisterStartupScript(Page.GetType(), "alertmsg", "<script>alert('訊息發送成功!!!');</script>");
        }

        private void bindReplyList()
        {
            ddlMsg.Items.Clear();

            DataTable dt = DataAccess.GetReplyList();

            ddlMsg.Items.Add(new ListItem("請選擇", ""));

            foreach (DataRow dr in dt.Rows)
            {
                var tmpContent = dr["Content"].ToString();
                ddlMsg.Items.Add(new ListItem(dr["NickName"].ToString() + "_" + (tmpContent.Length > 30 ? tmpContent.Substring(0, 30) : tmpContent),
                    dr["MsgId"].ToString() + "#" + dr["FromUserName"].ToString()));
            }
        }

        protected void btnRefresh_ServerClick(object sender, EventArgs e)
        {
            bindReplyList();
        }
    }
}
