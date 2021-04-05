
export const UTCToLocalDateTime = function (UTCDate: Date) {
  return new Date(UTCDate.toLocaleString());
};

/**
 * formate datetime toString like C#(but not look like).
 * @param CurrentDate JS Date
 * @param Formate y年,M月,d日,H:时(24),h:时(12),m:分,s:秒,z:时区(GMT±NN:00),按对应数字从后向前替换对应字母，多余的字母由0补齐，其他字符原样输出,时区仅支持一个字母z,使用时区整个替换字母z，12小时值将在h部分后添加' AM'或‘ PM’;
 */
export const FormateDate = function (CurrentDate: Date, Formate: string) {
  let Result = "";
  let C: string | null = null;
  let CI = 0;
  for (let i = Formate.length - 1; i >= 0; i--) {
    switch (Formate[i]) {
      case 'z':
        if (C === null) {
          C = "GMT"
            + (CurrentDate.getTimezoneOffset() > 0 ? "-" : "+")
            + (Math.abs(CurrentDate.getTimezoneOffset()) / 60 < 10 ? "0" : "")
            + ((-(CurrentDate.getTimezoneOffset())) / 60).toString()
            + ":00";
          CI = 1;
        }
        Result = C + Result;
        break;
      case 'f':
        if (C === null) {
          C = CurrentDate.getMilliseconds().toString();
          CI = C.length;
        }
        Result = IndexOfNumberString(C, --CI) + Result;
        break;
      case 's':
        if (C === null) {
          C = CurrentDate.getSeconds().toString();
          CI = C.length;
        }
        Result = IndexOfNumberString(C, --CI) + Result;
        break;
      case 'm':
        if (C === null) {
          C = CurrentDate.getMinutes().toString();
          CI = C.length;
        }
        Result = IndexOfNumberString(C, --CI) + Result;
        break;
      case 'H':
        if (C === null) {
          C = CurrentDate.getHours().toString();
          CI = C.length;
        }
        Result = IndexOfNumberString(C, --CI) + Result;
        break;
      case 'h':
        if (C === null) {
          C = (CurrentDate.getHours() % 12).toString();
          CI = C.length;
          C = C + ((CurrentDate.getHours() - 12) > 0 ? " PM" : " AM");
        }
        Result = C + Result;
        break;
      case 'd':
        if (C === null) {
          C = CurrentDate.getDate().toString();
          CI = C.length;
        }
        Result = IndexOfNumberString(C, --CI) + Result;
        break;
      case 'M':
        if (C === null) {
          C = (CurrentDate.getMonth() + 1).toString();
          CI = C.length;
        }
        Result = IndexOfNumberString(C, --CI) + Result;
        break;
      case 'y':
        if (C === null) {
          C = CurrentDate.getFullYear().toString();
          CI = C.length;
        }
        Result = IndexOfNumberString(C, --CI) + Result;
        break;

      default:
        C = null;
        Result = Formate[i] + Result;
        break;
    }
  }

  return Result;
}
/**
 * NumberString Index
 * @param Str Source String
 * @param Index <0 : return 0;=length : return .;>length : return 0;
 */
export const IndexOfNumberString = function (Str: string, Index: number) {
  return Index < 0 ? "0"
    : Index === Str.length ? "."
      : Index > Str.length ? "0"
        : Str[Index];
}

export const RemoveBreaks = function (OldString: string) {
  return OldString.replace(/(\r\n|\n|\r)/gm, "");
}

export const RemoveBreaksAndWriteSpaceBehind = function (OldString: string) {
  return OldString.replace(/(\r\n|\n|\r)\ */gm, "");
}

export const Insert = function <T>(Source: Array<T>, Index: number, Value: T) {
  Source.splice(Index, 0, Value);
}

export const InsertChar = function (Source: string, Index: number, Value: string): string {
  return Source.slice(0, Index) + Value + Source.slice(Index);
}

export const Index = function () {
  if (!String.prototype.splice) {
    String.prototype.splice = InsertChar;
  }
}

