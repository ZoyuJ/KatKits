#if ADO_SQLCLIENT
#if DATATABLE_SUPPORT
namespace KatKits
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data;
  using System.Data.SqlClient;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;

  using static global::KatKits.KatKits;

  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public class ColumnMapAttribute : Attribute
  {
    /// <summary>
    /// if the xls sheet has headers
    /// </summary>
    public string XLSColumnName { get; set; }
    /// <summary>
    /// DataTable column name
    /// </summary>
    public string TableColumnName { get; set; }
    /// <summary>
    /// an order in DataTable
    /// </summary>
    public int TableColumnOrder { get; set; }
    /// <summary>
    /// start at one,the column index in xls sheet
    /// </summary>
    public int XLSColumnIndex { get; set; }
    public bool AllowNull { get; set; }
    /// <summary>
    /// keep null or ignore when the type cannot set as a constant such as DateTime or Nullable<T>
    /// </summary>
    public object DefaultValue { get; set; } = null;
    /// <summary>
    /// ignore in attribute attaching
    /// </summary>
    public Type PropertyType { get; internal set; }
    /// <summary>
    /// ignore in attribute attaching
    /// </summary>
    public string PropertyName { get; internal set; }

    internal static IEnumerable<PropertyAndAttribute> FetchPropertiesAndAttributes(Type InputType)
    {
      IEnumerable<PropertyAndAttribute> Fetch()
      {
        return InputType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
      .Select(E => new PropertyAndAttribute { Property = E, Attribute = E.GetCustomAttributes(typeof(ColumnMapAttribute), false).Cast<ColumnMapAttribute>().FirstOrDefault() })
      .Where(E => E.Property.CanWrite && E.Property.CanRead && E.Attribute != null)
      .Where(E => E.Property.PropertyType.IsBasicDataTypeOrNullable())
      .Select(E =>
      {
        E.Attribute.PropertyName = E.Property.Name;
        E.Attribute.TableColumnName = E.Attribute.TableColumnName ?? (E.Property.GetCustomAttribute<ColumnAttribute>()?.Name);
        if (E.Property.PropertyType.IsNullableType()) E.Attribute.AllowNull = true;
        E.Attribute.PropertyType = E.Property.PropertyType;
        E.Attribute.DefaultValue = E.Attribute.DefaultValue ?? (E.Property.GetCustomAttribute<DefaultValueAttribute>()?.Value) ?? E.Property.PropertyType.DefaultBasicDataTypeValue();
        if (E.Property.PropertyType.IsEnum)
        {

        }
        return E;
      })
      .OrderBy(E => E.Attribute.TableColumnOrder);
      }
      if (InputType == null) return null;
      else
      {
        DataExchangeCacheEntity Funct = null;
        if (!DataExchangeEntities.TryGetValue(InputType, out Funct))
        {
          Funct = new DataExchangeCacheEntity();
          DataExchangeEntities.Add(InputType, Funct);
        }
        if (Funct.Obj2Dict == null)
        {
          Funct.Attributes = Fetch();
        }
        return Funct.Attributes;
      }

    }

    [Obsolete("use Kits.FetchPropertiesAndAttributes instead", true)]
    public static IEnumerable<ColumnMapAttribute> FromModel<T>() => FromModel(typeof(T));
    [Obsolete("use Kits.FetchPropertiesAndAttributes instead", true)]
    public static IEnumerable<ColumnMapAttribute> FromModel(Type ModelType)
    {

      return ModelType.GetProperties()
        .Select(E => new { Property = E, Attribute = E.GetCustomAttributes(typeof(ColumnMapAttribute), false).Cast<ColumnMapAttribute>().FirstOrDefault() })
        .Where(E => E.Attribute != null)
        .Where(E => E.Property.PropertyType.IsPrimitive
                || E.Property.PropertyType.IsEnum
                || E.Property.PropertyType.Equals(typeof(string))
                || E.Property.PropertyType.Equals(typeof(decimal))
                || E.Property.PropertyType.Equals(typeof(DateTime))
                )
        .OrderBy(E => E.Attribute.TableColumnOrder)
        .Select(E =>
        {
          E.Attribute.DefaultValue = E.Attribute.DefaultValue ?? E.Property.GetCustomAttributes(typeof(DefaultValueAttribute), false).Cast<DefaultValueAttribute>().FirstOrDefault()?.Value ?? E.Attribute.DefaultValue;
          E.Attribute.PropertyType = E.Property.PropertyType;
          E.Attribute.PropertyName = E.Property.Name;
          return E.Attribute;
        });
    }
    [Obsolete("use Kits.TypeToDataTable instead", true)]
    public static DataTable GenerateDataTable<T>() => GenerateDataTable(typeof(T));
    [Obsolete("use Kits.TypeToDataTable instead", true)]
    public static DataTable GenerateDataTable(Type ModelType)
    {
      var Table = new DataTable();
      var ColumnMap = FromModel(ModelType);
      ColumnMap.ToArray().ForEach(E =>
      {
        var Col = Table.Columns.Add(E.TableColumnName ?? E.PropertyName, E.PropertyType);
        Col.AllowDBNull = E.AllowNull;
        Col.DefaultValue = E.DefaultValue;
      });
      return Table;
    }
    //public static IEnumerable<T> FromDataTable<T>(DataTable Table,int Skip,Func<string, object,bool> Filter = null)
    //{
    //  var ColumnMap = FromModel<T>().ToArray();
    //  var ColumnMap2Table = ColumnMap.Join(
    //    Table.Columns.Cast<DataColumn>(),
    //    L => L.TableColumnName ?? L.PropertyName,
    //    R => R.ColumnName,
    //    (L, R) => new { Attribute = L, Column = R }
    //  );

    //  var RowDict = Table.Rows.Cast<DataRow>()
    //    .Skip(Skip)
    //    .Select(E => ColumnMap2Table.ToDictionary(M => M.Attribute.PropertyName, M => E[M.Column]));
    //  if (Filter != null) RowDict = RowDict.Where(E => E.All(EE => Filter(EE.Key, EE.Value)));
    //  return RowDict.Select(E => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(E)));
    //}
    //public static DataTable WriteToDataTable<T>(DataTable Table,IEnumerable<T> Items,bool WriteHeader)
    //{
    //  //Items.Select(E=>)
    //  return Table;
    //}
  }
  public class PropertyAndAttribute
  {
    public PropertyInfo Property { get; set; }
    public ColumnMapAttribute Attribute { get; set; }
  }

  //DataTable <-> IEnumerable<T>
  public static partial class KatKits
  {
    internal class DataExchangeCacheEntity
    {
      public IEnumerable<PropertyAndAttribute> Attributes;
      public Func<object, IDictionary<string, object>> Obj2Dict;
      public Func<Type, DataTable> Type2DataTable;
      public Func<DataTable, IEnumerable, DataTable> Enumerable2DataTable;
      public Func<DataTable, IEnumerable> DataTable2Enumerable;
      public Action<SqlCommand, object> Obj2SQLParamaters;
      public Func<DataRow, object> DataRow2Object;
    }
    internal static readonly MethodInfo AddToDictionaryMethod = typeof(IDictionary<string, object>).GetMethod(nameof(IDictionary.Add));
    internal static readonly ConstructorInfo DictionaryConstructor = typeof(Dictionary<string, object>).GetConstructors().FirstOrDefault(c => c.IsPublic && !c.GetParameters().Any());
    internal static readonly MethodInfo AddToListMethod = typeof(List<object>).GetMethod(nameof(List<object>.Add));
    internal static readonly ConstructorInfo ListConstructor = typeof(List<object>).GetConstructors().FirstOrDefault(c => c.IsPublic && !c.GetParameters().Any());
    internal static readonly MethodInfo AddColumnToDataTable = typeof(DataColumnCollection).GetMethods().FirstOrDefault(E => E.Name == nameof(DataColumnCollection.Add) && E.GetParameters().Length == 2);
    internal static readonly MethodInfo AddRowToDataTable = typeof(DataRowCollection).GetMethods().FirstOrDefault(E => E.Name == nameof(DataRowCollection.Add) && E.GetParameters().FirstOrDefault().Name == "values");
    internal static readonly PropertyInfo ColumnAllowNull = typeof(DataColumn).GetProperty(nameof(DataColumn.AllowDBNull));
    internal static readonly PropertyInfo ColumnDefaultValue = typeof(DataColumn).GetProperty(nameof(DataColumn.DefaultValue));

    internal static readonly Dictionary<Type, DataExchangeCacheEntity> DataExchangeEntities = new Dictionary<Type, DataExchangeCacheEntity>();




    /// <summary>
    /// Convert Obejct 2 Dictionary
    /// </summary>
    /// <param name="This"></param>
    /// <returns></returns>
    public static IDictionary<string, object> ObjectToDictionary(this object This)
    {
      Func<object, IDictionary<string, object>> CreateFunction()
      {
        var OutputType = typeof(IDictionary<string, object>);
        var InputType = This.GetType();
        var InputExpression = Expression.Parameter(typeof(object), "input");
        var TypedInputExpression = Expression.Convert(InputExpression, InputType);
        var OutputVariable = Expression.Variable(OutputType, "output");
        var ReturnTarget = Expression.Label(OutputType);
        var Body = new List<Expression> { Expression.Assign(OutputVariable, Expression.New(DictionaryConstructor)) };
        Body.AddRange(
          FetchPropertiesAndAttributes(InputType)
          .Select(E => Expression.Call(OutputVariable, AddToDictionaryMethod, Expression.Constant(E.Property.Name), Expression.Convert(Expression.Property(TypedInputExpression, E.Property.GetMethod), typeof(object))))
          );
        Body.Add(Expression.Return(ReturnTarget, OutputVariable));
        Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));
        var LambdaExpression = Expression.Lambda<Func<object, IDictionary<string, object>>>(
            Expression.Block(new[] { OutputVariable }, Body),
            InputExpression);
        return LambdaExpression.Compile();
      }
      if (This == null) return null;
      else
      {
        DataExchangeCacheEntity Funct = null;
        if (!DataExchangeEntities.TryGetValue(This.GetType(), out Funct))
        {
          Funct = new DataExchangeCacheEntity();
          DataExchangeEntities.Add(This.GetType(), Funct);
        }
        if (Funct.Obj2Dict == null)
        {
          Funct.Obj2Dict = CreateFunction();
        }
        return Funct.Obj2Dict(This);
      }
    }

    /// <summary>
    /// Generate DataTable From Type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static DataTable TypeToDataTable<T>() where T : new() => TypeToDataTable(typeof(T));
    /// <summary>
    /// Generate DataTable From Type
    /// </summary>
    /// <param name="This"></param>
    /// <returns></returns>
    public static DataTable TypeToDataTable(this Type This)
    {
      if (This == null) return null;
      if (This.IsBasicDataType()) return null;
      Func<Type, DataTable> CreateFunction()
      {
        var InputType = This;
        var OutputType = typeof(DataTable);
        var InputExpression = Expression.Parameter(typeof(object), "input");
        var TypedInputExpression = Expression.Convert(InputExpression, InputType);
        var OutputVariable = Expression.Variable(OutputType, "output");
        var ColumnsVariable = Expression.Property(OutputVariable, typeof(DataTable).GetProperty(nameof(DataTable.Columns)));
        var ReturnTarget = Expression.Label(OutputType);
        var DataTableConstructor = OutputType.GetConstructors().FirstOrDefault(E => E.IsPublic && !E.GetParameters().Any());
        var Body = new List<Expression> { Expression.Assign(OutputVariable, Expression.New(DataTableConstructor)) };
        var Col = Expression.Variable(typeof(DataColumn), "Col");
        Body.AddRange(
          FetchPropertiesAndAttributes(InputType)
          .Select(E =>
          {
            return Expression.Block(new ParameterExpression[] { Col },
              Expression.Assign(Col, Expression.Call(ColumnsVariable, AddColumnToDataTable, Expression.Constant(E.Attribute.TableColumnName ?? E.Property.Name), Expression.Constant(E.Property.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(E.Property.PropertyType) : E.Property.PropertyType))),
              Expression.Assign(Expression.Property(Col, ColumnAllowNull), Expression.Constant(E.Attribute.AllowNull)),
              Expression.Assign(Expression.Property(Col, ColumnDefaultValue), Expression.Convert(Expression.Constant(E.Attribute.DefaultValue), typeof(object)))
              );
          })
        );
        Body.Add(Expression.Return(ReturnTarget, OutputVariable));
        Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));
        var LambdaExpression = Expression.Lambda<Func<object, DataTable>>(
                Expression.Block(new[] { OutputVariable }, Body),
                InputExpression);
        return LambdaExpression.Compile();
      }
      DataExchangeCacheEntity Funct = null;
      if (!DataExchangeEntities.TryGetValue(This, out Funct))
      {
        Funct = new DataExchangeCacheEntity();
        DataExchangeEntities.Add(This, Funct);
      }
      if (Funct.Type2DataTable == null)
        Funct.Type2DataTable = CreateFunction();
      return Funct.Type2DataTable(This);
    }

    /// <summary>
    /// Save IEnumerable<T> To DataTable 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="This"></param>
    /// <returns></returns>
    public static DataTable EnumerableToDataTable<T>(this IEnumerable<T> This, DataTable Table = null) where T : new()
    {
      Func<DataTable, IEnumerable, DataTable> CreateFunction()
      {
        var OutputType = typeof(DataTable);

        var RowsProperty = typeof(DataTable).GetProperty(nameof(DataTable.Rows));

        var InputParaType = Expression.Parameter(typeof(Type), "_Type");
        var InputParaTable = Expression.Parameter(typeof(DataTable), "Table");
        var TypedInputParaTable = Expression.Convert(InputParaTable, typeof(DataTable));
        var InputParaEnumable = Expression.Parameter(typeof(IEnumerable), "Items");
        var TypedInputParaEnumerable = Expression.Convert(InputParaEnumable, typeof(IEnumerable));


        var OutputVariable = Expression.Variable(OutputType, "output");

        var ReturnTarget = Expression.Label(OutputType);

        var Vars = FetchPropertiesAndAttributes(typeof(T)).ToArray();

        var LoopItem = Expression.Variable(typeof(object), "Item");

        var Body = new List<Expression> {
          Expression.Assign(OutputVariable ,InputParaTable),
        };
        Body.Add(
          ForEach(
            TypedInputParaEnumerable,
            LoopItem,
            Expression.Call(
              Expression.Property(TypedInputParaTable, RowsProperty),
              AddRowToDataTable,
              Expression.NewArrayInit(
                typeof(object),
                Vars.Select(V =>
                  Expression.Convert(Expression.Property(Expression.Convert(LoopItem, typeof(T)), V.Property.Name), typeof(object))
                )
              )
            )
          )
        );
        Body.Add(Expression.Return(ReturnTarget, OutputVariable));
        Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));
        var LambdaExpression = Expression.Lambda<Func<DataTable, IEnumerable, DataTable>>(
               Expression.Block(new[] { OutputVariable }, Body),
               InputParaTable, InputParaEnumable);
        return LambdaExpression.Compile();
      }
      var Items = This.ToArray();
      if (Table == null) Table = TypeToDataTable<T>();

      DataExchangeCacheEntity Funct = null;
      if (!DataExchangeEntities.TryGetValue(typeof(T), out Funct))
      {
        Funct = new DataExchangeCacheEntity();
        DataExchangeEntities.Add(typeof(T), Funct);
      }
      if (Funct.Enumerable2DataTable == null)
        Funct.Enumerable2DataTable = CreateFunction();
      return Funct.Enumerable2DataTable(Table, Items);

    }

    private readonly static Dictionary<Type, Func<IEnumerable, DataTable, DataTable>> GenericDataTableWriters = new Dictionary<Type, Func<IEnumerable, DataTable, DataTable>>();
    public static DataTable GenericEnumerableToDataTable<T>(this IEnumerable<T> This, DataTable Table = null) where T : new()
    {
      var Items = This.ToArray();
      Table = Table ?? TypeToDataTable<T>();

      //if(GenericDataTableWriters)

      var OutputType = typeof(DataTable);

      var RowsProperty = typeof(DataTable).GetProperty(nameof(DataTable.Rows));

#region Inline Delegate
      var InputParaType = Expression.Parameter(typeof(Type), "_Type");
      var InputParaTable = Expression.Parameter(typeof(DataTable), "Table");
      var TypedInputParaTable = Expression.Convert(InputParaTable, typeof(DataTable));
      var InputParaEnumable = Expression.Parameter(typeof(IEnumerable<T>), "Items");
      var TypedInputParaEnumerable = Expression.Convert(InputParaEnumable, typeof(IEnumerable<T>));

      var OutputVariable = Expression.Variable(OutputType, "output");
      var ReturnTarget = Expression.Label(OutputType);
      var Vars = FetchPropertiesAndAttributes(typeof(T)).ToArray();
      var LoopItem = Expression.Variable(typeof(T), "Item");

      var Body = new List<Expression> {
          Expression.Assign(OutputVariable ,InputParaTable),
        };
      Body.Add(
        ForEachGeneric(
          InputParaEnumable,
          LoopItem,
          Expression.Call(
            Expression.Property(TypedInputParaTable, RowsProperty),
            AddRowToDataTable,
              Expression.NewArrayInit(
                typeof(object),
                Vars.Select(V =>
                  Expression.Convert(Expression.Property(LoopItem, V.Property.Name), typeof(object))
                )
              )
          )
        )
      );
      Body.Add(Expression.Return(ReturnTarget, OutputVariable));
      Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));
      var LambdaExpression = Expression.Lambda<Func<DataTable, IEnumerable<T>, DataTable>>(
             Expression.Block(new[] { OutputVariable }, Body),
             InputParaTable, InputParaEnumable);
      var InlineFunc = LambdaExpression.Compile();
