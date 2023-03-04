namespace KatKits
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    public static partial class TypeExtension
    {
        /// <summary>
        /// is type can convert to sql type (basic data type)
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static bool IsBasicDataType(this Type This)
          => This == null
              ? false
              : (This.IsPrimitive
                || This.IsEnum
                || This.Equals(typeof(DateTime))
                || This.Equals(typeof(string))
                || This.Equals(typeof(decimal))
                || This.Equals(typeof(Guid))
                || This.Equals(typeof(TimeSpan))
                || This.Equals(typeof(DateTimeOffset))
                || This.Equals(typeof(byte[])))
          ;
        /// <summary>
        /// (ignore)
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static bool _IsExpandDataType(this Type This) => This.Equals(typeof(DateTime?)) || This.Equals(typeof(Guid?)) || This.Equals(typeof(TimeSpan?)) || This.Equals(typeof(DateTimeOffset?));
        /// <summary>
        /// is anonymous type?
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static bool IsAnonymousType(this Type This) => Attribute.IsDefined(This, typeof(CompilerGeneratedAttribute), false)
                                                    && This.IsGenericType && This.Name.Contains("AnonymousType")
                                                    && (This.Name.StartsWith("<>") || This.Name.StartsWith("VB$"))
                                                    && This.Attributes.HasFlag(TypeAttributes.NotPublic);
        /// <summary>
        /// is nullable?
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type This) => Nullable.GetUnderlyingType(This) != null;
        /// <summary>
        /// can set null to member
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static bool CanBeNull(this Type This) => typeof(string).Equals(This) || IsNullableType(This);
        /// <summary>
        /// is basic data type or nullable basic data type
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static bool IsBasicDataTypeOrNullable(this Type This) => IsBasicDataType(Nullable.GetUnderlyingType(This)) || IsBasicDataType(This);
        public static readonly Dictionary<Type, object> __DEFAULT_DATATYPE_VALUE = new Dictionary<Type, object>() {
      { typeof(bool),false},
      { typeof(bool?),new bool?()},
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
      { typeof(byte[]),default(byte[])},
      { typeof(BitArray),default(BitArray)},
      //{ typeof(System.Data.Linq.Binary),default(System.Data.Linq.Binary)},
    };
        /// <summary>
        /// get default value
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static object DefaultBasicDataTypeValue(this Type This)
        {
            return This.IsEnum ? _DefaultEnumValue(This) : __DEFAULT_DATATYPE_VALUE[This];
        }
        internal static object _DefaultEnumValue(this Type This)
        {
            var D = __DEFAULT_DATATYPE_VALUE[Enum.GetUnderlyingType(This)];
            if (Enum.GetValues(This).Cast<object>().Select(E => Convert.ChangeType(E, Enum.GetUnderlyingType(This))).Any(E => E == D))
                return Enum.ToObject(This, 0);
            else
                return Enum.ToObject(This, Enum.GetValues(This).GetValue(0));
        }

        /// <summary>
        /// get underlying type from nullable,enum
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static Type GetUnderlyingType(this Type This)
        {
            var Tp = This;
            if (This.IsNullableType())
            {
                Tp = Nullable.GetUnderlyingType(This);
            }
            if (Tp.IsEnum)
            {
                return Enum.GetUnderlyingType(Tp);
            }

            return Tp;
        }
        public static IEnumerable<Type> GetTypesInNameSpace(this Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }

    }
}
