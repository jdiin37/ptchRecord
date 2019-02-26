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
using Newtonsoft.Json;

namespace PTCHRecordsys2
{
    public partial class main : System.Web.UI.Page
    {
        private string activeTab
        {
            get { return (string)ViewState["activeTab"]; }
            set { ViewState["activeTab"] = value; }
        }

        private static DataTable dt_pricecode;
        private static DataTable dt_socovt;

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "RenderPage", " RenderPage(" + this.Master.strdiv_id + "," + this.Master.strdiv_visits + "," + this.Master.strdiv_patinfo + ");", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "LeftRender", " LeftRender(" + this.Master.strleftisshow + ");", true);
          //ScriptManager.RegisterStartupScript(this, GetType(), "showBlockUI", " showBlockUI();", true);
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (dt_pricecode == null)
            {
                string SQL = "select trim(item_code) item_code,s_desc,unit from price_code where rec_status <> 'R' ";
                using (DevADODBConn conn = new DevADODBConn("pch01"))
                {
                    dt_pricecode = conn.GetData(SQL);
                }
            }

            if (dt_socovt == null)
            {
                string SQL = "select trim(item_code) item_code,issue_unit from socovt where rec_status<>'R' ";
                using (DevADODBConn conn = new DevADODBConn("pch01"))
                {
                    dt_socovt = conn.GetData(SQL);
                }
            }

            if (this.Master.strPageStatus == "Two" || this.Master.bl_changedate)
            {
                asp_panelPatOther.Enabled = false;
                asp_panelConsultation.Enabled = false; //Enabled means is loaded!!
                asp_panelIpdDiag.Enabled = false;
                asp_panelIpdInNote.Enabled = false;
                asp_panelIpdOrder.Enabled = false;
                asp_panelIpdOutNote.Enabled = false;
                asp_panelProgressNote.Enabled = false;
                asp_panelStatement.Enabled = false;
                asp_panelVisitRecord.Enabled = false;
            }

            switch (this.Master.visittype)
            {
                case null:
                case "":
                    tab_PatOther.Visible = false;
                    tab_VisitRecord.Visible = false;
                    tab_IpdOrder.Visible = false;
                    tab_IpdInNote.Visible = false;
                    tab_IpdOutNote.Visible = false;
                    tab_IpdDiag.Visible = false;
                    tab_Consultation.Visible = false;
                    tab_ProgressNote.Visible = false;
                    tab_Statement.Visible = false;
                    tab_LabReport.Visible = false;
                    if (this.Master.strPageStatus == "Two")
                    {
                        switch_btn("PatOther");
                        tab_PatOther.Visible = true;
                    }
                    break;
                case "住院":
                    tab_VisitRecord.Visible = false;
                    tab_IpdOrder.Visible = true;
                    tab_IpdInNote.Visible = true;
                    tab_IpdOutNote.Visible = true;
                    tab_IpdDiag.Visible = true;
                    tab_Consultation.Visible = true;
                    tab_ProgressNote.Visible = true;
                    tab_Statement.Visible = true;
                    tab_LabReport.Visible = true;
                    if (this.Master.bl_changedate) switch_btn("IpdOrder");
                    break;
                case "急診":
                case "門診":
                    tab_VisitRecord.Visible = true;
                    tab_IpdOrder.Visible = false;
                    tab_IpdInNote.Visible = false;
                    tab_IpdOutNote.Visible = false;
                    tab_IpdDiag.Visible = false;
                    tab_Consultation.Visible = false;
                    tab_ProgressNote.Visible = false;
                    tab_Statement.Visible = false;
                    tab_LabReport.Visible = true;
                    if (this.Master.bl_changedate) switch_btn("VisitRecord");
                    break;
            }
            if (this.Master.bl_changedate)
            {
                this.Master.bl_changedate = false;
            }  

