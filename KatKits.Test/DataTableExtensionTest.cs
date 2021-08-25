namespace KatKits.Test
{
  using Newtonsoft.Json;

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Diagnostics;
  using System.Linq;
  using System.Text;

  using Xunit;
  

  public class DataTableExtensionTest
  {
    //[Fact]
    //public void OrderedEqualTest()
    //{
    //  var L = new Type[] { typeof(string), typeof(int), typeof(decimal), };
    //  var R = new Type[] { typeof(string), typeof(int), typeof(decimal), };
    //  Assert.True(L.OrderedEqual(R));
    //}
    [Fact]
    public void FindIndexProperty() {
      var P = typeof(DataRow).FindIndexers("Item",typeof(string), typeof(object)).ToArray();
      Assert.True(P.Count() > 0);
    }
    [Theory]
    [InlineData("[]")]
    [InlineData("[{\"BOOL\":true,\"INT\":0,\"LONG\":0,\"SBYTE\":0,\"SHORT\":0,\"UINT\":0,\"ULONG\":0,\"BYTE\":0,\"USHORT\":0,\"SINGLE\":0.0,\"DOUBLE\":0.0,\"DECIMAL\":0.0,\"DATETIME\":\"1753-01-01T00:00:00\",\"GUID\":\"00000000-0000-0000-0000-000000000000\",\"TIMESPAN\":\"00:00:00\",\"DATETIMEOFFSET\":\"1753-01-01T00:00:00+08:00\",\"NBBOOL\":null,\"NBINT\":null,\"NBLONG\":null,\"NBSBYTE\":null,\"NBSHORT\":null,\"NBUINT\":null,\"NBULONG\":null,\"NBBYTE\":null,\"NBUSHORT\":null,\"NBSINGLE\":null,\"NBDOUBLE\":null,\"NBDECIMAL\":null,\"NBDATETIME\":null,\"NBGUID\":null,\"NBTIMESPAN\":null,\"NBDATETIMEOFFSET\":null,\"STRING\":\"\"}]")]
    public void DataTable2Enumerable(string JText)
    {
      var DT = JsonConvert.DeserializeObject<IEnumerable<TestClass>>(JText);
      var Tab = KatKits.EnumerableToDataTable<TestClass>(DT);
      var Ins = KatKits.AsEnumerable<TestClass>(Tab).ToArray();
      Assert.Equal(JsonConvert.SerializeObject(DT), JsonConvert.SerializeObject(Ins));
    }
    [Fact]
    public void CusDataTable2Enumerable() {
      var Tab = typeof(TestBoolIntClass).TypeToDataTable();
      Tab.Columns["This is Boolean"].DataType = typeof(int);
      Tab.Columns["This is Nullable Boolean"].DataType = typeof(int);
      Tab.Rows.Add(1,1);
      Tab.Rows.Add(null,0);
      var Ins = KatKits.AsEnumerable<TestBoolIntClass>(Tab).ToArray();
      Assert.NotNull(JsonConvert.SerializeObject(Ins));
    }
    [Fact]
    public void TaskJson() {
      Debug.WriteLine(JsonConvert.SerializeObject(new TestClass[] { new TestClass() }));
      Assert.Null(JsonConvert.SerializeObject(new TestClass[] { new TestClass() }));
    }
    //public void Enumerable2DataTable()
    //{

    //}

  }
  public class TestBoolIntClass {
    [ColumnMap(TableColumnName = "This is Boolean", TableColumnOrder = -1, AllowNull = false, DefaultValue = false)]
    public bool BOOL { get; set; } = false;
    [ColumnMap(TableColumnName = "This is Nullable Boolean", TableColumnOrder = -2)]
    public bool? NBBOOL { get; set; } = null;
  }
  public class TestClass
  {
    [ColumnMap(TableColumnName = "This is Boolean", TableColumnOrder = -1, AllowNull = false, DefaultValue = false)]
    public bool BOOL { get; set; } = false;
    [ColumnMap(TableColumnName = "This is Int32", TableColumnOrder = 3, AllowNull = false, DefaultValue = 0)]
    public int INT { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is Int64", TableColumnOrder = 4, AllowNull = false, DefaultValue = 0)]
    public long LONG { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is INT8", TableColumnOrder = 1, AllowNull = false, DefaultValue = 0)]
    public sbyte SBYTE { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is INT16", TableColumnOrder = 2, AllowNull = false, DefaultValue = 0)]
    public short SHORT { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is UInt32", TableColumnOrder = 5, AllowNull = false, DefaultValue = 0)]
    public uint UINT { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is UInt64", TableColumnOrder = 6, AllowNull = false, DefaultValue = 0)]
    public ulong ULONG { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is UINT8", TableColumnOrder = 7, AllowNull = false, DefaultValue = 0)]
    public byte BYTE { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is UINT16", TableColumnOrder = 8, AllowNull = false, DefaultValue = 0)]
    public short USHORT { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is SINGLE", TableColumnOrder = 9, AllowNull = false, DefaultValue = 0)]
    public float SINGLE { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is DOUBLE", TableColumnOrder = 10, AllowNull = false, DefaultValue = 0)]
    public double DOUBLE { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is DECIMAL", TableColumnOrder = 10, AllowNull = false, DefaultValue = 0)]
    public decimal DECIMAL { get; set; } = 0;
    [ColumnMap(TableColumnName = "This is DATETIME", TableColumnOrder = 10, AllowNull = false)]
    public DateTime DATETIME { get; set; } = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
    [ColumnMap(TableColumnName = "This is GUID", TableColumnOrder = 10, AllowNull = false)]
    public Guid GUID { get; set; } = Guid.Empty;
    [ColumnMap(TableColumnName = "This is TIMESPAN", TableColumnOrder = 10, AllowNull = false)]
    public TimeSpan TIMESPAN { get; set; } = TimeSpan.Zero;
    [ColumnMap(TableColumnName = "This is DATETIMEOFFSET", TableColumnOrder = 10, AllowNull = false)]
    public DateTimeOffset DATETIMEOFFSET { get; set; } = new DateTimeOffset(System.Data.SqlTypes.SqlDateTime.MinValue.Value);

    [ColumnMap(TableColumnName = "This is Nullable Boolean", TableColumnOrder = -2)]
    public bool? NBBOOL { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable Int32", TableColumnOrder = 3)]
    public int? NBINT { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable Int64", TableColumnOrder = 4)]
    public long? NBLONG { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable INT8", TableColumnOrder = 1)]
    public sbyte? NBSBYTE { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable INT16", TableColumnOrder = 2)]
    public short? NBSHORT { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable UInt32", TableColumnOrder = 5)]
    public uint? NBUINT { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable UInt64", TableColumnOrder = 6)]
    public ulong? NBULONG { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable UINT8", TableColumnOrder = 7)]
    public byte? NBBYTE { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable UINT16", TableColumnOrder = 8)]
    public short? NBUSHORT { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable SINGLE", TableColumnOrder = 9)]
    public float? NBSINGLE { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable DOUBLE", TableColumnOrder = 10)]
    public double? NBDOUBLE { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable DECIMAL", TableColumnOrder = 10)]
    public decimal? NBDECIMAL { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable DATETIME", TableColumnOrder = 10)]
    public DateTime? NBDATETIME { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable GUID", TableColumnOrder = 10)]
    public Guid? NBGUID { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable TIMESPAN", TableColumnOrder = 10)]
    public TimeSpan? NBTIMESPAN { get; set; } = null;
    [ColumnMap(TableColumnName = "This is Nullable DATETIMEOFFSET", TableColumnOrder = 10)]
    public DateTimeOffset? NBDATETIMEOFFSET { get; set; } = null;

    [ColumnMap(TableColumnName = "This is STRING", TableColumnOrder = 10, AllowNull = false, DefaultValue = "")]
    public string STRING { get; set; } = "";
  }
}
