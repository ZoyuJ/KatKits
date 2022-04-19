namespace KatKits.Exchange.OtherGrid {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    public struct Pager {
        /// <summary>
        /// what's the page number before render change to new items count per page, 
        /// we can use this property to skip correct count in order to continue query data from datasource
        /// </summary>
        public int? PerviousItemCountPerPage { get; set; }
        /// <summary>
        /// what's the page number before render change to new page number, 
        /// if items count per page changed, we can use this property to skip correct count in order to continue query data from datasource,
        /// so if PerviousItemCountPerPage has value, this property must has value, otherwise, a NullReferenceException will be thrown.
        /// </summary>
        public int? PerviousPageNumber { get; set; }
        /// <summary>
        /// how many items per page that render needs
        /// </summary>
        public int ItemCountPerPage { get; set; }
        /// <summary>
        /// which page that render needs
        /// </summary>
        public int RequirePageNumber { get; set; }
        /// <summary>
        /// total items cout
        /// </summary>
        public int TotalItemCount { get; set; }
        /// <summary>
        /// how many items need to skip
        /// </summary>
        public int Skip {
            get => PerviousItemCountPerPage.HasValue
                  ? (PerviousPageNumber ?? 0) * PerviousItemCountPerPage.Value
                  : (RequirePageNumber - 1) * ItemCountPerPage;
        }
        /// <summary>
        /// take item for specified page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Query"></param>
        /// <returns></returns>
        public IQueryable<T> TakePageItems<T>(IQueryable<T> Query) {
            FillPageProperty(Query);
            return Query.Skip(Skip).Take(ItemCountPerPage);
        }
        /// <summary>
        /// get total item count
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Query"></param>
        public void FillPageProperty<T>(IQueryable<T> Query) {
            this.TotalItemCount = Query.Count();
        }
    }
}
