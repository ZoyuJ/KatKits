namespace KatKits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static partial class DateTimeUtil
    {
        static readonly IReadOnlyCollection<int> NATURAL_MONTH_NUMBER_OF_NATURALYEAR = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        static readonly IReadOnlyCollection<int> NATURAL_QUARTER_NUMBER_OF_NATURALYEAR = new int[] { 1, 2, 3, 4 };

        /// <summary>
        /// first day of year
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Kind"></param>
        /// <returns></returns>
        public static DateTime BOY(int Year, DateTimeKind Kind = DateTimeKind.Utc) => new DateTime(Year, 1, 1, 0, 0, 0, Kind);
        /// <summary>
        /// first day of year
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static DateTime BOY(this in DateTime This) => new DateTime(This.Year, 1, 1, 0, 0, 0, This.Kind);
        /// <summary>
        /// last day of year
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Kind"></param>
        /// <returns></returns>
        public static DateTime EOY(int Year, DateTimeKind Kind = DateTimeKind.Utc) => new DateTime(Year, 12, 31, 0, 0, 0, Kind);
        /// <summary>
        /// last day of year
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static DateTime EOY(this in DateTime This) => new DateTime(This.Year, 12, 31, 0, 0, 0, This.Kind);

        /// <summary>
        /// first day of month
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="Kind"></param>
        /// <returns></returns>
        public static DateTime BOM(int Year,int Month,DateTimeKind Kind=DateTimeKind.Utc) => new DateTime(Year, Month, 1, 0, 0, 0, Kind);
        /// <summary>
        /// first day of month
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static DateTime BOM(this in DateTime This) => new DateTime(This.Year, This.Month, 1, 0, 0, 0, This.Kind);
        /// <summary>
        /// last day of month
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="Kind"></param>
        /// <returns></returns>
        public static DateTime EOM(int Year,int Month, DateTimeKind Kind = DateTimeKind.Utc) => new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month), 0, 0, 0, Kind);
        /// <summary>
        /// last day of month
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static DateTime EOM(this in DateTime This) => new DateTime(This.Year, This.Month, DateTime.DaysInMonth(This.Year, This.Month), 0, 0, 0, This.Kind);

        /// <summary>
        /// first day of quarter
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Quarter"></param>
        /// <param name="Kind"></param>
        /// <returns></returns>
        public static DateTime BOQ(int Year,int Quarter, DateTimeKind Kind = DateTimeKind.Utc) => new DateTime(Year, FirstMonthOfNaturalQuarter(Quarter), 1, 0, 0, 0, Kind);
        /// <summary>
        /// first day of quarter
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static DateTime BOQ(this in DateTime This) => new DateTime(This.Year, FirstMonthOfNaturalQuarter(MonthToQuarter(This.Month)), 1, 0, 0, 0, This.Kind);
        /// <summary>
        /// last day of quarter
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Quarter"></param>
        /// <param name="Kind"></param>
        /// <returns></returns>
        public static DateTime EOQ(int Year, int Quarter, DateTimeKind Kind = DateTimeKind.Utc) => new DateTime(Year, FirstMonthOfNaturalQuarter(Quarter) + 2, DateTime.DaysInMonth(Year, FirstMonthOfNaturalQuarter(Quarter) + 2), 0, 0, 0, Kind);
        /// <summary>
        /// last day of quarter
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static DateTime EOQ(this in DateTime This) => new DateTime(This.Year, FirstMonthOfNaturalQuarter(MonthToQuarter(This.Month)) + 2, DateTime.DaysInMonth(This.Year, FirstMonthOfNaturalQuarter(MonthToQuarter(This.Month)) + 2), 0, 0, 0, This.Kind);
        
        /// <summary>
        /// first day of week
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static DateTime BOW(this in DateTime This) => This.AddDays(-(int)This.DayOfWeek).Date;
        /// <summary>
        /// last day of week
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static DateTime EOW(this in DateTime This) => This.AddDays(6-(int)This.DayOfWeek).Date;
        
        /// <summary>
        /// first day of weekday
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static DateTime BOWorkday(this in DateTime This) => This.BOW().AddDays(1).Date;
        /// <summary>
        /// last day of weekday
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static DateTime EOWorkday(this in DateTime This) => This.EOW().AddDays(-1).Date;

        /// <summary>
        /// how many workdays?
        /// </summary>
        /// <param name="One"></param>
        /// <param name="Other"></param>
        /// <returns></returns>
        public static int CountWorkdays(DateTime One,DateTime Other)
        {
            var Delta = (int)((One - Other).TotalDays);
            return Delta == 0 ? 0 : Delta > 0 ? Enumerable.Range(1, Delta).Select(E => One.AddDays(E)).Count(E => E.DayOfWeek != DayOfWeek.Sunday && E.DayOfWeek != DayOfWeek.Saturday)
                : -(Enumerable.Range(1, Math.Abs(Delta)).Select(E => One.AddDays(-E)).Count(E => E.DayOfWeek != DayOfWeek.Sunday && E.DayOfWeek != DayOfWeek.Saturday));
        }

        /// <summary>
        /// which quarter that month belongs to
        /// </summary>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static int MonthToQuarter(int Month) => (Month - 1) / 3 + 1;
        /// <summary>
        /// get first month of quarter
        /// </summary>
        /// <param name="Quarter"></param>
        /// <returns></returns>
        public static int FirstMonthOfNaturalQuarter(int Quarter) => (Quarter - 1) * 3 + 1;

        /// <summary>
        /// get natural month number in natural year
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<int> MonthesInYear() => NATURAL_MONTH_NUMBER_OF_NATURALYEAR;
        /// <summary>
        /// get natural date of month in natural year
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="SpecifiedDayOfMonth"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> DatesInYear(int Year, int SpecifiedDayOfMonth = 1)
            => MonthesInYear().Select(E => new DateTime(Year, E, SpecifiedDayOfMonth));
        /// <summary>
        /// get natural month number in natural quarter
        /// </summary>
        /// <param name="Quarter"></param>
        /// <returns></returns>
        public static IEnumerable<int> MonthesInQuarter(int Quarter)
            => new[] { (Quarter - 1) * 3 + 1, (Quarter - 1) * 3 + 2, (Quarter - 1) * 3 + 3 };
        /// <summary>
        /// get natural date of month in natural quarter
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Quarter"></param>
        /// <param name="SpecifiedDayOfMonth"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> DatesInQuarter(int Year, int Quarter, int SpecifiedDayOfMonth = 1)
            => MonthesInQuarter(Quarter).Select(E => new DateTime(Year, E, SpecifiedDayOfMonth));

        public static IEnumerable<DateTime> MonthByMonth(DateTime Start, int Count)
            => Count >= 0
                ? Enumerable.Range(0, Count).Select(E => Start.AddMonths(E))
                : Enumerable.Range(0, -Count).Select(E => Start.AddMonths(-E));

        /// <summary>
        /// how many monthes between 2 dates, Next>This: result>0, Next<This: result<0, 2022-12-31 to 2023-01-01 is 1
        /// </summary>
        /// <param name="This"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        public static int DiffMonthes(this DateTime This, DateTime Next)
        {
            return (This.Year - Next.Year) * 12 + This.Month - Next.Month;
        }

        public static DateTime NextWorkday(this DateTime This)
        {
            return This.AddDays(This.DayOfWeek == DayOfWeek.Friday ? 3 : This.DayOfWeek == DayOfWeek.Saturday ? 2 : 1);
        }

    }

}
