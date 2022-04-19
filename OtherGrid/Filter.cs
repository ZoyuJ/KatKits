#define DYNAMIC_LINQ_Z

namespace KatKits.Exchange.OtherGrid {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
#if DYNAMIC_LINQ_Z
    using System.Linq.Dynamic;
#endif

    public enum FilterOp {
        //==
        [FilterOpSymbol(Symbol = "{0} = @0")]
        [FilterOpSymbol(Symbol = "{0} == null", Version = SymbolVersion.Nullable)]
        Equal,
        //!=
        [FilterOpSymbol(Symbol = "{0} != @0")]
        [FilterOpSymbol(Symbol = "{0} != null", Version = SymbolVersion.Nullable)]
        NotEqual,

        //>
        [FilterOpSymbol(Symbol = "{0} > @0")]
        Greate,
        //<
        [FilterOpSymbol(Symbol = "{0} < @0")]
        Less,
        //>=
        [FilterOpSymbol(Symbol = "{0} >= @0")]
        GreateOrEqual,
        //<=
        [FilterOpSymbol(Symbol = "{0} <= @0")]
        LessOrEqual,

        //string.StartsWith
        [FilterOpSymbol(Symbol = "{0}." + nameof(string.StartsWith) + "(@0)")]
        StartsWith,
        //!string.StartsWith
        [FilterOpSymbol(Symbol = "not {0}." + nameof(string.StartsWith) + "(@0)")]
        NotStartsWith,
        //string.EndsWith
        [FilterOpSymbol(Symbol = "{0}." + nameof(string.EndsWith) + "(@0)")]
        EndsWith,
        //!string.EndsWith
        [FilterOpSymbol(Symbol = "not {0}." + nameof(string.EndsWith) + "(@0)")]
        NotEndsWith,
        //!string.Contains
        [FilterOpSymbol(Symbol = "{0}." + nameof(string.Contains) + "(@0)")]
        Contains,
        //string.Contains
        [FilterOpSymbol(Symbol = "not {0}." + nameof(string.Contains) + "(@0)")]
        NotContains,

        //int & int == 0
        [FilterOpSymbol(Symbol = nameof(FilterOpSymbolAttribute._BitAndInstead) + "({0}, @0) == 0")]
        And,
        //int & int != 0
        [FilterOpSymbol(Symbol = nameof(FilterOpSymbolAttribute._BitAndInstead) + "({0}, @0) != 0")]
        NotAnd,

        //item in array
        [FilterOpSymbol(Symbol = "x")]
        Has,
        //item not in array
        [FilterOpSymbol(Symbol = "x")]
        Hasnt,
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    internal class FilterOpSymbolAttribute : Attribute {
        private static readonly Dictionary<FilterOp, FilterOpSymbolAttribute[]> OPS = new Dictionary<FilterOp, FilterOpSymbolAttribute[]>(Enum.GetValues(typeof(FilterOp)).Length * Enum.GetValues(typeof(SymbolVersion)).Length + 5);
        public string Symbol { get; set; }
        public SymbolVersion Version { get; set; } = SymbolVersion.Normal;
        public static string GetSymbol(FilterOp Op, SymbolVersion Ver = SymbolVersion.Normal) {
            return OPS.GetOrAdd(Op, () => typeof(FilterOp).GetMember(Op.ToString())[0].GetCustomAttributes<FilterOpSymbolAttribute>(false).ToArray()).FirstOrDefault(E => E.Version == Ver).Symbol;
        }
        public static int _BitAndInstead(int L, int R) => L & R;
        public static int? _BitAndInstead(int? L, int? R) => (!L.HasValue || !R.HasValue) ? null : L & R;
    }
    internal enum SymbolVersion {
        Normal,
        Nullable,
    }
    public struct Filter {
        public string Property { get; set; }
        public FilterOp Op { get; set; }
        public object Arg { get; set; }
        public IQueryable<T> Apply<T>(IQueryable<T> Query) {
            var EnT = typeof(T);
            var Props = Property.Split('.');
            PropertyInfo PnT = null;
            Props.ForEach(E => {
                PnT = PnT == null ? EnT.GetProperty(E) : PnT.PropertyType.GetProperty(E);
            });
#if !DYNAMIC_LINQ_Z
            var EnP = Expression.Parameter(EnT, "E");
            PropertyInfo PnT = null;
            MemberExpression TargetProp = null;
            Props.ForEach(E => {
                TargetProp = TargetProp == null ? Expression.Property(EnP, E) : Expression.Property(TargetProp, E);
                PnT = PnT == null ? EnT.GetProperty(E) : PnT.PropertyType.GetProperty(E);
            });
            var FilterMd = Kits.GetTargetFilterMethod(Op, TargetProp, PnT.PropertyType, Arg);
            var NotNullF = Expression.NotEqual(TargetProp, Expression.Constant(null));
            var L = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(NotNullF, FilterMd), EnP);
            return Query.Where(L);
#else
            //find specified expression string by Op and Version
            var Symb = FilterOpSymbolAttribute.GetSymbol(Op,Arg==null?SymbolVersion.Nullable:SymbolVersion.Normal);
            var TargetType = PnT.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(PnT.PropertyType) : PnT.PropertyType;
            //preprocess arg if it is not null
            if (Arg != null)
            {
                if (!PnT.PropertyType.Equals(Arg.GetType()))
                {
                    if (typeof(string).Equals(Arg.GetType()) || !typeof(System.Collections.IEnumerable).IsAssignableFrom(Arg.GetType()))
                        Arg = Convert.ChangeType(Arg, TargetType);
                    else
                        Arg = ((System.Collections.IEnumerable)Arg).Cast<object>().Select(E => E.ToString()).ToArray();
                }
            }
            //start to build expression
            string Exp = null;
            string __p = Property;
            //specified proc for has/hasnt of array,  --> "{} in @0" expression will throw 'expression need to return boolean' exception
            if (Op == FilterOp.Has)
            {
                Exp = string.Join("||", ((IEnumerable<string>)Arg).Select(E => $"{__p} == {E}"));
                return Query.Where(Exp);
            }
            else if(Op == FilterOp.Hasnt)
            {
                Exp = $"not ({string.Join("||", ((IEnumerable<string>)Arg).Select(E => $"{__p} == {E}"))})";
                return Query.Where(Exp);
            }
            //other expression didnt find exception temporarily
            else
            {
                //if arg is null
                if (Arg == null)
                {
                    Exp = string.Format(Symb, Property);
                    return Query.Where(Exp);
                }
                //if arg has value
                else
                {
                    Exp = string.Format(Symb, Property);
                    return Query.Where(Exp, Arg);
                }
            }
#endif
        }
    }
}
