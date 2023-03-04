//# define DYNAMIC_LINQ_Z
#define EFCore6
//#define EF6

namespace KatKits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Text.RegularExpressions;

#if EFCore6
    using Microsoft.EntityFrameworkCore;
#elif EF6
    using System.Data.Entity;
    using System.Data.Linq.SqlClient;
#endif

    public static class LinqQueryUtil
    {
#if EF6
        private static readonly MethodInfo DBLikeExpressionMethodInfo = typeof(DbFunctions).GetMethods().FirstOrDefault(E => E.Name == nameof(DbFunctions.Like) && E.GetParameters().Length == 2); 
#elif EFCore6
        private static readonly MethodInfo DBLikeExpressionMethodInfo = typeof(DbFunctionsExtensions).GetMethods().FirstOrDefault(E => E.Name == nameof(DbFunctionsExtensions.Like) && E.GetParameters().Length == 3);
#endif

        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> Query, Filter Filter) => Filter.Apply(Query);
        internal static Expression GetTargetFilterMethod(this FilterOp Op, Expression PropertyExp, Type PropertyType, object Arg)
        {
            var ArgType = Arg.GetType();
            if (Arg == null) { }
            else if (ArgType.Equals(typeof(JArray)) && !ArgType.Equals(typeof(String)))
            {
                Arg = (Arg as JArray).ToObject(typeof(List<>).MakeGenericType(PropertyType));
            }
            else
            {
                if (PropertyType.IsNullableType())
                    Arg = Convert.ChangeType(Arg, Nullable.GetUnderlyingType(PropertyType));
                else
                    Arg = Convert.ChangeType(Arg, PropertyType);
            }
            Expression GetArgParameterExp(bool AllowNull = true, bool IsList = false)
            {
                if (Arg == null)
                {
                    if (AllowNull)
                        return Expression.Constant(null, PropertyType);
                    else throw new InvalidCastException($"Filter {Op}'s arg cant be null");
                }
                else if (IsList)
                {
                    //deal Arg as JArray
                    if (Arg == null) throw new InvalidCastException($"Filter {Op}'s arg cant be null");
                    return Expression.Constant(Arg);
                }
                else
                {
                    return Expression.Convert(Expression.Constant(Arg), PropertyType);
                }
            }
            switch (Op)
            {
                case FilterOp.Equal:
                    return Expression.Equal(PropertyExp, GetArgParameterExp());
                case FilterOp.NotEqual:
                    return Expression.NotEqual(PropertyExp, GetArgParameterExp());
                case FilterOp.Greate:
                    return Expression.GreaterThan(PropertyExp, GetArgParameterExp());
                case FilterOp.Less:
                    return Expression.LessThan(PropertyExp, GetArgParameterExp());
                case FilterOp.GreateOrEqual:
                    return Expression.GreaterThanOrEqual(PropertyExp, GetArgParameterExp());
                case FilterOp.LessOrEqual:
                    return Expression.LessThanOrEqual(PropertyExp, GetArgParameterExp());
                case FilterOp.StartsWith:
                    return Expression.Call(PropertyExp, typeof(string).GetMethod(nameof(string.StartsWith)), GetArgParameterExp());
                case FilterOp.NotStartsWith:
                    return Expression.Not(Expression.Call(PropertyExp, typeof(string).GetMethod(nameof(string.StartsWith)), GetArgParameterExp()));
                case FilterOp.EndsWith:
                    return Expression.Call(PropertyExp, typeof(string).GetMethod(nameof(string.EndsWith)), GetArgParameterExp());
                case FilterOp.NotEndsWith:
                    return Expression.Not(Expression.Call(PropertyExp, typeof(string).GetMethod(nameof(string.EndsWith)), GetArgParameterExp()));
                case FilterOp.Contains:
                    return Expression.Call(PropertyExp, typeof(string).GetMethod(nameof(string.Contains)), GetArgParameterExp());
                case FilterOp.NotContains:
                    return Expression.Not(Expression.Call(PropertyExp, typeof(string).GetMethod(nameof(string.Contains)), GetArgParameterExp()));
                case FilterOp.False:
                    return Expression.Equal(Expression.And(PropertyExp, GetArgParameterExp(false, false)), Expression.Constant(0));
                case FilterOp.True:
                    return Expression.Not(Expression.Equal(BinaryExpression.And(PropertyExp, GetArgParameterExp(false, false)), Expression.Constant(0)));
                case FilterOp.Has:
                    return Expression.Call(GetArgParameterExp(false, true), Arg.GetType().GetMethod(nameof(List<int>.Contains)), PropertyExp);
                case FilterOp.Hasnt:
                    return Expression.Not(Expression.Call(GetArgParameterExp(false, true), Arg.GetType().GetMethod(nameof(List<int>.Contains)), PropertyExp));
#if EF6
                case FilterOp.WildCards:
                    return Expression.Call(DBLikeExpressionMethodInfo, PropertyExp, GetArgParameterExp(false, false));
                case FilterOp.ExceptWildCards:
                    return Expression.Not(Expression.Call(DBLikeExpressionMethodInfo, PropertyExp, GetArgParameterExp(false, false))); 
#elif EFCore6
                case FilterOp.WildCards:
                    return Expression.Call(DBLikeExpressionMethodInfo, Expression.Constant(EF.Functions), PropertyExp, GetArgParameterExp(false, false));
                case FilterOp.ExceptWildCards:
                    return Expression.Not(Expression.Call(DBLikeExpressionMethodInfo, Expression.Constant(EF.Functions), PropertyExp, GetArgParameterExp(false, false)));
#endif
                default:
                    throw new InvalidOperationException("Filter Not Found");
            }
        }
        internal static Expression GetPropertyExpression(this Expression Exp, IEnumerable<string> PropertyPath)
        {
            return PropertyPath.Aggregate(Exp, Expression.Property);
        }

        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> Query, IEnumerable<Filter> Filters)
        {
            Filters.ForEach(E =>
            {
                Query = Query.ApplyFilter(E);
            });
            return Query;
        }
        public static IQueryable<T> ApplyGroupers<T>(this IQueryable<T> Query, IEnumerable<Grouper> Grouper)
        {
            return ApplySorters(Query, Grouper.Select(E => new Sorter { Op = E.Op, Property = E.Property }));
        }
        public static IQueryable<T> ApplySorters<T>(this IQueryable<T> Query, IEnumerable<Sorter> Sorter)
        {
            var Q2 = Query;
            Q2 = Sorter.First().ApplyFirst(Q2);
            Sorter.Skip(1).ForEach(S =>
            {
                Q2 = S.Apply(Q2);
            });
            return Q2;
        }

