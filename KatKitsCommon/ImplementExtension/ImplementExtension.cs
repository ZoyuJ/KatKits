namespace KatKits.ImplementExtension {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public static partial class Kits {
    /// <summary>
    /// get value from nullable if has not value return default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Nullable"></param>
    /// <returns></returns>
    public static T ValueOrDefault<T>(this T? Nullable) where T : struct => Nullable.HasValue ? Nullable.Value : default(T);
    /// <summary>
    /// get new datetime instance from other datetime only with year, month, day, timezone
    /// </summary>
    /// <param name="This"></param>
    /// <returns></returns>
    public static DateTime ToDate(this DateTime This) => new DateTime(This.Year, This.Month, This.Day, 0, 0, 0, This.Kind);


    /// <summary>
    /// how many monthes between 2 dates, Next>This: result>0, Next<This: result<0, 2022-12-31 to 2023-01-01 is 1
    /// </summary>
    /// <param name="This"></param>
    /// <param name="Next"></param>
    /// <returns></returns>
    public static int DiffMonthes(this DateTime This, DateTime Next) {
      return (This.Year - Next.Year) * 12 + This.Month - Next.Month;
    }

  }
}
