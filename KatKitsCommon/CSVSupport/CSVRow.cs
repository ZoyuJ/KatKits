namespace CSV
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    using KatKits;

    public class CSVRow
    {
        internal CSVRow() { }
        public CSVRow(CSVTable Table)
        {
            this.Table = Table;
        }
        public CSVRow(CSVTable Table, IEnumerable<string> Values) : this(Table)
        {
            Values?.ForEach(E => this.AddField(E));
        }
        public readonly CSVTable Table;
        public readonly LinkedList<CSVField> Fields = new LinkedList<CSVField>();

        public string this[int Index] { get => Fields.Skip(Index).FirstOrDefault()?.Text; set => Fields.Skip(Index).First().Text = value; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Char1"></param>
        /// <param name="Column"></param>
        /// <param name="Escaped"></param>
        /// <param name="StrBd"></param>
        /// <returns>0:need more char,1:create new filed,2:create next row</returns>
        internal CSVReaderStatus NextChar(int Char1, ref bool Escaped, ref StringBuilder StrBd)
        {
            switch (Fields.Last.Value.NextChar(Char1, ref Escaped, ref StrBd))
            {
                case CSVReaderStatus.MoreChar:
                default:
                    return CSVReaderStatus.MoreChar;
                case CSVReaderStatus.NewField:
                    Fields.AddLast(new CSVField(this.Table, this));
                    return CSVReaderStatus.NewField;
                case CSVReaderStatus.NewRow:
                    return CSVReaderStatus.NewRow;
                case CSVReaderStatus.EndOfText:
                    return CSVReaderStatus.EndOfText;
            }
        }

        public string ToCSVString()
          => $"{string.Join(",", Fields.Select(E => E.ToCSVString()))}";

        public string[] ToArray() => this.Fields.Select(E => E.Text).ToArray();

        public CSVField AddField(string Value = "")
        {
            Fields.AddLast(new CSVField(this.Table, this, Value));
            return Fields.Last.Value;
        }


    }
}
