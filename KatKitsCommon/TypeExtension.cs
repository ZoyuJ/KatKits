﻿namespace KatKits {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.CompilerServices;
  using System.Text;

  //Type Check
  public static partial class KatKits {
    //public static DataTable ExcelToDataTable(FileInfo ExcelFile, string isHDR = "Yes")
    //{
    //  string conStr = "";
    //  switch (ExcelFile.Extension)
    //  {
    //    case ".xls": //Excel 97-03
    //      conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
    //      break;
    //    case ".xlsx": //Excel 07
    //      conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
    //      break;
    //  }
    //  conStr = String.Format(conStr, ExcelFile.FullName, isHDR);
    //  OleDbConnection connExcel = new OleDbConnection(conStr);
    //  OleDbCommand cmdExcel = new OleDbCommand();
    //  OleDbDataAdapter oda = new OleDbDataAdapter();
    //  DataTable dt = new DataTable();
    //  cmdExcel.Connection = connExcel;

    //  //Get the name of First Sheet
    //  connExcel.Open();
    //  DataTable dtExcelSchema;
    //  dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
    //  string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
    //  connExcel.Close();

    //  //Read Data from First Sheet
    //  connExcel.Open();
    //  cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
    //  oda.SelectCommand = cmdExcel;
    //  oda.Fill(dt);
    //  connExcel.Close();
    //  return dt;
    //}
    public static Type GetUnderlyingType(this Type This) {
      var Tp = This;
      if (This.IsNullableType()) {
        Tp = Nullable.GetUnderlyingType(This);
      }
      if (Tp.IsEnum) {
        return Enum.GetUnderlyingType(Tp);
      }
      return Tp;
    }
    public static bool IsBasicDataType(this Type This)
      => This == null
          ? false
          : This.IsPrimitive
            || This.IsEnum
            || This.Equals(typeof(DateTime))
            || This.Equals(typeof(string))
            || This.Equals(typeof(decimal))
            || This.Equals(typeof(Guid))
            || This.Equals(typeof(TimeSpan))
            || This.Equals(typeof(DateTimeOffset));
    public static bool _IsExpandDataType(this Type This) => This.Equals(typeof(DateTime?)) || This.Equals(typeof(Guid?)) || This.Equals(typeof(TimeSpan?)) || This.Equals(typeof(DateTimeOffset?));
    public static bool IsAnonymousType(this Type This) => Attribute.IsDefined(This, typeof(CompilerGeneratedAttribute), false)
                                                && This.IsGenericType && This.Name.Contains("AnonymousType")
                                                && (This.Name.StartsWith("<>") || This.Name.StartsWith("VB$"))
                                                && This.Attributes.HasFlag(TypeAttributes.NotPublic);
    public static bool IsNullableType(this Type This) => Nullable.GetUnderlyingType(This) != null;
    public static bool IsBasicDataTypeOrNullable(this Type This) => IsBasicDataType(Nullable.GetUnderlyingType(This)) || IsBasicDataType(This);
    private static readonly Dictionary<Type, object> __DEFAULT_DATATYPE_VALUE = new Dictionary<Type, object>() {
      { typeof(bool),false},
      { typeof(bool?),new bool?()},
      { typeof(int),0},
      { typeof(int?),new int?() },
      { typeof(long),0L},
      { typeof(long?),new long?()},
      { typeof(short),(short)0},
      { typeof(short?),new short?()},
      { typeof(byte),(byte)0},
      { typeof(byte?),new byte?()},
      { typeof(uint),(uint)0},
      { typeof(uint?),new uint?()},
      { typeof(ulong),(ulong)0},
      { typeof(ulong?),new ulong?()},
      { typeof(ushort),(ushort)0},
      { typeof(ushort?),new ushort?()},
      { typeof(sbyte),(sbyte)0},
      { typeof(sbyte?),new sbyte?()},
      { typeof(float),0f},
      { typeof(float?),new float?()},
      { typeof(double),0d},
      { typeof(double?),new double?()},
      { typeof(decimal),(decimal)0},
      { typeof(decimal?),new decimal?()},
      { typeof(string),null},
      { typeof(DateTime),System.Data.SqlTypes.SqlDateTime.MinValue.Value},
      { typeof(DateTime?),new DateTime?(System.Data.SqlTypes.SqlDateTime.MinValue.Value)},
      { typeof(TimeSpan),TimeSpan.Zero},
      { typeof(TimeSpan?),new TimeSpan?()},
      { typeof(DateTimeOffset),new DateTimeOffset(System.Data.SqlTypes.SqlDateTime.MinValue.Value)},
      { typeof(DateTimeOffset?),new DateTimeOffset?()},
      { typeof(Guid),Guid.Empty},
      { typeof(Guid?),new Guid?()},
    };
    public static object DefaultBasicDataTypeValue(this Type This) {
      return This.IsEnum ? Enum.ToObject(This, (int)(Enum.GetValues(This).GetValue(0))) : __DEFAULT_DATATYPE_VALUE[This];
    }

    //public static IEnumerable<PropertyInfo> FindIndexers(this Type This, params Type[] ParameterAndReturnTypes)
    //  => This.GetDefaultMembers()
    //      .OfType<PropertyInfo>()
    //      .Where(E => E.GetIndexParameters().Select(P => P.ParameterType).Append(E.PropertyType).OrderedEqual(ParameterAndReturnTypes));

    public static IEnumerable<PropertyInfo> FindIndexers(this Type This, string IndexName = "Item", params Type[] ParameterAndReturnTypes)
    => This.GetProperties()
        .Where(E => E.Name == IndexName && E.GetIndexParameters().Select(P => P.ParameterType).Append(E.PropertyType).OrderedEqual(ParameterAndReturnTypes));

  }
}
