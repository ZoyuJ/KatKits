

namespace KatKits.ADO_MSSQL
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Data.SqlClient;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Reflection;
  using System.Diagnostics;
  using System.ComponentModel.DataAnnotations.Schema;

  public class DBAccess : IDisposable
  {
    protected readonly SqlConnection Connection;
    //protected readonly CancellationTokenSource CTkS;
    protected DBAccess(string ConnectionString)
    {
      Connection = new SqlConnection(ConnectionString);
      Connection.Open();
    }
    protected DBAccess(DBAccess Outer, string TransactionName = null)
    {
      this.Connection = Outer.Connection;
      this.Outer = Outer;
      this.Transaction = Connection.BeginTransaction(TransactionName);
    }

    protected DBAccess Inner;
    protected readonly DBAccess Outer;

    public static DBAccess Create(string ConnectionString)
    {
      return new DBAccess(ConnectionString);
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

    public async Task<DataTable> QueryDataTableAsync(string CommandText, CommandType CommandType, object Paramaters = null, int TimedOut = 30)
    {
      using (var Command = Connection.CreateCommand())
      {
        if (Transaction != null) Command.Transaction = Transaction;
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        var Data = await Command.ExecuteReaderAsync();
        var Table = new DataTable();
        Table.Load(Data);
        return Table;
      }
    }
    public DataTable QueryDataTable(string CommandText, CommandType CommandType, object Paramaters = null, int TimedOut = 30)
    {
      using (var Command = Connection.CreateCommand())
      {
        if (Transaction != null) Command.Transaction = Transaction;
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        var Data = Command.ExecuteReader();
        var Table = new DataTable();
        Table.Load(Data);
        return Table;
      }
    }

    public async Task<T> QueryScalarAsync<T>(string CommandText, CommandType CommandType, object Paramaters = null, int TimedOut = 30)
    {
      using (var Command = Connection.CreateCommand())
      {
        if (Transaction != null) Command.Transaction = Transaction;
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        return (T)(await Command.ExecuteScalarAsync());
      }
    }
    public T QueryScalar<T>(string CommandText, CommandType CommandType, object Paramaters = null, int TimedOut = 30)
    {
      using (var Command = Connection.CreateCommand())
      {
        if (Transaction != null) Command.Transaction = Transaction;
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        return (T)(Command.ExecuteScalar());
      }
    }
    public async Task<object> QueryScalarAsync(string CommandText, CommandType CommandType, object Paramaters = null, int TimedOut = 30)
    {
      using (var Command = Connection.CreateCommand())
      {
        if (Transaction != null) Command.Transaction = Transaction;
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        return await Command.ExecuteScalarAsync();
      }
    }
    public object QueryScalar(string CommandText, CommandType CommandType, object Paramaters = null, int TimedOut = 30)
    {
      using (var Command = Connection.CreateCommand())
      {
        if (Transaction != null) Command.Transaction = Transaction;
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        return Command.ExecuteScalar();
      }
    }

    public async Task<int> QueryAsync(string CommandText, CommandType CommandType, object Paramaters = null, int TimedOut = 30)
    {
      using (var Command = Connection.CreateCommand())
      {
        if (Transaction != null) Command.Transaction = Transaction;
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        return await Command.ExecuteNonQueryAsync();
      }
    }
    public int Query(string CommandText, CommandType CommandType, object Paramaters = null, int TimedOut = 30)
    {
      using (var Command = Connection.CreateCommand())
      {
        if (Transaction != null) Command.Transaction = Transaction;
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        return Command.ExecuteNonQuery();
      }
    }

    protected readonly SqlTransaction Transaction;
    public DBAccess BeginTransaction(string TransactionName = null)
    {
      Inner = new DBAccess(this, TransactionName);
      return Inner;
    }
    protected bool TransCommited = false;
    public bool FinishTransaction(out DBAccess Outer)
    {
      Outer = this.Outer;
      if (Inner != null)
      {
        Inner.FinishTransaction(out _);
      }
      if (Transaction != null)
      {
        try
        {
          if (!TransCommited)
          {
            Transaction.Commit();
            TransCommited = true;
          }
          return true;
        }
        catch
        {
          try
          {
            Transaction.Rollback();
          }
          catch (Exception EE)
          {
            Debug.WriteLine($"Error At Transaction {Transaction}");
            Debug.WriteLine(EE);
            throw;
          }
        }
      }
      return false;
    }
    public DBAccess CancelTransaction()
    {
      if (Transaction != null)
      {
        Transaction.Dispose();
      }
      return Outer;
    }

    public void Dispose()
    {
      Inner?.Dispose();
      try
      {
        FinishTransaction(out _);
      }
      catch (ObjectDisposedException) { }
      catch (Exception E)
      {
        Debug.WriteLine("Failed On Committing Transaction");
        Debug.WriteLine(E);
      }
      if (Connection.State == System.Data.ConnectionState.Closed) Connection.Close();
      try
      {
        Connection.Dispose();
      }
      catch (ObjectDisposedException)
      { }
    }

  }

  //[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  //public sealed class TableNameAttribute : Attribute
  //{
  //  public TableNameAttribute(string TableName) {
  //    this.TableName = TableName;
  //  }
  //  public string TableName { get; private set; }
  //}





}
//namespace KatKits
//{
//  using System;
//  using System.Collections.Generic;
//  using System.Data;
//  using System.Data.SqlClient;
//  using System.Linq;
//  using System.Linq.Expressions;
//  using System.Reflection;

