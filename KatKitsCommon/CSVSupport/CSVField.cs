namespace CSV
{
    using System.Text;

    public class CSVField
    {
        internal const char COMMA = ',';
        internal const char QUOTE = '\"';
        internal const char RETURN = '\r';
        internal const char LINE_FEED = '\n';

        public CSVField(CSVTable Table, CSVRow Row, string Value = "")
        {
            this.Table = Table;
            this.Row = Row;
            this._Text = Value;
        }
        public readonly CSVTable Table;
        public readonly CSVRow Row;

        protected string _Text;
        public string Text { get => _Text; set => _Text = value; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Char1"></param>
        /// <param name="Escaped"></param>
        /// <param name="StrBd"></param>
        /// <returns>0:need nore char,1:create new field,2:create new line</returns>
        internal CSVReaderStatus NextChar(int Char1, ref bool Escaped, ref StringBuilder StrBd)
        {
            switch (CSVExpress.CharByChar(Char1, ref Escaped, ref StrBd))
            {
                case CSVReaderStatus.MoreChar:
                default:
                    return CSVReaderStatus.MoreChar;
                case CSVReaderStatus.NewField:
                    _Text = StrBd.ToString();
                    StrBd.Clear();
                    return CSVReaderStatus.NewField;
                case CSVReaderStatus.NewRow:
                    _Text = StrBd.ToString();
                    StrBd.Clear();
                    return CSVReaderStatus.NewRow;
                case CSVReaderStatus.EndOfText:
                    _Text = StrBd.ToString();
                    StrBd.Clear();
                    return CSVReaderStatus.EndOfText;
            }
        }

        //public void Set(string Val)
        //{
        //        Text = Val;
        //}
        //    public string Get()
        //    {
        //        return Text;
        //    }
        //public object Get()
        //{
        //  if (Column.DataType == null) throw new CSVFieldWithUnknowDataTypeException(Column.Text, Text);
        //  if (Column.DataType.Equals(typeof(string))) return Text;
        //  if (string.IsNullOrEmpty(Text)) return null;
        //  Type Target = Column.DataType;
        //  if (Column.DataType.IsNullableType())
        //  {
        //    Target = Nullable.GetUnderlyingType(Column.DataType);
        //  }
        //  else if (Column.DataType.IsEnum)
        //  {
        //    return Enum.Parse(Column.DataType, Text);
        //  }
        //  if (Target.IsPrimitive)
        //  {
        //    return Convert.ChangeType(Text, Target);
        //  }
        //  else
        //  {
        //    if (Target.Equals(typeof(decimal)))
        //      return decimal.Parse(Text);
        //    else if (Target.Equals(typeof(DateTime)))
        //      return DateTime.Parse(Text);
        //    else if (Target.Equals(typeof(Guid)))
        //      return Guid.Parse(Text);
        //    else if (Target.Equals(typeof(TimeSpan)))
        //      return TimeSpan.Parse(Text);
        //    else if (Target.Equals(typeof(DateTimeOffset)))
        //      return DateTimeOffset.Parse(Text);
        //  }
        //  return null;
        //}
        //public T GetValue<T>()
        //{
        //  return (T)Get();
        //}

        public override string ToString() => Text;
        public string ToCSVString()
        {
            StringBuilder StrBd = new StringBuilder(Text ?? "");
            bool HasToEnclosed = false;
            for (int i = StrBd.Length - 1; i >= 0; i--)
            {
                if (StrBd[i] == '\"')
                {
                    StrBd.Insert(i, '\"');
                    HasToEnclosed = true;
                }
                else
                {
                    if (!HasToEnclosed
                        && (
                          StrBd[i] == ','
                          || (StrBd[i] == '\n' && i > 0 && StrBd[i - 1] == '\r')
                        )
                      )
                        HasToEnclosed = true;
                }
            }
            if (HasToEnclosed)
            {
                StrBd.Insert(0, '\"');
                StrBd.Append('\"');
            }
            return StrBd.ToString();
        }
    }


}
