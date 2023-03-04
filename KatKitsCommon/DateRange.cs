namespace KatKits
{
    using System;


    public enum DateRangeKind
    {
        /** date input */
        Date = 1,
        /** week input */
        Week = 2,
        /** month input */
        Month = 3,
        /** year + quarter input */
        Quarter = 4,
        /** year input */
        Year = 5,

        /** date to date */
        Date2Date = 6,
        /** week to week */
        Week2Week = 7,
        /** year-month start to year-month end */
        Month2Month = 8,
        /** year-quarter start to year-quarter end */
        Quarter2Quarter = 9,
        /** year start to year end */
        Year2Year = 10,

    }
    public class DateRangeValue
    {
        public DateRangeKind Kind { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        //public int? Zone { get; set; }

        public DateTime? CalculatedFrom { get; private set; }
        public DateTime? CalculatedTo { get; private set; }

        public void Calc()
        {
            if (!From.HasValue) return;
            if (From.HasValue)
            {
                From = new DateTime(From.Value.Year, From.Value.Month, From.Value.Day, 0, 0, 0, DateTimeKind.Utc);
            }
            if (To.HasValue)
            {
                To = new DateTime(To.Value.Year, To.Value.Month, To.Value.Day, 0, 0, 0, DateTimeKind.Utc);
            }

            switch (Kind)
            {
                default:
                case DateRangeKind.Date:
                case DateRangeKind.Date2Date:
                    if (From.HasValue) CalculatedFrom = From;
                    if (To.HasValue) CalculatedTo = To.Value.AddDays(1);
                    return;
                case DateRangeKind.Week:
                    if (From.HasValue)
                    {
                        CalculatedFrom = From.Value.BOW();
                        CalculatedTo = From.Value.EOW().AddDays(1);
                    }
                    return;
                case DateRangeKind.Week2Week:
                    if (From.HasValue) CalculatedFrom = From.Value.BOW();
                    if (To.HasValue) CalculatedTo = To.Value.EOW().AddDays(1);
                    return;
                case DateRangeKind.Month:
                    if (From.HasValue)
                    {
                        CalculatedFrom = From.Value.BOM();
                        CalculatedTo = From.Value.EOM().AddDays(1);
                    }
                    return;
                case DateRangeKind.Month2Month:
                    if (From.HasValue) CalculatedFrom = From.Value.BOM();
                    if (To.HasValue) CalculatedTo = To.Value.EOM().AddDays(1);
                    return;
                case DateRangeKind.Quarter:
                    if (From.HasValue)
                    {
                        CalculatedFrom = new DateTime(From.Value.Year, DateTimeUtil.FirstMonthOfNaturalQuarter(From.Value.Month), 1, 0, 0, 0, DateTimeKind.Utc).BOQ();
                        CalculatedTo = new DateTime(From.Value.Year, DateTimeUtil.FirstMonthOfNaturalQuarter(From.Value.Month), 1, 0, 0, 0, DateTimeKind.Utc).EOQ().AddDays(1);
                    }
                    return;
                case DateRangeKind.Quarter2Quarter:
                    if (From.HasValue) CalculatedFrom = new DateTime(From.Value.Year, DateTimeUtil.FirstMonthOfNaturalQuarter(From.Value.Month), 1, 0, 0, 0, DateTimeKind.Utc).BOQ();
                    if (To.HasValue) CalculatedTo = new DateTime(To.Value.Year, DateTimeUtil.FirstMonthOfNaturalQuarter(To.Value.Month), 1, 0, 0, 0, DateTimeKind.Utc).EOQ().AddDays(1);
                    return;
                case DateRangeKind.Year:
                    if (From.HasValue) CalculatedFrom = From.Value.BOY();
                    if (To.HasValue) CalculatedTo = From.Value.EOY().AddDays(1);
                    return;
                case DateRangeKind.Year2Year:
                    if (From.HasValue) CalculatedFrom = From.Value.BOY();
                    if (To.HasValue) CalculatedTo = To.Value.EOY().AddDays(1);
                    return;
              
            }
        }

    }

}
