using System.Data.SQLite;

namespace Framework.Local
{
    internal class Baseinfo : DataAccessBase
    {
        internal byte[] GetSecurityKey()
        {
            try
            {
                var buffer = ExecuteScalar("Select MainKey From DBKey");
                if (buffer == null) return new byte[0];
                return (byte[])buffer;
            }
            catch
            {
                return null;
            }
        }

        internal bool InsertKey(byte[] securityKey)
        {
            try
            {
                const string sql = "Insert into DBKey(MainKey) Values($SecurityKey)";
                return ExecuteNonQuery(sql, new SQLiteParameter("$SecurityKey", System.Data.DbType.Binary) { Value = securityKey }) >= 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
