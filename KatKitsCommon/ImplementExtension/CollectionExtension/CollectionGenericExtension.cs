namespace KatKits.ImplementExtension.CollectionExtension {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;

  public static partial class Kits {
    /// <summary>
    /// combine Array.ForEach List<T>.ForEach and foreach block together
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Source"></param>
    /// <param name="Action"></param>
    public static void ForEach<T>(this IEnumerable<T> Source, Action<T> Action) {
      switch (Source) {
        case T[] A:
        Array.ForEach(A, Action);
        break;
        case List<T> L:
        L.ForEach(Action);
        break;
        default:
        foreach (var item in Source) {
          Action(item);
        }
        break;
      }

    }




    [Obsolete("use skip-take instead")]
    public static IEnumerable<T> Split<T>(this IEnumerable<T> This, int Start, int Length) {
      int Index = -1;
      foreach (var item in This) {
        Index++;
        if (Index >= Start && Index - Start <= Length - 1) {
          yield return item;
        }
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

    #region Sort
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

    #endregion

    #region Remove
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
    #endregion


    /// <summary>
    /// 枚举一段数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="This"></param>
    /// <param name="StartIndex"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    [Obsolete("use skip-take instead")]
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

    [Obsolete("use Enumerable.Any instead")]
    public static bool And<T>(this IEnumerable<T> This, Func<T, bool> Field) {
      foreach (var item in This) {
        if (Field(item) == false) return false;
      }
      return true;
    }
    [Obsolete("use Enumerable.Any instead")]
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
    [Obsolete("use Enumerable.Any instead")]
    public static bool Or<T>(this IEnumerable<T> This, Func<T, bool> Field) {
      foreach (var item in This) {
        if (Field(item)) return true;
      }
      return false;
    }
    [Obsolete("use Enumerable.Any instead")]
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
    [Obsolete("use Enumerable.Any instead")]
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

   

 
  }


}