//  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
//  public sealed class QueryParamaterAttribute : Attribute
//  {
    
//    public string TableName { get; set; }
//    public string ParamaterName { get; set; }
//    //public int? Size { get; set; }
//    public PropertyInfo Property { get; private set; }
//    public static IEnumerable<QueryParamaterAttribute> FetchAttributes<T>() => FetchAttributes(typeof(T));
//    public static IEnumerable<QueryParamaterAttribute> FetchAttributes(Type Type)
//    {

//      if (Attributes.TryGetValue(Type, out var Attrs))
//      {
//        return Attrs;
//      }
//      else
//      {
//        //var TableName = (Type.GetCustomAttribute<TableNameAttribute>()?.TableName) ?? (Type.GetCustomAttribute<TableAttribute>()?.Name);
//        var AttrsA = Type.GetProperties()
//             .Where(E => E.PropertyType.IsBasicDataType() || E.PropertyType.Equals(typeof(DataTable)))
//             .Select(E =>
//             {
//               var A = E.GetCustomAttribute<QueryParamaterAttribute>();
//               if (A != null)
//               {
//                 A.Property = E;
//                 if (A.ParamaterName == null) A.ParamaterName = A.Property.Name;
//               }
//               return A;
//             })
//             .Where(E => E != null).ToArray();
//        if (AttrsA.Length == 0)
//        {
//          AttrsA = Type.GetProperties()
//             .Where(E => E.PropertyType.IsBasicDataType() || E.PropertyType.Equals(typeof(DataTable)))
//             .Select(E => new QueryParamaterAttribute() { Property = E, ParamaterName = E.Name })
//             .ToArray();
//        }
//        Attributes.Add(Type, AttrsA);
//        return AttrsA;
//      }
//    }
//    private static readonly Dictionary<Type, IEnumerable<QueryParamaterAttribute>> Attributes = new Dictionary<Type, IEnumerable<QueryParamaterAttribute>>();

//  }
//  [Obsolete]
//  public enum SQLParameterScheme
//  {
//    Unknow = 0,
//    MSSQLServer = 1,
//    MySQL_INNODB = 2,
//    Orcale = 3,
//  }
//  public static partial class KatKits
//  {
//    private static readonly Dictionary<Type, Action<SqlCommand, object>> __AnonymousSQLParamatersFillersCache = new Dictionary<Type, Action<SqlCommand, object>>();
    
//    public static void FillMSSQLParamaters(this SqlCommand Command, object Object)
//    {
//      if (Object == null) return;
//      var Tp = Object.GetType();
//      Action<SqlCommand, object> CreateFunction()
//      {
//        var Attrs = QueryParamaterAttribute.FetchAttributes(Object.GetType());
//        var InputPara = Expression.Parameter(typeof(object), "Input");
//        var TypedInputPara = Expression.Convert(InputPara, Object.GetType());
//        var InputCommand = Expression.Parameter(typeof(SqlCommand), "Command");
//        var Para1Var = Expression.Variable(typeof(SqlParameter), "SQLPara1");
//        var Body = new List<Expression>();
//        Attrs.ToArray().ForEach(E =>
//        {

//          Body.Add(Expression.Assign(
//            Para1Var,
//            Expression.Call(
//              Expression.Property(
//                InputCommand,
//                typeof(SqlCommand).GetProperties().FirstOrDefault(P => P.PropertyType == typeof(SqlParameterCollection) && P.Name == nameof(SqlCommand.Parameters))
//              ),
//              typeof(SqlParameterCollection).GetMethod(nameof(SqlParameterCollection.AddWithValue)),
//              Expression.Constant(E.ParamaterName),
//              Expression.Convert(Expression.Property(TypedInputPara, E.Property), typeof(object))
//            )
//          ));
//          if (E.Property.PropertyType.Equals(typeof(System.Data.DataTable)))
//          {
//            Body.Add(
//              Expression.Assign(
//                Expression.Property(
//                  Para1Var,
//                  typeof(SqlParameter).GetProperty(nameof(SqlParameter.SqlDbType))
//                ),
//                Expression.Constant(SqlDbType.Structured)
//              )
//            );
//            if (E.TableName != null)
//              Body.Add(
//                Expression.Assign(
//                  Expression.Property(
//                    Para1Var,
//                    typeof(SqlParameter).GetProperty(nameof(SqlParameter.TypeName))
//                  ),
//                  Expression.Constant(E.TableName)
//                )
//              );
//          }

//        }

//        );

//        var Lam = Expression.Lambda<Action<SqlCommand, object>>(
//          Expression.Block(
//            new ParameterExpression[] { Para1Var }, Body
//          ),
//          new ParameterExpression[] { InputCommand, InputPara }
//        );
//        return Lam.Compile();
//      }
//      if (Tp.IsAnonymousType())
//      {
//        if (__AnonymousSQLParamatersFillersCache.TryGetValue(Tp, out var Act))
//        {
//          Act(Command, Object);
//        }
//        else
//        {
//          Act = CreateFunction();
//          Act(Command, Object);
//          __AnonymousSQLParamatersFillersCache.Add(Tp, Act);
//        }

//      }
//      else
//      {
//        if (!DataExchangeEntities.TryGetValue(Object.GetType(), out var Entity))
//        {
//          Entity = new DataExchangeCacheEntity();
//        }
//        if (Entity.Obj2SQLParamaters == null)
//          Entity.Obj2SQLParamaters = CreateFunction();
//        Entity.Obj2SQLParamaters(Command, Object);
//      }

//    }

//  }
//}
