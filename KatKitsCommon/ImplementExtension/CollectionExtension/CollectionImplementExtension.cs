namespace KatKits.ImplementExtension.CollectionExtension {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Text;

  public static partial class Kits {


    #region Dictionary
    public static TV TryGetValue<TK, TV>(this IDictionary<TK, TV> This, TK Key) where TV : class {
      if (This.TryGetValue(Key, out TV V)) {
        return V;
      }
      else {
        return null;
      }
    }
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> This, TKey Key, Func<TValue> Create) {
      if (This.TryGetValue(Key, out var _Value)) return _Value;
      else {
        _Value = Create();
        This.Add(Key, _Value);
        return _Value;
      }
    }
    public static object GetOrAdd(this IDictionary This, object Key, Func<object> Create) {
      if (This.Contains(Key)) return This[Key];
      else {
        object _Ins = Create();
        This.Add(Key, _Ins);
        return _Ins;
      }
    }
    /// <summary>
    /// add if can
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="This"></param>
    /// <param name="Key"></param>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> This, TKey Key, TValue Value) {
      if (This.ContainsKey(Key)) return false;
      This.Add(Key, Value);
      return true;
    }
    /// <summary>
    /// can get ,if not call delegate
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="This"></param>
    /// <param name="Key"></param>
    /// <param name="OnFail"></param>
    /// <returns></returns>
    public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> This, TKey Key, Func<TKey, TValue> OnFail) {
      if (This.TryGetValue(Key, out var V)) return V;
      else return OnFail(Key);
    }
    /// <summary>
    /// if cant get value from dict, return default TValue
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="This"></param>
    /// <param name="Key"></param>
    /// <returns></returns>
    public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> This, TKey Key) {
      if (This.TryGetValue(Key, out var V)) {
        return V;
      }
      else return default(TValue);
    }



    #endregion

  }
}