#endregion
      var DelegateIns = typeof(T).GetFields().FirstOrDefault(E => E.IsStatic && E.Name == nameof(GenericEnumerableToDataTable));
      DelegateIns.SetValue(null, InlineFunc);

      var WrapedInputParaTable = Expression.Parameter(typeof(DataTable), "Table");
      var WrapedInputEnumerable = Expression.Parameter(typeof(IEnumerable), "Items");

      var OutputVariable2 = Expression.Variable(OutputType, "output");
      var ReturnTarget2 = Expression.Label(OutputType);
      //var DynamicCall = InlineFunc.GetType().GetMethod(nameof(Delegate.DynamicInvoke));
      var GenFunc = typeof(Func<>).MakeGenericType(typeof(DataTable), typeof(IEnumerable<T>), typeof(DataTable));
      var Call = GenFunc.GetMethod("Invoke");

      var Body2 = new List<Expression>{
        Expression.Assign(OutputVariable2,
          Expression.Invoke(
            Expression.Convert(Expression.Field(null,DelegateIns),GenFunc),
            WrapedInputParaTable,WrapedInputEnumerable
          )
        )
      };







      return InlineFunc(Table, This);
    }

    private static readonly Dictionary<Type, Func<DataTable, IEnumerable>> DataTableToArrayConverters = new Dictionary<Type, Func<DataTable, IEnumerable>>();
    /// <summary>
    /// Read DataTable as IEnumerable<T>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Table"></param>
    /// <returns></returns>
    public static IEnumerable<T> DataTableToIEnumerable<T>(this DataTable Table) where T : new()
    {
      Func<DataTable, IEnumerable> CreateFunction()
      {
        var InputType = typeof(DataTable);
        var OutputType = typeof(List<T>);
        var InputParaTable = Expression.Parameter(InputType, "Table");
        var OutputVariable = Expression.Variable(OutputType, "output");
        var RowsProperty = typeof(DataTable).GetProperty(nameof(DataTable.Rows));
        var ReturnTarget = Expression.Label(OutputType);
        var BreakLabel = Expression.Label(typeof(int));
        var Vars = FetchPropertiesAndAttributes(typeof(T)).ToArray();
        var LoopItem = Expression.Variable(typeof(object), "Row");
        var Items = Expression.Variable(typeof(List<T>), "Items");
        var Body = new List<Expression> {
          Expression.Assign(Items,Expression.New(typeof(List<T>).GetConstructors().FirstOrDefault(E=>E.IsPublic && E.GetParameters().Length == 0))),
          Expression.Assign(OutputVariable,Items)
        };
        var ModelConstructor = typeof(T).GetConstructors().FirstOrDefault(c => c.IsPublic && !c.GetParameters().Any());

        var Item = Expression.Variable(typeof(T), "Item");

#if FRAMEWORK
        var _W = new Expression[] {
                  Expression.Assign(Item,Expression.New(ModelConstructor)),
                  Expression.Call(Items,typeof(List<T>).GetMethod("Add"),Item),
                }
               .Concat(Vars.Select(E =>
                   Expression.Assign(
                     Expression.Property(Item, E.Property),
                     Expression.Convert(
                       Expression.Call(null,
                         typeof(DataRowExtensions).GetMethods().FirstOrDefault(M => M.IsGenericMethod && M.Name == nameof(DataRowExtensions.Field)).MakeGenericMethod(E.Property.PropertyType),
                         Expression.Convert(LoopItem, typeof(DataRow)),
                         Expression.Constant(E.Attribute.TableColumnName ?? E.Property.Name)
                       ),
                       E.Property.PropertyType
                     )
                   )
                 )
               ).ToArray(); 
#elif STANDARD
        var _W = new Expression[] {
                  Expression.Assign(Item,Expression.New(ModelConstructor)),
                  Expression.Call(Items,typeof(List<T>).GetMethod("Add"),Item),
                }
          .Concat(Vars.Select(E =>
              Expression.Assign(
                Expression.Property(Item, E.Property),
                Expression.Convert(
                  Expression.Property(
                    Expression.Convert(LoopItem, typeof(DataRow)),
                    typeof(DataRow).FindIndexers(typeof(string),typeof(object)).Single(),
                    Expression.Constant(E.Attribute.TableColumnName ?? E.Property.Name)
                  ),
                  E.Property.PropertyType
                )
              )
            )
          ).ToArray();
#endif

        Body.Add(
          ForEach(
              Expression.Property(InputParaTable, RowsProperty),
              LoopItem,
              Expression.Block(_W)
          )
        );
        Body.Add(Expression.Return(ReturnTarget, OutputVariable));
        Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));
        var LambdaExpression = Expression.Lambda<Func<DataTable, IEnumerable>>(
               Expression.Block(new[] { OutputVariable, Items, Item }, Body),
               InputParaTable);
        return LambdaExpression.Compile();
      }

      DataExchangeCacheEntity Funct = null;
      if (!DataExchangeEntities.TryGetValue(typeof(T), out Funct))
      {
        Funct = new DataExchangeCacheEntity();
        DataExchangeEntities.Add(typeof(T), Funct);
      }
      if (Funct.DataTable2Enumerable == null)
        Funct.DataTable2Enumerable = CreateFunction();
      return Funct.DataTable2Enumerable(Table).Cast<T>();


    }
    /// <summary>
    /// yield return datatable row
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Table"></param>
    /// <returns></returns>
    public static IEnumerable<T> YieldTo<T>(this DataTable Table) where T : new()
    {
      return Table.Rows.Cast<DataRow>().YieldTo<T>();
    }
    /// <summary>
    /// yield return datatable row
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Rows"></param>
    /// <returns></returns>
    public static IEnumerable<T> YieldTo<T>(this IEnumerable<DataRow> Rows)
    {
      Func<DataRow, object> DataTableYieldToObjectsCreateFunction()
      {
        var InputType = typeof(DataRow);
        var OutputType = typeof(T);
        var InputPara = Expression.Parameter(InputType, "Row");
        var OutputVariable = Expression.Variable(OutputType, "output");
        //var RowsProperty = typeof(DataTable).GetProperty(nameof(DataTable.Rows));
        var ReturnTarget = Expression.Label(OutputType);
        //var BreakLabel = Expression.Label(typeof(int));
        var Vars = FetchPropertiesAndAttributes(typeof(T)).ToArray();
        //var LoopItem = Expression.Variable(typeof(object), "Row");
        var Item = Expression.Variable(typeof(T), "Item");
        var Body = new List<Expression> {
          Expression.Assign(Item,Expression.New(typeof(T).GetConstructors().FirstOrDefault(E=>E.IsPublic && E.GetParameters().Length == 0))),
          Expression.Assign(OutputVariable,Item)
        };
        Body.AddRange(
#if FRAMEWORK
          Vars.Select(E =>
            Expression.Assign(
              Expression.Property(Item, E.Property),
              Expression.Convert(
                Expression.Call(null,
                  typeof(DataRowExtensions).GetMethods().FirstOrDefault(M => M.IsGenericMethod && M.Name == nameof(DataRowExtensions.Field)).MakeGenericMethod(E.Property.PropertyType),
                  Expression.Convert(InputPara, typeof(DataRow)),
                  Expression.Constant(E.Attribute.TableColumnName ?? E.Property.Name)
                ),
                E.Property.PropertyType
              )
            )
          ) 
#elif STANDARD
          Vars.Select(E =>
            Expression.Assign(
              Expression.Property(Item, E.Property),
              Expression.Convert(
                  Expression.Property(
                    Expression.Convert(InputPara, typeof(DataRow)),
                    typeof(DataRow).FindIndexers(typeof(string), typeof(object)).Single(),
                    Expression.Constant(E.Attribute.TableColumnName ?? E.Property.Name)
                  ),
                  E.Property.PropertyType
              )
            )
          )
#endif
        );
        Body.Add(Expression.Return(ReturnTarget, OutputVariable));
        Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));
        var LambdaExpression = Expression.Lambda<Func<DataRow, object>>(
               Expression.Block(new[] { OutputVariable, Item }, Body),
               InputPara);
        return LambdaExpression.Compile();
      }
      if (!DataExchangeEntities.TryGetValue(typeof(T), out var Funct))
      {
        Funct = new DataExchangeCacheEntity();
        DataExchangeEntities.Add(typeof(T), Funct);
      }
      if (Funct.DataRow2Object == null)
        Funct.DataRow2Object = DataTableYieldToObjectsCreateFunction();
      return Rows.Select(E => Funct.DataRow2Object(E)).Cast<T>();
    }


  }

} 
#endif  
#endif
