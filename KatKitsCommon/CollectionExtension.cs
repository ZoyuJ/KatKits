namespace KatKits
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public static class CollectionUtil
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> This, TKey Key, Func<TValue> Create)
        {
            if (This.TryGetValue(Key, out var _Value)) return _Value;
            else
            {
                _Value = Create();
                This.Add(Key, _Value);
                return _Value;
            }
        }
        public static object GetOrAdd(this IDictionary This, object Key, Func<object> Create)
        {
            if (This.Contains(Key)) return This[Key];
            else
            {
                object _Ins = Create();
                This.Add(Key, _Ins);
                return _Ins;
            }
        }
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> This, TKey Key, TValue Value)
        {
            if (This.ContainsKey(Key)) return false;
            This.Add(Key, Value);
            return true;
        }
        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> This, TKey Key, Func<TKey, TValue> OnFail)
        {
            if (This.TryGetValue(Key, out var V)) return V;
            else return OnFail(Key);
        }
        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> This, TKey Key)
        {
            if (This.TryGetValue(Key, out var V))
            {
                return V;
            }
            else return default;
        }


        public static void ForEach<T>(this IEnumerable<T> Source, Action<T> Action)
        {
            switch (Source)
            {
                case T[] A:
                    Array.ForEach(A, Action);
                    break;
                case List<T> L:
                    L.ForEach(Action);
                    break;
                default:
                    foreach (var item in Source)
                    {
                        Action(item);
                    }
                    break;
            }

        }
        public static IEnumerable<T> Do<T>(this IEnumerable<T> Source, Action<T> Action)
        {
            return Source.Select(E => { Action(E); return E; });
        }



        public static LinkedListNode<T> GetNode<T>(this LinkedList<T> This, int Index)
        {
            if (Index >= This.Count) return null;
            var C = This.First;
            while (Index > 0 && C != null)
            {
                Index--;
                C = C.Next;
            }
            return C;
        }
    }
}
