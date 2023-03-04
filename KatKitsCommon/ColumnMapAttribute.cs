namespace KatKits
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ColumnMapAttribute : Attribute
    {
        /// <summary>
        /// if the xls sheet has headers
        /// </summary>
        public string XLSColumnName { get; set; }
        /// <summary>
        /// DataTable column name
        /// </summary>
        public string TableColumnName { get; set; }
        /// <summary>
        /// an order in DataTable
        /// </summary>
        public int TableColumnOrder { get; set; }
        /// <summary>
        /// start at one,the column index in xls sheet
        /// </summary>
        public int XLSColumnIndex { get; set; }
        public bool AllowNull { get; set; }
        /// <summary>
        /// keep null or ignore when the type cannot set as a constant such as DateTime or Nullable<T>
        /// </summary>
        public object DefaultValue { get; set; } = null;
        /// <summary>
        /// ignore in attribute attaching
        /// </summary>
        public Type PropertyType { get; internal set; }
        /// <summary>
        /// ignore in attribute attaching
        /// </summary>
        public string PropertyName { get; internal set; }

    }
}
