
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
  constructor() { super(); }
  public Invoke() {
    if (this.length > 0) {
      this.forEach(E => E());
    }
  }
  public Add(Action: Action0) {
    this.push(Action);
  }
  public Remove(Action: Action0) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class Event1<T> extends Array<Action1<T>>{
  public Invoke(Arg: T) {
    if (this.length > 0) {
      this.forEach(E => E(Arg));
    }
  }
  public Add(Action: Action1<T>) {
    this.push(Action);
  }
  public Remove(Action: Action1<T>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class Event2<T1, T2> extends Array<Action2<T1, T2>>{
  public Invoke(Arg1: T1, Arg2: T2) {
    if (this.length > 0) {
      this.forEach(E => E(Arg1, Arg2));
    }
  }
  public Add(Action: Action2<T1, T2>) {
    this.push(Action);
  }
  public Remove(Action: Action2<T1, T2>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class Event3<T1, T2, T3> extends Array<Action3<T1, T2, T3>>{
  public Invoke(Arg1: T1, Arg2: T2, Arg3: T3) {
    if (this.length > 0) {
      this.forEach(E => E(Arg1, Arg2, Arg3));
    }
  }
  public Add(Action: Action3<T1, T2, T3>) {
    this.push(Action);
  }
  public Remove(Action: Action3<T1, T2, T3>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class Event4<T1, T2, T3, T4> extends Array<Action4<T1, T2, T3, T4>>{
  public Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4) {
    if (this.length > 0) {
      this.forEach(E => E(Arg1, Arg2, Arg3, Arg4));
    }
  }
  public Add(Action: Action4<T1, T2, T3, T4>) {
    this.push(Action);
  }
  public Remove(Action: Action4<T1, T2, T3, T4>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class Event5<T1, T2, T3, T4, T5> extends Array<Action5<T1, T2, T3, T4, T5>>{
  public Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5) {
    if (this.length > 0) {
      this.forEach(E => E(Arg1, Arg2, Arg3, Arg4, Arg5));
    }
  }
  public Add(Action: Action5<T1, T2, T3, T4, T5>) {
    this.push(Action);
  }
  public Remove(Action: Action5<T1, T2, T3, T4, T5>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class Event6<T1, T2, T3, T4, T5, T6> extends Array<Action6<T1, T2, T3, T4, T5, T6>>{
  public Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6) {
    if (this.length > 0) {
      this.forEach(E => E(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6));
    }
  }
  public Add(Action: Action6<T1, T2, T3, T4, T5, T6>) {
    this.push(Action);
  }
  public Remove(Action: Action6<T1, T2, T3, T4, T5, T6>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class Event7<T1, T2, T3, T4, T5, T6, T7> extends Array<Action7<T1, T2, T3, T4, T5, T6, T7>>{
  public Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7) {
    if (this.length > 0) {
      this.forEach(E => E(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7));
    }
  }
  public Add(Action: Action7<T1, T2, T3, T4, T5, T6, T7>) {
    this.push(Action);
  }
  public Remove(Action: Action7<T1, T2, T3, T4, T5, T6, T7>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class Event8<T1, T2, T3, T4, T5, T6, T7, T8> extends Array<Action8<T1, T2, T3, T4, T5, T6, T7, T8>>{
  public Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8) {
    if (this.length > 0) {
      this.forEach(E => E(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8));
    }
  }
  public Add(Action: Action8<T1, T2, T3, T4, T5, T6, T7, T8>) {
    this.push(Action);
  }
  public Remove(Action: Action8<T1, T2, T3, T4, T5, T6, T7, T8>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}

export type AsyncAction0 = () => Promise<void>;
export type AsyncAction1<T1> = (Arg1: T1) => Promise<void>;
export type AsyncAction2<T1, T2> = (Arg1: T1, Arg2: T2) => Promise<void>;
export type AsyncAction3<T1, T2, T3> = (Arg1: T1, Arg2: T2, Arg3: T3) => Promise<void>;
export type AsyncAction4<T1, T2, T3, T4> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4) => Promise<void>;
export type AsyncAction5<T1, T2, T3, T4, T5> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5) => Promise<void>;
export type AsyncAction6<T1, T2, T3, T4, T5, T6> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6) => Promise<void>;
export type AsyncAction7<T1, T2, T3, T4, T5, T6, T7> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7) => Promise<void>;
export type AsyncAction8<T1, T2, T3, T4, T5, T6, T7, T8> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8) => Promise<void>;

export type AsyncFunc0<TResult> = () => Promise<TResult>;
export type AsyncFunc1<T1, TResult> = (Arg1: T1) => Promise<TResult>;
export type AsyncFunc2<T1, T2, TResult> = (Arg1: T1, Arg2: T2) => Promise<TResult>;
export type AsyncFunc3<T1, T2, T3, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3) => Promise<TResult>;
export type AsyncFunc4<T1, T2, T3, T4, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4) => Promise<TResult>;
export type AsyncFunc5<T1, T2, T3, T4, T5, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5) => Promise<TResult>;
export type AsyncFunc6<T1, T2, T3, T4, T5, T6, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6) => Promise<TResult>;
export type AsyncFunc7<T1, T2, T3, T4, T5, T6, T7, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7) => Promise<TResult>;
export type AsyncFunc8<T1, T2, T3, T4, T5, T6, T7, T8, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8) => Promise<TResult>;

