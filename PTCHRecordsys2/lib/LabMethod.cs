using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dev.ADODBSql;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Configuration;

namespace PTCHRecordsys2
{
    public class LabMethod
    {

        #region 取得檢驗資料
        public DataTable getExam_report(string reg_no, string startDate, string endDate)
        {
            //String strSQL = "select distinct  DECODE(oi_flag,'I','住診',DECODE(oi_flag,'O','門診','急診')) 診別,order_date 門診日期 ";
            String strSQL = "select distinct  DECODE(oi_flag,'I','住診',DECODE(oi_flag,'O','門診','急診')) oi_flag,order_date";
            strSQL += " from exam_report ";
            strSQL += " where reg_no = '";
            strSQL += reg_no + "'";
            //2013.05.27陳俊廷指示要加上區間 ︾
            strSQL += " and order_date >= '";
            strSQL += startDate + "'";
            strSQL += " and order_date <= '";
            strSQL += endDate + "'";
            //2013.05.27陳俊廷指示要加上區間 ︽
            strSQL += " and rec_status<>'R' and work_group<>'B' ";
            strSQL += " union ";
            strSQL += "select distinct  DECODE(oi_flag,'I','住診',DECODE(oi_flag,'O','門診','急診')) oi_flag,order_date ";
            strSQL += " from past_exam_report ";
            strSQL += " where reg_no = '";
            strSQL += reg_no + "'";
            //2013.05.27陳俊廷指示要加上區間 ︾
            strSQL += " and order_date >= '";
            strSQL += startDate + "'";
            strSQL += " and order_date <= '";
            strSQL += endDate + "'";
            //2013.05.27陳俊廷指示要加上區間 ︽
            strSQL += " and rec_status<>'R' and work_group<>'B' ";
            strSQL += " union ";
            strSQL += "select distinct  DECODE(oi_flag,'I','住診',DECODE(oi_flag,'O','門診','急診')) oi_flag,order_date ";
            strSQL += " from hispast@on1tcp:past_exam_report ";
            strSQL += " where reg_no = '";
            strSQL += reg_no + "'";
            //2013.05.27陳俊廷指示要加上區間 ︾
            strSQL += " and order_date >= '";
            strSQL += startDate + "'";
            strSQL += " and order_date <= '";
            strSQL += endDate + "'";
            //2013.05.27陳俊廷指示要加上區間 ︽
            strSQL += " and rec_status<>'R' and work_group<>'B' ";
            //strSQL += " order by 門診日期 desc ";
            strSQL += " order by order_date desc ";
            DataTable dt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dt = conn.GetData(strSQL);
            }
            return dt;
        }

