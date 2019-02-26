using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Data;
using Dev.ADODBSql;

namespace PTCHRecordsys2
{
    public partial class mainMaster : System.Web.UI.MasterPage
    {

        protected DataTable dt_VisitList
        {
            get { return (DataTable)ViewState["dt_VisitList"]; }
            set { ViewState["dt_VisitList"] = value; }
        }

        #region 參數傳遞
        public string strPageStatus //頁面三種狀態 One:初始狀態,Two:查詢病人狀態,Three:查詢就診日期狀態
        {
            get { return PageStatus.Value; }
            set { PageStatus.Value = value; }
        }

        public string strdiv_id
        {
            get { return div_IDisShow.Value; }
        }

        public Boolean bl_changedate = false;

        public string str_selectDate
        {
            get { return SelectDate.Value; }
            set { SelectDate.Value = value; }
        }

        public string strdiv_visits
        {
            get { return div_VisitsisShow.Value; }
        }

        public string strdiv_patinfo
        {
            get { return div_patinfoisShow.Value; }
        }

        public string strleftisshow
        {
            get { return leftisShow.Value; }
        }
        public string reg_no
        {
            get{ return asp_lbRegNo.Text; }
            set{ asp_lbRegNo.Text = value; }
        }
        public string pat_id
        {
            get { return asp_lbID.Text; }
            set { asp_lbID.Text = value; }
        }
        public string pat_name
        {
            get { return asp_lbPatName.Text; }
            set { asp_lbPatName.Text = value; }
        }
        public string pat_sex
        {
            get { return asp_lbSex.Text; }
            set { asp_lbSex.Text = value; }
        }
        public string pat_birth
        {
            get { return asp_lbBirthDate.Text; }
            set { asp_lbBirthDate.Text = value; }
        }
        public string opd_date
        {
            get { return asp_lbOpdDate.Text; }
            set { asp_lbOpdDate.Text = value; }
        }
        public string visittype
        {
            get { return asp_lbVisitType.Text; }
            set { asp_lbVisitType.Text = value; }
        }
        public string dep_no
        {
            get { return asp_ddlDepNo.SelectedValue; }
        }
        public string doc_code
        {
            get { return asp_ddlDocName.SelectedValue; }
        }
        public string bed_no
        {
            get { return asp_txtBedNo.Text; }
            set { asp_txtBedNo.Text = value; }
        }
        public string out_date
        {
            get { return asp_txtOutDate.Text; }
            set { asp_txtOutDate.Text = value; }
        }
        public string keybill
        {
            get { return asp_txtKeyBill.Text; }
            set { asp_txtKeyBill.Text = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["UID"] = "f890";
            //Session["UName"] = "郭璁翰";  
            isLogin();

            if (dt_VisitList != null) //為了postback後動態產生的物件不要不見,或許有更好的做法暫時先這樣
            {
                if (dt_VisitList.Rows.Count > 30)
                {
                    foreach (DataRow dr in dt_VisitList.Rows)
                    {
                        PH_visits_tbody.Controls.Add(new LiteralControl("<tr><td>"));
                        PH_visits_tbody.Controls.Add(creat_Linkbutton(dr));
                        PH_visits_tbody.Controls.Add(new LiteralControl("</td></tr>"));
                    }
                }
            }

            if (!IsPostBack)
            {
                queryDepNo(asp_ddlDepNo);
                queryDocName(asp_ddlDocName);
            }            
        }

        protected void isLogin()
        {
            try
            {
                bool flag = Session["UID"] != null ? true : false;

                if (!flag)
                {
                    panelLogin.Controls.Add(new LiteralControl("<a class='dropdown-toggle' data-toggle='dropdown' href='#'>您尚未登入&nbsp;<i class='fa fa-caret-down'></i></a>"));
                    panelLogin.Controls.Add(new LiteralControl("<ul class='dropdown-menu dropdown-user'>"));
                    panelLogin.Controls.Add(new LiteralControl(string.Format("<li><a href='{0}'><i class='fa fa-sign-in fa-fw'></i>&nbsp;登入</a></li>", ResolveUrl("~/Login.aspx"))));
                    panelLogin.Controls.Add(new LiteralControl("</ul>"));

                    Response.Redirect(ResolveUrl("Login.aspx"));
                }
                else
                {
                    panelLogin.Controls.Add(new LiteralControl(string.Format("<a class='dropdown-toggle' data-toggle='dropdown' href='#'>使用者:{0}&nbsp;<i class='fa fa-caret-down'></i></a>", Session["UName"].ToString())));
                    panelLogin.Controls.Add(new LiteralControl("<ul class='dropdown-menu dropdown-user'>"));
                    panelLogin.Controls.Add(new LiteralControl(string.Format("<li><a href='{0}'><i class='fa fa-sign-out fa-fw'></i>&nbsp;登出</a></li>", ResolveUrl("~/Logout.aspx"))));
                    panelLogin.Controls.Add(new LiteralControl("</ul>"));
                }
            }
            catch (Exception ex)
            {
                //PublicLib.handleError("", this.GetType().Name, ex.Message);
            }
        }

        #region Bind Select2
        protected void queryDocName(object sender)
        {
            try
            {
                DataTable dt = new DataTable();
                string sql = "select TRIM(description) As Text, TRIM(item_code) As Value from code_file where item_type = '09' ";
                using (DevADODBConn conn = new DevADODBConn("pch01"))
                {
                    dt = conn.GetData(sql);
                }
                DataView dv = dt.DefaultView;
                dv.Sort = "Text ASC";

                ((System.Web.UI.WebControls.DropDownList)sender).DataTextField = "Text";
                ((System.Web.UI.WebControls.DropDownList)sender).DataValueField = "Value";
                ((System.Web.UI.WebControls.DropDownList)sender).DataSource = dv;
                ((System.Web.UI.WebControls.DropDownList)sender).DataBind();
                ((System.Web.UI.WebControls.DropDownList)sender).Items.Insert(0, new ListItem("", ""));

            }
            catch (Exception ex)
            {
                //PublicLib.handleError("", this.GetType().Name, ex.Message);
            }
        }
        protected void queryDepNo(object sender)
        {
            try
            {
                DataTable dt = new DataTable();
                string sql = "select TRIM(description) As Text, TRIM(item_code) As Value from code_file where item_type = '07' and check_flag= 'Y'";
                using (DevADODBConn conn = new DevADODBConn("pch01"))
                {
                    dt = conn.GetData(sql);
                }
                DataView dv = dt.DefaultView;
                dv.Sort = "Text ASC";

                ((System.Web.UI.WebControls.DropDownList)sender).DataTextField = "Text";
                ((System.Web.UI.WebControls.DropDownList)sender).DataValueField = "Value";
                ((System.Web.UI.WebControls.DropDownList)sender).DataSource = dv;
                ((System.Web.UI.WebControls.DropDownList)sender).DataBind();
                ((System.Web.UI.WebControls.DropDownList)sender).Items.Insert(0, new ListItem("", ""));
            }
            catch (Exception ex)
            {
                //PublicLib.handleError("", this.GetType().Name, ex.Message);
            }
        }
        #endregion

        #region 查詢
        protected void aspbtnIDsearch_Click(object sender, EventArgs e)
        {
            initial_page();
            string ID = asp_txtIDsearch.Text;
            DataTable dt = new DataTable();
            string SQL = String.Empty;

            //找人
            if (ID.Length > 6)
            {
                SQL = string.Format("Select to_char(birth_date) birth_date, p_name, sex, reg_no, id_no From reg_file Where id_no ='{0}'", ID);
            }
            else
            {
                SQL = string.Format("Select to_char(birth_date) birth_date, p_name, sex, reg_no, id_no From reg_file Where reg_no ='{0}'", ID);
            }

            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dt = conn.GetData(SQL);
            }

            foreach (DataRow dr in dt.Rows)
            {
                asp_lbRegNo.Text = dr["reg_no"].ToString().Trim();
                asp_lbID.Text = dr["id_no"].ToString().Trim();
                asp_lbPatName.Text = dr["p_name"].ToString().Trim();
                asp_lbBirthDate.Text = dr["birth_date"].ToString().Trim();
                asp_lbSex.Text = (dr["sex"].ToString().Trim() == "2") ? "女" : "男";
            }

            //找區間
            search_opddate();
            //btn_date_Click(asp_btn_onemonth, EventArgs.Empty); //預設找一個月(Button版)
            strPageStatus = "Two";
            str_selectDate = "";
        }

