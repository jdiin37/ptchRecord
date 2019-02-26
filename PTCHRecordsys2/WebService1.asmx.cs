using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using Dev.ADODBSql;
using Newtonsoft.Json;

namespace PTCHRecordsys2
{
    /// <summary>
    /// WebService1 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下一行。
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        #region 殘骸
        [WebMethod(EnableSession = true)]
        public string GetPatInfo(string ID)
        {
            string strReturn = String.Empty;
            DataTable dtTemp = new DataTable();
            string SQL = String.Empty; ;

            if (ID.Length > 6)
            {
                SQL = string.Format("Select to_char(birth_date) birth_date, p_name, sex, reg_no From reg_file Where id_no ='{0}'", ID);
            }
            else
            {
                SQL = string.Format("Select to_char(birth_date) birth_date, p_name, sex, reg_no From reg_file Where reg_no ='{0}'", ID);
            }

            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dtTemp = conn.GetData(SQL);
            }

            strReturn = dtTemp.Rows.Count > 0 ? JsonConvert.SerializeObject(dtTemp, Newtonsoft.Json.Formatting.Indented) : "0";
            return strReturn;
        }


        [WebMethod(EnableSession = true)]
        public string GetPatVisitsList(string RegNo, string startDate, string endDate)
        {
            string strReturn = String.Empty;

            // Transact-SQL 陳述式 
            string strSQL = "select opd_date,dep_no,'' typeName,doc_code Dor,'' DorName , '' out_date,room_no, '' bed_no,reg_bill keybill,reg_no,ill_code,ill_code1,ill_code2,ill_code3,card_no ,insurance ";
            strSQL += " from v_all_opd_reg ";
            strSQL += " where reg_no = '";
            strSQL += RegNo + "'";
            strSQL += " and rec_status='A' ";
            strSQL += "and ((ins_remark<>'E' and ins_remark<>'J' and ins_remark<>'K' and ins_remark<>'U') or (ins_remark is null)) ";
            strSQL += " and opd_date >= '";
            strSQL += startDate + "' ";
            strSQL += " and opd_date <= '";
            strSQL += endDate + "' ";
            //strSQL += " union all ";
            ////strSQL += "select in_date 日期,dep_no 科別,'' 科別名稱,doc_code 醫師,'' 醫生姓名,to_char(out_date,'%Y/%m/%d') 出院日,'住院' 診別, bed_no 床號,rel_house_no keybill,reg_no,'' ill_code,'' ill_code1,'' ill_code2,'' ill_code3,card_no,insurance ";
            //strSQL += "select in_date ,dep_no ,'' typeName,doc_code Dor,'' DorName,to_char(out_date,'%Y/%m/%d') out_date,'住院' type, bed_no ,rel_house_no keybill,reg_no,'' ill_code,'' ill_code1,'' ill_code2,'' ill_code3,card_no,insurance ";
            //strSQL += " from ipdhis@on2tcp:bed_file ";
            //strSQL += " where reg_no = '";
            //strSQL += RegNo + "'  ";
            //strSQL += " and in_date >= '";
            //strSQL += startDate + "' ";
            //strSQL += " and in_date <= '";
            //strSQL += endDate + "' ";
            //strSQL += " union all ";
            ////strSQL += " select in_date 日期,dep_no 科別,'' 科別名稱,doc_code 醫師,'' 醫生姓名,to_char(out_date,'%Y/%m/%d') 出院日,'住院' 診別, bed_no 床號,rel_house_no keybill,reg_no,'' ill_code,'' ill_code1,'' ill_code2,'' ill_code3,card_no,insurance ";
            //strSQL += "select in_date ,dep_no ,'' typeName,doc_code Dor,'' DorName,to_char(out_date,'%Y/%m/%d') out_date,'住院' type, bed_no ,rel_house_no keybill,reg_no,'' ill_code,'' ill_code1,'' ill_code2,'' ill_code3,card_no,insurance ";
            //strSQL += " from ipdhis@on2tcp:hist_bed ";
            //strSQL += " where reg_no = '";
            //strSQL += RegNo + "'";
            //strSQL += " and out_date is not null  ";
            //strSQL += " and in_date >= '";
            //strSQL += startDate + "' ";
            //strSQL += " and in_date <= '";
            //strSQL += endDate + "' ";
            //strSQL += " order by  opd_date desc ";
            DataTable dt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dt = conn.GetData(strSQL);
            }

            strReturn = dt.Rows.Count > 0 ? JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented) : "0";
            return strReturn;
            //將table整理成<li>格式的html字串後放入div中
            //divRecord.InnerHtml = GenerateUL(dtBed);           
        }
        #endregion
    }
}