#if EF6
        public static IQueryable<T> WildCardsWhere<T>(this IQueryable<T> Query, string WildCardsPattern, string PropertyName, bool QueryMatched = true, bool IgnoreCase = true)
        {
            var Pat = WildCardsPattern.Replace("*", "%").Replace("?", "_");
            var OutputType = typeof(bool);
            var InputType = typeof(T);
            var InputPara = Expression.Parameter(InputType, "Row");
            var ReturnTarget = Expression.Label(OutputType);
            var Exp = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Call(
                        null,
                        DBLikeExpressionMethodInfo,
                            Expression.Property(InputPara, PropertyName),
                            Expression.Constant(Pat)
                    ),
                    Expression.Constant(QueryMatched)
                ),
                InputPara
                );
            return Query.Where(Exp);
        } 
#endif



    }

    public enum FilterOp
    {
        /// <summary>
        /// == for any basic data type and null
        /// </summary>
        Equal = 1,
        /// <summary>
        /// != for any basic data type and null
        /// </summary>
        NotEqual = -1,

        /// <summary>
        /// > for any basic data type and null
        /// </summary>
        Greate = 2,
        /// <summary>
        /// [<![CDATA[<]]> for any basic data type and null
        /// </summary>
        Less = 3,
        /// <summary>
        /// >= for any basic data type and null
        /// </summary>
        GreateOrEqual = -3,
        /// <summary>
        /// <= for any basic data type and null
        /// </summary>
        LessOrEqual = -2,

        /// <summary>
        /// string.StartsWith
        /// </summary>
        StartsWith = 6,
        /// <summary>
        /// !string.StartsWith
        /// </summary>
        NotStartsWith = -6,
        /// <summary>
        /// string.EndsWith
        /// </summary>
        EndsWith = 7,
        /// <summary>
        /// !string.EndsWith
        /// </summary>
        NotEndsWith = -7,
        /// <summary>
        /// string.Contains
        /// </summary>
        Contains = 8,
        /// <summary>
        /// !string.Contains
        /// </summary>
        NotContains = -8,

        /// <summary>
        /// bitwise and not equal 0
        /// </summary>
        True = 9,
        /// <summary>
        /// bitwise and equal 0
        /// </summary>
        False = -9,

        /// <summary>
        /// List.Contains
        /// </summary>
        Has = 10,
        /// <summary>
        /// !List.Contains
        /// </summary>
        Hasnt = -10,

        /// <summary>
        /// string Match with ?*
        /// </summary>
        WildCards = 11,
        /// <summary>
        /// string NOT Match with ?*
        /// </summary>
        ExceptWildCards = -11,
    }
    public class FilterItem
    {
        public FilterOp Op { get; set; }
        /// <summary>
        /// Basic data type or JArray
        /// </summary>
        public object Arg { get; set; }
        /// <summary>
        /// null: ignore next ,true: this && next , false: this || next
        /// </summary>
        public bool? AndOrWith { get; set; }
        public FilterItem Next { get; set; }

        public Expression CreateExp(Expression PropertyLocator)
        {
            if (Next == null || !AndOrWith.HasValue)
            {
                return LinqQueryUtil.GetTargetFilterMethod(Op, PropertyLocator, PropertyLocator.Type, Arg);
            }
            else
            {
                return AndOrWith.ValueOrDefault()
                        ? Expression.AndAlso(LinqQueryUtil.GetTargetFilterMethod(Op, PropertyLocator, PropertyLocator.Type, Arg), Next.CreateExp(PropertyLocator))
                        : Expression.OrElse(LinqQueryUtil.GetTargetFilterMethod(Op, PropertyLocator, PropertyLocator.Type, Arg), Next.CreateExp(PropertyLocator));
            }
        }

    }
    public class StringFilter : Filter
    {
        //public IQueryable<T> ApplyFilter<T>(IQueryable<T> Query,string PropertyName)
        //{
        //    switch (Op)
        //    {
        //        case FilterOp.Equal:
        //            return Query.Where(E => Selector(E) == Arg);
        //        case FilterOp.NotEqual:
        //            return Query.Where(E => Selector(E) != Arg);
        //        case FilterOp.StartsWith:
        //            return Query.Where(E => Selector(E).StartsWith(Arg??""));
        //        case FilterOp.NotStartsWith:
        //            return Query.Where(E => !Selector(E).StartsWith(Arg??""));
        //        case FilterOp.EndsWith:
        //            return Query.Where(E => Selector(E).EndsWith(Arg ?? ""));
        //        case FilterOp.NotEndsWith:
        //            return Query.Where(E => !Selector(E).EndsWith(Arg ?? ""));
        //        case FilterOp.Contains:
        //            return Query.Where(E => Selector(E).Contains(Arg ?? ""));
        //        case FilterOp.NotContains:
        //            return Query.Where(E => !Selector(E).Contains(Arg ?? ""));
        //        case FilterOp.WildCards:
        //            return Query.WildCardsWhere(Arg??"*", PropertyName);
        //        case FilterOp.ExceptWildCards:
        //            return Query.WildCardsWhere(Arg ?? "*", PropertyName, false);
        //        default:
        //            return Query;
        //    }


        //}
    }
    public class Filter : FilterItem
    {
        public string Property { get; set; }
        public IQueryable<T> Apply<T>(IQueryable<T> Query)
        {
            var EnT = typeof(T);
            var Props = Property.Split('.');

            var EnP = Expression.Parameter(EnT, "E");
            var TargetProp = EnP.GetPropertyExpression(Props);
            var FilterMd = CreateExp(TargetProp);
            var L = Expression.Lambda<Func<T, bool>>(FilterMd, EnP);
            //return Query.Provider.CreateQuery<T>(Expression.Call(typeof(Queryable), nameof(Queryable.Where), new[] { EnT, TargetProp.Type }, Query.Expression, L));
            return Query.Where(L);

        }
    }
    public class Pager
    {
        public int? PerviousItemCountPerPage { get; set; }
        public int? PerviousPageNumber { get; set; }
        public int ItemCountPerPage { get; set; }
        public int RequirePageNumber { get; set; }
        public int TotalItemCount { get; set; }
        public int Skip
        {
            get => PerviousItemCountPerPage.HasValue
                  ? (PerviousPageNumber ?? 0) * PerviousItemCountPerPage.Value
                  : (RequirePageNumber - 1) * ItemCountPerPage;
        }
        public IQueryable<T> TakePageItems<T>(IQueryable<T> Query)
        {
            FillPageProperty(Query);
            return Query.Skip(Skip).Take(ItemCountPerPage);
        }
        public void FillPageProperty<T>(IQueryable<T> Query)
        {
            this.TotalItemCount = Query.Count();
        }
    }
    public class Sorter
    {
        public string Property { get; set; }
        public SorterOp Op { get; set; }
        public IQueryable<T> Apply<T>(IQueryable<T> Query)
        {
            var EnT = typeof(T);
            var Props = Property.Split('.');
            var ParaExp = Expression.Parameter(EnT, "E");
            var PropExp = ParaExp.GetPropertyExpression(Props);
            var OrderMethodName = Op == SorterOp.ASC ? nameof(Queryable.ThenBy) : nameof(Queryable.ThenByDescending);
            return Query.Provider.CreateQuery<T>(Expression.Call(typeof(Queryable), OrderMethodName, new[] { EnT, PropExp.Type }, Query.Expression, Expression.Lambda(PropExp, ParaExp)));
        }

        public IQueryable<T> ApplyFirst<T>(IQueryable<T> Query)
        {
            var EnT = typeof(T);
            var Props = Property.Split('.');
            var ParaExp = Expression.Parameter(EnT, "E");
            var PropExp = ParaExp.GetPropertyExpression(Props);
            var OrderMethodName = Op == SorterOp.ASC ? nameof(Queryable.OrderBy) : nameof(Queryable.OrderByDescending);
            return Query.Provider.CreateQuery<T>(Expression.Call(typeof(Queryable), OrderMethodName, new[] { EnT, PropExp.Type }, Query.Expression, Expression.Lambda(PropExp, ParaExp)));
        }
    }
    public enum SorterOp
    {
        Raw = 0,
        ASC,
        DESC,
    }
    public class Grouper
    {
        public string Property { get; set; }
        public SorterOp Op { get; set; }
    }



}
