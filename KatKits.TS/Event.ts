export namespace KatKits {
  export type Action0 = () => void;
  export type Action1<T1> = (Arg1: T1) => void;
  export type Action2<T1, T2> = (Arg1: T1, Arg2: T2) => void;
  export type Action3<T1, T2, T3> = (Arg1: T1, Arg2: T2, Arg3: T3) => void;
  export type Action4<T1, T2, T3, T4> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4) => void;
  export type Action5<T1, T2, T3, T4, T5> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5) => void;
  export type Action6<T1, T2, T3, T4, T5, T6> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6) => void;
  export type Action7<T1, T2, T3, T4, T5, T6, T7> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7) => void;
  export type Action8<T1, T2, T3, T4, T5, T6, T7, T8> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8) => void;
  export type Func0<TResult> = () => TResult;
  export type Func1<T1, TResult> = (Arg1: T1) => TResult;
  export type Func2<T1, T2, TResult> = (Arg1: T1, Arg2: T2) => TResult;
  export type Func3<T1, T2, T3, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3) => TResult;
  export type Func4<T1, T2, T3, T4, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4) => TResult;
  export type Func5<T1, T2, T3, T4, T5, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5) => TResult;
  export type Func6<T1, T2, T3, T4, T5, T6, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6) => TResult;
  export type Func7<T1, T2, T3, T4, T5, T6, T7, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7) => TResult;
  export type Func8<T1, T2, T3, T4, T5, T6, T7, T8, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8) => TResult;

  export class Event0 extends Array<Action0>{
    public Invoke() {
      if (this.length > 0) {
        this.forEach(E => E())
      }
    }
    public Add(Action: Action0) {
      this.push(Action);
    }
    public Remove(Action: Action0) {
      this.splice(this.findIndex(E => E === Action), 1);
    }
  }
  export class Event1<TArg> extends Array<Action1<TArg>>{
    public Invoke(Arg: TArg) {
      if (this.length > 0) {
        this.forEach(E => E(Arg))
      }
    }
    public Add(Action: Action1<TArg>) {
      this.push(Action);
    }
    public Remove(Action: Action1<TArg>) {
      this.splice(this.findIndex(E => E === Action), 1);
    }
  }
  export class Event2<TArg1, TArg2> extends Array<Action2<TArg1, TArg2>>{
    public Invoke(Arg1: TArg1, Arg2: TArg2) {
      if (this.length > 0) {
        this.forEach(E => E(Arg1, Arg2))
      }
    }
    public Add(Action: Action2<TArg1, TArg2>) {
      this.push(Action);
    }
    public Remove(Action: Action2<TArg1, TArg2>) {
      this.splice(this.findIndex(E => E === Action), 1);
    }
  }
  export class Event3<TArg1, TArg2, TArg3> extends Array<Action3<TArg1, TArg2, TArg3>>{
    public Invoke(Arg1: TArg1, Arg2: TArg2, Arg3: TArg3) {
      if (this.length > 0) {
        this.forEach(E => E(Arg1, Arg2, Arg3))
      }
    }
    public Add(Action: Action3<TArg1, TArg2, TArg3>) {
      this.push(Action);
    }
    public Remove(Action: Action3<TArg1, TArg2, TArg3>) {
      this.splice(this.findIndex(E => E === Action), 1);
    }
  }



}