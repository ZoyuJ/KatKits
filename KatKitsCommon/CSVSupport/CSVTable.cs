/// <summary>
/// RFC 4180
/// </summary>
namespace CSV
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class CSVTable
    {
        public readonly string TableName;

        //public readonly LinkedList<CSVColumn> Columns = new LinkedList<CSVColumn>();
        public readonly LinkedList<CSVRow> Rows = new LinkedList<CSVRow>();

        //public CSVTable(string TableName)
        //{
        //  var Attrs = CSVTableAttribute.FetchAttributes<T>(TableName);
        //  Attrs._Columns.ForEach(E => Columns.AddLast(new CSVColumn() { DataType = E.Property.PropertyType, Text = E.ColumnName }));
        //}
        public CSVTable()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Char1"></param>
        /// <param name="Column"></param>
        /// <param name="Escaped"></param>
        /// <param name="StrBd"></param>
        /// <returns>0:need more char,2:create next row,3:done</returns>
        internal CSVReaderStatus NextChar(int Char1, ref bool Escaped, ref StringBuilder StrBd)
        {
            switch (Rows.Last.Value.NextChar(Char1, ref Escaped, ref StrBd))
            {
                case CSVReaderStatus.MoreChar:
                default:
                    return CSVReaderStatus.MoreChar;
                case CSVReaderStatus.NewField:
                    return CSVReaderStatus.NewField;
                case CSVReaderStatus.NewRow:
                    Rows.AddLast(new CSVRow(this));
                    Rows.Last.Value.Fields.AddLast(new CSVField(this, Rows.Last.Value));
                    return CSVReaderStatus.NewRow;
                case CSVReaderStatus.EndOfText:
                    return CSVReaderStatus.EndOfText;
            }
        }
        public void ParseFromStream(TextReader Reader)
        {
            this.Rows.Clear();
            var StrBd = new StringBuilder();
            var Escaped = true;
            this.Rows.AddLast(new CSVRow(this));
            this.Rows.Last.Value.Fields.AddLast(new CSVField(this, Rows.First.Value));
            var CharInt = 0;
            while ((CharInt = Reader.Read()) != -1)
            {
                switch (this.NextChar(CharInt, ref Escaped, ref StrBd))
                {
                    case CSVReaderStatus.NewField:
                        break;
                    case CSVReaderStatus.NewRow:
                        break;
                }
            }
            _ = this.NextChar(-1, ref Escaped, ref StrBd);
        }

        public void ParseFromText(string CSVText)
        {
            this.Rows.Clear();
            var StrBd = new StringBuilder();
            var Escaped = true;
            this.Rows.AddLast(new CSVRow(this));
            this.Rows.Last.Value.Fields.AddLast(new CSVField(this, Rows.First.Value));
            for (int i = 0; i < CSVText.Length; i++)
            {
                switch (this.NextChar((char)CSVText[i], ref Escaped, ref StrBd))
                {
                    case CSVReaderStatus.NewField:
                        break;
                    case CSVReaderStatus.NewRow:
                        break;
                }
            }
            _ = this.NextChar(-1, ref Escaped, ref StrBd);
        }
        //public void ParseFromText(string CSVText, bool HasHeader)
        //{
        //  ParseFromText(CSVText);
        //  if (HasHeader)
        //  {
        //    var FR = this.Rows.First;
        //    FR.Value.Fields.Zip(this.Columns, (L, R) => new { FField = L, Head = R }).ToArray().ForEach(E => E.Head.Text = E.FField.Text);
        //    this.Rows.RemoveFirst();
        //  }
        //}

        public string ToCSVString() =>
          Rows.Count == 0 ? "" : $"{string.Join("\r\n", Rows.Select(E => E.ToCSVString()))}";
        //public string ToCSVString(bool WithHeader) =>
        //  WithHeader ? $"{string.Join(",", Columns)}\r\n{ToString()}" : ToString();

        //public CSVRow AddRow(IEnumerable<string> Values)
        //{
        //    var Row = new CSVRow(this);
        //    if(Values!==null && Values.)
        //}

        public CSVRow AddRow(IEnumerable<string> Values = null)
        {
            Rows.AddLast(new CSVRow(this, Values));
            return Rows.Last.Value;
        }

    }
}
