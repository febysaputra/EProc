using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;

namespace EProcurement.Models
{
    public class DBconn
    {
        protected SqlConnection conn;

        public bool Open(string Connection = "EProcConn")
        {
            conn = new SqlConnection(@WebConfigurationManager.ConnectionStrings[Connection].ToString());
            try
            {
                bool b = true;
                if (conn.State.ToString() != "Open")
                {
                    conn.Open();
                }
                return b;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int ToInt(object s)
        {
            try
            {
                return Int32.Parse(s.ToString());
            }
            catch
            {
                return 0;
            }
        }

        public int DataInsert(string sql)
        {
            int LastId = 0;
            string query = sql + ";SELECT @@Identity;";
            try
            {
                if (conn.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    LastId = this.ToInt(cmd.ExecuteScalar());
                }
                return this.ToInt(LastId);
            }
            catch
            {
                return 0;
            }
        }

        public int DataEdit(string sql)
        {
            int LastId = 0;
            string query = sql;
            try
            {
                if (conn.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    LastId = this.ToInt(cmd.ExecuteScalar());
                }
                return this.ToInt(LastId);
            }
            catch
            {
                return 0;
            }
        }

        public int DataDelete(string sql)
        {
            string query = sql;
            try
            {
                if (conn.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
                return 2;
            }
            catch
            {
                return 0;
            }
        }

        public SqlCommand DataSelect(string sql)
        {
            string query = sql;
            try
            {
                if (conn.State.ToString() == "Open")
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    return cmd;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}