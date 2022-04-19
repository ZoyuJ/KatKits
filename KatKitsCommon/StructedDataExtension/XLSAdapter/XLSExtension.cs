namespace KatKits.StructedDataExtension.XLSAdapter {
  using global::KatKits.ImplementExtension.CollectionExtension;

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Text;
  
  

  public static partial class Kits {
    /// <summary>
    /// read xls sheet into datatable,we already got header first
    /// </summary>
    /// <param name="Sheet"></param>
    /// <param name="TableType"></param>
    /// <param name="HeaderColumnIndexes"></param>
    /// <param name="IgnoreCase"></param>
    /// <param name="StartAt"></param>
    /// <param name="EndAt"></param>
    /// <param name="Preprocesses"></param>
    /// <returns></returns>
    public static DataTable AsDataTable(this IExcelWorkSheetAdapter Sheet, Type TableType,
          KeyValuePair<string, int>[] HeaderColumnIndexes,
          bool IgnoreCase = false, int StartAt = 0, int? EndAt = null,
          Dictionary<string, Func<IExcelCellAdapter, object>> Preprocesses = null) {
      var Table = TableType.TypeToDataTable();
      var Attrs = KatKits.StructedDataExtension.Kits.FetchPropertiesAndAttributes(TableType);
      var Ordered = (
            IgnoreCase
            ? Attrs
              .Join(
                HeaderColumnIndexes,
                L => L.Attribute.XLSColumnName.ToUpper(),
                R => R.Key.Trim().ToUpper(),
                (L, R) => new {
                  ColumnIndex = R.Value,
                  ColumnHeader = R.Key,
                  Attribute = L
                }
              )
            : Attrs
              .Join(
                HeaderColumnIndexes,
                L => L.Attribute.XLSColumnName,
                R => R.Key,
                (L, R) => new {
                  ColumnIndex = R.Value,
                  ColumnHeader = R.Key,
                  Attribute = L
                }
             )
          )
          .OrderBy(E => E.Attribute.Attribute.TableColumnOrder)
          .ToArray();
      var Rs = Sheet.Skip(StartAt);
      if (EndAt.HasValue)
        Rs = Rs.Take(EndAt.Value - StartAt);
      Rs.Select(R =>
          Ordered.Select(C => (Preprocesses?.ValueOrDefault(C.Attribute.Attribute.PropertyName) ?? ((Ce) => Ce.GetValue()))(R[C.ColumnIndex])).ToArray()
      )
      .ForEach(E => Table.Rows.Add(E));
      return Table;
    }
    /// <summary>
    /// read xls sheet into datatable,we already got header first
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Sheet"></param>
    /// <param name="HeaderColumnIndexes"></param>
    /// <param name="IgnoreCase"></param>
    /// <param name="StartAt"></param>
    /// <param name="EndAt"></param>
    /// <param name="Preprocesses"></param>
    /// <returns></returns>
    public static DataTable AsDataTable<T>(
        this IExcelWorkSheetAdapter Sheet,
        KeyValuePair<string, int>[] HeaderColumnIndexes,
        bool IgnoreCase = false, int StartAt = 0, int? EndAt = null,
        Dictionary<string, Func<IExcelCellAdapter, object>> Preprocesses = null)
        => AsDataTable(Sheet, typeof(T), HeaderColumnIndexes, IgnoreCase, StartAt, EndAt, Preprocesses);
    /// <summary>
    /// read xls sheet into datatable,has no header
    /// </summary>
    /// <param name="Sheet"></param>
    /// <param name="TableType"></param>
    /// <param name="StartAt"></param>
    /// <param name="EndAt"></param>
    /// <param name="Preprocesses"></param>
    /// <returns></returns>
    public static DataTable AsDataTable(
        this IExcelWorkSheetAdapter Sheet, Type TableType,
        int StartAt, int? EndAt = null,
        Dictionary<string, Func<IExcelCellAdapter, object>> Preprocesses = null) {
      var Table = TableType.TypeToDataTable();
      var Ordered =
            StructedDataExtension.Kits.FetchPropertiesAndAttributes(TableType)
            .OrderBy(E => E.Attribute.TableColumnOrder)
            .ToArray();
      var Rs = Sheet.Skip(StartAt);
      if (EndAt.HasValue)
        Rs = Rs.Take(EndAt.Value - StartAt);
      Rs.Select(R =>
          Ordered.Select(C => (Preprocesses?.ValueOrDefault(C.Attribute.PropertyName) ?? ((Ce) => Ce.GetValue()))(R[C.Attribute.XLSColumnIndex])).ToArray()
      )
      .ForEach(E => Table.Rows.Add(E));
      return Table;
    }
    /// <summary>
    /// read xls sheet into datatable,has no header
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Sheet"></param>
    /// <param name="StartAt"></param>
    /// <param name="EndAt"></param>
    /// <param name="Preprocesses"></param>
    /// <returns></returns>
    public static DataTable AsDataTable<T>(
        this IExcelWorkSheetAdapter Sheet,
        int StartAt, int? EndAt = null,
        Dictionary<string, Func<IExcelCellAdapter, object>> Preprocesses = null) where T : new()
        => AsDataTable(Sheet, typeof(T), StartAt, EndAt, Preprocesses);
    /// <summary>
    /// read xls sheet into datatable,specified header row index
    /// </summary>
    /// <param name="Sheet"></param>
    /// <param name="TableType"></param>
    /// <param name="HeaderRow"></param>
    /// <param name="StartAt"></param>
    /// <param name="EndAt"></param>
    /// <param name="IgnoreCase"></param>
    /// <param name="Preprocesses"></param>
    /// <returns></returns>
    public static DataTable AsDataTable(
    this IExcelWorkSheetAdapter Sheet, Type TableType,
    int? HeaderRow, int StartAt, int? EndAt = null, bool IgnoreCase = false,
    Dictionary<string, Func<IExcelCellAdapter, object>> Preprocesses = null) {

      if (HeaderRow.HasValue) {
        var _Row = Sheet[HeaderRow.Value];
        var Header = _Row
            .Select(C => new KeyValuePair<string, int>(((string)C.GetValue()).Trim(), C.ColumnIndex))
            .ToArray();
        return AsDataTable(Sheet, TableType, Header, IgnoreCase, StartAt, EndAt, Preprocesses);
      }
      else {
        return AsDataTable(Sheet, TableType, StartAt, EndAt, Preprocesses);
      }


    }
    /// <summary>
    /// read xls sheet into datatable,specified header row index
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Sheet"></param>
    /// <param name="HeaderRow"></param>
    /// <param name="StartAt"></param>
    /// <param name="EndAt"></param>
    /// <param name="IgnoreCase"></param>
    /// <param name="Preprocesses"></param>
    /// <returns></returns>
    public static DataTable AsDataTable<T>(
            this IExcelWorkSheetAdapter Sheet,
            int? HeaderRow, int StartAt, int? EndAt = null, bool IgnoreCase = false,
            Dictionary<string, Func<IExcelCellAdapter, object>> Preprocesses = null)
        => AsDataTable(Sheet, typeof(T), HeaderRow, StartAt, EndAt, IgnoreCase, Preprocesses);

  }
}
