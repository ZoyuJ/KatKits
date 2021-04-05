export declare const UTCToLocalDateTime: (UTCDate: Date) => Date;
/**
 * formate datetime toString like C#(but not look like).
 * @param CurrentDate JS Date
 * @param Formate y年,M月,d日,H:时(24),h:时(12),m:分,s:秒,z:时区(GMT±NN:00),按对应数字从后向前替换对应字母，多余的字母由0补齐，其他字符原样输出,时区仅支持一个字母z,使用时区整个替换字母z，12小时值将在h部分后添加' AM'或‘ PM’;
 */
export declare const FormateDate: (CurrentDate: Date, Formate: string) => string;
/**
 * NumberString Index
 * @param Str Source String
 * @param Index <0 : return 0;=length : return .;>length : return 0;
 */
export declare const IndexOfNumberString: (Str: string, Index: number) => string;
export declare const RemoveBreaks: (OldString: string) => string;
export declare const RemoveBreaksAndWriteSpaceBehind: (OldString: string) => string;
export declare const Insert: <T>(Source: T[], Index: number, Value: T) => void;
export declare const InsertChar: (Source: string, Index: number, Value: string) => string;
export declare const Index: () => void;
