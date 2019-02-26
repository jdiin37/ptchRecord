<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PTCHRecordsys2.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>屏基病歷查詢系統</title>
    <link rel="shortcut icon" href="favicon.ico" type="image/x-icon" />
    <link rel="apple-touch-icon" href="favicon.ico" />
    <link href="Css/LoginTemplate/login-box.css" rel="stylesheet" type="text/css" />
    <link href="Css/LoginTemplate/templatemo_style.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        body
        {
            font-family: "Microsoft Jhenghei";
        }
        .style15
        {
            width: 189px;
            height: 37px;
        }
        .style17
        {
            color: #000000;
        }
        .input.bigcheck
        {
            height: 15px;
            width: 20px;
            left: inherit;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div id="templatemo_header_wrapper">
        <div id="templatemo_header">
            <table width="100%">
                <tr>
                    <td>
                        <img alt="" class="style15" src="Images/PTCH_Medical_Records_System.png" />
                    </td>
                </tr>
            </table>
        </div>
        <!-- end of header -->
    </div>
    <!-- end of header_wrapper -->
    <div id="templatemo_content_wrapper_outer">
        <div id="templatemo_content_wrapper_inner">
            <div id="templatemo_content_wrapper">
                <div id="templatemo_main">
                    <div style="padding: 50px 0 0 250px;">
                        <div id="login-box">
                            <h2>
                                登入</h2>
                            <br />
                            若您沒有系統使用權限將無法登入本系統,請先向您的單位主管申請系統使用權限.
                            <div id="login-box-name" style="margin-top: 20px;">
                                職編：</div>
                            <div id="login-box-field" style="margin-top: 20px;">
                                <asp:TextBox ID="txtUID" runat="server" CssClass="form-login" MaxLength="2048"></asp:TextBox>
                            </div>
                            <div id="login-box-name">
                                密碼：</div>
                            <div id="login-box-field">
                                <asp:TextBox ID="txtPwd" runat="server" CssClass="form-login" TextMode="Password" MaxLength="2048"></asp:TextBox>
                            </div>
                            <p>
                                <br />
                            </p>
                            <br />
                            <br />
                            <asp:ImageButton ID="btnSubmit" runat="server" ImageUrl="Css/LoginTemplate/Images/login-btn.png" Height="42px" Width="103px" Style="margin-left: 90px;" OnClick="btnSubmit_Click" />
                            <asp:Label ID="Label1" runat="server" Text="time" Visible="False"></asp:Label>
                        </div>
                    </div>
                </div>
                <!-- end of main -->
            </div>
        </div>
    </div>
    <div id="templatemo_footer_wrapper">
        <div id="templatemo_footer">
            <ul class="footer_menu">
                <li class="style17">如果您使用IE8 (含)以下版本瀏覽器，建議您升級您的IE瀏覽器或使用Google Chrome以獲得最佳瀏覽體驗</li>
                <%--<li class="style17">@Module_Publiclink</li>--%>
            </ul>
            <span class="style17">Copyright © 屏基醫療財團法人屏東基督教醫院 | 系統維護人員：郭璁翰 分機：1731~1733</span>
        </div>
        <!-- end of footer -->
    </div>
    <!-- end of footer_wrapper -->
    </form>
</body>
</html>
