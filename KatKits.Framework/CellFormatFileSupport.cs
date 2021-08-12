namespace KatKits
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public static partial class KatKits
  {
    public static void L()
    {
      
    }
#if CLOSEDXML

    /// <summary>
    /// write to worksheet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Workbook"></param>
    /// <param name="Items"></param>
    /// <param name="WithHeader"></param>
    /// <returns></returns>
    public static IXLWorksheet WriteToSheet<T>(this XLWorkbook Workbook, IEnumerable<T> Items, bool WithHeader) where T : new()
    {
      var Sheet = Workbook.Worksheets.Add(Items.EnumerableToDataTable(null));
      if (!WithHeader)
      {
        Sheet.Row(1).Delete();
      }
      return Sheet;
    }
    /// <summary>
    /// Read Sheet to DataTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Sheet"></param>
    /// <param name="HasHeader"></param>
    /// <param name="IgnoreCase"></param>
    /// <returns></returns>
    public static DataTable ReadFromSheetToDataTable<T>(this IXLWorksheet Sheet, bool HasHeader, bool IgnoreCase) where T : new()
    {
      var Table = typeof(T).TypeToDataTable();
      if (HasHeader)
      {
        var Header = Sheet.Rows(1, 1).CellsUsed().Cast<IXLCell>().Select(E => new { Header = E.GetString(), ColIndex = E.Address.ColumnNumber }).ToArray();
        var Ordered = (
          IgnoreCase
          ? FetchPropertiesAndAttributes(typeof(T))
            .Join(
              Header,
              L => L.Attribute.XLSColumnName.Trim().ToUpper(),
              R => R.Header.Trim().ToUpper(),
              (L, R) => new { Header = R, Attribute = L }
            )
          : FetchPropertiesAndAttributes(typeof(T))
            .Join(
              Header,
              L => L.Attribute.XLSColumnName,
              R => R.Header,
              (L, R) => new { Header = R, Attribute = L }
           )
        )
        .OrderBy(E => E.Attribute.Attribute.TableColumnOrder)
        .ToArray();
        Sheet.RowsUsed()
          .Skip(1)
          .Select(E =>
            Ordered.Select(EE =>
              CellValueConverters.GetOrAdd(
                 EE.Attribute.Property.PropertyType,
                 () => CreateFunction(EE.Attribute.Property.PropertyType)
              )(E.Cell(EE.Header.ColIndex))
            )
            .ToArray()
          )
          .ToArray()
          .ForEach(E => Table.Rows.Add(E));
      }
      else
      {
        var Header = Sheet.FirstRowUsed().CellsUsed().Cast<IXLCell>().Select(E => new { Header = E.Address.ColumnLetter, ColIndex = E.Address.ColumnNumber }).ToArray();
        var Ordered =
          FetchPropertiesAndAttributes(typeof(T))
          .Join(
            Header,
            L => L.Attribute.XLSColumnIndex,
            R => R.ColIndex,
            (L, R) => new { Header = R, Attribute = L }
          )
          .OrderBy(E => E.Attribute.Attribute.TableColumnOrder)
          .ToArray();
        Sheet.RowsUsed()
          .Select(E =>
            Ordered.Select(EE =>
               CellValueConverters.GetOrAdd(
                 EE.Attribute.Property.PropertyType,
                 () => CreateFunction(EE.Attribute.Property.PropertyType, EE.Attribute.Attribute.DefaultValue)
              )(E.Cell(EE.Header.ColIndex))
            ).ToArray()
          )
          .ToArray()
          .ForEach(E => Table.Rows.Add(E));
      }
      return Table;
      Func<IXLCell, object> CreateFunction(Type Target, object Default = null)
      {
        var InputType = typeof(IXLCell);
        var InputPara = Expression.Parameter(InputType, "Cell");
        var OutputVariable = Expression.Variable(typeof(object), "output");
        var ReturnTarget = Expression.Label(typeof(object));
        var ConvertedVar = Expression.Variable(Target.IsNullableType() ? Nullable.GetUnderlyingType(Target) : Target);
        var Body = new Expression[] {
            Expression.IfThenElse(
              Expression.Call(
                InputPara,
                InputType.GetMethod(nameof(IXLCell.TryGetValue)).MakeGenericMethod(Target.IsNullableType()?Nullable.GetUnderlyingType(Target):Target),
                ConvertedVar
              ),
              Expression.Assign(OutputVariable,Expression.Convert(ConvertedVar,typeof(object))),
              Expression.Assign(OutputVariable,Expression.Convert(Expression.Convert(Expression.Constant(Default),Target),typeof(object)))
            ),
            Expression.Return(ReturnTarget, OutputVariable),
            Expression.Label(ReturnTarget, Expression.Constant(null, typeof(object)))
        };
        var LambdaExpression = Expression.Lambda<Func<IXLCell, object>>(
               Expression.Block(new[] { OutputVariable, ConvertedVar }, Body),
               InputPara);
        Debug.WriteLine($"ReadFromSheetToDataTable+CreateFunction({Target.FullName},{Default ?? "NULL"})" + LambdaExpression.ToString());
        return LambdaExpression.Compile();
      }

    }
    [Obsolete("No Impl", true)]
    public static IEnumerable<T> ReadFromSheetToEnumerable<T>(this IXLWorksheet Sheet, bool HasHeader, bool IgnoreCase) where T : new()
    {
      List<T> Items = new List<T>();
      var Header = Sheet.Rows(1, 1).CellsUsed().Cast<IXLCell>().Select(E => new { Header = HasHeader ? E.GetString() : E.Address.ColumnLetter, ColIndex = E.Address.ColumnNumber }).ToArray();
      var Ordered = (
         IgnoreCase
         ? FetchPropertiesAndAttributes(typeof(T))
           .Join(
             Header,
             L => L.Attribute.XLSColumnName.Trim().ToUpper(),
             R => R.Header.Trim().ToUpper(),
             (L, R) => new { Header = R, Attribute = L }
           )
         : FetchPropertiesAndAttributes(typeof(T))
           .Join(
             Header,
             L => L.Attribute.XLSColumnName,
             R => R.Header,
             (L, R) => new { Header = R, Attribute = L }
          )
       )
       .OrderBy(E => E.Attribute.Attribute.TableColumnOrder)
       .ToArray();

      //Func<IXLCells,object> CreateFunction(Type Target, PropertyAndAttribute[] Attrs)
      //{
      //  var InputType = typeof(IXLCells);
      //  var OutputType = typeof(object);
      //  var InputPara = Expression.Parameter(InputType, "Cells");
      //  var OutputVariable = Expression.Variable(OutputType, "output");
      //  var ReturnTarget = Expression.Label(OutputType);
      //  var Body = new List<Expression> {
      //    Expression.Assign(OutputVariable,Expression.New(Target.GetConstructors().FirstOrDefault(E=>E.IsPublic&&E.GetParameters().Length == 0))),
      //  };


      //  Body.AddRange(
      //      Attrs.Select(E => Expression.Assign(
      //        Expression.Property(OutputVariable,E.Property),
      //        Expression.Call(InputPara
      //      )
      //  );
      //  Body.Add(Expression.Return(ReturnTarget, OutputVariable));
      //  Body.Add(Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)));

      //  var LambdaExpression = Expression.Lambda<Func<IXLCell[], object>>(
      //         Expression.Block(new[] { OutputVariable }, Body),
      //         InputPara);
      //  return LambdaExpression.Compile();
      //}

      return Items;
    }
    private static readonly Dictionary<Type, Func<IXLCell, object>> CellValueConverters = new Dictionary<Type, Func<IXLCell, object>>(); 

#endif
  }
}
