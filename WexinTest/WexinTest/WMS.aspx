<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WMS.aspx.cs" Inherits="WexinTest.WMS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="css/bootstrap.min.css" />
    <link rel="stylesheet" href="css/font-awesome.css" />
    <style type="text/css">
        body
        {
            background: #ddd;
        }
        .main
        {
            background: white;
            margin: 20px auto;
            padding: 10px;
            width: 80%;
        }
        .center
        {
            margin: 0 auto;
            width: 95%;
            padding: 20px;
            border-width: 1px;
            border-style: solid;
            border-color: #ddd;
        }
        .title
        {
            margin: 0 auto;
            width: 95%;
            padding: 20px;
            border-width: 1px;
            border-style: solid;
            border-color: #ddd;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main panel panel-default">
        <div class="title panel-heading">
            <h1 class="panel-title">
                <i class="icon-large icon-external-link-sign"></i> 客戶回應</h1>
        </div>
        <div class=" center well">
            <div class="form-group">
            <label for="ddlMsg">待回應消息：</label>
                <asp:DropDownList CssClass="form-control" ID="ddlMsg" runat="server">
                </asp:DropDownList>
            </div>
            <div class="form-group">
            <label for="txtReply">回覆內容：</label>
                <asp:TextBox ID="txtReply" class="form-control" TextMode="MultiLine" Rows="5" runat="server"
                    Width="70%"></asp:TextBox>
            </div>
            <div class="form-group">
                <button id="btnSend" runat="server" class="btn btn-default" onserverclick="btnSend_Click">
                    <i class=" icon-cloud-upload">送出回應</i></button>
                                <button id="btnRefresh" runat="server" class="btn btn-default" onserverclick="btnRefresh_ServerClick">
                    <i class=" icon-refresh">刷新待回覆清單</i></button>
            </div>
        </div>
    </div>
    
    
    
    </form>
</body>
</html>
