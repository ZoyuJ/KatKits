namespace KatKits
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Reflection;

    public static partial class ImplUtil
    {

    


 

     





        public static T ValueOrDefault<T>(this T? Nullable) where T : struct => Nullable.HasValue ? Nullable.Value : default(T);
        public static T? ToNullable<T>(object Val) where T : struct
        {
            if (Val == null) return null;
            return (T?)Convert.ChangeType(Val, typeof(T));
        }

 
     


        public static bool SetValue<T>(this ref T Property, T Value) where T : struct
        {
            if (!EqualityComparer<T>.Default.Equals(Property, Value))
            {
                Property = Value;
                return true;
            }
            return false;
        }

        public static bool SetValue<T, TVal>(this T Instance, string Property, TVal Value)
        {
            Func<T, TVal, bool> CreateFunction()
            {
                var OutputType = typeof(bool);
                var InputType_Obj = typeof(T);
                var InputType_Val = typeof(TVal);
                var InputExpression_Obj = Expression.Parameter(InputType_Obj, "obj");
                var InputExpression_Val = Expression.Parameter(InputType_Val, "val");
                //var TypedInputExpression = Expression.Convert(InputExpression, InputType);
                var OutputVariable = Expression.Variable(OutputType, "output");
                var ReturnTarget = Expression.Label(OutputType);

                var Body = new List<Expression> {
                    Expression.Assign(OutputVariable,Expression.NotEqual(Expression.PropertyOrField(InputExpression_Obj,Property),InputExpression_Val)),
                    Expression.IfThen(OutputVariable,Expression.Assign(Expression.PropertyOrField(InputExpression_Obj,Property),InputExpression_Val)),
                    Expression.Return(ReturnTarget, OutputVariable),
                    Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)),
                };
                var LambdaExpression = Expression.Lambda<Func<T, TVal, bool>>(
                    Expression.Block(new[] { OutputVariable }, Body),
                    InputExpression_Obj, InputExpression_Val);
                return LambdaExpression.Compile();
            }

            return CreateFunction()(Instance, Value);

        }
        public static bool SetValue<T, TVal>(this T Instance, string Property, TVal Value, Func<TVal, TVal, bool> Comparer)
        {
            Func<T, TVal, Func<TVal, TVal, bool>, bool> CreateFunction()
            {
                var OutputType = typeof(bool);
                var InputType_Obj = typeof(T);
                var InputType_Val = typeof(TVal);
                var InputExpression_Obj = Expression.Parameter(InputType_Obj, "obj");
                var InputExpression_Val = Expression.Parameter(InputType_Val, "val");
                var OutputVariable = Expression.Variable(OutputType, "output");
                var ReturnTarget = Expression.Label(OutputType);

                var Comp = Expression.Invoke(Expression.Constant(Comparer), Expression.PropertyOrField(InputExpression_Obj, Property), InputExpression_Val);

                var Body = new List<Expression> {
                    Expression.Assign(OutputVariable,Comp),
                    Expression.IfThen(OutputVariable,Expression.Assign(Expression.PropertyOrField(InputExpression_Obj,Property),InputExpression_Val)),
                    Expression.Return(ReturnTarget, OutputVariable),
                    Expression.Label(ReturnTarget, Expression.Constant(null, OutputType)),
                };
                var LambdaExpression = Expression.Lambda<Func<T, TVal, Func<TVal, TVal, bool>, bool>>(
                    Expression.Block(new[] { OutputVariable }, Body),
                    InputExpression_Obj, InputExpression_Val);
                return LambdaExpression.Compile();
            }
            return CreateFunction()(Instance, Value, Comparer);
        }


        //public static int TotalMonthes(in DateTime Start,in DateTime End)
        //{
        //    var Y_m = (Start.Year-End.Year)
        //}

 

    }

   
}
