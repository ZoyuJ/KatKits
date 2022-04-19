

#if NEWTONSOFT_JSON
namespace KatKits.StructedDataExtension {
  using KatKits.ImplementExtension.CollectionExtension;

  using Newtonsoft.Json.Linq;

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;

  public static partial class Kits {
    private static readonly Dictionary<Type, Dictionary<string, Action<JToken, object>>> _UpdateInstanceByJsonCache = new Dictionary<Type, Dictionary<string, Action<JToken, object>>>();
    public static void UpdateInstanceByJSON(string Json, object Instance) {
      var K = JObject.Parse(Json);
      UpdateInstance(K, Instance);
    }
    public static void UpdateInstance(this JObject This, object Instance) {
      var Tp = Instance.GetType();
      var Method = typeof(JToken).GetMethods().First(E => E.Name == nameof(JToken.ToObject) && E.IsGenericMethod && E.GetParameters().Length == 0);
      Action<JToken, object> CreateFunction(string Name) {
        var JValue = Expression.Parameter(typeof(JToken), "JValue");
        var Target = Expression.Parameter(typeof(object), "Target");
        var TypedTarget = Expression.Convert(Target, Tp);
        var Body = new List<Expression>();
        var TargetType = Instance.GetType();
        var P = TargetType.GetProperty(Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (P != null) {
          var M = Method.MakeGenericMethod(P.PropertyType);
          Body.Add(Expression.Assign(Expression.Property(TypedTarget, P), Expression.Convert(Expression.Call(JValue, M), P.PropertyType)));
        }
        else {
          var F = TargetType.GetField(Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
          if (F != null) {
            var M = Method.MakeGenericMethod(F.FieldType);
            Body.Add(Expression.Assign(Expression.Field(TypedTarget, F), Expression.Convert(Expression.Call(JValue, M), F.FieldType)));
          }
        }
        if (Body.Count == 0) return null;
        var Lam = Expression.Lambda<Action<JToken, object>>(
          Expression.Block(
             Body
          ),
          new ParameterExpression[] { JValue, Target }
        );
        return Lam.Compile();
      }
      if (_UpdateInstanceByJsonCache.TryGetValue(Tp, out var MemberActs)) {
        (This as IDictionary<string, JToken>).ForEach(E => {
          if (MemberActs.TryGetValue(E.Key, out var Act)) {
            Act(E.Value, Instance);
          }
          else {
            Act = CreateFunction(E.Key);
            if (Act != null) {
              MemberActs.Add(E.Key, Act);
              Act(E.Value, Instance);
            }
          }
        });
      }
      else {
        MemberActs = (This as IDictionary<string, JToken>).Select(J => new KeyValuePair<string, Action<JToken, object>>(J.Key, CreateFunction(J.Key))).Where(E => E.Value != null).ToDictionary(E => E.Key, E => E.Value);
        _UpdateInstanceByJsonCache.Add(Tp, MemberActs);
        MemberActs.ForEach(E => {
          E.Value(This[E.Key], Instance);
        });
      }
    }
  }
} 
#endif