        private void search_opddate()
        {
            string startDate = string.Empty;
            string endDate = DateTime.Today.Date.ToShortDateString();

            switch (ddl_OpdList.SelectedValue)
            {
                case "month":
                    startDate = DateTime.Now.AddMonths(-1).ToShortDateString();
                    break;
                case "3months":
                    startDate = DateTime.Now.AddMonths(-3).ToShortDateString();
                    break;
                case "6months":
                    startDate = DateTime.Now.AddMonths(-6).ToShortDateString();
                    break;
                case "year":
                    startDate = DateTime.Now.AddYears(-1).ToShortDateString();
                    break;
                case "3years":
                    startDate = DateTime.Now.AddYears(-3).ToShortDateString();
                    break;
                case "5years":
                    startDate = DateTime.Now.AddYears(-5).ToShortDateString();
                    break;
                case "free":
                    startDate = txt_FreeDate1.Text;
                    endDate = txt_FreeDate2.Text;
                    break;
                default://以上都不成立執行預設值
                    break;
            }

            string strSQL = "select to_char(opd_date,'%Y/%m/%d') opd_date,dep_no,'' typeName,doc_code Dor,'' DorName , '' out_date,DECODE(room_no,'ER','急診','門診') type, '' bed_no,reg_bill keybill,reg_no,ill_code,ill_code1,ill_code2,ill_code3,card_no ,insurance ";
            strSQL += " from v_all_opd_reg ";
            strSQL += " where reg_no = '";
            strSQL += asp_lbRegNo.Text + "'";
            strSQL += " and rec_status='A' ";
            strSQL += "and ((ins_remark<>'E' and ins_remark<>'J' and ins_remark<>'K' and ins_remark<>'U') or (ins_remark is null)) ";
            strSQL += " and opd_date >= '";
            strSQL += startDate + "' ";
            strSQL += " and opd_date <= '";
            strSQL += endDate + "' ";
            strSQL += " union all ";
            //strSQL += "select in_date 日期,dep_no 科別,'' 科別名稱,doc_code 醫師,'' 醫生姓名,to_char(out_date,'%Y/%m/%d') 出院日,'住院' 診別, bed_no 床號,rel_house_no keybill,reg_no,'' ill_code,'' ill_code1,'' ill_code2,'' ill_code3,card_no,insurance ";
            strSQL += "select to_char(in_date,'%Y/%m/%d') ,dep_no ,'' typeName,doc_code Dor,'' DorName,to_char(out_date,'%Y/%m/%d') out_date,'住院' type, bed_no ,rel_house_no keybill,reg_no,'' ill_code,'' ill_code1,'' ill_code2,'' ill_code3,card_no,insurance ";
            strSQL += " from ipdhis@on2tcp:bed_file ";
            strSQL += " where reg_no = '";
            strSQL += asp_lbRegNo.Text + "'  ";
            strSQL += " and in_date >= '";
            strSQL += startDate + "' ";
            strSQL += " and in_date <= '";
            strSQL += endDate + "' ";
            strSQL += " union all ";
            //strSQL += " select in_date 日期,dep_no 科別,'' 科別名稱,doc_code 醫師,'' 醫生姓名,to_char(out_date,'%Y/%m/%d') 出院日,'住院' 診別, bed_no 床號,rel_house_no keybill,reg_no,'' ill_code,'' ill_code1,'' ill_code2,'' ill_code3,card_no,insurance ";
            strSQL += "select to_char(in_date,'%Y/%m/%d') ,dep_no ,'' typeName,doc_code Dor,'' DorName,to_char(out_date,'%Y/%m/%d') out_date,'住院' type, bed_no ,rel_house_no keybill,reg_no,'' ill_code,'' ill_code1,'' ill_code2,'' ill_code3,card_no,insurance ";
            strSQL += " from ipdhis@on2tcp:hist_bed ";
            strSQL += " where reg_no = '";
            strSQL += asp_lbRegNo.Text + "'";
            strSQL += " and out_date is not null  ";
            strSQL += " and in_date >= '";
            strSQL += startDate + "' ";
            strSQL += " and in_date <= '";
            strSQL += endDate + "' ";
            strSQL += " order by  opd_date desc ";

            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dt_VisitList = conn.GetData(strSQL);
            }