        public DataTable getAllexam_report(string reg_no, string order_date, string oi_flag, Boolean vhistory_data)
        {
            String strSQL = "select exam_type,item_code,seq_no,'' normal_data,'' unit,'' NWD,* ";
            strSQL += " from exam_report ";
            strSQL += " where reg_no = '";
            strSQL += reg_no + "'";
            strSQL += " and order_date = '";
            strSQL += order_date + "'";
            strSQL += " and oi_flag='";
            strSQL += oi_flag + "'";
            strSQL += " and rec_status<>'R' and work_group<>'B' ";
            if (vhistory_data)
            {
                strSQL += " union ";
                strSQL += "select exam_type,item_code,seq_no,'' normal_data,'' unit,'' NWD,* ";
                strSQL += " from past_exam_report ";
                strSQL += " where reg_no = '";
                strSQL += reg_no + "'";
                strSQL += " and order_date = '";
                strSQL += order_date + "'";
                strSQL += " and oi_flag='";
                strSQL += oi_flag + "'";
                strSQL += " and rec_status<>'R' and work_group<>'B' ";
                strSQL += " union ";
                strSQL += "select exam_type,item_code,seq_no,'' normal_data,'' unit,'' NWD,* ";
                strSQL += " from hispast@on1tcp:past_exam_report ";
                strSQL += " where reg_no = '";
                strSQL += reg_no + "'";
                strSQL += " and order_date = '";
                strSQL += order_date + "'";
                strSQL += " and oi_flag='";
                strSQL += oi_flag + "'";
                strSQL += " and rec_status<>'R' and work_group<>'B' ";
            }
            strSQL += " order by 1,2,3 ";
            DataTable dt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dt = conn.GetData(strSQL);
            }
            return dt;
        }

        #endregion

        #region=取得描述性檢查報告=
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reg_no">病歷號</param>
        /// <param name="item_code">檢查代號</param>
        /// <param name="seq_no">序號</param>
        /// <param name="exam_time">檢查時間</param>
        /// <param name="bill_no">處方單號</param>
        /// <param name="oi_flag">門急住診註記</param>
        /// <param name="order_date">處方日期</param>
        /// <param name="report_date">報告日期</param>
        /// <returns></returns>
        public DataTable getReportData(string reg_no, string item_code, string seq_no, string exam_time, string bill_no, string oi_flag, string order_date, string report_date)
        {
            string SQL = "";
            DateTime now = DateTime.Now;
            //取得描述性報告資料
            SQL = "select * from exam_text_data ";
            SQL += " where reg_no ='" + reg_no + "'";
            SQL += " and order_date='" + order_date + "'";
            SQL += " and  bill_no='" + bill_no + "'";
            SQL += " and oi_flag='" + oi_flag + "'";
            SQL += " and item_code = '" + item_code + "'";
            SQL += " and exam_time = '" + exam_time + "'";
            SQL += " and seq_no='" + seq_no + "' and rec_status<>'R'";
            if ((Convert.ToInt16(now.Year) - Convert.ToInt16(report_date.Substring(0, 4))) > 1)
            {
                SQL += " union";
                SQL += " select * from past_exam_text";
                SQL += " where reg_no ='" + reg_no + "'";
                SQL += " and order_date='" + order_date + "'";
                SQL += " and  bill_no='" + bill_no + "'";
                SQL += " and item_code = '" + item_code + "'";
                SQL += " and exam_time = '" + exam_time + "'";
                SQL += " and seq_no='" + seq_no + "' and rec_status<>'R'";
                SQL += " union";
                //SQL += " select text_data from '+ publiclib_DB_switch('pch01_hispast','past_exam_text'";
                SQL += " select * from  hispast@on1tcp:past_exam_text ";
                SQL += " where reg_no ='" + reg_no + "'";
                SQL += " and order_date='" + order_date + "'";
                SQL += " and  bill_no='" + bill_no + "'";
                SQL += " and item_code = '" + item_code + "'";
                SQL += " and exam_time = '" + exam_time + "'";
                SQL += " and seq_no='" + seq_no + "' and rec_status<>'R'";
            }
            DataTable dtreport = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dtreport = conn.GetData(SQL);
            }
            return dtreport;
        }
        #endregion

        #region =判斷檢驗異常值=
        public string getNWD(string exam_data, string min_normal, string max_normal, string min_danger, string max_danger, string data_type)
        {
            float value, min_nor, max_nor, min_dgr, max_dgr;
            string denominator, s_denominator;//分母
            string Result = "";
            string[] exam_data1;
            value = 0;
            min_dgr = 0;
            min_nor = 0;
            max_dgr = 0;
            max_nor = 0;

            if ((data_type == "") || (data_type == " ") || (exam_data == "CHECK IN"))
            {

                Result = "?";

                return Result;

            }
            if (exam_data.Split(';').Length > 1)
            {
                exam_data1 = exam_data.Split(';');
                exam_data = exam_data1[0];
            }
            if (data_type == "A")   //定量
            {

                if (!isNumeric(exam_data) && (!Pos(">", exam_data)) &&

                  (!Pos("<", exam_data)) && (!Pos("大於", exam_data)) && (!Pos("小於", exam_data)))
                {

                    Result = "";

                    return Result;

                }

                try
                {
                    if (isNumeric(exam_data) && (!Pos(">", exam_data)) &&

                      (!Pos("<", exam_data)) && (!Pos("大於", exam_data)) && (!Pos("小於", exam_data)))

                        value = Convert.ToSingle(exam_data);

                    else if ((Pos(">", exam_data)) || (Pos("大於", exam_data)))
                    {

                        if (max_danger != "")
                        {
                            value = Convert.ToSingle(max_danger);
                        }
                        else
                        {

                            if ((exam_data.Substring(0, 1) == ">") && (isNumeric(exam_data.Substring(1, exam_data.Length))))

                                value = Convert.ToSingle(exam_data.Substring(1, exam_data.Length));

                            else

                                value = Convert.ToSingle(max_normal);

                            //value= Convert.ToSingle(exam_data);

                        }

                    }

                    else if ((Pos("<", exam_data)) || (Pos("小於", exam_data)))
                    {

                        if (min_danger != "")

                            value = Convert.ToSingle(min_danger);

                        else
                        {

                            if ((exam_data.Substring(0, 1) == "<") && (isNumeric(exam_data.Substring(1, exam_data.Length))))

                                value = Convert.ToSingle(exam_data.Substring(1, exam_data.Length));

                            else

                                value = Convert.ToSingle(min_normal);

                        }

                    }

                    if (isNumeric(min_normal))

                        min_nor = Convert.ToSingle(min_normal);

                    if (isNumeric(max_normal))

                        max_nor = Convert.ToSingle(max_normal);

                    if (isNumeric(min_danger))

                        min_dgr = Convert.ToSingle(min_danger);

                    if (isNumeric(max_danger))

                        max_dgr = Convert.ToSingle(max_danger);

                    if ((min_danger != "") && (value <= min_dgr))

                        Result = "D";

                    else if ((min_danger != "") && (min_normal != "") && (min_dgr < value) && (value < min_nor))

                        Result = "W";

                    else if ((min_danger == "") && (min_normal != "") && (value < min_nor))

                        Result = "W";

                    else if ((min_normal != "") && (max_normal != "") && (min_nor <= value) && (value <= max_nor))

                        Result = "N";

                    else if ((max_normal != "") && (max_danger != "") && (max_nor < value) && (value < max_dgr))

                        Result = "W";

                    else if ((max_normal != "") && (max_danger == "") && (max_nor < value))

                        Result = "W";

                    else if ((max_danger != "") && (max_dgr <= value))

                        Result = "D";

                }
                catch
                {

                    Result = "";

                    return Result;

                }

            }

            else if (data_type == "B")  //半定量
            {

                s_denominator = exam_data.Substring(PosInt("", exam_data) + 1, exam_data.Length); ;

                denominator = min_danger.Substring(PosInt("", min_danger) + 1, min_danger.Length);

                if (denominator == "")

                    denominator = max_danger.Substring(PosInt("", max_danger) + 1, max_danger.Length);

                if (denominator == "")

                    return Result;

                if ((isNumeric(s_denominator)) && (isNumeric(denominator))) //一般半定量 例 132
                {

                    if (Convert.ToInt16(denominator) < Convert.ToInt16(s_denominator))

                        Result = "D";

                }

                else // 特殊半定量 如 阿米巴 132X(+) 以上
                {

                    if (PosInt("+", s_denominator) != 0)
                    {

                        s_denominator = s_denominator.Substring(0, PosInt("X", s_denominator) - 1);

                        denominator = denominator.Substring(0, PosInt("X", s_denominator) - 1);

                        if (!isNumeric(s_denominator))

                            return Result;

                        if (!isNumeric(denominator))

                            return Result;

                        if (Convert.ToInt16(denominator) < Convert.ToInt16(s_denominator))

                            Result = "D";

                    }

                }

            }

            else if (data_type == "D")  //(定性1D 說明正常值以外的文字,都屬危險值)
            {

                if ((exam_data.ToLower() == min_normal.ToLower()) ||

                   (exam_data.ToLower() == max_normal.ToLower()))

                    Result = "N";

                else

                    Result = "D";

            }

            //--20130904修改，文字只要比對前3碼即可(良蘭提)

            else if (data_type == "C")  //(定性1C 說明正常值以外的文字,都屬警告值)
            {
                Result = "W";
                if (exam_data.Length > 3)
                {
                    if (min_normal.Length > 0)
                    {
                        try
                        {
                            if (exam_data.Substring(0, 3).ToLower() == min_normal.Substring(0, 3).ToLower())
                                Result = "N";
                        }
                        catch (Exception ex)
                        {
                            Result = "";
                        }

                    }
                    if (max_normal.Length > 0)
                    {

                        try
                        {
                            if (exam_data.Substring(0, 3).ToLower() == max_normal.Substring(0, 3).ToLower())
                                Result = "N";
                        }
                        catch (Exception ex)
                        {
                            Result = "";
                        }
                    }
                }
                if (exam_data.Length == 1)
                {
                    if (min_normal.Length > 0)
                    {
                        if (exam_data.ToLower() == min_normal.ToLower())
                            Result = "N";

                    }
                    if (max_normal.Length > 0)
                    {
                        if (exam_data.ToLower() == max_normal.ToLower())
                            Result = "N";
                    }
                }
            }

            else if (data_type == "E")    //(定性2E 說明危險值高標或低標有設定,才算危險值)
            {

                if (PosInt(max_danger.ToUpper(), exam_data.ToUpper()) != 0)

                    Result = "D";

                if (PosInt(min_danger.ToUpper(), exam_data.ToUpper()) != 0)

                    Result = "D";

                if (PosInt(min_normal.ToUpper(), exam_data.ToUpper()) != 0)

                    Result = "N";

                if (PosInt(max_normal.ToUpper(), exam_data.ToUpper()) != 0)

                    Result = "N";

            }
            return Result;

        }

        #region =判斷是否為數值=
        public Boolean isNumeric(string str)
        {
            Double n;
            bool isNumeric = Double.TryParse(str, out n);
            return isNumeric;
        }
        #endregion

        #region =Pos(字串中搜尋某一字元,回傳true or false)=
        public Boolean Pos(string tmpChar, string tmpString)
        {
            //string to Array
            string[] searchString = tmpString.Select(c => c.ToString()).ToArray();
            foreach (string str in searchString)
            {
                if (tmpString == str)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region =PosInt(字串中搜尋某一字元,並回傳此字元的位置)=
        public int PosInt(string tmpChar, string tmpString)
        {
            //string to Array
            string[] searchString = tmpString.Select(c => c.ToString()).ToArray();
            for (int i = 0; i < searchString.Length; i++)
            {
                if (tmpString == searchString[i])
                {
                    return i;
                }
            }
            return 0;
        }
        #endregion

        #endregion

        #region 取得檢驗項目標準資料明細記錄sub_item_code
        public DataTable getSub_item_code()
        {
            String strSQL = "select NVL(normal_data,(select normal_data from sub_item_code1 where item_code=sub_item_code.item_code and seq_no=sub_item_code.seq_no)) normal_data,* ";
            strSQL += " from sub_item_code ";
            DataTable dt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dt = conn.GetData(strSQL);
            }
            return dt;
        }
        
        #endregion

        #region 取得各個科室是否有資料
        public DataTable getTab(string reg_no, Boolean vhistory_data, string startDate, string endDate)
        {
            DataTable dtTab = new DataTable();
            dtTab.Columns.Add("Desc", typeof(String));

            // 檢驗和檢驗彙總 
            String strSQL = "select distinct oi_flag,order_date ";
            strSQL += " from exam_report ";
            strSQL += " where reg_no = '";
            strSQL += reg_no + "'";
            strSQL += " and order_date >= '";
            strSQL += startDate + "'";
            strSQL += " and order_date <= '";
            strSQL += endDate + "'";
            strSQL += " and rec_status<>'R' ";
            strSQL += " and work_group<>'B' ";
            if (vhistory_data)
            {
                strSQL += " union ";
                strSQL += " select distinct oi_flag,order_date ";
                strSQL += " from past_exam_report ";
                strSQL += " where reg_no = '";
                strSQL += reg_no + "'";
                strSQL += " and order_date >= '";
                strSQL += startDate + "'";
                strSQL += " and order_date <= '";
                strSQL += endDate + "'";
                strSQL += " and rec_status<>'R' ";
                strSQL += " and work_group<>'B' ";
                strSQL += " union ";
                strSQL += " select distinct oi_flag,order_date ";
                strSQL += " from  hispast@on1tcp:past_exam_report ";
                strSQL += " where reg_no = '";
                strSQL += reg_no + "'";
                strSQL += " and order_date >= '";
                strSQL += startDate + "'";
                strSQL += " and order_date <= '";
                strSQL += endDate + "'";
                strSQL += " and rec_status<>'R' ";
                strSQL += " and work_group<>'B' ";
            }
            DataTable tmpDt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(strSQL);
            }

            if (tmpDt.Rows.Count > 0)
            {
                dtTab.Rows.Add(new object[] { "檢驗" });
                dtTab.Rows.Add(new object[] { "檢驗彙總" });
            }

            //X光
            strSQL = "select distinct oi_flag,opd_date,report_date ";
            strSQL += " from xray_sub_data ";
            strSQL += " where reg_no = '";
            strSQL += reg_no + "'";
            strSQL += " and opd_date >= '";
            strSQL += startDate + "'";
            strSQL += " and opd_date <= '";
            strSQL += endDate + "'";
            strSQL += " and rec_status='A' ";
            strSQL += " and xray_type<>'0' ";
            if (vhistory_data)
            {
                strSQL += " union ";
                strSQL += "select distinct oi_flag,opd_date,report_date ";
                strSQL += " from past_xray_sub_data ";
                strSQL += " where reg_no = '";
                strSQL += reg_no + "'";
                strSQL += " and opd_date >= '";
                strSQL += startDate + "'";
                strSQL += " and opd_date <= '";
                strSQL += endDate + "'";
                strSQL += " and rec_status='A' ";
                strSQL += " and xray_type<>'0' ";
                strSQL += " union ";
                strSQL += "select distinct oi_flag,opd_date,report_date ";
                strSQL += " from hispast@on1tcp:past_xray_sub_data ";
                strSQL += " where reg_no = '";
                strSQL += reg_no + "'";
                strSQL += " and opd_date >= '";
                strSQL += startDate + "'";
                strSQL += " and opd_date <= '";
                strSQL += endDate + "'";
                strSQL += " and rec_status='A' ";
                strSQL += " and xray_type<>'0' ";
            }
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            if (tmpDt.Rows.Count > 0)
            {
                dtTab.Rows.Add(new object[] { "X光" });
            }

            //微生物
            string[] start_data = Convert.ToDateTime(startDate).ToString("yyyy/MM/dd").Split('/');
            string[] end_data = Convert.ToDateTime(endDate).ToString("yyyy/MM/dd").Split('/');
            //strSQL = "select distinct id_data,rep_date,rep_time ";
            //String strSQL = "select distinct id_data,rep_date,rep_time ";
            strSQL = "select distinct rep_date ";
            strSQL += " from ordresult ";
            strSQL += " where chart_no = '";
            strSQL += reg_no + "'";
            strSQL += " and rep_date >= '";
            strSQL += (Convert.ToInt16(start_data[0]) - 1911).ToString() + start_data[1] + start_data[2] + "'";
            strSQL += " and rep_date <= '";
            strSQL += (Convert.ToInt16(end_data[0]) - 1911).ToString() + end_data[1] + end_data[2] + "'";
            if (vhistory_data)
            {
                strSQL += " union ";
                //strSQL += "select distinct id_data,rep_date,rep_time ";
                strSQL += " select distinct rep_date ";
                strSQL += " from past_ordresult ";
                strSQL += " where chart_no = '";
                strSQL += reg_no + "'";
                strSQL += " and rep_date >= '";
                strSQL += (Convert.ToInt16(start_data[0]) - 1911).ToString() + start_data[1] + start_data[2] + "'";
                strSQL += " and rep_date <= '";
                strSQL += endDate + "'";
                strSQL += " union ";
                //strSQL += "select distinct id_data,rep_date,rep_time ";
                strSQL += " select distinct rep_date ";
                strSQL += " from hispast@on1tcp:past_ordresult ";
                strSQL += " where chart_no = '";
                strSQL += reg_no + "'";
                strSQL += " and rep_date >= '";
                strSQL += (Convert.ToInt16(start_data[0]) - 1911).ToString() + start_data[1] + start_data[2] + "'";
                strSQL += " and rep_date <= '";
                strSQL += (Convert.ToInt16(end_data[0]) - 1911).ToString() + end_data[1] + end_data[2] + "'";
            }
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            if (tmpDt.Rows.Count > 0)
            {
                dtTab.Rows.Add(new object[] { "微生物" });
            }

            //檢查
            strSQL = "select distinct oi_flag,opd_date,report_date ";
            strSQL += " from check_wait_data ";
            strSQL += " where reg_no = '";
            strSQL += reg_no + "'";
            strSQL += " and opd_date >= '";
            strSQL += startDate + "'";
            strSQL += " and opd_date <= '";
            strSQL += endDate + "'";
            strSQL += " and rec_status='A' ";
            strSQL += " and test_type='2' ";
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            if (tmpDt.Rows.Count > 0)
            {
                dtTab.Rows.Add(new object[] { "檢查" });
            }

            //精液分析
            strSQL = "select * ";
            strSQL += " from exam_semen ";
            strSQL += " where reg_no = '";
            strSQL += reg_no + "'";
            strSQL += " and opd_date >= '";
            strSQL += startDate + "'";
            strSQL += " and opd_date <= '";
            strSQL += endDate + "'";
            strSQL += " and rec_status='A' ";
            strSQL += " and test_status='1' ";
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            if (tmpDt.Rows.Count > 0)
            {
                dtTab.Rows.Add(new object[] { "精液分析" });
            }

            //病理報告(只讀一年內)
            strSQL = "select distinct patid ";
            strSQL += " from pat010f ";
            strSQL += " where reg_no = '00";
            strSQL += reg_no + "'";
            strSQL += " and aply_hsptl='00' ";
            if (!vhistory_data)
            {
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                dt = dt.AddDays(-365);
                strSQL += " and tk_date>= ";
                strSQL += dt.ToString("yyyy/M/dd") + "'";
            }
            strSQL += " order by patid desc ";
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            if (tmpDt.Rows.Count > 0)
            {
                dtTab.Rows.Add(new object[] { "病理報告" });
            }

            //手術
            strSQL = "select distinct a.or_date,a.or_no,a.reg_no ";
            strSQL += " from or_info a,op_note b ";
            strSQL += " where a.reg_no = '";
            strSQL += reg_no + "'";
            strSQL += " and b.reg_no= '";
            strSQL += reg_no + "'";
            strSQL += " and a.or_date >= '";
            strSQL += startDate + "'";
            strSQL += " and a.or_date <= '";
            strSQL += endDate + "'";
            strSQL += " and a.reg_no=b.reg_no and a.or_no=b.or_no and a.rec_status<>'R' order by a.or_date desc ";
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            if (tmpDt.Rows.Count > 0)
            {
                dtTab.Rows.Add(new object[] { "手術" });
            }

            //EKG
            strSQL = "select distinct STUDY_DATE ";
            strSQL += " from ECGREPORT ";
            strSQL += " where PATIENT_ID = '";
            strSQL += reg_no + "'";
            strSQL += " and STUDY_DATE >= '";
            strSQL += start_data[0] + start_data[1] + start_data[2] + "'";
            strSQL += " and STUDY_DATE <= '";
            strSQL += end_data[0] + end_data[1] + end_data[2] + "'";
            strSQL += "  order by STUDY_DATE desc ";
            tmpDt = GetEKGSqlData(strSQL);
            if (tmpDt.Rows.Count > 0)
            {
                dtTab.Rows.Add(new object[] { "EKG" });
            }


            //腫瘤科報告
            strSQL = "select * ";
            strSQL += " from neo_report ";
            strSQL += " where reg_no = '";
            strSQL += reg_no + "'";
            strSQL += " and report_date >= '";
            strSQL += startDate + "'";
            strSQL += " and report_date <= '";
            strSQL += endDate + "'";
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                tmpDt = conn.GetData(strSQL);
            }
            if (tmpDt.Rows.Count > 0)
            {
                dtTab.Rows.Add(new object[] { "腫瘤科報告" });
            }

            //捷格表單系統
            strSQL = "select a.*,b.examname ";
            strSQL += " from css_rep a,css_tmp b ";
            strSQL += " where spatientid= '";
            strSQL += reg_no + "'";
            strSQL += " and screatedate >= '";
            strSQL += startDate + "'";
            strSQL += " and screatedate <= '";
            strSQL += endDate + "'";
            strSQL += " and a.stype=b.examid ";
            strSQL += "and a.sprotect<>'D' ";
            //tmpDt = GetSqlData(strSQL);
            //if (tmpDt.Rows.Count > 0)
            //{
            //    dtTab.Rows.Add(new object[] { "特殊表單" });
            //}

            return dtTab;
        }

        #endregion

        #region = GetEKGSqlData =
        public DataTable GetEKGSqlData(string sqlString)
        {
            DataTable dt = new DataTable();
            try
            {
                string connString = WebConfigurationManager.ConnectionStrings["connEKG"].ConnectionString.ToString();
                SqlConnection sqlconn = new SqlConnection(connString);
                SqlCommand sqlCmd = new SqlCommand(sqlString, sqlconn);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCmd);
                sqlDataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                string SQL = "";
                SQL += "Insert into SystemErrorLog ";
                SQL += "    (ErrorType, ErrorContent, ErrorTime, Remark) ";
                SQL += "Values ";
                SQL += "    ('Form PTCHRecordsys2 資料庫讀取失敗', '" + ex.ToString().Replace("'", "''") + "', getdate(), '" + sqlString.Replace("'", "''") + "')";
                SaveData(SQL);
                return dt;
            }
        }
        #endregion

        #region = SaveData =
        

        public void SaveData(string sqlString)
        {
            string connString = WebConfigurationManager.ConnectionStrings["connWriteLog"].ConnectionString.ToString();
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction("SaveTrans");
            cmd.Connection = conn;
            cmd.Transaction = trans;

            try
            {
                cmd.CommandText = sqlString;
                cmd.ExecuteNonQuery();
                trans.Commit();
            }
            catch
            {
                trans.Rollback("SaveTrans");
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region =getFormatYyyyMd(將資料庫的datetime格式轉為yyyy/M/d)=
        /// <summary>
        /// 將資料庫的datetime格式轉為yyyy/M/d
        /// </summary>
        /// <param name="datetime">datetime</param>
        /// <returns>yyyy/M/d</returns>
        public string getFormatYyyyMd(string datetime)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(datetime);
                return dt.ToString("yyyy/M/d");
            }
            catch
            {
                return "";
            }

        }
        #endregion

        public DataTable getCode_file(string item_type)
        {
            String strSQL = "select * from code_file ";
            strSQL += " where item_type= '";
            strSQL += item_type + "'";
            DataTable dt = new DataTable();
            using (DevADODBConn conn = new DevADODBConn("pch01"))
            {
                dt = conn.GetData(strSQL);
            }
            return dt;
        }
    }
}