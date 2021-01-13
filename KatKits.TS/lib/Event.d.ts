export declare type Action0 = () => void;
export declare type Action1<T1> = (Arg1: T1) => void;
export declare type Action2<T1, T2> = (Arg1: T1, Arg2: T2) => void;
export declare type Action3<T1, T2, T3> = (Arg1: T1, Arg2: T2, Arg3: T3) => void;
export declare type Action4<T1, T2, T3, T4> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4) => void;
export declare type Action5<T1, T2, T3, T4, T5> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5) => void;
export declare type Action6<T1, T2, T3, T4, T5, T6> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6) => void;
export declare type Action7<T1, T2, T3, T4, T5, T6, T7> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7) => void;
export declare type Action8<T1, T2, T3, T4, T5, T6, T7, T8> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8) => void;
export declare type Func0<TResult> = () => TResult;
export declare type Func1<T1, TResult> = (Arg1: T1) => TResult;
export declare type Func2<T1, T2, TResult> = (Arg1: T1, Arg2: T2) => TResult;
export declare type Func3<T1, T2, T3, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3) => TResult;
export declare type Func4<T1, T2, T3, T4, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4) => TResult;
export declare type Func5<T1, T2, T3, T4, T5, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5) => TResult;
export declare type Func6<T1, T2, T3, T4, T5, T6, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6) => TResult;
export declare type Func7<T1, T2, T3, T4, T5, T6, T7, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7) => TResult;
export declare type Func8<T1, T2, T3, T4, T5, T6, T7, T8, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8) => TResult;
export declare class Event0 extends Array<Action0> {
    constructor();
    Invoke(): void;
    Add(Action: Action0): void;
    Remove(Action: Action0): void;
}
export declare class Event1<T> extends Array<Action1<T>> {
    Invoke(Arg: T): void;
    Add(Action: Action1<T>): void;
    Remove(Action: Action1<T>): void;
}
export declare class Event2<T1, T2> extends Array<Action2<T1, T2>> {
    Invoke(Arg1: T1, Arg2: T2): void;
    Add(Action: Action2<T1, T2>): void;
    Remove(Action: Action2<T1, T2>): void;
}
export declare class Event3<T1, T2, T3> extends Array<Action3<T1, T2, T3>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3): void;
    Add(Action: Action3<T1, T2, T3>): void;
    Remove(Action: Action3<T1, T2, T3>): void;
}
export declare class Event4<T1, T2, T3, T4> extends Array<Action4<T1, T2, T3, T4>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4): void;
    Add(Action: Action4<T1, T2, T3, T4>): void;
    Remove(Action: Action4<T1, T2, T3, T4>): void;
}
export declare class Event5<T1, T2, T3, T4, T5> extends Array<Action5<T1, T2, T3, T4, T5>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5): void;
    Add(Action: Action5<T1, T2, T3, T4, T5>): void;
    Remove(Action: Action5<T1, T2, T3, T4, T5>): void;
}
export declare class Event6<T1, T2, T3, T4, T5, T6> extends Array<Action6<T1, T2, T3, T4, T5, T6>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6): void;
    Add(Action: Action6<T1, T2, T3, T4, T5, T6>): void;
    Remove(Action: Action6<T1, T2, T3, T4, T5, T6>): void;
}
export declare class Event7<T1, T2, T3, T4, T5, T6, T7> extends Array<Action7<T1, T2, T3, T4, T5, T6, T7>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7): void;
    Add(Action: Action7<T1, T2, T3, T4, T5, T6, T7>): void;
    Remove(Action: Action7<T1, T2, T3, T4, T5, T6, T7>): void;
}
export declare class Event8<T1, T2, T3, T4, T5, T6, T7, T8> extends Array<Action8<T1, T2, T3, T4, T5, T6, T7, T8>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8): void;
    Add(Action: Action8<T1, T2, T3, T4, T5, T6, T7, T8>): void;
    Remove(Action: Action8<T1, T2, T3, T4, T5, T6, T7, T8>): void;
}
export declare type AsyncAction0 = () => Promise<void>;
export declare type AsyncAction1<T1> = (Arg1: T1) => Promise<void>;
export declare type AsyncAction2<T1, T2> = (Arg1: T1, Arg2: T2) => Promise<void>;
export declare type AsyncAction3<T1, T2, T3> = (Arg1: T1, Arg2: T2, Arg3: T3) => Promise<void>;
export declare type AsyncAction4<T1, T2, T3, T4> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4) => Promise<void>;
export declare type AsyncAction5<T1, T2, T3, T4, T5> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5) => Promise<void>;
export declare type AsyncAction6<T1, T2, T3, T4, T5, T6> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6) => Promise<void>;
export declare type AsyncAction7<T1, T2, T3, T4, T5, T6, T7> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7) => Promise<void>;
export declare type AsyncAction8<T1, T2, T3, T4, T5, T6, T7, T8> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8) => Promise<void>;
export declare type AsyncFunc0<TResult> = () => Promise<TResult>;
export declare type AsyncFunc1<T1, TResult> = (Arg1: T1) => Promise<TResult>;
export declare type AsyncFunc2<T1, T2, TResult> = (Arg1: T1, Arg2: T2) => Promise<TResult>;
export declare type AsyncFunc3<T1, T2, T3, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3) => Promise<TResult>;
export declare type AsyncFunc4<T1, T2, T3, T4, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4) => Promise<TResult>;
export declare type AsyncFunc5<T1, T2, T3, T4, T5, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5) => Promise<TResult>;
export declare type AsyncFunc6<T1, T2, T3, T4, T5, T6, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6) => Promise<TResult>;
export declare type AsyncFunc7<T1, T2, T3, T4, T5, T6, T7, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7) => Promise<TResult>;
export declare type AsyncFunc8<T1, T2, T3, T4, T5, T6, T7, T8, TResult> = (Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8) => Promise<TResult>;
export declare class AsyncEvent0 extends Array<AsyncAction0> {
    constructor();
    Invoke(): Promise<void>;
    Add(Action: AsyncAction0): void;
    Remove(Action: AsyncAction0): void;
}
export declare class AsyncEvent1<T> extends Array<AsyncAction1<T>> {
    Invoke(Arg: T): Promise<void>;
    Add(Action: AsyncAction1<T>): void;
    Remove(Action: AsyncAction1<T>): void;
}
export declare class AsyncEvent2<T1, T2> extends Array<AsyncAction2<T1, T2>> {
    Invoke(Arg1: T1, Arg2: T2): Promise<void>;
    Add(Action: AsyncAction2<T1, T2>): void;
    Remove(Action: AsyncAction2<T1, T2>): void;
}
export declare class AsyncEvent3<T1, T2, T3> extends Array<AsyncAction3<T1, T2, T3>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3): Promise<void>;
    Add(Action: AsyncAction3<T1, T2, T3>): void;
    Remove(Action: AsyncAction3<T1, T2, T3>): void;
}
export declare class AsyncEvent4<T1, T2, T3, T4> extends Array<AsyncAction4<T1, T2, T3, T4>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4): Promise<void>;
    Add(Action: AsyncAction4<T1, T2, T3, T4>): void;
    Remove(Action: AsyncAction4<T1, T2, T3, T4>): void;
}
export declare class AsyncEvent5<T1, T2, T3, T4, T5> extends Array<AsyncAction5<T1, T2, T3, T4, T5>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5): Promise<void>;
    Add(Action: AsyncAction5<T1, T2, T3, T4, T5>): void;
    Remove(Action: AsyncAction5<T1, T2, T3, T4, T5>): void;
}
export declare class AsyncEvent6<T1, T2, T3, T4, T5, T6> extends Array<AsyncAction6<T1, T2, T3, T4, T5, T6>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6): Promise<void>;
    Add(Action: AsyncAction6<T1, T2, T3, T4, T5, T6>): void;
    Remove(Action: AsyncAction6<T1, T2, T3, T4, T5, T6>): void;
}
export declare class AsyncEvent7<T1, T2, T3, T4, T5, T6, T7> extends Array<AsyncAction7<T1, T2, T3, T4, T5, T6, T7>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7): Promise<void>;
    Add(Action: AsyncAction7<T1, T2, T3, T4, T5, T6, T7>): void;
    Remove(Action: AsyncAction7<T1, T2, T3, T4, T5, T6, T7>): void;
}
export declare class AsyncEvent8<T1, T2, T3, T4, T5, T6, T7, T8> extends Array<AsyncAction8<T1, T2, T3, T4, T5, T6, T7, T8>> {
    Invoke(Arg1: T1, Arg2: T2, Arg3: T3, Arg4: T4, Arg5: T5, Arg6: T6, Arg7: T7, Arg8: T8): Promise<void>;
    Add(Action: AsyncAction8<T1, T2, T3, T4, T5, T6, T7, T8>): void;
    Remove(Action: AsyncAction8<T1, T2, T3, T4, T5, T6, T7, T8>): void;
}