            PH_visits_tbody.Controls.Clear();
            ddl_visits.Items.Clear();

            if (dt_VisitList.Rows.Count > 30)
            {
                foreach (DataRow dr in dt_VisitList.Rows)
                {
                    PH_visits_tbody.Controls.Add(new LiteralControl("<tr><td>"));
                    PH_visits_tbody.Controls.Add(creat_Linkbutton(dr));
                    PH_visits_tbody.Controls.Add(new LiteralControl("</td></tr>"));
                }
            }
            else
            {
                foreach (DataRow dr in dt_VisitList.Rows)
                {
                    ddl_visits.Items.Add(creat_ddlOption(dr));
                }
                ddl_visits.Items.Insert(0, new ListItem("請選擇就診日期", ""));
            }
        }

        private ListItem creat_ddlOption(DataRow dr)
        {
            ListItem newListItem = new ListItem();
            newListItem.Text = dr["opd_date"].ToString() + dr["type"].ToString();
            newListItem.Value = string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}", dr["opd_date"].ToString().Trim(), dr["type"].ToString().Trim(), dr["dep_no"].ToString().Trim(), dr["dor"].ToString().Trim(), dr["bed_no"].ToString().Trim(), dr["out_date"].ToString().Trim(), dr["keybill"].ToString().Trim());

