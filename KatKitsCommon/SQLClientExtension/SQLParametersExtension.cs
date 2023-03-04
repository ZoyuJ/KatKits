#define MSSQL
//#define SQLITE

namespace KatKits.SQLClientExtension {
  using System;
  using System.Collections.Generic;
  using System.Data;
#if MSSQL
  using System.Data.SqlClient;
#endif
#if SQLITE
  using Microsoft.Data.Sqlite;
#endif
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;
  using global::KatKits.ImplementExtension.CollectionExtension;

  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public sealed class QueryParamaterAttribute : Attribute {

    public string TableName { get; set; }
    public string ParamaterName { get; set; }
    //public int? Size { get; set; }
    public PropertyInfo Property { get; private set; }
    public static IEnumerable<QueryParamaterAttribute> FetchAttributes<T>() => FetchAttributes(typeof(T));
    public static IEnumerable<QueryParamaterAttribute> FetchAttributes(Type Type) {

      if (Attributes.TryGetValue(Type, out var Attrs)) {
        return Attrs;
      }
      else {
        //var TableName = (Type.GetCustomAttribute<TableNameAttribute>()?.TableName) ?? (Type.GetCustomAttribute<TableAttribute>()?.Name);
        var AttrsA = Type.GetProperties()
             .Where(E => E.PropertyType.IsBasicDataType() || E.PropertyType.Equals(typeof(DataTable)))
             .Select(E => {
               var A = E.GetCustomAttribute<QueryParamaterAttribute>();
               if (A != null) {
                 A.Property = E;
                 if (A.ParamaterName == null) A.ParamaterName = A.Property.Name;
               }
               return A;
             })
             .Where(E => E != null).ToArray();
        if (AttrsA.Length == 0) {
          AttrsA = Type.GetProperties()
             .Where(E => E.PropertyType.IsBasicDataType() || E.PropertyType.Equals(typeof(DataTable)))
             .Select(E => new QueryParamaterAttribute() { Property = E, ParamaterName = E.Name })
             .ToArray();
        }
        Attributes.Add(Type, AttrsA);
        return AttrsA;
      }
    }
    private static readonly Dictionary<Type, IEnumerable<QueryParamaterAttribute>> Attributes = new Dictionary<Type, IEnumerable<QueryParamaterAttribute>>();

  }
  [Obsolete]
  public enum SQLParameterScheme {
    Unknow = 0,
    MSSQLServer = 1,
    MySQL_INNODB = 2,
    Orcale = 3,
  }
  public static class Kits {

#if MSSQL
    internal static readonly Dictionary<Type, Action<SqlCommand, object>> _Cache_MSSQL_Obj2SQLParamaters = new Dictionary<Type, Action<SqlCommand, object>>();
    public static void FillMSSQLParamaters(this SqlCommand Command, object Object) {
      if (Object == null) return;
      var Tp = Object.GetType();
      Action<SqlCommand, object> CreateFunction() {
        var Attrs = QueryParamaterAttribute.FetchAttributes(Object.GetType());
        var InputPara = Expression.Parameter(typeof(object), "Input");
        var TypedInputPara = Expression.Convert(InputPara, Object.GetType());
        var InputCommand = Expression.Parameter(typeof(SqlCommand), "Command");
        var Para1Var = Expression.Variable(typeof(SqlParameter), "SQLPara1");
        var Body = new List<Expression>();
        Attrs.ToArray().ForEach(E => {

          Body.Add(Expression.Assign(
            Para1Var,
            Expression.Call(
              Expression.Property(
                InputCommand,
                typeof(SqlCommand).GetProperties().FirstOrDefault(P => P.PropertyType == typeof(SqlParameterCollection) && P.Name == nameof(SqlCommand.Parameters))
              ),
              typeof(SqlParameterCollection).GetMethod(nameof(SqlParameterCollection.AddWithValue)),
              Expression.Constant(E.ParamaterName),
              Expression.Convert(Expression.Property(TypedInputPara, E.Property), typeof(object))
            )
          ));
          if (E.Property.PropertyType.Equals(typeof(System.Data.DataTable))) {
            Body.Add(
              Expression.Assign(
                Expression.Property(
                  Para1Var,
                  typeof(SqlParameter).GetProperty(nameof(SqlParameter.SqlDbType))
                ),
                Expression.Constant(SqlDbType.Structured)
              )
            );
            if (E.TableName != null)
              Body.Add(
                Expression.Assign(
                  Expression.Property(
                    Para1Var,
                    typeof(SqlParameter).GetProperty(nameof(SqlParameter.TypeName))
                  ),
                  Expression.Constant(E.TableName)
                )
              );
          }

        }

        );

        var Lam = Expression.Lambda<Action<SqlCommand, object>>(
          Expression.Block(
            new ParameterExpression[] { Para1Var }, Body
          ),
          new ParameterExpression[] { InputCommand, InputPara }
        );
        return Lam.Compile();
      }

      _Cache_MSSQL_Obj2SQLParamaters.GetOrAdd(Tp, CreateFunction)(Command, Object);

    }
#endif

#if SQLITE
    public static readonly Dictionary<Type, Action<SqliteCommand, object>> _Cache_SQLITE_Obj2SQLParamaters = new Dictionary<Type, Action<SqliteCommand, object>>();
    public static void FillSQLITEParameters(this SqliteCommand Command, object Object) {
      if (Object == null) return;
      var Tp = Object.GetType();
      var SQLCOMMANDTYPE = typeof(SqliteCommand);
      var SQLPARACOLLECTIONTYPE = typeof(SqliteParameterCollection);
      Action<SqliteCommand, object> CreateFunction() {
        var Attrs = QueryParamaterAttribute.FetchAttributes(Object.GetType());
        var InputPara = Expression.Parameter(typeof(object), "Input");
        var TypedInputPara = Expression.Convert(InputPara, Object.GetType());
        var InputCommand = Expression.Parameter(SQLCOMMANDTYPE, "Command");
        var Para1Var = Expression.Variable(typeof(SqliteParameter), "SQLPara1");
        var Body = new List<Expression>();
        Attrs.ToArray().ForEach(E => {
          if (E.Property.PropertyType.Equals(typeof(System.Data.DataTable))) {
            Body.Add(Expression.Assign(
               Para1Var,
               Expression.Call(
                 Expression.Property(
                   InputCommand,
                   SQLCOMMANDTYPE.GetProperties().FirstOrDefault(P => P.PropertyType == SQLPARACOLLECTIONTYPE && P.Name == nameof(SqliteCommand.Parameters))
                 ),
                 SQLPARACOLLECTIONTYPE.GetMethod(nameof(SqliteParameterCollection.AddWithValue)),
                 Expression.Constant(E.ParamaterName),
                 Expression.Convert(Expression.Call(null, typeof(StructedDataExtension.Kits).GetMethod(nameof(StructedDataExtension.Kits.ToXML)), Expression.Property(TypedInputPara, E.Property)), typeof(object))
               )
             ));
          }
          else if (E.Property.PropertyType.Equals(typeof(DateTime))) {
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
          else {
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
      _Cache_SQLITE_Obj2SQLParamaters.GetOrAdd(Tp, CreateFunction)(Command, Object);
    }
#endif


  }
}
