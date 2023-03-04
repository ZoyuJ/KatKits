
namespace KatKits.DB
{
    using Utility.Kits;

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using KatKits;
    using System.Linq.Expressions;

    public class SQLServerDBAccess : IDisposable
    {
        //internal static readonly DBConnectionPool Connections = new DBConnectionPool();
        protected readonly string ConnectionString;
        protected readonly SqlConnection Connection;
        public SQLServerDBAccess(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            Connection = new SqlConnection(ConnectionString);
            Connection.Open();
        }

        public SQLServerDBAccess(SqlConnection Connection)
        {
            this.Connection = Connection;
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public async Task InsertTableToDBAsync(string TableName, DataTable Table, int TimedOut = 30)
        {
            using (var bulk = new SqlBulkCopy(Connection))
            {
                bulk.DestinationTableName = TableName ?? Table.TableName;
                await bulk.WriteToServerAsync(Table);
            }
        }
        public void InsertTableToDB(string TableName, DataTable Table, int TimedOut = 30)
        {
            using (var bulk = new SqlBulkCopy(Connection))
            {
                bulk.DestinationTableName = TableName ?? Table.TableName;
                bulk.WriteToServer(Table);
            }
        }

        public async Task<DataTable> QueryDataTableAsync(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30)
        {

            using (var Command = Connection.CreateCommand())
            {
                if (WithTransaction)
                {
                    Command.Transaction = Connection.BeginTransaction();
                }
                Command.CommandText = CommandText;
                Command.CommandType = CommandType;
                Command.CommandTimeout = TimedOut;
                if (Paramaters != null) Command.FillParamaters(Paramaters);
                var Data = await Command.ExecuteReaderAsync();
                var Table = new DataTable();
                Table.Load(Data);
                try
                {
                    Command.Transaction?.Commit();
                    return Table;
                }
                catch (Exception E)
                {
                    try
                    {
                        Command.Transaction.Rollback();
                        throw E;
                    }
                    catch (Exception E2)
                    {
                        throw new DBAccessRollbackException(E, E2);
                    }
                }
            }
        }
        public DataTable QueryDataTable(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30)
        {
            using (var Command = Connection.CreateCommand())
            {
                if (WithTransaction)
                {
                    Command.Transaction = Connection.BeginTransaction();
                }
                Command.CommandText = CommandText;
                Command.CommandType = CommandType;
                Command.CommandTimeout = TimedOut;
                if (Paramaters != null) Command.FillParamaters(Paramaters);
                var Data = Command.ExecuteReader();
                var Table = new DataTable();
                Table.Load(Data);
                try
                {
                    Command.Transaction?.Commit();
                    return Table;
                }
                catch (Exception E)
                {
                    try
                    {
                        Command.Transaction.Rollback();
                        throw E;
                    }
                    catch (Exception E2)
                    {
                        throw new DBAccessRollbackException(E, E2);
                    }
                }
            }
        }

        public async Task<T> QueryScalarAsync<T>(string CommandText, CommandType CommandType, object Paramaters = null,bool WithTransaction=false, int TimedOut = 30)
        {

            using (var Command = Connection.CreateCommand())
            {
                if (WithTransaction) {
                    Command.Transaction = Connection.BeginTransaction();
                }
                Command.CommandText = CommandText;
                Command.CommandType = CommandType;
                Command.CommandTimeout = TimedOut;
                if (Paramaters != null) Command.FillParamaters(Paramaters);
                var Resp = (T)(await Command.ExecuteScalarAsync());
                try
                {
                    Command.Transaction?.Commit();
                    return Resp;
                }
                catch (Exception E)
                {
                    try
                    {
                        Command.Transaction.Rollback();
                        throw E;
                    }
                    catch (Exception E2)
                    {
                        throw new DBAccessRollbackException(E,E2);
                    }
                }
            }

        }
        public object QueryScalar(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30)
        {
            using (var Command = Connection.CreateCommand())
            {
                if (WithTransaction)
                {
                    Command.Transaction = Connection.BeginTransaction();
                }
                Command.CommandText = CommandText;
                Command.CommandType = CommandType;
                Command.CommandTimeout = TimedOut;
                if (Paramaters != null) Command.FillParamaters(Paramaters);
                var Resp = Command.ExecuteScalar();
                try
                {
                    Command.Transaction?.Commit();
                    return Resp;
                }
                catch (Exception E)
                {
                    try
                    {
                        Command.Transaction.Rollback();
                        throw E;
                    }
                    catch (Exception E2)
                    {
                        throw new DBAccessRollbackException(E, E2);
                    }
                }
            }
        }
        public T QueryScalar<T>(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30)
        {
            var R = QueryScalar(CommandText, CommandType, Paramaters, WithTransaction, TimedOut);
            return (T)R;
        }
        public async Task<object> QueryScalarAsync(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30)
        {

            using (var Command = Connection.CreateCommand())
            {
                if (WithTransaction)
                {
                    Command.Transaction = Connection.BeginTransaction();
                }
                Command.CommandText = CommandText;
                Command.CommandType = CommandType;
                Command.CommandTimeout = TimedOut;
                if (Paramaters != null) Command.FillParamaters(Paramaters);
                var Resp = await Command.ExecuteScalarAsync();
                try
                {
                    Command.Transaction?.Commit();
                    return Resp;
                }
                catch (Exception E)
                {
                    try
                    {
                        Command.Transaction.Rollback();
                        throw E;
                    }
                    catch (Exception E2)
                    {
                        throw new DBAccessRollbackException(E, E2);
                    }
                }
            }

        }

        public async Task<int> ExecuteAsync(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30)
        {
            using (var Command = Connection.CreateCommand())
            {
                if (WithTransaction)
                {
                    Command.Transaction = Connection.BeginTransaction();
                }
                Command.CommandText = CommandText;
                Command.CommandType = CommandType;
                Command.CommandTimeout = TimedOut;
                if (Paramaters != null) Command.FillParamaters(Paramaters);
                var Resp = await Command.ExecuteNonQueryAsync();
                try
                {
                    Command.Transaction?.Commit();
                    return Resp;
                }
                catch (Exception E)
                {
                    try
                    {
                        Command.Transaction.Rollback();
                        throw E;
                    }
                    catch (Exception E2)
                    {
                        throw new DBAccessRollbackException(E, E2);
                    }
                }
            }
        }
        public int Execute(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30)
        {
            using (var Command = Connection.CreateCommand())
            {
                if (WithTransaction)
                {
                    Command.Transaction = Connection.BeginTransaction();
                }
                Command.CommandText = CommandText;
                Command.CommandType = CommandType;
                Command.CommandTimeout = TimedOut;
                if (Paramaters != null) Command.FillParamaters(Paramaters);
                var Resp = Command.ExecuteNonQuery();
                try
                {
                    Command.Transaction?.Commit();
                    return Resp;
                }
                catch (Exception E)
                {
                    try
                    {
                        Command.Transaction.Rollback();
                        throw E;
                    }
                    catch (Exception E2)
                    {
                        throw new DBAccessRollbackException(E, E2);
                    }
                }
            }
        }


        public void Dispose()
        {
            this.Connection.Close();
            this.Connection.Dispose();
        }






    }

