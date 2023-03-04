namespace KatKits
{

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static partial class StructuredDataUtil
    {


        ///// <summary>
        ///// write to worksheet
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="Workbook"></param>
        ///// <param name="Items"></param>
        ///// <param name="WithHeader"></param>
        ///// <returns></returns>
        //public static ISheet WriteToSheet<T>(this IWorkbook Workbook, IEnumerable<T> Items, bool WithHeader) where T : new()
        //{
        //    var Sheet = Workbook.Worksheets.Add(Items.ToDataTable(null));
        //    if (!WithHeader)
        //    {
        //        Sheet.Row(1).Delete();
        //    }
        //    return Sheet;
        //}

        ///// <summary>
        ///// find where header is, and get header column index
        ///// </summary>
        ///// <param name="Sheet"></param>
        ///// <param name="HeaderMatchs"></param>
        ///// <param name="HeaderWithColumnIndex"></param>
        ///// <returns></returns>
        //public static int GetExcelHeader(this ISheet Sheet,string[] HeaderMatchs,out KeyValuePair<string,int>[] HeaderWithColumnIndex)
        //{
        //    for (int i = Sheet.FirstRowNum; i < Sheet.LastRowNum; i++)
        //    {
        //        HeaderWithColumnIndex = Sheet.GetRow(i).Cells
        //            .Join(
        //                HeaderMatchs, 
        //                L => L.StringCellValue.Replace(" ", "").ToLower(), 
        //                R => R.Replace(" ", "").ToLower(), 
        //                (L, R) => new KeyValuePair<string,int>(R,L.ColumnIndex)
        //            ).ToArray();
        //        if(HeaderWithColumnIndex.Length == HeaderMatchs.Length)
        //            return i;
        //    }
        //    HeaderWithColumnIndex = null;
        //    return -1;
        //}
        ///// <summary>
        ///// read xls with a confired header and column index
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="Sheet"></param>
        ///// <param name="HeaderColumnIndexes"></param>
        ///// <param name="IgnoreCase"></param>
        ///// <param name="StartAt"></param>
        ///// <param name="EndAt"></param>
        ///// <param name="Preprocesses"></param>
        ///// <returns></returns>
        //public static DataTable AsDataTable<T>(
        //    this ISheet Sheet,
        //    KeyValuePair<string, int>[] HeaderColumnIndexes,
        //    bool IgnoreCase = false, int? StartAt = null, int? EndAt = null,
        //    Dictionary<string, Func<ICell, object>> Preprocesses = null) where T : new()
        //{
        //    var Table = typeof(T).TypeToDataTable();
        //    var Attrs = FetchPropertiesAndAttributes(typeof(T));
        //    var Ordered = (
        //          IgnoreCase
        //          ? Attrs
        //            .Join(
        //              HeaderColumnIndexes,
        //              L => L.Attribute.XLSColumnName.ToUpper(),
        //              R => R.Key.Trim().ToUpper(),
        //              (L, R) => new {
        //                  ColumnIndex = R.Value,
        //                  ColumnHeader = R.Key,
        //                  Attribute = L
        //              }
        //            )
        //          : Attrs
        //            .Join(
        //              HeaderColumnIndexes,
        //              L => L.Attribute.XLSColumnName,
        //              R => R.Key,
        //              (L, R) => new {
        //                  ColumnIndex = R.Value,
        //                  ColumnHeader = R.Key,
        //                  Attribute = L
        //              }
        //           )
        //        )
        //        .OrderBy(E => E.Attribute.Attribute.TableColumnOrder)
        //        .ToArray();
        //    List<ExcelCellReadException> Exceps = new List<ExcelCellReadException>();
        //    _EachRowInWorkSheet(Sheet, StartAt, EndAt)
        //        .Select(E =>
        //        Ordered.Select(EE =>
        //            (Preprocesses?.ValueOrDefault(EE.Attribute.Attribute.PropertyName) ?? ((C) => _GetCellValue(C)))(E.GetCell(EE.ColumnIndex,MissingCellPolicy.RETURN_NULL_AND_BLANK))
        //        )
        //        .ToArray()
        //      )
        //      .ForEach(E =>
        //      {
        //          Table.Rows.Add(E);
        //      });
        //    return Table;
        //}
        ///// <summary>
        ///// xls has no header, use column index that were declared in attribute
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="Sheet"></param>
        ///// <param name="StartAt"></param>
        ///// <param name="EndAt"></param>
        ///// <param name="Preprocesses"></param>
        ///// <returns></returns>
        //public static DataTable AsDataTable<T>(this ISheet Sheet, int StartAt, int? EndAt = null, Dictionary<string, Func<ICell, object>> Preprocesses = null) where T : new()
        //{
        //    var Table = typeof(T).TypeToDataTable();
        //    var Ordered =
        //          FetchPropertiesAndAttributes(typeof(T))
        //          .OrderBy(E => E.Attribute.TableColumnOrder)
        //          .ToArray();
        //    _EachRowInWorkSheet(Sheet, StartAt, EndAt)
        //        .Select(E =>
        //            Ordered.Select(EE =>
        //                (Preprocesses?.ValueOrDefault(EE.Attribute.PropertyName) ?? ((C) => _GetCellValue(C)))(E.GetCell(EE.Attribute.XLSColumnIndex,MissingCellPolicy.RETURN_NULL_AND_BLANK))
        //            )
        //            .ToArray()
        //            )
        //        .ForEach(E => Table.Rows.Add(E));
        //    return Table;
        //}
        ///// <summary>
        ///// come from ~\WebUI\Common\ExcelHelpers.cs -> getCellValue
        ///// </summary>
        ///// <param name="cell"></param>
        ///// <returns></returns>
        //private static object _GetCellValue(ICell cell)
        //{
        //    if (cell == null) return null;
        //    object result = null;
        //    switch (cell.CellType)
        //    {
        //        case CellType.Blank:
        //            break;
        //        case CellType.Boolean:
        //            result = cell.BooleanCellValue;
        //            break;
        //        case CellType.Numeric:
        //            if (DateUtil.IsCellDateFormatted(cell))
        //                result = cell.DateCellValue;
        //            else
        //                result = cell.NumericCellValue;
        //            break;
        //        case CellType.String:
        //            result = cell.StringCellValue;
        //            break;
        //        case CellType.Error:
        //            result = cell.ErrorCellValue;
        //            break;
        //        case CellType.Formula:
        //            switch (cell.CachedFormulaResultType)
        //            {
        //                case CellType.Blank:
        //                    result = "";
        //                    break;
        //                case CellType.Boolean:
        //                    result = cell.BooleanCellValue;
        //                    break;
        //                case CellType.Error:
        //                    result = cell.ErrorCellValue;
        //                    break;
        //                case CellType.Formula:
        //                    result = cell.CellFormula;
        //                    break;
        //                case CellType.Numeric:
        //                    if (DateUtil.IsCellDateFormatted(cell))
        //                        result = cell.DateCellValue;
        //                    else
        //                        result = cell.NumericCellValue;
        //                    break;
        //                case CellType.String:
        //                    result = cell.StringCellValue;
        //                    break;
        //                case CellType.Unknown:
        //                    result = null;
        //                    break;
        //                default:
        //                    break;
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //    return result;
        //}
        ///// <summary>
        ///// Read Sheet to DataTable
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="Sheet"></param>
        ///// <param name="HeaderRow">has header? and where is it</param>
        ///// <param name="IgnoreCase">if has header, the letter case is not important</param>
        ///// <param name="StartAt">where is the content start at</param>
        ///// <param name="EndAt">where is the content end at, if set null, read to end</param>
        ///// <param name="Preprocesses">how to get and convert cell value,default will call _GetCellValue</param>
        ///// <returns></returns>
        //public static DataTable AsDataTable<T>(
        //    this ISheet Sheet,
        //    int? HeaderRow, int StartAt, int? EndAt = null, bool IgnoreCase = false,
        //    Dictionary<string, Func<ICell, object>> Preprocesses = null) where T : new()
        //{

        //    if (HeaderRow.HasValue)
        //    {
        //        var _Row = Sheet.GetRow(HeaderRow.Value);
        //        var Header = _Row.Cells.Skip(_Row.FirstCellNum).Take(_Row.LastCellNum - _Row.FirstCellNum)
        //            .Select(C => new KeyValuePair<string, int>(C.StringCellValue.Trim(), C.ColumnIndex))
        //            .ToArray();
        //        return AsDataTable<T>(Sheet, Header, IgnoreCase, StartAt, EndAt, Preprocesses);
        //    }
        //    else
        //    {
        //        return AsDataTable<T>(Sheet, StartAt, EndAt, Preprocesses);
        //    }


        //}
        //private static IEnumerable<IRow> _EachRowInWorkSheet(ISheet Sheet, int? Start = 0, int? End = null)
        //{
        //    if (!End.HasValue) End = Sheet.LastRowNum;
        //    if (!Start.HasValue) Start = Sheet.FirstRowNum;
        //    for (int i = Start.Value; i <= End.Value; i++)
        //    {
        //        yield return Sheet.GetRow(i);
        //    }
        //}

        public static DataTable AsDataTable<TBook, TSheet, TRow, TCell>(this IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell> Sheet, Type TableType,
            KeyValuePair<string, int>[] HeaderColumnIndexes,
            bool IgnoreCase = false, int StartAt = 0, int? EndAt = null,
            Dictionary<string, Func<IExcelCellAdapter<TBook, TSheet, TRow, TCell>, object>> Preprocesses = null)
        {
            var Table = TableType.TypeToDataTable();
            var Attrs = DataTableUtil.FetchPropertiesAndAttributes(TableType);
            var Ordered = (
                  IgnoreCase
                  ? Attrs
                    .Join(
                      HeaderColumnIndexes,
                      L => L.Attribute.XLSColumnName.ToUpper(),
                      R => R.Key.Trim().ToUpper(),
                      (L, R) => new
                      {
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
                      (L, R) => new
                      {
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
            .ForEach(E =>
            Table.Rows.Add(E)
            );
            return Table;
        }
        public static DataTable AsDataTable<T, TBook, TSheet, TRow, TCell>(
            this IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell> Sheet,
            KeyValuePair<string, int>[] HeaderColumnIndexes,
            bool IgnoreCase = false, int StartAt = 0, int? EndAt = null,
            Dictionary<string, Func<IExcelCellAdapter<TBook, TSheet, TRow, TCell>, object>> Preprocesses = null)
            => AsDataTable(Sheet, typeof(T), HeaderColumnIndexes, IgnoreCase, StartAt, EndAt, Preprocesses);
        public static DataTable AsDataTable<TBook, TSheet, TRow, TCell>(
            this IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell> Sheet, Type TableType,
            int StartAt, int? EndAt = null,
            Dictionary<string, Func<IExcelCellAdapter<TBook, TSheet, TRow, TCell>, object>> Preprocesses = null)
        {
            var Table = TableType.TypeToDataTable();
            var Ordered =
                  DataTableUtil.FetchPropertiesAndAttributes(TableType)
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
        public static DataTable AsDataTable<T, TBook, TSheet, TRow, TCell>(
            this IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell> Sheet,
            int StartAt, int? EndAt = null,
            Dictionary<string, Func<IExcelCellAdapter<TBook, TSheet, TRow, TCell>, object>> Preprocesses = null) where T : new()
            => AsDataTable(Sheet, typeof(T), StartAt, EndAt, Preprocesses);
        public static DataTable AsDataTable<TBook, TSheet, TRow, TCell>(
        this IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell> Sheet, Type TableType,
        int? HeaderRow, int StartAt, int? EndAt = null, bool IgnoreCase = false,
        Dictionary<string, Func<IExcelCellAdapter<TBook, TSheet, TRow, TCell>, object>> Preprocesses = null)
        {

            if (HeaderRow.HasValue)
            {
                var _Row = Sheet[HeaderRow.Value];
                var Header = _Row
                    .Select(C => new KeyValuePair<string, int>(((string)C.GetValue()).Trim(), C.ColumnIndex))
                    .ToArray();
                return AsDataTable(Sheet, TableType, Header, IgnoreCase, StartAt, EndAt, Preprocesses);
            }
            else
            {
                return AsDataTable(Sheet, TableType, StartAt, EndAt, Preprocesses);
            }


        }
        public static DataTable AsDataTable<T, TBook, TSheet, TRow, TCell>(
                this IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell> Sheet,
                int? HeaderRow, int StartAt, int? EndAt = null, bool IgnoreCase = false,
                Dictionary<string, Func<IExcelCellAdapter<TBook, TSheet, TRow, TCell>, object>> Preprocesses = null)
            => AsDataTable(Sheet, typeof(T), HeaderRow, StartAt, EndAt, IgnoreCase, Preprocesses);



    }

 

    public interface IExcelWorkBookAdapter<TBook, TSheet, TRow, TCell> : ICollection<IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell>>, IEquatable<IExcelWorkBookAdapter<TBook, TSheet, TRow, TCell>>, IDisposable
    {
        TBook Ref { get; }
        IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell> this[int Index] { get; }
        /// <summary>
        /// overwrite to current file,memory?,nic?
        /// </summary>
        void Save();
        /// <summary>
        /// save to new file,memory?,nic?
        /// </summary>
        /// <param name="Stream"></param>
        void SaveTo(Stream Stream);
    }

    public interface IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell> : ICollection<IExcelRowAdapter<TBook, TSheet, TRow, TCell>>, IEquatable<IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell>>
    {
        IExcelWorkBookAdapter<TBook, TSheet, TRow, TCell> Book { get; }
        TSheet Ref { get; }
        IExcelRowAdapter<TBook, TSheet, TRow, TCell> this[int RowIndex] { get; }

        /// <summary>
        /// sheet name
        /// </summary>
        string Name { get; set; }
        int Index { get; }
        /// <summary>
        /// index(base on 0) or first used row
        /// </summary>
        int FirstRowIndex { get; }
        /// <summary>
        /// index(base on 0) or last used row
        /// </summary>
        int LastRowIndex { get; }
        /// <summary>
        /// index(base on 0) or first used column
        /// </summary>
        int FirstColumnIndex { get; }
        /// <summary>
        /// index(base on 0) or last used column
        /// </summary>
        int LastColumnIndex { get; }

    }


    public interface IExcelRowAdapter<TBook, TSheet, TRow, TCell> : ICollection<IExcelCellAdapter<TBook, TSheet, TRow, TCell>>, IEquatable<IExcelRowAdapter<TBook, TSheet, TRow, TCell>>
    {
        IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell> Sheet { get; }
        TRow Ref { get; }
        IExcelCellAdapter<TBook, TSheet, TRow, TCell> this[int ColumnIndex] { get; }
        /// <summary>
        /// the index of this row
        /// </summary>
        int Index { get; }
        /// <summary>
        /// first column index of this row
        /// </summary>
        int FirstCellIndex { get; }
        /// <summary>
        /// last column index of this row
        /// </summary>
        int LastCellIndex { get; }

    }

    public interface IExcelCellAdapter<TBook, TSheet, TRow, TCell> : IEquatable<IExcelCellAdapter<TBook, TSheet, TRow, TCell>>
    {
        IExcelWorkSheetAdapter<TBook, TSheet, TRow, TCell> Sheet { get; }
        TCell Ref { get; }
        /// <summary>
        /// the row index of this cell,base on 0
        /// </summary>
        int RowIndex { get; }
        /// <summary>
        /// the column index of this cell,base on 0
        /// </summary>
        int ColumnIndex { get; }
        /// <summary>
        /// get value from cell
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool TryGetValue<T>(out T Value);
        object GetValue();
        /// <summary>
        /// set value into cell
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        void SetValue<T>(T Value);
    }




}
