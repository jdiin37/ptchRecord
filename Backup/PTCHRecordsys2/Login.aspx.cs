using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dev.ADODBSql;
using System.Data;

namespace PTCHRecordsys2
{
    public partial class Login : System.Web.UI.Page
    {
        protected System.Data.DataTable dt
        {
            get { return (System.Data.DataTable)ViewState["dt"]; }
            set { ViewState["dt"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, ImageClickEventArgs e)
        {
            bool flag = true;
            string strAlert = String.Empty;
            string strUserName = String.Empty;
            if (flag)
            {
                string strSql = string.Format("Select userid,username From users Where userid='{0}' And userpasswd='{1}'", txtUID.Text.Trim(), txtPwd.Text.Trim());

                using (DevADODBConn conn = new DevADODBConn("pch01"))
                {
                    dt = conn.GetData(strSql);
                }
                if (dt.Rows.Count <= 0)
                {
                    flag = false;
                    strAlert += "此帳號不存在或密碼錯誤";
                }
                else
                {
                    strUserName = dt.Rows[0]["username"].ToString().Trim();
                }
            }

            if (flag)
            {
                string strSql = string.Format("Select FIRST 1 a.prog_id From users b Inner Join ctrl_mpriv a On a.user_id=b.userid Where a.user_id='{0}' Or prog_id='{1}'", dt.Rows[0]["userid"].ToString(), "-");

                DataTable dt2 = new DataTable();
                using (DevADODBConn conn = new DevADODBConn("pch01"))
                {
                    dt2 = conn.GetData(strSql);
                }
                if (dt2.Rows.Count <= 0)
                {
                    flag = false;
                    strAlert += "此帳號沒有本系統權限";
                }
            }

            if (flag)
            {
                //string strSql = "If NOT EXISTS (Select ID From HMC_Permit Where UID=@UID And ProcID='hmc0000p')" +
                //    " Begin Insert Into HMC_Permit (ID, UID, ProcID, Cre_Date, Cre_User) Values (NEWID(), @UID, @ProcID, @Cre_Date, @Cre_User) End";
                //SqlCommand cmd = new SqlCommand(strSql);
                //cmd.Parameters.Add("@UID", System.Data.SqlDbType.VarChar).Value = dt.Rows[0]["userid"].ToString();
                //cmd.Parameters.Add("@ProcID", System.Data.SqlDbType.VarChar).Value = "hmc0000p";
                //cmd.Parameters.Add("@Cre_Date", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                //cmd.Parameters.Add("@Cre_User", System.Data.SqlDbType.VarChar).Value = dt.Rows[0]["userid"].ToString();
                //DevConn conn = new DevConn();
                //conn.ExecuteQuery(cmd);

                //Session["UID"] = dt.Rows[0]["userid"].ToString();

                Session["UID"] = txtUID.Text.Trim();
                Session["UName"] = strUserName;

                Response.Redirect(ResolveUrl("~/Main.aspx"));
            }
        }
    }
}