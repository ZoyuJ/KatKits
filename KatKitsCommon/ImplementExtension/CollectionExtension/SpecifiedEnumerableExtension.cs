namespace KatKits.ImplementExtension.CollectionExtension {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public static partial class Kits {

    public static IEnumerable<DateTime> MonthByMonth(DateTime Start, int Count)
    => Count >= 0
        ? Enumerable.Range(0, Count).Select(E => Start.AddMonths(E))
        : Enumerable.Range(0, -Count).Select(E => Start.AddMonths(-E));
    public static bool OrderedEqual<T>(this IEnumerable<T> This, IEnumerable<T> Others, int SourceOffset = 0, int OthersOffset = 0, int Length = 0, bool MatchLength = true) {
      var _Source = Length == 0 ? This.Skip(SourceOffset) : This.Skip(SourceOffset).Take(Length);
      var _Other = Length == 0 ? Others.Skip(OthersOffset) : Others.Skip(SourceOffset).Take(Length);
      if (MatchLength && _Source.Count() != _Other.Count()) return false;
      return !_Source.Zip(_Other, (L, R) => L.Equals(R)).Any(E => E == false);
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
