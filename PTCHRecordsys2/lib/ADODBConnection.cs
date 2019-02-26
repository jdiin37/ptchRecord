using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;
using ADODB;

namespace Dev.ADODBSql
{
    public class DevADODBConn : IDisposable
    {
        private static ADODB.Connection conn;

        #region 創建SQL連線
        public DevADODBConn(string DSN)
        {
            try
            {
                if (conn == null)
                {
                    conn = new ADODB.Connection();
                    conn.Open(DSN, null, null, 0);
                }
            }
            catch (Exception ex)
            {
                //PublicLib.handleError("", this.GetType().Name, ex.Message);
            }
        }

        public void Dispose()
        {
            if (conn != null)
            {
                conn.Close();
                conn = null;
            }
        }
        #endregion

        public bool ExecuteQuery(string strSql)
        {
            bool flag = false;

            try
            {
                conn.BeginTrans();

                ADODB.Command cmd = new ADODB.Command();
                cmd.ActiveConnection = conn;
                cmd.CommandTimeout = 90;
                cmd.CommandType = ADODB.CommandTypeEnum.adCmdText;
                cmd.CommandText = strSql;

                object objRecordsAffected = Type.Missing;
                object objParams = Type.Missing;
                cmd.Execute(out objRecordsAffected, ref objParams, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);

                conn.CommitTrans();

                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;

                conn.RollbackTrans();

                //PublicLib.handleError("", this.GetType().Name, ex.Message + string.Format("({0})", strSql));
            }

            return flag;
        }

        #region Select資料
        public DataTable GetData(string strSql)
        {
            DataTable dt = new DataTable();
            dt.Locale = System.Globalization.CultureInfo.InvariantCulture;

            try
            {
                ADODB.Recordset rs = new ADODB.Recordset();
                rs.Open(strSql, conn,ADODB.CursorTypeEnum.adOpenForwardOnly,ADODB.LockTypeEnum.adLockReadOnly,-1);
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                adapter.Fill(dt, rs);
                adapter.Dispose();
                rs.Close();
            }
            catch (Exception ex)
            {
                //PublicLib.handleError("", this.GetType().Name, ex.Message + string.Format("({0})", strSql));
            }

            return dt;
        }

        public DataTable GetDataWithError(string strSql,ref string msg)
        {
            DataTable dt = new DataTable();
            dt.Locale = System.Globalization.CultureInfo.InvariantCulture;

            try
            {
                ADODB.Recordset rs = new ADODB.Recordset();
                rs.Open(strSql, conn, ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly, -1);
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                adapter.Fill(dt, rs);
                adapter.Dispose();
                rs.Close();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                //PublicLib.handleError("", this.GetType().Name, ex.Message + string.Format("({0})", strSql));
            }

            return dt;
        }

        public DataTable GetData(string strSql, string strSqlData)
        {
            DataTable dt = new DataTable();

            try
            {
                conn.BeginTrans();

                ADODB.Command cmd = new ADODB.Command();
                cmd.ActiveConnection = conn;
                cmd.CommandTimeout = 90;
                cmd.CommandType = ADODB.CommandTypeEnum.adCmdText;
                cmd.CommandText = strSql;

                object objRecordsAffected = Type.Missing;
                object objParams = Type.Missing;
                cmd.Execute(out objRecordsAffected, ref objParams, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);

                ADODB.Recordset rs = new ADODB.Recordset();
                rs.Open(strSqlData, conn, ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly, -1);
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                adapter.Fill(dt, rs);

                adapter.Dispose();
                rs.Close();

                conn.CommitTrans();
            }
            catch (Exception ex)
            {
                conn.RollbackTrans();

                //PublicLib.handleError("", this.GetType().Name, ex.Message + string.Format("({0})", strSql));
            }

            return dt;
        }
        #endregion
    }
}