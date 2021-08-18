namespace KatKits
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.CompilerServices;
  using System.Text;

  //Type Check
  public static partial class KatKits
  {

    public static bool IsBasicDataType(this Type This)
      => This == null
          ? false
          : This.IsPrimitive
            || This.IsEnum
            || This.Equals(typeof(DateTime))
            || This.Equals(typeof(string))
            || This.Equals(typeof(decimal))
            || This.Equals(typeof(Guid))
            || This.Equals(typeof(TimeSpan))
            || This.Equals(typeof(DateTimeOffset));
    public static bool _IsExpandDataType(this Type This) => This.Equals(typeof(DateTime?)) || This.Equals(typeof(Guid?)) || This.Equals(typeof(TimeSpan?)) || This.Equals(typeof(DateTimeOffset?));
    public static bool IsAnonymousType(this Type This) => Attribute.IsDefined(This, typeof(CompilerGeneratedAttribute), false)
                                                && This.IsGenericType && This.Name.Contains("AnonymousType")
                                                && (This.Name.StartsWith("<>") || This.Name.StartsWith("VB$"))
                                                && This.Attributes.HasFlag(TypeAttributes.NotPublic);
    public static bool IsNullableType(this Type This) => Nullable.GetUnderlyingType(This) != null;
    public static bool IsBasicDataTypeOrNullable(this Type This) => IsBasicDataType(Nullable.GetUnderlyingType(This)) || IsBasicDataType(This);
    private static readonly Dictionary<Type, object> __DEFAULT_DATATYPE_VALUE = new Dictionary<Type, object>() {
      { typeof(int),0},
      { typeof(int?),new int?() },
      { typeof(long),0L},
      { typeof(long?),new long?()},
      { typeof(short),(short)0},
      { typeof(short?),new short?()},
      { typeof(byte),(byte)0},
      { typeof(byte?),new byte?()},
      { typeof(uint),(uint)0},
      { typeof(uint?),new uint?()},
      { typeof(ulong),(ulong)0},
      { typeof(ulong?),new ulong?()},
      { typeof(ushort),(ushort)0},
      { typeof(ushort?),new ushort?()},
      { typeof(sbyte),(sbyte)0},
      { typeof(sbyte?),new sbyte?()},
      { typeof(float),0f},
      { typeof(float?),new float?()},
      { typeof(double),0d},
      { typeof(double?),new double?()},
      { typeof(decimal),(decimal)0},
      { typeof(decimal?),new decimal?()},
      { typeof(string),null},
      { typeof(DateTime),DateTime.UtcNow},
      { typeof(DateTime?),new DateTime?()},
      { typeof(TimeSpan),TimeSpan.Zero},
      { typeof(TimeSpan?),new TimeSpan?()},
      { typeof(DateTimeOffset),DateTimeOffset.UtcNow},
      { typeof(DateTimeOffset?),new DateTimeOffset?()},
      { typeof(Guid),Guid.Empty},
      { typeof(Guid?),new Guid?()},
    };
    public static object DefaultBasicDataTypeValue(this Type This)
    {
      return This.IsEnum ? Enum.ToObject(This, (int)(Enum.GetValues(This).GetValue(0))) : __DEFAULT_DATATYPE_VALUE[This];
    }

    public static IEnumerable<PropertyInfo> FindIndexers(this Type This, params Type[] ParameterAndReturnTypes)
      => This.GetDefaultMembers()
          .OfType<PropertyInfo>()
          .Where(E => E.GetIndexParameters().Select(P => P.ParameterType).Append(E.PropertyType).OrderedEqual(ParameterAndReturnTypes));

  }
}
