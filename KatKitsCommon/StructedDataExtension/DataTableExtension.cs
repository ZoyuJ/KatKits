
namespace KatKits.StructedDataExtension {
  using global::KatKits.ImplementExtension.CollectionExtension;

  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data;
  using System.Data.SqlClient;
  using System.IO;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;

  using static global::KatKits.Kits;

  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public class ColumnMapAttribute : Attribute {
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

  }
  public class PropertyAndAttribute
  {
    public PropertyInfo Property { get; set; }
    public ColumnMapAttribute Attribute { get; set; }
  }

  public static partial class Kits {
    internal static readonly Dictionary<Type, PropertyAndAttribute[]> _Cache_PropertyColumnMapAttributes = new Dictionary<Type, PropertyAndAttribute[]>();
    internal static readonly Dictionary<Type, Func<object, IDictionary<string, object>>> _Cache_Obj2Dict = new Dictionary<Type, Func<object, IDictionary<string, object>>>();
  
    internal static readonly Dictionary<Type, Func<DataTable, IEnumerable, DataTable>> _Cache_Enumerable2DataTable = new Dictionary<Type, Func<DataTable, IEnumerable, DataTable>>();
  
  
    

    internal static readonly MethodInfo AddToDictionaryMethod = typeof(IDictionary<string, object>).GetMethod(nameof(IDictionary.Add));
    internal static readonly ConstructorInfo DictionaryConstructor = typeof(Dictionary<string, object>).GetConstructors().FirstOrDefault(c => c.IsPublic && !c.GetParameters().Any());
    internal static readonly MethodInfo AddToListMethod = typeof(List<object>).GetMethod(nameof(List<object>.Add));
    internal static readonly ConstructorInfo ListConstructor = typeof(List<object>).GetConstructors().FirstOrDefault(c => c.IsPublic && !c.GetParameters().Any());
    internal static readonly MethodInfo AddColumnToDataTable = typeof(DataColumnCollection).GetMethods().FirstOrDefault(E => E.Name == nameof(DataColumnCollection.Add) && E.GetParameters().Length == 2);
    internal static readonly MethodInfo AddRowToDataTable = typeof(DataRowCollection).GetMethods().FirstOrDefault(E => E.Name == nameof(DataRowCollection.Add) && E.GetParameters().FirstOrDefault().Name == "values");
    internal static readonly PropertyInfo ColumnAllowNull = typeof(DataColumn).GetProperty(nameof(DataColumn.AllowDBNull));
    internal static readonly PropertyInfo ColumnDefaultValue = typeof(DataColumn).GetProperty(nameof(DataColumn.DefaultValue));
    internal static readonly MethodInfo DataTableFileValueConverterMethod = typeof(Kits).GetMethod(nameof(Kits.__EXPPREFIX_ConvertDataTaleFieldValue), BindingFlags.Static | BindingFlags.NonPublic);

    public static IEnumerable<PropertyAndAttribute> FetchPropertiesAndAttributes(Type InputType) {
      IEnumerable<PropertyAndAttribute> Fetch() {
        return InputType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
      .Select(E => new PropertyAndAttribute { Property = E, Attribute = E.GetCustomAttributes(typeof(ColumnMapAttribute), false).Cast<ColumnMapAttribute>().FirstOrDefault() })
      .Where(E => E.Property.CanWrite && E.Property.CanRead && E.Attribute != null)
      .Where(E => E.Property.PropertyType.IsBasicDataTypeOrNullable())
      .Select(E => {
        E.Attribute.PropertyName = E.Property.Name;
        E.Attribute.TableColumnName = E.Attribute.TableColumnName ?? (E.Property.GetCustomAttribute<ColumnAttribute>()?.Name);
        if (E.Property.PropertyType.IsNullableType()) E.Attribute.AllowNull = true;
        E.Attribute.PropertyType = E.Property.PropertyType;
        E.Attribute.DefaultValue = E.Attribute.DefaultValue ?? (E.Property.GetCustomAttribute<DefaultValueAttribute>()?.Value) ?? E.Property.PropertyType.DefaultBasicDataTypeValue();
        if (E.Property.PropertyType.IsEnum) {

        }
        return E;
      })
      .OrderBy(E => E.Attribute.TableColumnOrder);
      }
      if (InputType == null) return null;
      else {
        return _Cache_PropertyColumnMapAttributes.GetOrAdd(InputType, () => Fetch().ToArray());
        //DataExchangeCacheEntity Funct = null;
        //if (!DataExchangeEntities.TryGetValue(InputType, out Funct)) {
        //  Funct = new DataExchangeCacheEntity();
        //  DataExchangeEntities.Add(InputType, Funct);
        //}
        //if (Funct.Obj2Dict == null) {
        //  Funct.Attributes = Fetch();
        //}
        //return Funct.Attributes;
      }

    }


