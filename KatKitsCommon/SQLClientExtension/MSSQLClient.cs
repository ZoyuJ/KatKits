namespace KatKits.SQLClientExtension {
  using global::KatKits.SQLClientExtension.Exceptions;

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SqlClient;
  using System.Text;
  using System.Threading.Tasks;

  public class MSSQLClient : SQLClientAbstract<SqlConnection> {
    protected readonly string ConnectionString;
    public MSSQLClient(string ConnectionString) :base(new SqlConnection(ConnectionString)) {
      this.ConnectionString = ConnectionString;
    }

    public async Task InsertTableToDBAsync(string TableName, DataTable Table, int TimedOut = 30) {
      using (var bulk = new SqlBulkCopy(Connection)) {
        bulk.DestinationTableName = TableName ?? Table.TableName;
        await bulk.WriteToServerAsync(Table);
      }
    }
    public override void InsertTableToDB(string TableName, DataTable Table, int TimedOut = 30) {
      using (var bulk = new SqlBulkCopy(Connection)) {
        bulk.DestinationTableName = TableName ?? Table.TableName;
        bulk.WriteToServer(Table);
      }
    }

    public async Task<DataTable> QueryDataTableAsync(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30) {

      using (var Command = Connection.CreateCommand()) {
        if (WithTransaction) {
          Command.Transaction = Connection.BeginTransaction();
        }
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        var Data = await Command.ExecuteReaderAsync();
        var Table = new DataTable();
        Table.Load(Data);
        try {
          Command.Transaction?.Commit();
          return Table;
        }
        catch (Exception E) {
          try {
            Command.Transaction.Rollback();
            throw E;
          }
          catch (Exception E2) {
            throw new DBAccessRollbackException(E, E2);
          }
        }
      }
    }
    public override DataTable QueryDataTable(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30) {
      using (var Command = Connection.CreateCommand()) {
        if (WithTransaction) {
          Command.Transaction = Connection.BeginTransaction();
        }
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        var Data = Command.ExecuteReader();
        var Table = new DataTable();
        Table.Load(Data);
        try {
          Command.Transaction?.Commit();
          return Table;
        }
        catch (Exception E) {
          try {
            Command.Transaction.Rollback();
            throw E;
          }
          catch (Exception E2) {
            throw new DBAccessRollbackException(E, E2);
          }
        }
      }
    }

    public async Task<T> QueryScalarAsync<T>(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30) {

      using (var Command = Connection.CreateCommand()) {
        if (WithTransaction) {
          Command.Transaction = Connection.BeginTransaction();
        }
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        var Resp = (T)(await Command.ExecuteScalarAsync());
        try {
          Command.Transaction?.Commit();
          return Resp;
        }
        catch (Exception E) {
          try {
            Command.Transaction.Rollback();
            throw E;
          }
          catch (Exception E2) {
            throw new DBAccessRollbackException(E, E2);
          }
        }
      }

    }
    public override object QueryScalar(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30) {
      using (var Command = Connection.CreateCommand()) {
        if (WithTransaction) {
          Command.Transaction = Connection.BeginTransaction();
        }
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        var Resp = Command.ExecuteScalar();
        try {
          Command.Transaction?.Commit();
          return Resp;
        }
        catch (Exception E) {
          try {
            Command.Transaction.Rollback();
            throw E;
          }
          catch (Exception E2) {
            throw new DBAccessRollbackException(E, E2);
          }
        }
      }
    }
    public override T QueryScalar<T>(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30) {
      return (T)QueryScalar(CommandText, CommandType, Paramaters, WithTransaction, TimedOut);
    }
    public async Task<object> QueryScalarAsync(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30) {

      using (var Command = Connection.CreateCommand()) {
        if (WithTransaction) {
          Command.Transaction = Connection.BeginTransaction();
        }
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        var Resp = await Command.ExecuteScalarAsync();
        try {
          Command.Transaction?.Commit();
          return Resp;
        }
        catch (Exception E) {
          try {
            Command.Transaction.Rollback();
            throw E;
          }
          catch (Exception E2) {
            throw new DBAccessRollbackException(E, E2);
          }
        }
      }

    }

    public async Task<int> ExecuteAsync(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30) {
      using (var Command = Connection.CreateCommand()) {
        if (WithTransaction) {
          Command.Transaction = Connection.BeginTransaction();
        }
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        var Resp = await Command.ExecuteNonQueryAsync();
        try {
          Command.Transaction?.Commit();
          return Resp;
        }
        catch (Exception E) {
          try {
            Command.Transaction.Rollback();
            throw E;
          }
          catch (Exception E2) {
            throw new DBAccessRollbackException(E, E2);
          }
        }
      }
    }
    public override int Execute(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30) {
      using (var Command = Connection.CreateCommand()) {
        if (WithTransaction) {
          Command.Transaction = Connection.BeginTransaction();
        }
        Command.CommandText = CommandText;
        Command.CommandType = CommandType;
        Command.CommandTimeout = TimedOut;
        if (Paramaters != null) Command.FillMSSQLParamaters(Paramaters);
        var Resp = Command.ExecuteNonQuery();
        try {
          Command.Transaction?.Commit();
          return Resp;
        }
        catch (Exception E) {
          try {
            Command.Transaction.Rollback();
            throw E;
          }
          catch (Exception E2) {
            throw new DBAccessRollbackException(E, E2);
          }
        }
      }
    }

  }
}
