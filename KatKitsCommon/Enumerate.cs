namespace KatKits {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;

  public static partial class KatKits {
    public static void ForEach<T>(this T[] This, Action<T> Action) => Array.ForEach(This, Action);

    public static IEnumerable<T> Split<T>(this IEnumerable<T> This, int Start, int Length) {
      int Index = -1;
      foreach (var item in This) {
        Index++;
        if (Index >= Start && Index - Start <= Length - 1) {
          yield return item;
        }
      }
    }

    public static TV TryGetValue<TK, TV>(this Dictionary<TK, TV> This, TK Key) where TV : class {
      if (This.TryGetValue(Key, out TV V)) {
        return V;
      }
      else {
        return null;
      }
    }

    public static T GetLinkedListItem<T>(this LinkedList<T> This,int Index)
    {
      return This.Skip(Index).FirstOrDefault();
    }
    public static IEnumerable<LinkedListNode<T>> LinkedListItems<T>(this LinkedList<T> This)
    {
      var H = This.First;
      while (H != null)
      {
        yield return H;
        H = H.Next;
      }
      yield break;
    }

    /// <summary>
    /// 自定义排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="This"></param>
    /// <param name="Compare"></param>
    public static void Sort<T>(this List<T> This, Func<T, T, int> Compare) {
      T Temp = default(T);
      for (int i = 0; i < This.Count - 1; i++) {
        for (int j = 0; j < This.Count - 1 - i; j++) {
          if (Compare(This[j], This[j + 1]) < 0) {
            Temp = This[j + 1];
            This[j + 1] = This[j];
            This[j] = Temp;
          }
        }
      }
    }
    /// <summary>
    /// 选择移除
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="This"></param>
    /// <param name="Match"></param>
    public static void RemoveSelect<T>(this List<T> This, Func<T, bool> Match) {
      for (int i = This.Count - 1; i >= 0; i--) {
        if (Match(This[i])) {
          This.RemoveAt(i);
        }
      }
    }
    public static bool RemoveSelectLast<T>(this List<T> This, Func<T, bool> Match) {
      for (int i = This.Count - 1; i >= 0; i--) {
        if (Match(This[i])) {
          This.RemoveAt(i);
          return true;
        }
      }
      return false;
    }
    public static bool RemoveSelectFirst<T>(this List<T> This, Func<T, bool> Match) {
      for (int i = 0; i < This.Count; i--) {
        if (Match(This[i])) {
          This.RemoveAt(i);
          return true;
        }
      }
      return false;
    }
    public static int RemoveSelectOne<T>(this List<T> This, Func<T, bool> Match) {
      for (int i = This.Count - 1; i >= 0; i--) {
        if (Match(This[i])) {
          This.RemoveAt(i);
          return i;
        }
      }
      return -1;
    }
    public static T RemoveSelectOne2<T>(this List<T> This, Func<T, bool> Match) where T : class {
      for (int i = This.Count - 1; i >= 0; i--) {
        if (Match(This[i])) {
          var Res = This[i];
          This.RemoveAt(i);
          return Res;
        }
      }
      return null;
    }
    /// <summary>
    /// 枚举一段数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="This"></param>
    /// <param name="StartIndex"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static IEnumerable<T> ListSplit<T>(this IList<T> This, int StartIndex, int Length) {
      int Director = Length >> 31 | 1;
      Length = StartIndex + Math.Abs(Length);
      while (StartIndex < Length && StartIndex > 0 && StartIndex < This.Count) {
        StartIndex += Director;
        yield return This[StartIndex];
      }
    }
    /// <summary>
    /// 环形枚举一段数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="This"></param>
    /// <param name="StartIndex"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static IEnumerable<T> ListLoopSplit<T>(this IList<T> This, int StartIndex, int Length) {
      int Director = Length >> 31 | 1;
      Length = StartIndex + Math.Abs(Length);
      while (StartIndex < Length) {
        StartIndex = (StartIndex + This.Count) % This.Count;
        yield return This[StartIndex];
        StartIndex += Director;
      }
    }

    public static bool And<T>(this IEnumerable<T> This, Func<T, bool> Field) {
      foreach (var item in This) {
        if (Field(item) == false) return false;
      }
      return true;
    }
    public static bool? And<T>(this IEnumerable<T> This, Func<T, bool?> Field) {
      foreach (var item in This) {
        var Res = Field(item);
        if (Res.HasValue) {
          if (Res.Value == false) return false;
        }
        else {
          return null;
        }
      }
      return true;
    }
    public static bool Or<T>(this IEnumerable<T> This, Func<T, bool> Field) {
      foreach (var item in This) {
        if (Field(item)) return true;
      }
      return false;
    }
    public static bool? Or<T>(this IEnumerable<T> This, Func<T, bool?> Field) {
      foreach (var item in This) {
        var Res = Field(item);
        if (Res.HasValue) {
          if (Res.Value) return true;
        }
        else {
          return null;
        }
      }
      return false;
    }

    /// <summary>
    /// All True:True All False:False Has Null:Null Else:Null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="This"></param>
    /// <param name="Field"></param>
    /// <returns></returns>
    public static bool? IsUnity<T>(this IEnumerable<T> This, Func<T, bool?> Field) {
      bool? AllRes = null;
      foreach (var item in This) {
        var Res = Field(item);
        if (Res.HasValue) {
          if (AllRes.HasValue) {
            if (AllRes.Value != Res.Value) return null;
          }
          else {
            AllRes = Res.Value;
          }
        }
        else {
          return null;
        }
      }
      return AllRes.Value;
    }

    public static bool OrderedEqual<T>(this IEnumerable<T> This,IEnumerable<T> Others,int SourceOffset = 0,int OthersOffset = 0,int Length = 0,bool MatchLength = true)
    {
      var _Source = Length == 0 ? This.Skip(SourceOffset) : This.Skip(SourceOffset).Take(Length);
      var _Other = Length == 0 ? Others.Skip(OthersOffset) : Others.Skip(SourceOffset).Take(Length);
      if (MatchLength && _Source.Count() != _Other.Count()) return false;
      return _Source.Zip(_Other, (L, R) => L.Equals(R)).Any(E => false);
    }

    public static IEnumerable<uint> ReverseUIntRange(uint Start, uint Length) {
      return new _ReverseUIntRange(Start, Length);
    }
    public static IEnumerable<uint> UIntRange(uint Start, uint Length) {
      return new _UIntRange(Start, Length);
    }
  }
  public class _ReverseUIntRange : IEnumerable<uint>, IEnumerator<uint> {
    public _ReverseUIntRange(uint Start, uint Length) {
      this.Start = Start;
      this.Length = Length;
      Reset();
    }
    public IEnumerator<uint> GetEnumerator() {
      return this;
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return this;
    }


    public uint Current { get; private set; }
    public uint Start { get; private set; }
    public uint Length { get; private set; }
    private uint Step { get; set; } = 0;
    public bool MoveNext() {
      Step++;
      Current = --Current;
      return Current >= 0 && Step <= Length;
    }

    public void Reset() {
      Current = Start;
      Step = 0;
    }

    object IEnumerator.Current { get => Current; }

    public void Dispose() {
      Reset();
    }
  }
  public class _UIntRange : IEnumerable<uint>, IEnumerator<uint> {
    public _UIntRange(uint Start, uint Length) {
      this.Start = Start;
      this.Length = Length;
      Reset();
    }
    public IEnumerator<uint> GetEnumerator() {
      return this;
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return this;
    }

    public uint Current { get; private set; }
    public uint Start { get; private set; }
    public uint Length { get; private set; }
    private uint Step { get; set; } = 0;

    public bool MoveNext() {
      Step++;
      Current++;
      return Step <= Length;
    }

    public void Reset() {
      Current = Start - 1;
      Step = 0;
    }

    object IEnumerator.Current { get => Current; }

    public void Dispose() {
      Reset();
    }
  }

}