    /// <summary>
    /// Convert Obejct 2 Dictionary
    /// </summary>
    /// <param name="This"></param>
    /// <returns></returns>
    public static IDictionary<string, object> ObjectToDictionary(this object This) {
      Func<object, IDictionary<string, object>> CreateFunction() {
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
      else {
        return _Cache_Obj2Dict.GetOrAdd(This.GetType(), CreateFunction)(This);
      }
    }

    internal static readonly Dictionary<Type, Func<Type, DataTable>> _Cache_Type2DataTable = new Dictionary<Type, Func<Type, DataTable>>();
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
    public static DataTable TypeToDataTable(this Type This) {
      if (This == null) return null;
      if (This.IsBasicDataType()) return null;
      Func<Type, DataTable> CreateFunction() {
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
          .Select(E => {
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
      return _Cache_Type2DataTable.GetOrAdd(This, CreateFunction)(This);
    }


    /// <summary>
    /// Save IEnumerable<T> To DataTable 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="This"></param>
    /// <param name="Table">wanna append to exists datatable</param>
    /// <returns></returns>
    public static DataTable ToDataTable<T>(this IEnumerable<T> This, DataTable Table = null) where T : new() {
      Func<DataTable, IEnumerable, DataTable> CreateFunction() {
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
      return _Cache_Enumerable2DataTable.GetOrAdd(typeof(T), CreateFunction)(Table, Items);
    }

    ///// <summary>
    ///// Save IEnumerable<T> To DataTable 
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="This"></param>
    ///// <returns></returns>
    //public static DataTable EnumerableToDataTable<T>(this IEnumerable<T> This, DataTable Table = null) where T : new() {
    //  Func<DataTable, IEnumerable, DataTable> CreateFunction() {
    //    var OutputType = typeof(DataTable);

    //    var RowsProperty = typeof(DataTable).GetProperty(nameof(DataTable.Rows));

    //    var InputParaType = Expression.Parameter(typeof(Type), "_Type");
    //    var InputParaTable = Expression.Parameter(typeof(DataTable), "Table");
    //    var TypedInputParaTable = Expression.Convert(InputParaTable, typeof(DataTable));
    //    var InputParaEnumable = Expression.Parameter(typeof(IEnumerable), "Items");
    //    var TypedInputParaEnumerable = Expression.Convert(InputParaEnumable, typeof(IEnumerable));


    //    var OutputVariable = Expression.Variable(OutputType, "output");

    //    var ReturnTarget = Expression.Label(OutputType);

    //    var Vars = FetchPropertiesAndAttributes(typeof(T)).ToArray();

    //    var LoopItem = Expression.Variable(typeof(object), "Item");

    //    var Body = new List<Expression> {
    //      Expression.Assign(OutputVariable ,InputParaTable),
    //    };
    //    Body.Add(
    //      ForEach(
    //        TypedInputParaEnumerable,
    //        LoopItem,
    //        Expression.Call(
    //          Expression.Property(TypedInputParaTable, RowsProperty),
    //          AddRowToDataTable,
    //          Expression.NewArrayInit(
    //            typeof(object),
    //            Vars.Select(V =>
    //              Expression.Convert(Expression.Property(Expression.Convert(LoopItem, typeof(T)), V.Property.Name), typeof(object))
    //            )
    //          )
    //        )
    //      )
    //    );
    //    Body.Add(Expression.Return(ReturnTarget, OutputVariable));
    //    Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));
    //    var LambdaExpression = Expression.Lambda<Func<DataTable, IEnumerable, DataTable>>(
    //           Expression.Block(new[] { OutputVariable }, Body),
    //           InputParaTable, InputParaEnumable);
    //    return LambdaExpression.Compile();
    //  }
    //  var Items = This.ToArray();
    //  if (Table == null) Table = TypeToDataTable<T>();
    //  return _Cache_Enumerable2DataTable.GetOrAdd(typeof(T), CreateFunction)(Table, Items);
    //}

    //private readonly static Dictionary<Type, Func<IEnumerable, DataTable, DataTable>> GenericDataTableWriters = new Dictionary<Type, Func<IEnumerable, DataTable, DataTable>>();
//    public static DataTable GenericEnumerableToDataTable<T>(this IEnumerable<T> This, DataTable Table = null) where T : new() {
//      var Items = This.ToArray();
//      Table = Table ?? TypeToDataTable<T>();

//      //if(GenericDataTableWriters)

//      var OutputType = typeof(DataTable);

//      var RowsProperty = typeof(DataTable).GetProperty(nameof(DataTable.Rows));

//#region Inline Delegate
//      var InputParaType = Expression.Parameter(typeof(Type), "_Type");
//      var InputParaTable = Expression.Parameter(typeof(DataTable), "Table");
//      var TypedInputParaTable = Expression.Convert(InputParaTable, typeof(DataTable));
//      var InputParaEnumable = Expression.Parameter(typeof(IEnumerable<T>), "Items");
//      var TypedInputParaEnumerable = Expression.Convert(InputParaEnumable, typeof(IEnumerable<T>));

//      var OutputVariable = Expression.Variable(OutputType, "output");
//      var ReturnTarget = Expression.Label(OutputType);
//      var Vars = FetchPropertiesAndAttributes(typeof(T)).ToArray();
//      var LoopItem = Expression.Variable(typeof(T), "Item");

//      var Body = new List<Expression> {
//          Expression.Assign(OutputVariable ,InputParaTable),
//        };
//      Body.Add(
//        ForEachGeneric(
//          InputParaEnumable,
//          LoopItem,
//          Expression.Call(
//            Expression.Property(TypedInputParaTable, RowsProperty),
//            AddRowToDataTable,
//              Expression.NewArrayInit(
//                typeof(object),
//                Vars.Select(V =>
//                  Expression.Convert(Expression.Property(LoopItem, V.Property.Name), typeof(object))
//                )
//              )
//          )
//        )
//      );
//      Body.Add(Expression.Return(ReturnTarget, OutputVariable));
//      Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));
//      var LambdaExpression = Expression.Lambda<Func<DataTable, IEnumerable<T>, DataTable>>(
//             Expression.Block(new[] { OutputVariable }, Body),
//             InputParaTable, InputParaEnumable);
//      var InlineFunc = LambdaExpression.Compile();
//#endregion
//      var DelegateIns = typeof(T).GetFields().FirstOrDefault(E => E.IsStatic && E.Name == nameof(GenericEnumerableToDataTable));
//      DelegateIns.SetValue(null, InlineFunc);

//      var WrapedInputParaTable = Expression.Parameter(typeof(DataTable), "Table");
//      var WrapedInputEnumerable = Expression.Parameter(typeof(IEnumerable), "Items");

//      var OutputVariable2 = Expression.Variable(OutputType, "output");
//      var ReturnTarget2 = Expression.Label(OutputType);
//      //var DynamicCall = InlineFunc.GetType().GetMethod(nameof(Delegate.DynamicInvoke));
//      var GenFunc = typeof(Func<>).MakeGenericType(typeof(DataTable), typeof(IEnumerable<T>), typeof(DataTable));
//      var Call = GenFunc.GetMethod("Invoke");

//      var Body2 = new List<Expression>{
//        Expression.Assign(OutputVariable2,
//          Expression.Invoke(
//            Expression.Convert(Expression.Field(null,DelegateIns),GenFunc),
//            WrapedInputParaTable,WrapedInputEnumerable
//          )
//        )
//      };

//      return InlineFunc(Table, This);
//    }

    //private static readonly Dictionary<Type, Func<DataTable, IEnumerable>> DataTableToArrayConverters = new Dictionary<Type, Func<DataTable, IEnumerable>>();

    private static T __EXPPREFIX_ConvertDataTaleFieldValue<T>(object Field) {
      return Convert.IsDBNull(Field) ? default(T) : (T)Convert.ChangeType(Field, typeof(T).GetUnderlyingType());
    }


    //internal static readonly Dictionary<Type, Func<DataTable, IEnumerable>> _Cache_DataTable2Enumerable = new Dictionary<Type, Func<DataTable, IEnumerable>>();
    internal static readonly Dictionary<Type, Func<DataRow, object>> _Cache_DataRow2Object = new Dictionary<Type, Func<DataRow, object>>();
    /// <summary>
    /// yield return datatable row
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Table"></param>
    /// <returns></returns>
    public static IEnumerable<T> AsEnumerable<T>(this DataTable Table) where T : new() {
      return Table.Rows.Cast<DataRow>().AsEnumerable<T>();
    }
    /// <summary>
    /// yield return datatable row
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Rows"></param>
    /// <returns></returns>
    public static IEnumerable<T> AsEnumerable<T>(this IEnumerable<DataRow> Rows) {
      Func<DataRow, object> DataTableYieldToObjectsCreateFunction() {
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
          Vars.Select(E =>
            Expression.Assign(
              Expression.Property(Item, E.Property),
              Expression.Call(
                null,
                DataTableFileValueConverterMethod.MakeGenericMethod(E.Property.PropertyType),
                Expression.Property(
                  Expression.Convert(InputPara, typeof(DataRow)),
                  typeof(DataRow).FindIndexers("Item", typeof(string), typeof(object)).Single(),
                  Expression.Constant(E.Attribute.TableColumnName ?? E.Property.Name)
                )
              )
            )
          )
        );
        Body.Add(Expression.Return(ReturnTarget, OutputVariable));
        Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));
        var LambdaExpression = Expression.Lambda<Func<DataRow, object>>(
               Expression.Block(new[] { OutputVariable, Item }, Body),
               InputPara);
        return LambdaExpression.Compile();
      }
      var Func = _Cache_DataRow2Object.GetOrAdd(typeof(T), DataTableYieldToObjectsCreateFunction);
      return Rows.Select(E => Func(E)).Cast<T>();
    }

    /// <summary>
    /// yield return datatable row
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Table"></param>
    /// <param name="AnonymousObject"></param>
    /// <returns></returns>
    public static IEnumerable<T> AsAnonymousEnumerable<T>(this DataTable Table, T AnonymousObject) => Table.Rows.Cast<DataRow>().AsAnonymousEnumerable(AnonymousObject);
    /// <summary>
    /// yield return datatable row
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Rows"></param>
    /// <param name="AnonymousObject"></param>
    /// <returns></returns>
    public static IEnumerable<T> AsAnonymousEnumerable<T>(this IEnumerable<DataRow> Rows, T AnonymousObject, IEnumerable<string> ColumnNames = null) {
      Func<DataRow, object> DataTableYieldToObjectsCreateFunction() {
        var InputType = typeof(DataRow);
        var OutputType = typeof(T);
        var InputPara = Expression.Parameter(InputType, "Row");
        var OutputVariable = Expression.Variable(OutputType, "output");
        var ReturnTarget = Expression.Label(OutputType);
        var Vars = ColumnNames == null ? FetchPropertiesAndAttributes(typeof(T)).ToArray() : FetchPropertiesAndAttributes(typeof(T)).Join(ColumnNames, L => L.Attribute.TableColumnName ?? L.Property.Name, R => R, (L, R) => L).ToArray();

        var Body = new List<Expression>()
        {
                    Expression.Assign(
                        OutputVariable,
                        Expression.New(
                            OutputType.GetConstructors()[0],
                            Vars.Select(E =>
                                Expression.Call(
                                    null,
                                    DataTableFileValueConverterMethod.MakeGenericMethod(E.Attribute.PropertyType),
                                    Expression.Property(
                                        Expression.Convert(InputPara, typeof(DataRow)),
                                        typeof(DataRow).FindIndexers("Item", typeof(string), typeof(object)).Single(),
                                        Expression.Constant(E.Attribute.TableColumnName)
                                    )
                                )
                            )

                        )
                    )
                };
        Body.Add(Expression.Return(ReturnTarget, OutputVariable));
        Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));
        var LambdaExpression = Expression.Lambda<Func<DataRow, object>>(
               Expression.Block(new[] { OutputVariable }, Body),
               InputPara);
        return LambdaExpression.Compile();
      }
      var Func = _Cache_DataRow2Object.GetOrAdd(typeof(T), DataTableYieldToObjectsCreateFunction);
      return Rows.Select(E => Func(E)).Cast<T>();
    }

    /// <summary>
    /// datatable 2 xml
    /// </summary>
    /// <param name="Table"></param>
    /// <returns></returns>
    public static string ToXML(this DataTable Table) {
      using (var MS = new MemoryStream()) {
        Table.WriteXml(MS);
        MS.Position = 0;
        using (var SR = new StreamReader(MS)) {
          return SR.ReadToEnd();
        }
      }
    }

    /// <summary>
    /// pick 2 column and convert to dictionary
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="Table"></param>
    /// <param name="KeyColumn"></param>
    /// <param name="ValueColumn"></param>
    /// <returns></returns>
    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DataTable Table, string KeyColumn, string ValueColumn) {
      return Table.Rows.Cast<DataRow>().ToDictionary(E => __EXPPREFIX_ConvertDataTaleFieldValue<TKey>(E[KeyColumn]), E => __EXPPREFIX_ConvertDataTaleFieldValue<TValue>(E[ValueColumn]));
    }


  }

} 
