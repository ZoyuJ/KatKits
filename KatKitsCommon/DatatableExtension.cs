namespace KatKits
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static partial class DataTableUtil
    {

        private class DataExchangeCacheEntity
        {
            public IEnumerable<PropertyAndAttribute> Attributes;
            public Func<object, IDictionary<string, object>> Obj2Dict;
            public Func<Type, DataTable> Type2DataTable;
            public Func<DataTable, IEnumerable, DataTable> Enumerable2DataTable;
            public Func<DataTable, IEnumerable> DataTable2Enumerable;
            //public Action<SqlCommand, object> Obj2SQLParamaters;
            public Func<DataRow, object> DataRow2Object;
        }
        private static readonly MethodInfo AddToDictionaryMethod = typeof(IDictionary<string, object>).GetMethod(nameof(IDictionary.Add));
        private static readonly ConstructorInfo DictionaryConstructor = typeof(Dictionary<string, object>).GetConstructors().FirstOrDefault(c => c.IsPublic && !c.GetParameters().Any());
        private static readonly MethodInfo AddToListMethod = typeof(List<object>).GetMethod(nameof(List<object>.Add));
        private static readonly ConstructorInfo ListConstructor = typeof(List<object>).GetConstructors().FirstOrDefault(c => c.IsPublic && !c.GetParameters().Any());
        private static readonly MethodInfo AddColumnToDataTable = typeof(DataColumnCollection).GetMethods().FirstOrDefault(E => E.Name == nameof(DataColumnCollection.Add) && E.GetParameters().Length == 2);
        private static readonly MethodInfo AddRowToDataTable = typeof(DataRowCollection).GetMethods().FirstOrDefault(E => E.Name == nameof(DataRowCollection.Add) && E.GetParameters().FirstOrDefault().Name == "values");
        private static readonly PropertyInfo ColumnAllowNull = typeof(DataColumn).GetProperty(nameof(DataColumn.AllowDBNull));
        private static readonly PropertyInfo ColumnDefaultValue = typeof(DataColumn).GetProperty(nameof(DataColumn.DefaultValue));

        private static readonly Dictionary<Type, DataExchangeCacheEntity> DataExchangeEntities = new Dictionary<Type, DataExchangeCacheEntity>();
        public static void ClearExpressionCache() => DataExchangeEntities.Clear();

        ///// <summary>
        ///// generate ColumnMapAttribute instance from System.Data.Linq.Mapping.ColumnAttribute
        ///// the xls related info cannot be filled
        ///// </summary>
        ///// <param name="This"></param>
        ///// <param name="Property"></param>
        ///// <returns></returns>
        //private static ColumnMapAttribute ConvertFromLinqMapping(this System.Data.Linq.Mapping.ColumnAttribute This, PropertyInfo Property) {
        //    return This == null ? null : new ColumnMapAttribute {
        //        AllowNull = Property.PropertyType.IsNullableType(),
        //        PropertyName = Property.Name,
        //        TableColumnName = This.Name ?? Property.Name,
        //        PropertyType = Property.PropertyType,
        //        DefaultValue = Property.PropertyType.DefaultBasicDataTypeValue()
        //    };
        //}
        /// <summary>
        /// get all properties info and attribute from datatable/table mapping
        /// </summary>
        /// <param name="InputType"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyAndAttribute> FetchPropertiesAndAttributes(Type InputType)
        {
            IEnumerable<PropertyAndAttribute> Fetch()
            {
                return (InputType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                  .Select(E => new PropertyAndAttribute { Property = E, Attribute = E.GetCustomAttributes(typeof(ColumnMapAttribute), false).Cast<ColumnMapAttribute>().FirstOrDefault() })
                  .Where(E => E.Property.CanWrite && E.Property.CanRead && E.Attribute != null)
                  .Where(E => E.Property.PropertyType.IsBasicDataTypeOrNullable())
                  .Select(E => {
                      E.Attribute.PropertyName = E.Property.Name;
                      E.Attribute.TableColumnName = E.Attribute.TableColumnName ?? (E.Property.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>()?.Name) /*?? (E.Property.GetCustomAttribute<System.Data.Linq.Mapping.ColumnAttribute>()?.Name)*/;
                      if (E.Property.PropertyType.IsNullableType()) E.Attribute.AllowNull = true;
                      E.Attribute.PropertyType = E.Property.PropertyType;
                      E.Attribute.DefaultValue = E.Attribute.DefaultValue ?? (E.Property.GetCustomAttribute<DefaultValueAttribute>()?.Value) ?? E.Property.PropertyType.DefaultBasicDataTypeValue();
                      return E;
                  })
                  .OrderBy(E => E.Attribute.TableColumnOrder));
            }
            //IEnumerable<PropertyAndAttribute> FetchByLinqMapping() {
            //    return InputType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //      .Where(E => E.CanWrite && E.CanRead && E.PropertyType.IsBasicDataTypeOrNullable())
            //      .Select(E => new PropertyAndAttribute { Property = E, Attribute = E.GetCustomAttribute<System.Data.Linq.Mapping.ColumnAttribute>().ConvertFromLinqMapping(E) })
            //      .Where(E => E.Attribute != null);
            //}
            IEnumerable<PropertyAndAttribute> FetchByAnonymousType()
            {
                var Ps = InputType.GetConstructors()[0].GetParameters();
                if (Ps.Any(E => !E.ParameterType.IsBasicDataTypeOrNullable())) throw new ArgumentException("Only support convetable CLR type");
                return Ps.Select(E => new PropertyAndAttribute
                {
                    Property = null,
                    Attribute = new ColumnMapAttribute
                    {
                        AllowNull = E.ParameterType.CanBeNull(),
                        PropertyName = E.Name,
                        TableColumnName = E.Name,
                        PropertyType = E.ParameterType,
                        DefaultValue = E.DefaultValue
                    }
                });
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
                if (Funct.Attributes == null)
                {
                    if (InputType.IsAnonymousType()) Funct.Attributes = FetchByAnonymousType();
                    else
                    {
                        Funct.Attributes = Fetch();
                        //if (Funct.Attributes.Count() == 0) Funct.Attributes = FetchByLinqMapping();
                        if (Funct.Attributes.Count() == 0) Funct.Attributes = FetchByAnonymousType();
                    }
                }
                return Funct.Attributes;
            }

        }
        public class PropertyAndAttribute
        {
            public PropertyInfo Property { get; set; }
            public ColumnMapAttribute Attribute { get; set; }
        }


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
        /// Generate empty DataTable from Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DataTable TypeToDataTable<T>() => TypeToDataTable(typeof(T));
        /// <summary>
        /// Generate empty DataTable from Type
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
                var OutputVariable = Expression.Variable(OutputType, "output");// DataTable output
                var ColumnsVariable = Expression.Property(OutputVariable, typeof(DataTable).GetProperty(nameof(DataTable.Columns)));//output.columns
                var ReturnTarget = Expression.Label(OutputType); // return output
                var DataTableConstructor = OutputType.GetConstructors().FirstOrDefault(E => E.IsPublic && !E.GetParameters().Any());//new DataTable()
                var Body = new List<Expression> { Expression.Assign(OutputVariable, Expression.New(DataTableConstructor)) };//output = new DataTable()
                var Col = Expression.Variable(typeof(DataColumn), "Col");//DataColumn Col
                Body.AddRange(
                  FetchPropertiesAndAttributes(InputType)
                  .Select(E => {
                      return Expression.Block(new ParameterExpression[] { Col },
                //Col = output.columns.add(Attr.Name,Attr.Type);
                Expression.Assign(Col, Expression.Call(ColumnsVariable, AddColumnToDataTable, Expression.Constant(E.Attribute.TableColumnName ?? E.Attribute.PropertyName), Expression.Constant(E.Attribute.PropertyType.GetUnderlyingType()))),
                //Col .AllowDBNull = Attr.AllowNull;
                Expression.Assign(Expression.Property(Col, ColumnAllowNull), Expression.Constant(E.Attribute.AllowNull)),
                //Col.DefaultValue = Attr.DefaultValue;
                Expression.Assign(Expression.Property(Col, ColumnDefaultValue), Expression.Convert(Expression.Constant(E.Attribute.DefaultValue), typeof(object)))
                );
                  })
                );
                Body.Add(Expression.Return(ReturnTarget, OutputVariable));// return output
                Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));// return output
                var LambdaExpression = Expression.Lambda<Func<object, DataTable>>(
                        Expression.Block(new[] { OutputVariable }, Body),
                        InputExpression); // (input)=>{xxxxx;}
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
        /// <param name="Table">wanna append to exists datatable</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> This, DataTable Table = null)
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
                          Expression.Convert(Expression.Property(Expression.Convert(LoopItem, typeof(T)), V.Attribute.PropertyName), typeof(object))
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

        /// <summary>
        /// Read DataTable as IEnumerable<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Table"></param>
        /// <returns></returns>
        [Obsolete("has no idea about converting SQLType:BIT to C#Type:bool, Use AsEnumerable instead")]
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

                var _W = new Expression[] {
                  Expression.Assign(Item,Expression.New(ModelConstructor)),
                  Expression.Call(Items,typeof(List<T>).GetMethod("Add"),Item),
                }
                        .Concat(Vars.Select(E =>
                          E.Property.PropertyType.Equals(typeof(bool))
                          ? Expression.Assign(
                              Expression.Property(Item, E.Property),
                              Expression.Call(
                                null,
                                typeof(System.Convert).GetMethods().FirstOrDefault(Md => Md.Name == nameof(Convert.ToBoolean) && Md.GetParameters()[0].ParameterType.Equals(typeof(int))),
                                Expression.Call(null,
                                  typeof(DataRowExtensions).GetMethods().FirstOrDefault(M => M.IsGenericMethod && M.Name == nameof(DataRowExtensions.Field)).MakeGenericMethod(typeof(int)),
                                  Expression.Convert(LoopItem, typeof(DataRow)),
                                  Expression.Constant(E.Attribute.TableColumnName ?? E.Property.Name)
                                )
                              )
                            )
                            //TODO bool?
                            : Expression.Assign(
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
        [Obsolete("has no idea about converting SQLType:BIT to C#Type:bool, Use AsEnumerable instead")]
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
        [Obsolete("has no idea about converting SQLType:BIT to C#Type:bool, Use AsEnumerable instead")]
        public static IEnumerable<T> YieldTo<T>(this IEnumerable<DataRow> Rows)
        {
            /*
             datatable class:K
            List<K> Ks = new List<K>();
            foreach(var Row in DataTable.Rows){
              K k1 = new K();
              k1.S1 = Row["S1"];
              K1.S2 = Row["S2"];
              ....
              Ks.Add(k1);
            }  
            return Ks;



             */
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

        /// <summary>
        /// DBNull to null,string to target type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Field"></param>
        /// <returns></returns>
        private static T __EXPPREFIX_ConvertDataTaleFieldValue<T>(object Field)
        {
            return Convert.IsDBNull(Field) ? default(T) : (typeof(T).Equals(typeof(byte[])) ? (T)Field : (T)Convert.ChangeType(Field, typeof(T).GetUnderlyingType()));
        }

        public static IEnumerable<PropertyInfo> FindIndexers(this Type This, string IndexName = "Item", params Type[] ParameterAndReturnTypes)
          => This.GetProperties()
            .Where(E => E.Name == IndexName
                        && E.GetIndexParameters().Select(P => P.ParameterType)
                            .Concat(new Type[] { E.PropertyType })
                            .OrderedEqual(ParameterAndReturnTypes));
        public static bool OrderedEqual<T>(this IEnumerable<T> This, IEnumerable<T> Others, int SourceOffset = 0, int OthersOffset = 0, int Length = 0, bool MatchLength = true)
        {
            var _Source = Length == 0 ? This.Skip(SourceOffset) : This.Skip(SourceOffset).Take(Length);
            var _Other = Length == 0 ? Others.Skip(OthersOffset) : Others.Skip(SourceOffset).Take(Length);
            if (MatchLength && _Source.Count() != _Other.Count()) return false;
            return !_Source.Zip(_Other, (L, R) => L.Equals(R)).Any(E => E == false);
        }
        /// <summary>
        /// yield return datatable row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Table"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this DataTable Table) where T : new()
        {
            return Table.Rows.Cast<DataRow>().AsEnumerable<T>();
        }

        /// <summary>
        /// yield return datatable row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Rows"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerable<DataRow> Rows, IEnumerable<string> ColumnNames = null)
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
                var Vars = ColumnNames == null ? FetchPropertiesAndAttributes(typeof(T)).ToArray() : FetchPropertiesAndAttributes(typeof(T)).Join(ColumnNames, L => L.Attribute.TableColumnName ?? L.Property.Name, R => R, (L, R) => L).ToArray();
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
                        typeof(DataTableUtil).GetMethod(nameof(__EXPPREFIX_ConvertDataTaleFieldValue), BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(E.Property.PropertyType),
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
            if (!DataExchangeEntities.TryGetValue(typeof(T), out var Funct))
            {
                Funct = new DataExchangeCacheEntity();
                DataExchangeEntities.Add(typeof(T), Funct);
            }
            if (Funct.DataRow2Object == null)
                Funct.DataRow2Object = DataTableYieldToObjectsCreateFunction();
            return Rows.Select(E => Funct.DataRow2Object(E)).Cast<T>();
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
        public static IEnumerable<T> AsAnonymousEnumerable<T>(this IEnumerable<DataRow> Rows, T AnonymousObject, IEnumerable<string> ColumnNames = null)
        {
            Func<DataRow, object> DataTableYieldToObjectsCreateFunction()
            {
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
                                    typeof(DataTableUtil).GetMethod(nameof(__EXPPREFIX_ConvertDataTaleFieldValue), BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(E.Attribute.PropertyType),
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
            if (!DataExchangeEntities.TryGetValue(typeof(T), out var Funct))
            {
                Funct = new DataExchangeCacheEntity();
                DataExchangeEntities.Add(typeof(T), Funct);
            }
            if (Funct.DataRow2Object == null)
                Funct.DataRow2Object = DataTableYieldToObjectsCreateFunction();
            return Rows.Select(E => Funct.DataRow2Object(E)).Cast<T>();
        }

        /// <summary>
        /// pick 2 column and convert to dictionary
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Table"></param>
        /// <param name="KeyColumn"></param>
        /// <param name="ValueColumn"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DataTable Table, string KeyColumn, string ValueColumn)
        {
            return Table.Rows.Cast<DataRow>().ToDictionary(E => __EXPPREFIX_ConvertDataTaleFieldValue<TKey>(E[KeyColumn]), E => __EXPPREFIX_ConvertDataTaleFieldValue<TValue>(E[ValueColumn]));
        }

        public static string ToXML(this DataTable Table)
        {
            using (var MS = new MemoryStream())
            {
                Table.WriteXml(MS);
                MS.Position = 0;
                using (var SR = new StreamReader(MS))
                {
                    return SR.ReadToEnd();
                }
            }
        }




        private static Expression ForEach(Expression collection, ParameterExpression loopVar, Expression loopContent)
        {

            var enumeratorVar = Expression.Variable(typeof(IEnumerator), "enumerator");
            var getEnumeratorCall = Expression.Call(collection, typeof(IEnumerable).GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);

            var moveNextCall = Expression.Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext"));

            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(new[] { enumeratorVar },
                enumeratorAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(moveNextCall, Expression.Constant(true)),
                        Expression.Block(new[] { loopVar }.ToArray(),
                            Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current")),
                            loopContent
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }
        private static Expression ForEachGeneric(Expression collection, ParameterExpression loopVar, Expression loopContent)
        {
            var elementType = loopVar.Type;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var getEnumeratorCall = Expression.Call(collection, enumerableType.GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);

            // The MoveNext method's actually on IEnumerator, not IEnumerator<T>
            var moveNextCall = Expression.Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext"));

            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(new[] { enumeratorVar },
                enumeratorAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(moveNextCall, Expression.Constant(true)),
                        Expression.Block(new[] { loopVar },
                            Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current")),
                            loopContent
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }
    }
}
