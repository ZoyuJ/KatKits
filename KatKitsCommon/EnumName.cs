namespace KatKits {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public static partial class KatKits {
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
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
  public sealed class EnumFieldNameAttribute : Attribute {
    public readonly string DisplayName;
    public EnumFieldNameAttribute(string DisplayName) => this.DisplayName = DisplayName;
  }

}
