﻿namespace KatKits
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Text;

  //Expression Extension
  public static partial class KatKits
  {
    internal static Expression ForEach(Expression collection, ParameterExpression loopVar, Expression loopContent)
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
    internal static Expression ForEachGeneric(Expression collection, ParameterExpression loopVar, Expression loopContent)
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
