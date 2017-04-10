using Goobeer.DB.ReflectionHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace Goobeer.DB
{
    /// <summary>
    /// 数据库助手类 (需要详细记录发生的事件)
    /// 打开数据库时，可能遇到异常
    /// </summary>
    public class RDBRepertory
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string ConnectionString { get; set; }
        private string ProviderName { get; set; }

        private DbConnection Conn { get; set; }
        public DbCommandBuilder CmdBuilder { get; set; }
        private DbProviderFactory DBProvider { get; set; }

        public delegate void ExceptionNoticeHandler(Exception ex);
        public event ExceptionNoticeHandler exceptionNoticeEvent;

        public RDBRepertory(string connectionString, string providerName)
        {
            ConnectionString = connectionString;
            ProviderName = providerName;
            try
            {
                DBProvider = DbProviderFactories.GetFactory(ProviderName);
                Conn = DBProvider.CreateConnection();
                Conn.ConnectionString = ConnectionString;
                //Adapter = DBProvider.CreateDataAdapter();
                CmdBuilder = DBProvider.CreateCommandBuilder();
                //CmdBuilder.DataAdapter = Adapter;
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
        }

        #region CreateDbConn
        /// <summary>
        /// 在原有的DBProvider上(在当前数据库连接上)创建新的数据库操作连接
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateDbConn()
        {
            var conn = DBProvider.CreateConnection();
            conn.ConnectionString = ConnectionString;
            return conn;
        }

        /// <summary>
        /// 在新的DBProvider上创建新的数据库操作连接
        /// </summary>
        /// <param name="dbPrividerName"></param>
        /// <returns></returns>
        public DbConnection CreateDbConn(string dbPrividerName,string connectionString)
        {
            var conn = DbProviderFactories.GetFactory(dbPrividerName).CreateConnection();
            conn.ConnectionString = connectionString;
            return conn;
        }

        /// <summary>
        /// 创建数据库连接(在一个app中，同时连接多个数据库)
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        public DbConnection CreateDbConn(DbProviderFactory dbProvider)
        {
            return dbProvider.CreateConnection();
        }
        #endregion

        #region CreateDbCmd
        /// <summary>
        /// 创建新的 DbCommand
        /// </summary>
        /// <returns></returns>
        public DbCommand CreateDbCmd()
        {
            return DBProvider.CreateCommand();
        }

        public DbCommand CreateDbCmd(DbProviderFactory dbProvider)
        {
            return dbProvider.CreateCommand();
        }
        #endregion

        #region CreateDbAdapter
        public DbDataAdapter CreateDbAdapter()
        {
            return DBProvider.CreateDataAdapter();
        }

        public DbDataAdapter CreateDbAdapter(DbProviderFactory dbProvider)
        {
            return dbProvider.CreateDataAdapter();
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql)
        {
            object obj = null;
            DbCommand cmd = CreateDbCmd();
            cmd.CommandText = sql;
            obj = ExecuteScalar(cmd);
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public object ExecuteScalar(DbCommand cmd)
        {
            object obj = null;
            DbConnection conn = CreateDbConn();

            cmd.Connection = conn;
            obj = ExecuteScalar(cmd, conn);
            return obj;
        }

        /// <summary>
        /// 允许 跨库执行sql语句
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public object ExecuteScalar(DbCommand cmd, DbConnection conn)
        {
            object obj = null;
            try
            {
                cmd.Connection = conn;
                conn.Open();
                obj = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return obj;
        }

        public async Task<object> ExecuteScalarAsync(string sql)
        {
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            return await ExecuteScalarAsync(cmd);
        }

        public async Task<object> ExecuteScalarAsync(DbCommand cmd)
        {
            object obj = null;
            using (DbConnection conn = CreateDbConn())
            {
                conn.Open();
                cmd.Connection = conn;
                obj = await cmd.ExecuteScalarAsync();
            }
            return obj;
        }

        #endregion

        #region ExecuteScalarPresistent
        public object ExecuteScalarPresistent(string sql)
        {
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            return ExecuteScalarPresistent(cmd);
        }

        public object ExecuteScalarPresistent(DbCommand cmd)
        {
            object obj = null;
            cmd.Connection = Conn;
            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                obj = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent!=null)
                {
                    exceptionNoticeEvent(ex);
                }
            }

            return obj;
        }
        #endregion

        #region ExecuteNonQuery
        public int ExecuteNonQuery(string sql)
        {
            int result = 0;
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            result = ExecuteNonQuery(cmd);
            return result;
        }

        public int ExecuteNonQuery(DbCommand cmd)
        {
            int result = 0;
            var conn = CreateDbConn();
            cmd.Connection = conn;
            result = ExecuteNonQuery(cmd, conn);
            return result;
        }

        /// <summary>
        /// 允许跨库执行非sql语句
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbCommand cmd, DbConnection conn)
        {
            int result = 0;
            try
            {
                conn.Open();
                cmd.Connection = conn;
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }
        #endregion

        #region ExecuteNonQueryPresistent
        public int ExecuteNonQueryPresistent(string sql)
        {
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            return ExecuteNonQueryPresistent(cmd);
        }

        public int ExecuteNonQueryPresistent(DbCommand cmd)
        {
            int result = 0;
            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                cmd.Connection = Conn;
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                cmd.Dispose();
            }
            return result;
        }
        #endregion

        #region ExecuteNonQueryAsync
        public async Task<int> ExecuteNonQueryAsync(string sql)
        {
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            return await ExecuteNonQueryAsync(cmd);
        }

        public async Task<int> ExecuteNonQueryAsync(DbCommand cmd)
        {
            var conn = CreateDbConn();
            cmd.Connection = conn;
            return await ExecuteNonQueryAsync(cmd, conn);
        }

        public async Task<int> ExecuteNonQueryAsync(DbCommand cmd, DbConnection conn)
        {
            int result = 0;
            try
            {
                conn.Open();
                cmd.Connection = conn;
                result = await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }
        #endregion

        #region Select
        public DataTable Select(string sql)
        {
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            return Select(cmd);
        }

        public List<T> Select<T>(string sql) where T : class, new()
        {
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            return Select<T>(cmd);
        }

        /// <summary>
        /// 通过Adapter进行select操作
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public DataTable Select(DbCommand cmd)
        {
            DataTable result = new DataTable();
            var adapter = CreateDbAdapter();
            try
            {
                var conn = CreateDbConn();
                cmd.Connection = conn;
                adapter.SelectCommand = cmd;
                adapter.Fill(result);
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                cmd.Dispose();
                adapter.Dispose();
            }
            return result;
        }

        public List<T> Select<T>(DbCommand cmd) where T :class,new()
        {
            var conn = CreateDbConn();
            return Select<T>(cmd, conn);
        }

        /// <summary>
        /// 使用dataReader进行select 操作
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public DataTable Select(DbCommand cmd, DbConnection conn)
        {
            DataTable result = new DataTable();
            try
            {
                cmd.Connection = conn;
                conn.Open();
                DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new DataTable();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        DataColumn col = new DataColumn(reader.GetName(i));
                        result.Columns.Add(col);
                    }
                    while (reader.Read())
                    {
                        DataRow row = result.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader[i];
                        }
                        result.Rows.Add(row);
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }

        public List<T> Select<T>(DbCommand cmd, DbConnection conn) where T : class, new()
        {
            List<T> result = new List<T>();
            try
            {
                cmd.Connection = conn;
                conn.Open();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            T t = new T();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string fieldName = reader.GetName(i);
                                var pi = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                                var setMethod = EntityReflection<T>.CreateSetDelegate(t, pi);

                                object item = reader[i];

                                setMethod.DynamicInvoke(t, item);
                            }
                            result.Add(t);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 可以同时 查询 多个数据库(主要用于对一个数据库的 多个链接，查询操作)
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public DataTable Select(DbDataAdapter adapter)
        {
            var conn = CreateDbConn();
            return Select(adapter, conn);
        }

        /// <summary>
        /// 可以同时 查询 多个数据库(主要用于对多个数据库的 多个链接，查询操作)
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public DataTable Select(DbDataAdapter adapter, DbConnection conn)
        {
            DataTable result = new DataTable();
            try
            {
                adapter.SelectCommand.Connection = conn;
                adapter.Fill(result);
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                adapter.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }
        #endregion

        #region SelectAsync
        public async Task<DataTable> SelectAsync(string sql)
        {
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            return await SelectAsync(cmd);
        }

        public async Task<List<T>> SelectAsync<T>(string sql) where T :class,new()
        {
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            return await SelectAsync<T>(cmd);
        }

        public async Task<DataTable> SelectAsync(DbCommand cmd)
        {
            DataTable result = new DataTable();
            try
            {
                using (var conn = CreateDbConn())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    var dataReader = await cmd.ExecuteReaderAsync();

                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        DataColumn col = new DataColumn();
                        col.DataType = dataReader.GetFieldType(i);
                        result.Columns.Add(col);
                    }

                    bool state = await dataReader.ReadAsync();
                    while (state)
                    {
                        var row = result.NewRow();
                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            row[i] = dataReader[i];
                        }
                        result.Rows.Add(row);
                        state = await dataReader.ReadAsync();
                    }

                    dataReader.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            return result;
        }

        public async Task<List<T>> SelectAsync<T>(DbCommand cmd) where T : class, new()
        {
            List<T> result = new List<T>();
            try
            {
                using (var conn = CreateDbConn())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    var dataReader = await cmd.ExecuteReaderAsync();
                    bool state = await dataReader.ReadAsync();
                    while (state)
                    {
                        T t = new T();
                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            string fieldName = dataReader.GetName(i);
                            var pi = typeof(T).GetProperty(fieldName);
                            var setMethod = EntityReflection<T>.CreateSetDelegate(t, pi);
                            var item = dataReader[i];
                            setMethod.DynamicInvoke(t, item);
                        }
                        result.Add(t);
                        state = await dataReader.ReadAsync();
                    }
                    dataReader.Close();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }

            return result;
        }
        #endregion

        #region SelectPresistent
        public DataTable SelectPresistent(string sql)
        {
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            return SelectPresistent(cmd);
        }

        public List<T> SelectPresistent<T>(string sql) where T : class, new()
        {
            var cmd = CreateDbCmd();
            cmd.CommandText = sql;
            return SelectPresistent<T>(cmd);
        }

        public DataTable SelectPresistent(DbCommand cmd)
        {
            DataTable result = new DataTable();
            try
            {
                cmd.Connection = Conn;
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                DbDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new DataTable();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        DataColumn col = new DataColumn(reader.GetName(i));
                        result.Columns.Add(col);
                    }
                    while (reader.Read())
                    {
                        DataRow row = result.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader[i];
                        }
                        result.Rows.Add(row);
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                cmd.Dispose();
            }
            return result;
        }

        public List<T> SelectPresistent<T>(DbCommand cmd) where T : class, new()
        {
            List<T> result = new List<T>();
            try
            {
                cmd.Connection = Conn;
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            T t = new T();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string fieldName = reader.GetName(i);
                                var pi = typeof(T).GetProperty(fieldName);
                                var setMethod = EntityReflection<T>.CreateSetDelegate(t, pi);
                                var item = reader[i];
                                setMethod.DynamicInvoke(t, item);
                            }
                            result.Add(t);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                cmd.Dispose();
                Conn.Close();
            }
            return result;
        }
        #endregion
        
        #region BatExecute
        public int BatExecute(DataSet dataSet, DbCommand cmd)
        {
            int result = 0;
            try
            {
                using (var conn = CreateDbConn())
                {
                    using (var adapter = CreateDbAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataSet);
                        adapter.InsertCommand = CmdBuilder.GetInsertCommand();
                        adapter.DeleteCommand = CmdBuilder.GetDeleteCommand();
                        adapter.UpdateCommand = CmdBuilder.GetUpdateCommand();
                        conn.Open();
                        result = adapter.Update(dataSet);
                    }
                }
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                cmd.Dispose();
                dataSet.Dispose();
            }

            return result;
        }

        public int BatExecute(DataSet dataSet)
        {
            int result = 0;
            try
            {
                using (var conn = CreateDbConn())
                {
                    using (var adapter = CreateDbAdapter())
                    {
                        adapter.Fill(dataSet);
                        adapter.InsertCommand = CmdBuilder.GetInsertCommand();
                        adapter.DeleteCommand = CmdBuilder.GetDeleteCommand();
                        adapter.UpdateCommand = CmdBuilder.GetUpdateCommand();
                        conn.Open();
                        result = adapter.Update(dataSet);
                    }
                }
            }
            catch (Exception ex)
            {
                if (exceptionNoticeEvent != null)
                {
                    exceptionNoticeEvent(ex);
                }
            }
            finally
            {
                dataSet.Dispose();
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 单数据库 事务
        /// </summary>
        /// <param name="cmd"></param>
        public void SeriesTransaction(List<DbCommand> cmds)
        {
            using (var conn = CreateDbConn())
            {
                try
                {
                    var trans = conn.BeginTransaction();
                    using (TransactionScope scope = new TransactionScope())
                    {
                        foreach (var cmd in cmds)
                        {
                            if (ExecuteNonQuery(cmd,conn) <= 0)
                            {
                                throw new ArgumentException();
                            }
                        }
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    if (exceptionNoticeEvent != null)
                    {
                        exceptionNoticeEvent(ex);
                    }
                }
            }
        }

        public void CloseDB()
        {
            if (Conn.State!=ConnectionState.Closed)
            {
                Conn.Close();
            }
        }

        //高效批量插入:System.Data.SqlClient.SqlBulkCopy  [BCP]

        //TODO 高效 插入,更新 操作

        //TODO 对分区的 支持？

        //易扩展、易维护、易测试(灵活)

        //延迟查询

        //性能优化
    }
}
