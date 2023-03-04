namespace CSV {
  using System.Collections.Generic;
  using System.Data;
  using System.IO;
  using System.Linq;
  using System.Text;

  internal enum CSVReaderStatus {
    MoreChar = 0,
    NewField = 1,
    NewRow = 2,
    EndOfText = 3,
  }

  public static class CSVExpress {
    /// <summary>
    /// parse csv chars one by one
    /// </summary>
    /// <param name="Char1">end of stream=-1</param>
    /// <param name="Escaped">init=true</param>
    /// <param name="StrBd">init=""</param>
    /// <returns>0:need nore char,1:create new field,2:create new line</returns>
    internal static CSVReaderStatus CharByChar(int Char1, ref bool Escaped, ref StringBuilder StrBd) {
      if (Char1 == -1) {
        if (StrBd.Length > 2 && StrBd[0] == CSVField.QUOTE && StrBd[StrBd.Length - 1] == CSVField.QUOTE) {
          StrBd.Remove(0, 1);
          StrBd.Remove(StrBd.Length - 1, 1);
        }
        StrBd.Replace("\"\"", "\"");
        return CSVReaderStatus.EndOfText;
      }
      var CH = (char)Char1;
      if (CH == CSVField.QUOTE) {
        Escaped = !Escaped;
        //return 0;
      }
      if (!Escaped) {
        StrBd.Append(CH);
      }
      else {
        if (CH == CSVField.COMMA) {
          //next field
          if (StrBd.Length > 2 && StrBd[0] == CSVField.QUOTE && StrBd[StrBd.Length - 1] == CSVField.QUOTE) {
            StrBd.Remove(0, 1);
            StrBd.Remove(StrBd.Length - 1, 1);
          }
          StrBd.Replace("\"\"", "\"");
          return CSVReaderStatus.NewField;
        }
        else if (CH == CSVField.LINE_FEED && StrBd.Length > 0 && StrBd[StrBd.Length - 1] == CSVField.RETURN) {
          //next line
          StrBd.Remove(StrBd.Length - 1, 1);
          if (StrBd.Length > 2 && StrBd[0] == CSVField.QUOTE && StrBd[StrBd.Length - 1] == CSVField.QUOTE) {
            StrBd.Remove(0, 1);
            StrBd.Remove(StrBd.Length - 1, 1);
          }
          StrBd.Replace("\"\"", "\"");
          return CSVReaderStatus.NewRow;
        }
        else {
          StrBd.Append(CH);
        }
      }

      return CSVReaderStatus.MoreChar;


    }
    ///// <summary>
    ///// read first row as header,not stop at /r/n, stop at end of first csv row
    ///// </summary>
    ///// <param name="Reader"></param>
    ///// <returns></returns>
    //public static LinkedList<CSVColumn> ReadHeaders(TextReader Reader) {
    //  var Header = new LinkedList<CSVColumn>();
    //  Header.AddLast(new CSVColumn());
    //  var CharInt = 0;
    //  var Escaped = true;
    //  var StrBd = new StringBuilder();
    //  while ((CharInt = Reader.Read()) != -1) {
    //    switch (CharByChar(CharInt, ref Escaped, ref StrBd)) {
    //      case CSVReaderStatus.MoreChar:
    //      default:
    //        break;
    //      case CSVReaderStatus.NewField:
    //        Header.Last.Value.Text = StrBd.ToString();
    //        StrBd.Clear();
    //        Header.AddLast(new CSVColumn());
    //        break;
    //      case CSVReaderStatus.NewRow:
    //      case CSVReaderStatus.EndOfText:
    //        Header.Last.Value.Text = StrBd.ToString();
    //        StrBd.Clear();
    //        //Header.AddLast(new CSVColumn());
    //        return Header;
    //    }
    //  }
    //  return Header;
    //}
    /// <summary>
    /// read csv text row by row
    /// </summary>
    /// <param name="Reader"></param>
    /// <param name="Columns"></param>
    /// <returns></returns>
    public static IEnumerable<CSVRow> ReadRow(TextReader Reader) {
      var CharInt = 0;
      var Escaped = true;
      var StrBd = new StringBuilder();
      var CSVRow = new CSVRow();
            //CSVRow.Fields.AddLast(new CSVField(CurrentCol.Value));
            CSVRow.AddField();
      while ((CharInt = Reader.Read()) != -1) {
        switch (CharByChar(CharInt, ref Escaped, ref StrBd)) {
          case CSVReaderStatus.MoreChar:
          default:
            continue;
          case CSVReaderStatus.NewField:
            CSVRow.Fields.Last.Value.Text = StrBd.ToString();
            StrBd.Clear();
                        CSVRow.AddField();
                        continue;
          case CSVReaderStatus.NewRow:
            CSVRow.Fields.Last.Value.Text = StrBd.ToString();
            StrBd.Clear();
            yield return CSVRow;
            CSVRow = new CSVRow();
                        CSVRow.AddField();
                        continue;
        }
      }
      _ = CharByChar(-1, ref Escaped, ref StrBd);
      CSVRow.Fields.Last.Value.Text = StrBd.ToString();
      StrBd.Clear();
      yield return CSVRow;
      yield break;
    }

    public static DataTable LoadCSVRowToDataTable(this DataTable This, CSVRow Row, bool EmptyIsNull = false) {
      if (This.Columns.Count != Row.Fields.Count) throw new System.Exception();
      This.Rows.Add(Row.Fields.Select(E => (EmptyIsNull && string.IsNullOrEmpty(E.Text)) ? null : E.Text));
      return This;
    }
    public static DataTable LoadCSVRowsToDataTable(this DataTable This, IEnumerable<CSVRow> Rows, bool EmptyIsNull = false,bool TryAllRows = false) {
      if (TryAllRows) {
        var Excepts = new List<System.Exception>();
        foreach (var item in Rows) {
          try {
            LoadCSVRowToDataTable(This, item, EmptyIsNull);
          }
          catch (System.Exception E) {
            Excepts.Add(E);
          }
        }
        if (Excepts.Count > 0) throw new System.AggregateException(Excepts);
      }
      else {
        foreach (var item in Rows) {
          LoadCSVRowToDataTable(This, item, EmptyIsNull);
        }
      }
      
      return This;
    }
    
    

  }
}
