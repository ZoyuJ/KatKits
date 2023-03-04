#define ADO


namespace KatKits
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;

    using KatKits.DB;

#if ADO
    using System.Data.SqlClient;
#endif
#if SQLITE
    using Microsoft.Data.Sqlite; 
#endif

    public static partial class DBAccessUtil
    {

#if SQLITE
        public static readonly Dictionary<Type, Action<SqliteCommand, object>> SQLParameterFillerCache = new Dictionary<Type, Action<SqliteCommand, object>>();
        public static void FillParameters(this SqliteCommand Command, object Object)
        {
            if (Object == null) return;
            var Tp = Object.GetType();
            var SQLCOMMANDTYPE = typeof(SqliteCommand);
            var SQLPARACOLLECTIONTYPE = typeof(SqliteParameterCollection);
            Action<SqliteCommand, object> CreateFunction()
            {
                var Attrs = QueryParamaterAttribute.FetchAttributes(Object.GetType());
                var InputPara = Expression.Parameter(typeof(object), "Input");
                var TypedInputPara = Expression.Convert(InputPara, Object.GetType());
                var InputCommand = Expression.Parameter(SQLCOMMANDTYPE, "Command");
                var Para1Var = Expression.Variable(typeof(SqliteParameter), "SQLPara1");
                var Body = new List<Expression>();
                Attrs.ToArray().ForEach(E =>
                {
                    if (E.Property.PropertyType.Equals(typeof(System.Data.DataTable)))
                    {
                        Body.Add(Expression.Assign(
                           Para1Var,
                           Expression.Call(
                             Expression.Property(
                               InputCommand,
                               SQLCOMMANDTYPE.GetProperties().FirstOrDefault(P => P.PropertyType == SQLPARACOLLECTIONTYPE && P.Name == nameof(SqliteCommand.Parameters))
                             ),
                             SQLPARACOLLECTIONTYPE.GetMethod(nameof(SqliteParameterCollection.AddWithValue)),
                             Expression.Constant(E.ParamaterName),
                             Expression.Convert(Expression.Call(null, typeof(Kits).GetMethod(nameof(Kits.ToXML)), Expression.Property(TypedInputPara, E.Property)), typeof(object))
                           )
                         ));
                    }
                    else if (E.Property.PropertyType.Equals(typeof(DateTime)))
                    {
                        Body.Add(Expression.Assign(
                           Para1Var,
                           Expression.Call(
                             Expression.Property(
                               InputCommand,
                               SQLCOMMANDTYPE.GetProperties().FirstOrDefault(P => P.PropertyType == SQLPARACOLLECTIONTYPE && P.Name == nameof(SqliteCommand.Parameters))
                             ),
                             SQLPARACOLLECTIONTYPE.GetMethod(nameof(SqliteParameterCollection.AddWithValue)),
                             Expression.Constant(E.ParamaterName),
                             Expression.Convert(Expression.Call(Expression.Property(TypedInputPara, E.Property), typeof(DateTime).GetMethod(nameof(DateTime.ToString)), Expression.Constant("yyyy-MM-dd HH:mm:ss.ffff")), typeof(object))
                           )
                         ));
                    }
                    else
                    {
                        Body.Add(Expression.Assign(
                             Para1Var,
                             Expression.Call(
                               Expression.Property(
                                 InputCommand,
                                 SQLCOMMANDTYPE.GetProperties().FirstOrDefault(P => P.PropertyType == SQLPARACOLLECTIONTYPE && P.Name == nameof(SqliteCommand.Parameters))
                               ),
                               SQLPARACOLLECTIONTYPE.GetMethod(nameof(SqliteParameterCollection.AddWithValue)),
                               Expression.Constant(E.ParamaterName),
                               Expression.Convert(Expression.Property(TypedInputPara, E.Property), typeof(object))
                             )
                           ));
                    }

                }

                );

                var Lam = Expression.Lambda<Action<SqliteCommand, object>>(
                  Expression.Block(
                    new ParameterExpression[] { Para1Var }, Body
                  ),
                  new ParameterExpression[] { InputCommand, InputPara }
                );
                return Lam.Compile();
            }
            SQLParameterFillerCache.GetOrAdd(Tp, CreateFunction)(Command, Object);
        } 
#endif

    }
}