            return newListItem;
        }

        #region 動態製造 table row Linkbutton
        private LinkButton creat_Linkbutton(DataRow dr)
        {
            LinkButton newLinkButton = new LinkButton();

            string Type_Css;

            if (dr["type"].ToString().Contains("門"))
            {
                Type_Css = "<h4>" + dr["opd_date"].ToString() + "　" + "<span class=\"label label-info\">" + dr["type"].ToString() + "</span></h4>";
            }
            else if (dr["type"].ToString().Contains("急"))
            {
                Type_Css = "<h4>" + dr["opd_date"].ToString() + "　" + "<span class=\"label label-danger\">" + dr["type"].ToString() + "</span></h4>";
            }
            else if (dr["type"].ToString().Contains("住"))
            {
                Type_Css = "<h4>" + dr["opd_date"].ToString() + "　" + "<span class=\"label label-warning\">" + dr["type"].ToString() + "</span></h4>";
            }
            else
            {
                Type_Css = "<h4>" + dr["opd_date"].ToString() + "　" + "<span class=\"label label-default\">" + dr["type"].ToString() + "</span></h4>";
            }
            newLinkButton.CssClass = "row-btn";
            newLinkButton.Text = Type_Css;
            newLinkButton.ID = string.Format("{0}&{1}&{2}&{3}&{4}&{5}&{6}", dr["opd_date"].ToString().Trim(), dr["type"].ToString().Trim(), dr["dep_no"].ToString().Trim(), dr["dor"].ToString().Trim(), dr["bed_no"].ToString().Trim(), dr["out_date"].ToString().Trim(), dr["keybill"].ToString().Trim());
            newLinkButton.Click += new EventHandler(Lbtn_click);
            return newLinkButton;
        }

        protected void Lbtn_click(object sender, EventArgs e)
        {
            LinkButton thisbutton = (LinkButton)sender;
            string id = thisbutton.ID;
            string[] sArray = id.Split('&');
            asp_lbOpdDate.Text = sArray[0];
            asp_lbVisitType.Text = sArray[1];
            asp_ddlDepNo.Text = sArray[2];
            asp_lbDepNo.Text = asp_ddlDepNo.SelectedItem.Text;
            asp_ddlDocName.Text = sArray[3];
            asp_lbDocName.Text = asp_ddlDocName.SelectedItem.Text;
            asp_txtBedNo.Text = sArray[4];
            asp_txtOutDate.Text = sArray[5];
            asp_txtKeyBill.Text = sArray[6];
            str_selectDate = id;
            strPageStatus = "Three";
            bl_changedate = true;
        }
        #endregion

        #endregion
       
        #region 回復初始狀態
        private void initial_page()
        {
            dt_VisitList = null;
            asp_lbRegNo.Text = "";
            asp_lbID.Text = "";
            asp_lbPatName.Text = "";
            asp_lbBirthDate.Text = "";
            asp_lbSex.Text = "";

            asp_lbOpdDate.Text = "";
            asp_lbVisitType.Text = "";
            asp_ddlDepNo.Text = "";
            asp_lbDepNo.Text = "";
            asp_ddlDocName.Text = "";
            asp_lbDocName.Text = "";
            asp_txtBedNo.Text = "";
            asp_txtOutDate.Text = "";
            strPageStatus = "One";
        }
        #endregion

        protected void asp_btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("main.aspx", false);
        }

        protected void ddl_visits_SelectedIndexChanged(object sender, EventArgs e)
        {
            string desc = ddl_visits.SelectedValue;
            if (desc == "")
            {
                asp_lbOpdDate.Text = "";
                asp_lbVisitType.Text = "";
                asp_ddlDepNo.Text = "";
                asp_lbDepNo.Text = "";
                asp_ddlDocName.Text = "";
                asp_lbDocName.Text = "";
                asp_txtBedNo.Text = "";
                asp_txtOutDate.Text = "";
                asp_txtKeyBill.Text = "";
                strPageStatus = "Two";
                bl_changedate = true;
                return;
            }
            string[] sArray = desc.Split('&');
            asp_lbOpdDate.Text = sArray[0];
            asp_lbVisitType.Text = sArray[1];
            asp_ddlDepNo.Text = sArray[2];
            asp_lbDepNo.Text = asp_ddlDepNo.SelectedItem.Text;
            asp_ddlDocName.Text = sArray[3];
            asp_lbDocName.Text = asp_ddlDocName.SelectedItem.Text;
            asp_txtBedNo.Text = sArray[4];
            asp_txtOutDate.Text = sArray[5];
            asp_txtKeyBill.Text = sArray[6];
            strPageStatus = "Three";
            bl_changedate = true;
        }
    }
}