    public static class SQLServerDBAccessUtil
    {
        public static readonly Dictionary<Type, Action<SqlCommand, object>> SQLServerParamatersFillersCache = new Dictionary<Type, Action<SqlCommand, object>>();
        public static void FillParamaters(this SqlCommand Command, object Object)
        {
            if (Object == null) return;
            var Tp = Object.GetType();
            Action<SqlCommand, object> CreateFunction()
            {
                var Attrs = QueryParamaterAttribute.FetchAttributes(Object.GetType());
                var InputPara = Expression.Parameter(typeof(object), "Input");
                var TypedInputPara = Expression.Convert(InputPara, Object.GetType());
                var InputCommand = Expression.Parameter(typeof(SqlCommand), "Command");
                var Para1Var = Expression.Variable(typeof(SqlParameter), "SQLPara1");
                var Body = new List<Expression>();
                Attrs.ToArray().ForEach(E =>
                {

                    Body.Add(Expression.IfThenElse(Expression.Equal(Expression.Convert(Expression.Property(TypedInputPara, E.Property), typeof(object)), Expression.Constant(null)),
                          Expression.Assign(
                      Para1Var,
                      Expression.Call(
                        Expression.Property(
                          InputCommand,
                          typeof(SqlCommand).GetProperties().FirstOrDefault(P => P.PropertyType == typeof(SqlParameterCollection) && P.Name == nameof(SqlCommand.Parameters))
                        ),
                        typeof(SqlParameterCollection).GetMethod(nameof(SqlParameterCollection.AddWithValue)),
                        Expression.Constant(E.ParamaterName),
                        Expression.Convert(Expression.Constant(DBNull.Value), typeof(object))
                      )
                    ),
                        Expression.Assign(
                      Para1Var,
                      Expression.Call(
                        Expression.Property(
                          InputCommand,
                          typeof(SqlCommand).GetProperties().FirstOrDefault(P => P.PropertyType == typeof(SqlParameterCollection) && P.Name == nameof(SqlCommand.Parameters))
                        ),
                        typeof(SqlParameterCollection).GetMethod(nameof(SqlParameterCollection.AddWithValue)),
                        Expression.Constant(E.ParamaterName),
                        Expression.Convert(Expression.Property(TypedInputPara, E.Property), typeof(object))
                      )
                    )
                        )
                        );
                    if (E.Property.PropertyType.Equals(typeof(System.Data.DataTable)))
                    {
                        Body.Add(
                          Expression.Assign(
                            Expression.Property(
                              Para1Var,
                              typeof(SqlParameter).GetProperty(nameof(SqlParameter.SqlDbType))
                            ),
                            Expression.Constant(SqlDbType.Structured)
                          )
                        );
                        if (E.TableName != null)
                            Body.Add(
                              Expression.Assign(
                                Expression.Property(
                                  Para1Var,
                                  typeof(SqlParameter).GetProperty(nameof(SqlParameter.TypeName))
                                ),
                                Expression.Constant(E.TableName)
                              )
                            );
                    }

                }

                );

                var Lam = Expression.Lambda<Action<SqlCommand, object>>(
                  Expression.Block(
                    new ParameterExpression[] { Para1Var }, Body
                  ),
                  new ParameterExpression[] { InputCommand, InputPara }
                );
                return Lam.Compile();
            }
            if (SQLServerParamatersFillersCache.TryGetValue(Tp, out var Act))
            {
                Act(Command, Object);
            }
            else
            {
                Act = CreateFunction();
                Act(Command, Object);
                lock (SQLServerParamatersFillersCache)
                {
                    SQLServerParamatersFillersCache.TryAdd(Tp, Act);
                }
            }

        }
    }



}
