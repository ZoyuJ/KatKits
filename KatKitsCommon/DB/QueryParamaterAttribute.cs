namespace KatKits.DB
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class QueryParamaterAttribute : Attribute
    {
        public string TableName { get; set; }
        public string ParamaterName { get; set; }
        //public int? Size { get; set; }
        public PropertyInfo Property { get; private set; }
        public static IEnumerable<QueryParamaterAttribute> FetchAttributes<T>() => FetchAttributes(typeof(T));
        public static IEnumerable<QueryParamaterAttribute> FetchAttributes(Type Type)
        {

            if (Attributes.TryGetValue(Type, out var Attrs))
            {
                return Attrs;
            }
            else
            {
                //var TableName = (Type.GetCustomAttribute<TableNameAttribute>()?.TableName) ?? (Type.GetCustomAttribute<TableAttribute>()?.Name);
                var AttrsA = Type.GetProperties()
                     .Where(E => E.PropertyType.IsBasicDataTypeOrNullable() || E.PropertyType.Equals(typeof(DataTable)))
                     .Select(E =>
                     {
                         var A = E.GetCustomAttribute<QueryParamaterAttribute>();
                         if (A != null)
                         {
                             A.Property = E;
                             if (A.ParamaterName == null) A.ParamaterName = A.Property.Name;
                         }
                         return A;
                     })
                     .Where(E => E != null).ToArray();
                if (AttrsA.Length == 0)
                {
                    AttrsA = Type.GetProperties()
                       .Where(E => E.PropertyType.IsBasicDataTypeOrNullable() || E.PropertyType.Equals(typeof(DataTable)))
                       .Select(E => new QueryParamaterAttribute() { Property = E, ParamaterName = E.Name })
                       .ToArray();
                }
                Attributes.Add(Type, AttrsA);
                return AttrsA;
            }
        }
        private static readonly Dictionary<Type, IEnumerable<QueryParamaterAttribute>> Attributes = new Dictionary<Type, IEnumerable<QueryParamaterAttribute>>();

    }
}
