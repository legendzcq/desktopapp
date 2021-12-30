using CdelService.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
namespace CdelService.Local
{
    public abstract class DataAccessBase
    {
        protected readonly SQLiteConnection Conn;

        protected DataAccessBase()
        {
            string connString = "data source=" + Common._path + "\\db\\db.db";
            Conn = new SQLiteConnection(connString);

        }
        protected void OpenConn()
        {
            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
            }
        }

        protected void CloseConn()
        {
            try
            {
                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected DataTable ExecuteTable(string sql)
        {
			var adpt = new SQLiteDataAdapter(sql, Conn);
            var dt = new DataTable();
            adpt.Fill(dt);
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected int ExecuteNonQuery(string sql)
        {
            try
            {
                Conn.Open();
                var cmd = new SQLiteCommand(sql, Conn);
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                Conn.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        protected int ExecuteNonQuery(string sql, params SQLiteParameter[] @params)
        {
            try
            {
                Conn.Open();
                var cmd = new SQLiteCommand(sql, Conn);
                foreach (var item in @params)
                {
                    cmd.Parameters.Add(item);
                }
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                Conn.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected object ExecuteScalar(string sql)
        {
            try
            {
                Conn.Open();
                var cmd = new SQLiteCommand(sql, Conn);
                return cmd.ExecuteScalar();
            }
            finally
            {
                Conn.Close();
            }
        }
    }
}
