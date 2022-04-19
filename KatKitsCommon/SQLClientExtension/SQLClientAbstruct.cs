
namespace KatKits.SQLClientExtension {
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.Common;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Threading.Tasks;

  public abstract class SQLClientAbstract<T>:IAsyncDisposable where T : DbConnection {
    protected readonly T Connection;
    //protected readonly string ConnectionString;
    public SQLClientAbstract(DbConnection Connection) {
      this.Connection = Connection as T;
      Connection.Open();
    }
    public virtual int TimedOutCal(int RowCount, decimal CalculationCost = 0M, decimal PingDualDir = 200, decimal Per = 5M, decimal Scale = 1.5M)
=> Math.Max((int)Math.Ceiling((RowCount * Per + CalculationCost + PingDualDir) * Scale / 1000m), 30);


    public abstract void InsertTableToDB(string TableName, DataTable Table, int TimedOut = 30);
    public abstract DataTable QueryDataTable(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30);
    public abstract object QueryScalar(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30);
    public abstract TValue QueryScalar<TValue>(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30);
    public abstract int Execute(string CommandText, CommandType CommandType, object Paramaters = null, bool WithTransaction = false, int TimedOut = 30);

   
    public virtual ValueTask DisposeAsync() {
       return new ValueTask(this.Connection.CloseAsync().ContinueWith((Tk)=> this.Connection.Dispose()));
    }
  }


}