export class AsyncEvent0 extends Array<AsyncAction0>{
  constructor() { super(); }
  public async Invoke(): Promise<void> {
    if (this.length <= 0) return;
    for (const Delegate of this) {
      await Delegate();
    }
  }
  public Add(Action: AsyncAction0) {
    this.push(Action);
  }
  public Remove(Action: AsyncAction0) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class AsyncEvent1<T> extends Array<AsyncAction1<T>>{
  public async Invoke(Arg: T): Promise<void> {
    if (this.length <= 0) return;
    for (const Delegate of this) {
      await Delegate(Arg);
    }
  }
  public Add(Action: AsyncAction1<T>) {
    this.push(Action);
  }
  public Remove(Action: AsyncAction1<T>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class AsyncEvent2<T1, T2> extends Array<AsyncAction2<T1, T2>>{
  public async Invoke(Arg1: T1, Arg2: T2): Promise<void> {
    if (this.length <= 0) return;
    for (const Delegate of this) {
      await Delegate(Arg1, Arg2);
    }
  }
  public Add(Action: AsyncAction2<T1, T2>) {
    this.push(Action);
  }
  public Remove(Action: AsyncAction2<T1, T2>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class AsyncEvent3<T1, T2, T3> extends Array<AsyncAction3<T1, T2, T3>>{
  public async Invoke(Arg1: T1, Arg2: T2, Arg3: T3): Promise<void> {
    if (this.length <= 0) return;
    for (const Delegate of this) {
      await Delegate(Arg1, Arg2, Arg3);
    }
  }
  public Add(Action: AsyncAction3<T1, T2, T3>) {
    this.push(Action);
  }
  public Remove(Action: AsyncAction3<T1, T2, T3>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class AsyncEvent4<T1, T2, T3, T4> extends Array<AsyncAction4<T1, T2, T3, T4>>{
  public async Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4): Promise<void> {
    if (this.length <= 0) return;
    for (const Delegate of this) {
      await Delegate(Arg1, Arg2, Arg3, Arg4);
    }
  }
  public Add(Action: AsyncAction4<T1, T2, T3, T4>) {
    this.push(Action);
  }
  public Remove(Action: AsyncAction4<T1, T2, T3, T4>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class AsyncEvent5<T1, T2, T3, T4, T5> extends Array<AsyncAction5<T1, T2, T3, T4, T5>>{
  public async Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5): Promise<void> {
    if (this.length <= 0) return;
    for (const Delegate of this) {
      await Delegate(Arg1, Arg2, Arg3, Arg4, Arg5);
    }
  }
  public Add(Action: AsyncAction5<T1, T2, T3, T4, T5>) {
    this.push(Action);
  }
  public Remove(Action: AsyncAction5<T1, T2, T3, T4, T5>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class AsyncEvent6<T1, T2, T3, T4, T5, T6> extends Array<AsyncAction6<T1, T2, T3, T4, T5, T6>>{
  public async Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6): Promise<void> {
    if (this.length <= 0) return;
    for (const Delegate of this) {
      await Delegate(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6);
    }
  }
  public Add(Action: AsyncAction6<T1, T2, T3, T4, T5, T6>) {
    this.push(Action);
  }
  public Remove(Action: AsyncAction6<T1, T2, T3, T4, T5, T6>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class AsyncEvent7<T1, T2, T3, T4, T5, T6, T7> extends Array<AsyncAction7<T1, T2, T3, T4, T5, T6, T7>>{
  public async Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7): Promise<void> {
    if (this.length <= 0) return;
    for (const Delegate of this) {
      await Delegate(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7);
    }
  }
  public Add(Action: AsyncAction7<T1, T2, T3, T4, T5, T6, T7>) {
    this.push(Action);
  }
  public Remove(Action: AsyncAction7<T1, T2, T3, T4, T5, T6, T7>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}
export class AsyncEvent8<T1, T2, T3, T4, T5, T6, T7, T8> extends Array<AsyncAction8<T1, T2, T3, T4, T5, T6, T7, T8>>{
  public async Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8): Promise<void> {
    if (this.length <= 0) return;
    for (const Delegate of this) {
      await Delegate(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8);
    }
  }
  public Add(Action: AsyncAction8<T1, T2, T3, T4, T5, T6, T7, T8>) {
    this.push(Action);
  }
  public Remove(Action: AsyncAction8<T1, T2, T3, T4, T5, T6, T7, T8>) {
    this.splice(this.findIndex(E => E === Action), 1);
  }
}