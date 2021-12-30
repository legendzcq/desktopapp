using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using Framework.Utility;

namespace Framework.Local
{
    public abstract class DataAccessBase
    {
        protected readonly SQLiteConnection Conn;

        protected DataAccessBase()
        {
			string connString = "data source=" + SystemInfo.AppDataPath + "\\db\\db.db";
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
	        //Trace.WriteLine(sql);
			var adpt = new SQLiteDataAdapter(sql, Conn);
            var dt = new DataTable();
            adpt.Fill(dt);
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        /// <remarks>Sqllite参数采用$....的命名</remarks>
        protected DataTable ExecuteTable(string sql, Dictionary<string, object> @params)
        {
            try
            {
                var adpt = new SQLiteDataAdapter(sql, Conn);
                foreach (var item in @params)
                {
                    adpt.SelectCommand.Parameters.AddWithValue(item.Key, item.Value);
                }
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        protected DataTable ExecuteTable(string sql, params SQLiteParameter[] @params)
        {
			try
            {
				//Trace.WriteLine(sql);
                var adpt = new SQLiteDataAdapter(sql, Conn);
                foreach (var item in @params)
                {
                    adpt.SelectCommand.Parameters.Add(item);
                }
                var dt = new DataTable();
                adpt.Fill(dt);
                return dt;
            }
            catch
            {
                return null;
            }
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
        protected int ExecuteNonQuery(string sql, Dictionary<string, object> @params)
        {
            try
            {
                Conn.Open();
                var cmd = new SQLiteCommand(sql, Conn);
                foreach (var item in @params)
                {
                    cmd.Parameters.AddWithValue(item.Key, item.Value);
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
        /// <param name="params"></param>
        /// <returns></returns>
        protected int ExecuteNonQuery(string sql, params SQLiteParameter[] @params)
        {
	        try
            {
				//Trace.WriteLine(sql);
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
        /// <param name="sqls"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        protected bool ExecuteNonQuery(string[] sqls, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (sqls == null) throw new ArgumentNullException("sqls");
            if (sqls.Length == 0) return true;
            if (sqls.Length == 1) return ExecuteNonQuery(sqls[0]) >= -1;
            Conn.Open();
            var tran = Conn.BeginTransaction(isolationLevel);
            try
            {
                var cmd = new SQLiteCommand
                {
                    Connection = Conn,
                    Transaction = tran
                };
                foreach (var sql in sqls)
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                return false;
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
				//Trace.WriteLine(sql);
                Conn.Open();
                var cmd = new SQLiteCommand(sql, Conn);
                return cmd.ExecuteScalar();
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
        protected object ExecuteScalar(string sql, Dictionary<string, object> @params)
        {
            try
            {
                Conn.Open();
                var cmd = new SQLiteCommand(sql, Conn);
                foreach (KeyValuePair<string, object> item in @params)
                {
                    cmd.Parameters.AddWithValue(item.Key, item.Value);
                }
                return cmd.ExecuteScalar();
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
        protected object ExecuteScalar(string sql, params SQLiteParameter[] @params)
        {
			try
            {
				//Trace.WriteLine(sql);
                Conn.Open();
                var cmd = new SQLiteCommand(sql, Conn);
                foreach (SQLiteParameter item in @params)
                {
                    cmd.Parameters.Add(item);
                }
                return cmd.ExecuteScalar();
            }
            finally
            {
                Conn.Close();
            }
        }
    }
}
