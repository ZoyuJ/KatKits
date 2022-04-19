namespace KatKits.ImplementExtension {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public static partial class Kits {
    public static TEnum ToEnum<TEnum>(this string This) => (TEnum)Enum.Parse(typeof(TEnum), This);
    public static string ToEnumName(this Enum This) => EnumGetName(This);
    /// <summary>
    /// 使用EnumFieldNameAttribute
    /// </summary>
    /// <param name="EnumField"></param>
    /// <returns></returns>
    private static string EnumGetName(Enum EnumField) {
      try {
        return ((EnumField.GetType().GetMember(EnumField.ToString())
        .First(E => E.DeclaringType == EnumField.GetType())
        .GetCustomAttributes(typeof(EnumFieldNameAttribute), false)
        .FirstOrDefault() as EnumFieldNameAttribute)?.DisplayName)
          ?? Enum.GetName(EnumField.GetType(), EnumField);
      }
      catch (InvalidOperationException) {
        return EnumField.ToString();
      }
    }

    /// <summary>
    /// parse string to nullable Enum
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Arg"></param>
    /// <param name="IgnoreCase"></param>
    /// <returns></returns>
    public static T? TryParseEnum<T>(string Arg, bool IgnoreCase = true) where T : struct {
      if (Enum.TryParse<T>(Arg, IgnoreCase, out var Res)) {
        return Res;
      }
      return null;
    }

    /// <summary>
    /// a number is added from some enum value
    /// </summary>
    /// <param name="This"></param>
    /// <param name="EnumType"></param>
    /// <returns></returns>
    public static IEnumerable<int> ToFlags(this int This, Type EnumType) {
      return Enum.GetValues(EnumType).Cast<int>().Where(E => (E & This) == E).Where(E => E != 0);
    }
    /// <summary>
    /// a number is added from some enum value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="This"></param>
    /// <returns></returns>
    public static IEnumerable<int> ToFlags<T>(this int This) {
      return ToFlags(This, typeof(T))
;
    }
    /// <summary>
    /// convert enum flags(1,2,4,8...) to index(1,2,3,4...), Index from 1
    /// </summary>
    /// <param name="This"></param>
    /// <returns></returns>
    public static IEnumerable<int> ToFlagsToIndex(this int This, Type EnumType) {
      return This.ToFlags(EnumType).Select(E => (int)Math.Log(E, 2));
    }
    /// <summary>
    /// /// convert enum flags(1,2,4,8...) to index(1,2,3,4...), Index from 1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="This"></param>
    /// <returns></returns>
    public static IEnumerable<int> ToFlagsToIndex<T>(this int This) {
      return This.ToFlagsToIndex(typeof(T));
    }

  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
  public sealed class EnumFieldNameAttribute : Attribute {
    public readonly string DisplayName;
    public EnumFieldNameAttribute(string DisplayName) => this.DisplayName = DisplayName;
  }

}