            string btnID = Request["__EVENTARGUMENT"]; // btnID
            string btnClick = Request["__EVENTTARGET"]; // btnClick
            if (btnID != "" && btnID != null)
            {
                switch_btn(btnID);
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "ActiveTab", " ActiveTab('" + activeTab + "');", true);
            //ScriptManager.RegisterStartupScript(this, GetType(), "removeBlockUI", " removeBlockUI();", true); 
        }

        #region postback function
        private void switch_btn(string btnId)
        {
            switch (btnId)
            {
                case "Statement": //敘述醫囑
                    activeTab = "asp_panelStatement";
                    if (asp_panelStatement.Enabled) return;
                    try
                    {
                        getStatementOrder();
                        asp_panelStatement.Enabled = true;                       
                    }
                    catch (Exception ex)
                    {
                    }  
                    break;
                case "Consultation": //會診
                    activeTab = "asp_panelConsultation";
                    if (asp_panelConsultation.Enabled) return;
                    try
                    {
                        getOrdconst();
                        asp_panelConsultation.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                    }  
                    break;
                case "IpdOrder":  //住院醫囑
                    activeTab = "asp_panelIpdOrder";
                    if (asp_panelIpdOrder.Enabled) return;
                    try
                    {
                        getIpdOrder();
                        asp_panelIpdOrder.Enabled = true;                                              
                    }
                    catch (Exception ex)
                    {
                    }  
                    break;
                case "IpdInNote": //入院摘要
                    activeTab = "asp_panelIpdInNote";
                    if (asp_panelIpdInNote.Enabled || asp_panelIpdOutNote.Enabled) return;
                    try
                    {
                        getOrdso();
                        asp_panelIpdInNote.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                    }
                    break;
                case "IpdOutNote": //出院摘要
                    activeTab = "asp_panelIpdOutNote";
                    if (asp_panelIpdInNote.Enabled || asp_panelIpdOutNote.Enabled) return;                  
                    try
                    {
                        getOrdso();
                        asp_panelIpdOutNote.Enabled = true;                        
                    }
                    catch (Exception ex)
                    {
                    }  
                    break;
                case "IpdDiag": //診斷與處置
                    activeTab = "asp_panelIpdDiag";
                    if (asp_panelIpdDiag.Enabled) return;
                    try
                    {
                        getOrdDisease();
                        asp_panelIpdDiag.Enabled = true;                        
                    }
                    catch (Exception ex)
                    {
                    }  
                    break;
                case "LabReport": //檢驗檢查報告資料
                    activeTab = "asp_panelLabReport";
                    if (asp_panelLabReport.Enabled) return;
                    try
                    {
                        asp_panelLabReport.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                    }  
                    break;
                case "PatOther": //病人相關資料
                    activeTab = "asp_panelPatOther"; 
                    if (asp_panelPatOther.Enabled) return;
                    try
                    {
                        PatOther();
                        asp_panelPatOther.Enabled = true;                                              
                    }
                    catch (Exception ex)
                    {
                    }  
                    break;
                case "ProgressNote": //ProgressNote
                    activeTab = "asp_panelProgressNote";
                    if (asp_panelProgressNote.Enabled) return;
                    try
                    {
                        getProgressNotes();
                        asp_panelProgressNote.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                    }  
                    break;
                case "VisitRecord": //門診紀錄
                    activeTab = "asp_panelVisitRecord";
                    if (asp_panelVisitRecord.Enabled) return;
                    try
                    {
                        getLillcode();
                        getMemo();
                        getOpdOrder();
                        asp_panelVisitRecord.Enabled = true;                       
                    }
                    catch (Exception ex)
                    {
                    }  
                    break;
                default://以上都不成立執行預設值
                    break;
            }
        }
        #endregion

        #region 頁面使用函數

        private string findPrice_code(string item_code, string column)
        {
            DataRow dr = dt_pricecode.AsEnumerable().Where(p => p.Field<String>("item_code") == item_code).FirstOrDefault();
            if (dr == null)
            {
                return "";
            }
            return dr[column].ToString();
        }

        #region = Locate(尋找table某個欄位是否有特定值) =
        protected Boolean Locate(DataTable dt, string colum, string str)
        {
            DataRow dr = dt.AsEnumerable().Where(p => p.Field<String>(colum) == str).FirstOrDefault();
            if (dr == null)
            {
                return false;
            }
            return true;
        }
        #endregion
        #region = Locate(回傳table某個欄位特定值) =
        protected string Locate(DataTable dt, string comparColum, string comparStr, string returnColum)
        {
            DataRow dr = dt.AsEnumerable().Where(p => p.Field<String>(comparColum) == comparStr).FirstOrDefault();
            if (dr == null)
            {
                return "";
            }
            return dr[returnColum].ToString();
        }
        #endregion
        private string fordso(string desctype)
        {
            if (desctype == "01") return "Chief Complaint";
            if (desctype == "02") return "Present Illness";
            if (desctype == "03") return "Birth History";
            if (desctype == "04") return "Past History";
            if (desctype == "05") return "Operation History";
            if (desctype == "06") return "Family History";
            if (desctype == "07") return "住院治療經過";
            if (desctype == "08") return "檢驗資料";
            if (desctype == "09") return "合併症(comorbidity)、併發症(complication)";
            if (desctype == "10") return "出院指示";
            if (desctype == "11") return "Impression";
            if (desctype == "12") return "檢查資料";
            if (desctype == "13") return "其他";
            if (desctype == "14") return "病理報告(包括病理發現)";
            if (desctype == "15") return "出院時情況";
            if (desctype == "16") return "放射線報告";
            if (desctype == "17") return "主訴";
            if (desctype == "18") return "病史";
            if (desctype == "19") return "主要手術或處置及次要手術或處置之日期、名稱及發現";
            if (desctype == "20") return "體檢發現";
            if (desctype == "21") return "Physical Examination-Ear Nose Throat Head and Neck";
            if (desctype == "22") return "Physical Examination-Chest and Heart";
            if (desctype == "23") return "Physical Examination-Abdomen";
            if (desctype == "24") return "Physical Examination-Extremities";
            if (desctype == "25") return "Physical Examination-Skin";
            if (desctype == "26") return "Physical Examination-Neurological Examination";
            if (desctype == "27") return "others (PV,泌尿系統, NG,Foley, Endo, Tracheal tube)";
            if (desctype == "28") return "Physical Examination-Special Finding( History, Physical examination, Laboratory or image study)";
            if (desctype == "29") return "Physical Examination-General";
            if (desctype == "30") return "Plan of Treatment";
            if (desctype == "31") return "入院診斷";
            if (desctype == "32") return "出院診斷";
            if (desctype == "33") return "出院帶藥";
            if (desctype == "38") return "Allergy History";
            if (desctype == "39") return "血型";
            if (desctype == "40") return "病史來源";
            if (desctype == "41") return "職業";
            if (desctype == "42") return "Vital sign";
            if (desctype == "43") return "Review Of Systems";
            if (desctype == "44") return "緊急聯絡人電話";
            if (desctype == "45") return "婚姻";
            if (desctype == "46") return "族群";
            if (desctype == "48") return "Physical Examination-Digital Examination";
            if (desctype == "49") return "Physical Examination-Back";
            if (desctype == "50") return "Admission Date";
            if (desctype == "51") return "Transfer from";
            if (desctype == "53") return "入院狀態";
            if (desctype == "54") return "出生體重 (g)";
            if (desctype == "55") return "VS Comment1";
            if (desctype == "56") return "VS Comment2";
            if (desctype == "57") return "VS Comment3";
            return "";
        }

        private string getOrder_type(string order_type)
        {
            string type_desc = "";
            switch (order_type)
            {
                case "O":
                    type_desc = "出院帶藥";
                    break;
                case "U":
                    type_desc = "住院藥品";
                    break;
                case "L":
                    type_desc = "檢驗";
                    break;
                case "X":
                    type_desc = "X光";
                    break;
                case "E":
                    type_desc = "檢查";
                    break;
            }
            return type_desc;
        }

        private string getIssue_unit(string item_code)
        {
            //String SQL = "select item_code,issue_unit from socovt where item_code= '";
            //SQL += item_code + "'";
            //SQL += " and rec_status<>'R' ";
            //DataTable dt = new DataTable();
            //using (DevADODBConn conn = new DevADODBConn("pch01"))
            //{
            //    dt = conn.GetData(SQL);
            //}
            //if (dt.Rows.Count == 0)
            //    return "";
            //else
            //    return dt.Rows[0]["issue_unit"].ToString();

            DataRow dr = dt_socovt.AsEnumerable().Where(p => p.Field<String>("item_code") == item_code).FirstOrDefault();
            if (dr == null)
            {
                return "";
            }
            return dr["issue_unit"].ToString();
        }

        #endregion

        #region VisitRecord 門診紀錄
        private void getOpdOrder()
        {
            // Transact-SQL 陳述式 
            //String strSQL = "select tran_date, order_no,insurance,item_code,take_qty,take_times,take_way,take_day,qty,er_status,include_flag ";
            //String strSQL = " select insurance 險,item_code 醫令碼,'' 醫令名稱,take_qty 劑量,'' 單位,take_times 頻次,take_way 途徑,take_day 日,qty 總量,er_status 急,include_flag 計 ";
            String strSQL = " select insurance ,item_code ,'' item_name,take_qty ,'' unit,take_times ,take_way ,take_day ,qty,'' charge_unit ,er_status ,include_flag  ";
            strSQL += " from v_all_opd_tran ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and opd_date = '";
            strSQL += this.Master.opd_date + "'";
            strSQL += " and reg_bill= '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status='A' and ins_flag in ('0','N') ";
            strSQL += " and b_status not in ('M','m','D','r') and pay_type not in ('DB','NN','ER') ";
            strSQL += " union ";
            //strSQL += " select insurance 險,item_code 醫令碼,'' 醫令名稱,take_qty 劑量,'' 單位,take_times 頻次,take_way 途徑,take_day 日,qty 總量,er_status 急,include_flag 計 ";
            strSQL += " select insurance ,item_code ,'' item_name,take_qty ,'' unit,take_times ,take_way ,take_day ,qty,'' charge_unit ,er_status ,include_flag  ";
            strSQL += " from opd_tran ";
            strSQL += " where  reg_no='";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and opd_date = '";
            strSQL += this.Master.opd_date + "'";
            strSQL += " and reg_bill= '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status='";
            strSQL += "A" + "'";
            strSQL += " and ins_flag in ('0','N') and b_status not in ('M','m','D','r') and pay_type not in ('DB','NN','ER') order by 1,2 ";
            DataTable dt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dt = conn.GetData(strSQL);
            }

            asp_LitOpdOrder.Text = "";
            foreach (DataRow dr in dt.Rows)
            {
                asp_LitOpdOrder.Text += getOpdOrderRow(dr); 
            }
        }

        private string getOpdOrderRow(DataRow dr)
        {
            string htmltr = "<tr>";

            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["insurance"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["item_code"].ToString().Trim());
            //htmltr += string.Format("<td title=\"{0}\">{0}</td>", getPrice_code(dr["item_code"].ToString().Trim(), "s_desc"));
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", findPrice_code(dr["item_code"].ToString().Trim(), "s_desc"));
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["take_qty"].ToString().Trim());

            //string unit = getPrice_code(dr["item_code"].ToString().Trim(), "unit");
            string unit = findPrice_code(dr["item_code"].ToString().Trim(), "unit");
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", unit);
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["take_times"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["take_way"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["take_day"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["qty"].ToString().Trim());
            if (unit == "PU" || unit == "gt")
            {
                unit = "瓶";
            }
            string Issue_unit = getIssue_unit(dr["item_code"].ToString().Trim());
            if (Issue_unit != "")
            {
                unit = Issue_unit;
            }
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", unit);
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["er_status"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["include_flag"].ToString().Trim());

            htmltr += "</tr>";
            return htmltr;
        }

        private void getLillcode()
        {
            // Transact-SQL 陳述式 
            //String strSQL = "select opd_date 日期,dep_no 科別,doc_code 醫師,'' 出院日,DECODE(room_no,'ER','急診','門診') 診別,'' 床號,reg_bill keybill,reg_no,ill_code,ill_code1,ill_code2,ill_code3,card_no,insurance,0 rel_house_no ";
            String strSQL = "select opd_date ,dep_no ,doc_code ,'' ,DECODE(room_no,'ER','急診','門診') ,'' ,reg_bill keybill,reg_no,ill_code,ill_code1,ill_code2,ill_code3,card_no,insurance,0 rel_house_no ";
            strSQL += " from v_all_opd_reg ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and opd_date = '";
            strSQL += this.Master.opd_date + "'";
            strSQL += " and reg_bill= '";
            strSQL += this.Master.keybill + "'";
            //strSQL += " and rec_status='A' and ins_remark='1' ";
            strSQL += " and rec_status='A'";
            DataTable dt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dt = conn.GetData(strSQL);
            }

            asp_lbVisitRecord_diagM.Text = "";
            asp_lbVisitRecord_diag1.Text = "";
            asp_lbVisitRecord_diag2.Text = "";
            asp_lbVisitRecord_diag3.Text = "";

            if (dt.Rows.Count > 0)
            {
                asp_lbVisitRecord_diagM.Text = "<span class=\"text-warning\">" + dt.Rows[0]["ill_code"].ToString() + "</span>" + geticd_9(dt.Rows[0]["ill_code"].ToString());
                asp_lbVisitRecord_diag1.Text = "<span class=\"text-warning\">" + dt.Rows[0]["ill_code1"].ToString() + "</span>" + geticd_9(dt.Rows[0]["ill_code1"].ToString());
                asp_lbVisitRecord_diag2.Text = "<span class=\"text-warning\">" + dt.Rows[0]["ill_code2"].ToString() + "</span>" + geticd_9(dt.Rows[0]["ill_code2"].ToString());
                asp_lbVisitRecord_diag3.Text = "<span class=\"text-warning\">" + dt.Rows[0]["ill_code3"].ToString() + "</span>" + geticd_9(dt.Rows[0]["ill_code3"].ToString());
            }
        }

        private string geticd_9(string icd9_code)
        {
            if (icd9_code.Trim() == "") return "";
            // Transact-SQL 陳述式 
            DataTable tmpDt = new DataTable();
            String strSQL = " select e_name,c_name from icd9 where icd9_code= '";
            strSQL += icd9_code + "'";
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            if (tmpDt.Rows.Count > 0)
            {
                return tmpDt.Rows[0]["e_name"].ToString().Trim() + "  " + tmpDt.Rows[0]["c_name"].ToString().Trim();
            }
            else
            {
                strSQL = " select e_name,c_name from icd10 where icd10_code= '";
                strSQL += icd9_code + "'";
                using (DevADODBConn conn = new DevADODBConn("pch01"))
                {
                    tmpDt = conn.GetData(strSQL);
                }
                if (tmpDt.Rows.Count > 0)
                {
                    return tmpDt.Rows[0]["e_name"].ToString().Trim() + "  " + tmpDt.Rows[0]["c_name"].ToString().Trim();
                }
                else
                {
                    return "";
                }
            }
        }

        private void getMemo()
        {
            // Transact-SQL 陳述式 
            String strSQL = "select * from pat_so ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and tre_date = '";
            strSQL += this.Master.opd_date + "'";
            strSQL += " and bill_no= '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status<> 'R' ";
            strSQL += " union ";
            strSQL += " select * from past_pat_so  ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and tre_date = '";
            strSQL += this.Master.opd_date + "'";
            strSQL += " and bill_no= '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status<> 'R' ";
            strSQL += " union ";
            strSQL += " select * from hispast@on1tcp:past_pat_so ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and tre_date = '";
            strSQL += this.Master.opd_date + "'";
            strSQL += " and bill_no= '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status<> 'R' ";
            strSQL += " order by so_flag desc,seq_no ";

            DataTable dt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                 dt = conn.GetData(strSQL);
            }

            asp_lbVisitRecord_memo.Text = "";
            foreach (DataRow row in dt.Rows)
            {
                asp_lbVisitRecord_memo.Text += row["description"].ToString().TrimEnd(null) + "<br />";
            }
        }
        #endregion

        #region PatOther 病人相關資料
        private void PatOther()
        {
            string tmpStr = "";
            //取得txtDEsc資料(病患歷史紀錄)
            tmpStr = getMediAdr() + getMediAdrA() + getMediAdrB() + getIcData();
            if (tmpStr.Trim() == "")
            {
                tmpStr = "<span class=\"text-primary\">無通報或感染資料</span><br>";
            }
            asp_LitPatOther.Text = tmpStr;
        }
        #region =取得過敏通報 =
        private string getMediAdr() 
        {
            // 1.取得過敏通報 
            //String strSQL = "select op_date as 通報日期,op_time as 時間,bed_no as 床號,DECODE(oi_flag,''I'',''住診'',''O'',''門診'',''E'',''急診'','''') 診別' + ',remark1 內容,remark2,remark3,remark4,remark5 from medi_adr ";
            //SQL.Append("select op_date as 通報日期,op_time as 時間,bed_no as 床號,oi_flag as 診別,remark1 as 內容,remark2,remark3,remark4,remark5 ");
            string SQL = "select op_date,op_time,bed_no,oi_flag,remark1,remark2,remark3,remark4,remark5 ";
            SQL += " from medi_adr ";
            SQL += " where reg_no = '";
            SQL += this.Master.reg_no + "'";
            SQL += " and rec_status <> '";
            SQL += "R" + "'";
            DataTable tmpDt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(SQL);
            }
            string str = "";
            if (tmpDt.Rows.Count != 0)
            {
                str += "<font size='4'><strong>過敏通報：</strong></font><br>";
                foreach (DataRow rows in tmpDt.Rows)
                {
                    DateTime 時間 = Convert.ToDateTime(rows["op_time"].ToString());
                    str += "&nbsp;&nbsp;&nbsp; <span class=\"text-primary\">   通報日期：" + rows["op_date"].ToString() + " 時間:" + 時間.ToString("yyyy/MM/dd") + " 床號：" + rows["bed_no"].ToString() + " 診別：" + rows["oi_flag"].ToString() + " 內容：" + rows["remark1"].ToString() + rows["remark2"].ToString() + rows["remark3"].ToString() + rows["remark4"].ToString() + rows["remark5"].ToString() + "</span><br>";
                }
            }
            return str;
        }
        #endregion
        #region =取得過敏紀錄 =
        private string getMediAdrA()
        {
            // 2.取得過敏記錄 
            //String strSQL = "select a.tran_date 資料日期,a.doc_code||''(''||trim(b.description)||'')'' 醫師,contents 內容 ";
            String SQL = "select a.tran_date ,a.doc_code  Dor,trim(b.description)  DorName ,contents  ,medi_name ";
            SQL += " from pat_notes a, code_file b ";
            SQL += " where a.reg_no = '";
            SQL += this.Master.reg_no + "'";
            SQL += " and a.rec_status = '";
            SQL += "A" + "'";
            SQL += " and a.doc_code = b.item_code";
            SQL += " and b.item_type = '";
            SQL += "09" + "'";
            DataTable tmpDt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(SQL);
            }
            string str = "";
            if (tmpDt.Rows.Count != 0)
            {
                str += "";
                if (tmpDt.Rows.Count != 0)
                {
                    str += "<font size='4' ><strong>過敏記錄：</strong></font><br>";
                    foreach (DataRow rows in tmpDt.Rows)
                    {
                        DateTime 資料日期 = Convert.ToDateTime(rows["tran_date"].ToString());
                        //str += "&nbsp;&nbsp;&nbsp;<font size='3' color='blue'><strong>   資料日期：" + 資料日期.ToString("yyyy/MM/dd") + " 醫師：" + rows["Dor"].ToString() + "(" + rows["DorName"].ToString() + ")  內容：" + rows["contents"].ToString() + " 藥品：" + rows["medi_name"].ToString().TrimEnd(null) + "</strong></font><br>";
                        str += "&nbsp;&nbsp;&nbsp;<span class=\"text-primary\">   資料日期：" + 資料日期.ToString("yyyy/MM/dd") + " 醫師：" + rows["Dor"].ToString() + "(" + rows["DorName"].ToString() + ")  " + " 藥品：" + rows["medi_name"].ToString().TrimEnd(null) + "</span><br>";
                    }
                }
            }
            return str;
        }
        #endregion
        #region =取得過去病史 =
        private string getMediAdrB()
        {
            //String strSQL = "select a.tran_date 資料日期,a.doc_code||''(''||trim(b.description)||'')'' 醫師,contents 內容 ";
            String SQL = "select a.tran_date,a.doc_code,trim(b.description) dorName ,contents  ";
            SQL += " from pat_notes a, code_file b ";
            SQL += " where a.reg_no = '";
            SQL += this.Master.reg_no + "'";
            SQL += " and a.rec_status = '";
            SQL += "B" + "'";
            SQL += " and a.doc_code = b.item_code ";
            SQL += " and b.item_type = '";
            SQL += "09" + "'";
            DataTable tmpDt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(SQL);
            }
            string str = "";
            if (tmpDt.Rows.Count != 0)
            {
                str += "<font size='4'><strong>過去病史：</strong></font><br>";
                foreach (DataRow rows in tmpDt.Rows)
                {
                    DateTime 資料日期 = Convert.ToDateTime(rows["tran_date"].ToString());
                    str += "&nbsp;&nbsp;&nbsp;<span class=\"text-primary\">    過去病史" + 資料日期.ToString("yyyy/MM/dd") + " 醫師:" + rows["doc_code"].ToString() + "(" + rows["dorName"].ToString() + ") 內容:" + rows["contents"].ToString() + "</span><br>";
                }
            }
            return str;
        }
        #endregion
        #region =取得感控通報資料 =
        private string getIcData()
        {
            //String strSQL = "select a.order_date 通報日期,a.doc_code||''(''||trim(b.description)||'')'' 醫師,a.icd9||'' ''||c.c_name 診斷碼 , ps2, back_code from ic_data a, code_file b, icd9_1 c";
            String SQL = " select a.order_date,a.doc_code,trim(b.description) dorName, a.icd9  ,NVL(c.c_name, d.c_name) as c_name , ps2, back_code  ";
            SQL += " from ic_data a ";
            SQL += " left join code_file b on a.doc_code = b.item_code and b.item_type ='09' ";
            SQL += "    left join icd9 c on a.icd9=c.icd9_code ";
            SQL += "    left join icd10 d on a.icd9=d.icd10_code ";
            SQL += " where a.reg_no = '";
            SQL += this.Master.reg_no + "'";
            DataTable tmpDt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(SQL);
            }
            string str = "";
            if (tmpDt.Rows.Count != 0)
            {
                str += "<font size='4' ><strong>感控通報資料：</strong></font> <br>";
                foreach (DataRow rows in tmpDt.Rows)
                {
                    DateTime 通報日期 = Convert.ToDateTime(rows["order_date"].ToString());
                    str += "&nbsp;&nbsp;&nbsp;<span class=\"text-primary\">   通報日期:" + 通報日期.ToString("yyyy/MM/dd") + " 醫師:" + rows["doc_code"].ToString() + "(" + rows["dorName"].ToString() + ") 診斷碼:" + rows["icd9"].ToString() + " 診斷碼名稱:" + rows["c_name"].ToString().TrimEnd(null) + getPs2(rows["ps2"].ToString(), rows["back_code"].ToString()) + "</span><br>";
                }

            }
            return str;
        }
        #endregion
        #region =取得通報結果 =
        public string getPs2(string ps2, string back_code)
        {
            string str = "";
            if (ps2.Trim() != "")
            {
                if (ps2.ToString().Substring(7, 1) == "1")
                    str = "【一採結果回覆：陰性 ";
                else if (ps2.ToString().Substring(7, 1) == "2")
                    str = "【一採結果回覆：陽性 ";
                else if (ps2.ToString().Substring(7, 1) == "3")
                    str = "【一採結果回覆：未確定 ";
                else
                    str = "【一採結果回覆:";
            }
            if (back_code.Trim() != "")
            {
                if (back_code == "1")
                    str += "二採結果回覆：陰性】";
                else if (back_code == "2")
                    str += "二採結果回覆：陽性】";
                else if (back_code == "3")
                    str += "二採結果回覆：未確定】";
                else
                    str += "二採結果回覆：】";
            }
            return str;
        }
        #endregion
        #endregion

        #region IpdOrder 住院醫囑
        private void getIpdOrder()
        {
            asp_LitIpdOrder.Text = "";
            DataTable tmpDt = new DataTable();
            tmpDt = getOrddtl();
            if (tmpDt.Rows.Count == 0)
            {
                tmpDt = getIpdTran();
            }
           
            foreach (DataRow dr in tmpDt.Rows)
            {
                asp_LitIpdOrder.Text += getIpdOrderRow(dr);
            }
        }

        private string getIpdOrderRow(DataRow dr)
        {
            string htmltr = "<tr>";

            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["insurance"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["item_code"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", findPrice_code(dr["item_code"].ToString().Trim(), "s_desc"));
            //htmltr += string.Format("<td title=\"{0}\">{0}</td>", getPrice_code(dr["item_code"].ToString().Trim(), "s_desc"));
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["take_qty"].ToString().Trim());

            htmltr += string.Format("<td title=\"{0}\">{0}</td>", findPrice_code(dr["item_code"].ToString().Trim(), "unit"));
            //htmltr += string.Format("<td title=\"{0}\">{0}</td>", getPrice_code(dr["item_code"].ToString().Trim(), "unit"));
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["take_times"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["take_way"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["qty"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["take_day"].ToString().Trim());

            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["beg_date"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["end_date"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["powder_flag"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", getOrder_type(dr["order_type"].ToString().Trim()));
            
            htmltr += "</tr>";
            return htmltr;
        }

        private DataTable getOrddtl()
        {
            // Transact-SQL 陳述式 
            //String strSQL = "select insurance ,item_code ,take_qty ,take_times ,take_way ,qty ,take_day ,ipd_date beg_date ,ipd_date end_date,'' powder_flag,'' order_type ";
            //String strSQL = "select insurance 險別,item_code 醫令碼,'' 醫令名稱,take_qty 劑量,'' 單位,take_times 頻次,take_way 服法,qty 總量,take_day 日數,beg_date 起服日 ,end_date 停服日,powder_flag 磨粉,order_type 類別說明 ";
            String strSQL = "select insurance ,item_code ,'' itemName,take_qty ,'' unit,take_times,take_way,qty ,take_day ,to_char(beg_date) beg_date ,to_char(end_date) end_date ,powder_flag ,order_type"; //case when order_type = 'U' then 0 else 1 end sortItem ";
            strSQL += " from orddtl ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and rel_house_no = '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status='A' ";
            //2014.11.10 素華要求將塑膠空針(item_code='703003',703010,703005..等)的資料不要顯示 ︾
            //塑膠空針的所有item_code可以select item_code from price_code where charge = '12' and s_desc like '%塑膠空針%'來找出
            strSQL += " and (item_code != '702020' and item_code != '702021' and item_code != '702022'  ";
            strSQL += " and item_code != '702030' and item_code != '702050' and item_code != '703001'  ";
            strSQL += " and item_code != '703003' and item_code != '703005' and item_code != '703010' ) ";
            //2014.11.10 素華要求將塑膠空針(item_code='703003',703010,703005..等)的資料不要顯示 ︽
            strSQL += " union all  ";
            //strSQL += " select insurance 險別,item_code 醫令碼,'' 醫令名稱,take_qty 劑量,'' 單位,take_times 頻次,take_way 服法,qty 總量,take_day 日數,beg_date 起服日 ,end_date 停服日,powder_flag 磨粉,order_type 類別說明 ";
            strSQL += " select insurance ,item_code ,'' itemName,take_qty ,'' unit,take_times,take_way,qty ,take_day ,to_char(beg_date)  ,to_char(end_date) ,powder_flag ,order_type";//,case when order_type = 'U' then 0 else 1 end sortItem ";
            strSQL += " from hist_orddtl ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and rel_house_no = '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status='A'  ";
            //2014.11.8評鑑指示要將停服日為空白的資料排序於最上面,並將類別說明也做排序 ︾
            strSQL += " order by end_date";//,sortItem asc ";
            //2014.11.8評鑑指示要將停服日為空白的資料排序於最上面,並將類別說明也做排序 ︽
            DataTable tmpDt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch02"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            return tmpDt;
        }

        private DataTable getIpdTran()
        {
            // Transact-SQL 陳述式 
            //String strSQL = "select insurance ,item_code ,take_qty ,take_times ,take_way ,qty ,take_day ,ipd_date beg_date ,ipd_date end_date,'' powder_flag,'' order_type ";
            String strSQL = "select insurance ,item_code ,'' itemName,take_qty ,'' unit,take_times ,take_way ,qty ,take_day ,to_char(ipd_date) beg_date ,to_char(ipd_date) end_date,'' powder_flag,'' order_type ";
            strSQL += " from ipd_tran ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and house_no = '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status='A' ";
            strSQL += " and b_status<>'I' and b_status<>'D' and b_status<>'M' ";
            strSQL += " union all  ";
            strSQL += " select insurance ,item_code ,'' itemName,take_qty ,'' unit,take_times ,take_way ,qty ,take_day ,to_char(ipd_date) beg_date ,to_char(ipd_date) end_date,'' powder_flag,'' order_type ";
            strSQL += " from hist_ipd_tran ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and house_no = '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status='A'  ";
            strSQL += " and b_status<>'I' and b_status<>'D' and b_status<>'M'  ";
            //2014.11.10 素華要求將塑膠空針(item_code='703003',703010,703005..等)的資料不要顯示 ︾
            //塑膠空針的所有item_code可以select item_code from price_code where charge = '12' and s_desc like '%塑膠空針%'來找出
            strSQL += " and (item_code != '702020' and item_code != '702021' and item_code != '702022'  ";
            strSQL += " and item_code != '702030' and item_code != '702050' and item_code != '703001'  ";
            strSQL += " and item_code != '703003' and item_code != '703005' and item_code != '703010' ) ";
            //2014.11.10 素華要求將塑膠空針(item_code='703003',703010,703005..等)的資料不要顯示 ︽
            //2014.11.8評鑑指示要將停服日為空白的資料排序於最上面,並將類別說明也做排序 ︾
            strSQL += " order by end_date,order_type asc ";
            //2014.11.8評鑑指示要將停服日為空白的資料排序於最上面,並將類別說明也做排序 ︽

            DataTable tmpDt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch02"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            return tmpDt;
        }
        #endregion

        #region IpdInNote IpdOutNote 入/出院摘要
        public void getOrdso()
        {
            asp_LitIpdInNote.Text = "";
            asp_LitIpdOutNote.Text = "";
            //WebServiceReg ws = new WebServiceReg();
            string exMsg = "";
            DataTable dtOrdso = new DataTable();
            string strSQL = " select * ";
            strSQL += " from ming.ordso ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and house_no = '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status='A' ";

            using (DevADODBConn conn = new DevADODBConn("pch02"))
            {
                dtOrdso = conn.GetDataWithError(strSQL, ref exMsg);
            }

            if (dtOrdso == null)
            {
                asp_LitIpdInNote.Text += exMsg;
                return;
            }
            string[] desc_typein = new string[] { "41", "40", "44", "51", "50", "42", "39", "45", "46", "53", "54", "01", "02", "38", "03", "04", "05", "06", "11", "30", "43", "29", "21", "22", "23", "48", "49", "24", "25", "26", "27", "28", "55", "56", "57" };
            string[] desc_typeout = new string[] { "31", "32", "17", "18", "20", "19", "07", "09", "08", "12", "16", "14", "13", "15", "33", "10" };
            DateTime Vs_round;
            string vsRound = "";
            try
            {
                Vs_round = Convert.ToDateTime(Locate(dtOrdso, "desc_type", "59", "description"));
                vsRound = Vs_round.ToString("yyyyMMdd");
            }
            catch { }
            //取得入院摘要
            foreach (string str in desc_typein)
            {
                if (str == "01")
                {
                    if (Locate(dtOrdso, "desc_type", str))
                    {
                        //txtMemoIn.Text += "【" + fordso(str) + "】" + "\n";
                        //txtMemoIn.Text += vsRound + Locate(dtOrdso, "desc_type", str, "description") + "\n";
                        asp_LitIpdInNote.Text += "【" + fordso(str) + "】" + "<br>";
                        asp_LitIpdInNote.Text += "<span class=\"text-primary\">" + vsRound + Locate(dtOrdso, "desc_type", str, "description") + "</span><br>";
                    }
                    continue;
                }
                if (str == "46")
                {
                    string tmpStr = "";
                    if (Locate(dtOrdso, "desc_type", "46"))
                    {
                        tmpStr += Locate(dtOrdso, "desc_type", "46", "description");
                    }
                    if (Locate(dtOrdso, "desc_type", "47"))
                    {
                        tmpStr += ";" + Locate(dtOrdso, "desc_type", "46", "description");
                    }
                    if (Locate(dtOrdso, "desc_type", "46") && tmpStr != "")
                    {
                        //txtMemoIn.Text += "【" + fordso(str) + "】" + "\n";
                        //txtMemoIn.Text += tmpStr + "\n";
                        asp_LitIpdInNote.Text += "【" + fordso(str) + "】" + "<br>";
                        asp_LitIpdInNote.Text += "<span class=\"text-primary\">" + tmpStr + "</span><br>";
                    }
                    continue;
                }
                if (str == "51")
                {
                    string tmpStr = "";
                    if (Locate(dtOrdso, "desc_type", "51"))
                    {
                        tmpStr += Locate(dtOrdso, "desc_type", "51", "description");
                    }
                    if (Locate(dtOrdso, "desc_type", "52"))
                    {
                        tmpStr += ";" + Locate(dtOrdso, "desc_type", "52", "description");
                    }
                    if (Locate(dtOrdso, "desc_type", "51") && tmpStr != "")
                    {
                        //txtMemoIn.Text += "【" + fordso(str) + "】" + "\n";
                        //txtMemoIn.Text += tmpStr + "\n";
                        asp_LitIpdInNote.Text += "【" + fordso(str) + "】" + "<br>";
                        asp_LitIpdInNote.Text += "<span class=\"text-primary\">" + tmpStr + "</span><br>";
                    }

                    continue;
                }
                if (Locate(dtOrdso, "desc_type", str))
                {
                    //txtMemoIn.Text += "【" + fordso(str) + "】" + "\n";
                    //txtMemoIn.Text += Locate(dtOrdso, "desc_type", str, "description") + "\n";
                    asp_LitIpdInNote.Text += "【" + fordso(str) + "】" + "<br>";
                    asp_LitIpdInNote.Text += "<span class=\"text-primary\">" + Locate(dtOrdso, "desc_type", str, "description") + "</span><br>";
                }
            }

            //取得出院摘要
            foreach (string str in desc_typeout)
            {
                if (Locate(dtOrdso, "desc_type", str))
                {
                    //txtMemoOut.Text += "【" + fordso(str) + "】" + "\n";
                    //txtMemoOut.Text += Locate(dtOrdso, "desc_type", str, "description") + "\n";
                    asp_LitIpdOutNote.Text += "【" + fordso(str) + "】" + "<br>";
                    asp_LitIpdOutNote.Text += "<span class=\"text-primary\">" + Locate(dtOrdso, "desc_type", str, "description") + "</span><br>";
                }
            }
        }
        #endregion

        #region IpdDiag 診斷與處置
        private void getOrdDisease()
        {
            asp_LitIpdDiag.Text = "";
            DataTable tmpDt = new DataTable();
            string strSQL = "select in_disea ,in_disea1 ,in_disea2 ,in_disea3 ,in_disea4 ,in_disea5 ,in_disea6 ,in_disea7 ,in_op ,in_op1 ,";
            strSQL += "in_op2 ,in_op3 ,in_op4 ,in_op5 ,in_op6 ,in_op7  ";
            strSQL += " from ord_disease ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and rel_house_no = '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status='A' ";

            using (DevADODBConn conn = new DevADODBConn("pch02"))
            {
                tmpDt = conn.GetData(strSQL);
            }

            for (int i = 0; i < tmpDt.Columns.Count; i++)
            {
                if (tmpDt.Rows[0][tmpDt.Columns[i]].ToString() != "")
                    //txtMemoDisea.Text += ConvertDisease(tmpDt.Columns[i].ToString()) + ":" + tmpDt.Rows[0][tmpDt.Columns[i]].ToString() + ficdname(tmpDt.Rows[0][tmpDt.Columns[i]].ToString()) + "\n";
                    asp_LitIpdDiag.Text += ConvertDisease(tmpDt.Columns[i].ToString()) + ":<span class=\"text-warning\">" + tmpDt.Rows[0][tmpDt.Columns[i]].ToString() + "</span><span class=\"text-primary\">"+ ficdname(tmpDt.Rows[0][tmpDt.Columns[i]].ToString()) + "</span><br>";
            }
        }


        private string ficdname(string icd_code)
        {
            // Transact-SQL 陳述式 
            DataTable tmpDt = new DataTable();
            String strSQL = " select e_name from icd9 where icd9_code= '";
            strSQL += icd_code + "'";
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            if (tmpDt.Rows.Count > 0)
            {
                return tmpDt.Rows[0]["e_name"].ToString();
            }
            else
            {
                strSQL = " select e_name from icd10 where icd10_code= '";
                strSQL += icd_code + "'";
                using (DevADODBConn conn = new DevADODBConn("pch01"))
                {
                    tmpDt = conn.GetData(strSQL);
                }
                if (tmpDt.Rows.Count > 0)
                {
                    return tmpDt.Rows[0]["e_name"].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        #region = ConvertDisease(回傳欄位的中文名稱) =
        private string ConvertDisease(string Colum)
        {
            //String strSQL = "select in_disea 主診斷,in_disea1 次診斷1,in_disea2 次診斷2,in_disea3 次診斷3,in_disea4 次診斷4,in_disea5 次診斷5,in_disea6 次診斷6,in_disea7 次診斷7,in_op 主處置,in_op1 次處置1,";
            //strSQL += "in_op2 次處置2,in_op3 次處置3,in_op4 次處置4,in_op5 次處置5,in_op6 次處置6,in_op7 次處置7 ";
            DataTable dtTab = new DataTable();
            dtTab.Columns.Add("Colum", typeof(String));
            dtTab.Columns.Add("ColumName", typeof(String));
            dtTab.Rows.Add(new object[] { "in_disea", "主診斷" });
            dtTab.Rows.Add(new object[] { "in_disea1", "次診斷1" });
            dtTab.Rows.Add(new object[] { "in_disea2", "次診斷2" });
            dtTab.Rows.Add(new object[] { "in_disea3", "次診斷3" });
            dtTab.Rows.Add(new object[] { "in_disea4", "次診斷4" });
            dtTab.Rows.Add(new object[] { "in_disea5", "次診斷5" });
            dtTab.Rows.Add(new object[] { "in_disea6", "次診斷6" });
            dtTab.Rows.Add(new object[] { "in_disea7", "次診斷7" });
            dtTab.Rows.Add(new object[] { "in_op", "主處置" });
            dtTab.Rows.Add(new object[] { "in_op1", "主處置1" });
            dtTab.Rows.Add(new object[] { "in_op2", "主處置2" });
            dtTab.Rows.Add(new object[] { "in_op3", "主處置3" });
            dtTab.Rows.Add(new object[] { "in_op4", "主處置4" });
            dtTab.Rows.Add(new object[] { "in_op5", "主處置5" });
            dtTab.Rows.Add(new object[] { "in_op6", "主處置6" });
            dtTab.Rows.Add(new object[] { "in_op7", "主處置7" });
            DataView dv = dtTab.DefaultView;
            dv.Sort = "Colum asc";
            int i = dv.Find(Colum);
            if (i == -1)
            {
                return Colum;
            }
            else
            {
                return dv[i]["ColumName"].ToString();
            }

        }
        #endregion
        #endregion

        #region Consultation 會診
        private void getOrdconst()
        {
            asp_LitConsultation.Text = "";

            String strSQL = "select order_date,concode,b.description concodeName,pat_hist ,reply_doc,c.description reply_docName,ans_rep  ";
            strSQL += " from ordconst a,code_file b,code_file c ";
            strSQL += " where a.reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and a.house_no = '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and a.rec_status='A' and a.test_status<>'1' and b.item_type='09' and a.concode=b.item_code and c.item_type='09' and a.reply_doc=c.item_code ";
            DataTable tmpDt = new DataTable();

            using (DevADODBConn conn = new DevADODBConn("pch02"))
            {
                tmpDt = conn.GetData(strSQL);
            }

            if (tmpDt.Rows.Count > 0)
            {
                DateTime dt;
                try
                {
                    dt = Convert.ToDateTime(tmpDt.Rows[0][tmpDt.Columns["order_date"]].ToString());
                    //String strSQL = "select order_date,concode,b.description 主治醫師,pat_hist 病情摘要,reply_doc,c.description 回覆醫師,ans_rep 會診回覆 ";
                    //txtMemoconst.Text += "開單日期:" + dt.ToString("yyyyMMdd") + "  主治醫師:" + tmpDt.Columns["concodeName"].ToString() + "  回覆醫師:" + tmpDt.Columns["reply_docName"].ToString() + "\n";
                    //txtMemoconst.Text += "  病情摘要:" + tmpDt.Columns["pat_hist"].ToString() + "\n";
                    //txtMemoconst.Text += "  會診回覆:" + tmpDt.Columns["ans_rep"].ToString() + "\n";
                    asp_LitConsultation.Text += "開單日期:<span class=\"text-primary\">" + dt.ToString("yyyyMMdd") + "  </span>主治醫師:<span class=\"text-primary\">" + tmpDt.Columns["concodeName"].ToString() + "  </span>回覆醫師:<span class=\"text-primary\">" + tmpDt.Columns["reply_docName"].ToString() + "</span><br />";
                    asp_LitConsultation.Text += "  病情摘要:<span class=\"text-primary\">" + tmpDt.Columns["pat_hist"].ToString() + "</span><br />";
                    asp_LitConsultation.Text += "  會診回覆:<span class=\"text-primary\">" + tmpDt.Columns["ans_rep"].ToString() + "</span><br />";
                }
                catch { }
            }
            else
            {
                //txtMemoconst.Text = "無會診資料";
                asp_LitConsultation.Text += "<span class=\"text-primary\">無會診資料</span>";
            }
        }

        #endregion

        #region ProgressNote ProgressNote(病程)
        private void getProgressNotes()
        {
            asp_LitProgressNote.Text = "";
            String strSQL = " select desc ";
            strSQL += " from progress_note ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and rel_house_no = '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status = 'A' ";
            DataTable tmpDt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch02"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            if (tmpDt.Rows.Count > 0)
            {
                asp_LitProgressNote.Text += "  會診回覆:" + tmpDt.Columns["desc"].ToString() + "\n";
            }
            else
            {
                asp_LitProgressNote.Text = "無資料";
            }
        }

        #endregion

        #region Statement 敘述醫囑
        private void getStatementOrder()
        {
            asp_LitStatementOrder.Text = "";

            //String strSQL = "select note_type 類別,beg_date 起始日,statement 醫令,take_times 頻次,end_date 停止日,item_code 院碼 ";
            String strSQL = "select note_type ,to_char(beg_date) beg_date ,statement ,take_times ,to_char(end_date) end_date ,item_code  ";
            strSQL += " from ord_statement ";
            strSQL += " where reg_no = '";
            strSQL += this.Master.reg_no + "'";
            strSQL += " and rel_house_no = '";
            strSQL += this.Master.keybill + "'";
            strSQL += " and rec_status = 'A' ";
            //2014.11.8評鑑指示要將停服日為空白的資料排序於最上面 ︾
            strSQL += " order by end_date asc ";
            //2014.11.8評鑑指示要將停服日為空白的資料排序於最上面 ︽
            DataTable tmpDt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch02"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            
            foreach (DataRow dr in tmpDt.Rows)
            {
                asp_LitStatementOrder.Text += getStatementOrderRow(dr);
            }
        }

        private string getStatementOrderRow(DataRow dr)
        {
            string htmltr = "<tr>";

            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["note_type"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["beg_date"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["statement"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["take_times"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["end_date"].ToString().Trim());
            htmltr += string.Format("<td title=\"{0}\">{0}</td>", dr["item_code"].ToString().Trim());

            htmltr += "</tr>";
            return htmltr;
        }

        #endregion
    }
}