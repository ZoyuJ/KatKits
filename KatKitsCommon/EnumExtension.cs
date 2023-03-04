namespace KatKits
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    public static class EnumUtil
    {
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public class EnumValueNameAttribute : Attribute
        {
            public EnumValueNameAttribute(string Name) { this.Name = Name; }
            public readonly string Name;
        }

        public static string GetEnumValueName<T>(this T Value)
        {
            var Ep = typeof(T);
            if (Ep.IsEnum)
            {
                return Ep.GetMember(Value.ToString()).FirstOrDefault()?.GetCustomAttribute<EnumValueNameAttribute>()?.Name ?? Value.ToString();
            }
            throw new InvalidCastException("We need a enum type");
        }
        public static T? TryParseEnum<T>(string Arg, bool IgnoreCase = true) where T : struct
        {
            if (Enum.TryParse<T>(Arg, IgnoreCase, out var Res))
            {
                return Res;
            }
            return null;
        }
        public static IEnumerable<int> ToFlags(this int This, Type EnumType)
        {
            return Enum.GetValues(EnumType).Cast<int>().Where(E => (E & This) == E).Where(E => E != 0);
        }
        public static IEnumerable<int> ToFlags<T>(this int This)
        {
            return ToFlags(This, typeof(T));
        }
        /// <summary>
        /// Index from 1
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IEnumerable<int> ToFlagsToIndex(this int This, Type EnumType)
        {
            return This.ToFlags(EnumType).Select(E => (int)Math.Log(E, 2));
        }
        public static IEnumerable<int> ToFlagsToIndex<T>(this int This)
        {
            return This.ToFlagsToIndex(typeof(T));
        }
    }
}
