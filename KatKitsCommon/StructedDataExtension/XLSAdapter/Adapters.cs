namespace KatKits.StructedDataExtension.XLSAdapter {
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;

  //public abstract class ExcelReadException : Exception {
  //  public ExcelReadException() : base() { }
  //  public ExcelReadException(string Message, Exception Exp) : base(Message, Exp) { }
  //}
  //public class ExcelCellReadException : ExcelReadException {
  //  public int RowIndex, ColumnIndex;
  //  public ExcelCellReadException(string Message, Exception Exp, ICell Cell) : base(Message, Exp) {
  //    RowIndex = Cell.RowIndex;
  //    ColumnIndex = Cell.ColumnIndex;
  //  }

  //}

  public interface IExcelWorkBookAdapter : ICollection<IExcelWorkSheetAdapter>, IEquatable<IExcelWorkBookAdapter>, IDisposable {
    TBook Ref<TBook>();
    IExcelWorkSheetAdapter this[int Index] { get; }
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

  public interface IExcelWorkSheetAdapter : ICollection<IExcelRowAdapter>, IEquatable<IExcelWorkSheetAdapter> {
    IExcelWorkBookAdapter Book { get; }
    TSheet Ref<TSheet>();
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

    IExcelRowAdapter this[int RowIndex] { get; }

  }
  //public interface IExcelColumnAdapter
  //{
  //    IEnumerable<IExcelCellAdapter> Cells { get; }
  //}
  public interface IExcelRowAdapter : ICollection<IExcelCellAdapter>, IEquatable<IExcelRowAdapter> {
    IExcelWorkSheetAdapter Sheet { get; }
    TRow Ref<TRow>();
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

    IExcelCellAdapter this[int ColumnIndex] { get; }

  }
  public interface IExcelCellAdapter : IEquatable<IExcelCellAdapter> {
    IExcelWorkSheetAdapter Sheet { get; }
    TCell Ref<TCell>();
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
