"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Index = exports.InsertChar = exports.Insert = exports.RemoveBreaksAndWriteSpaceBehind = exports.RemoveBreaks = exports.IndexOfNumberString = exports.FormateDate = exports.UTCToLocalDateTime = void 0;
var UTCToLocalDateTime = function (UTCDate) {
    return new Date(UTCDate.toLocaleString());
};
exports.UTCToLocalDateTime = UTCToLocalDateTime;
/**
 * formate datetime toString like C#(but not look like).
 * @param CurrentDate JS Date
 * @param Formate y年,M月,d日,H:时(24),h:时(12),m:分,s:秒,z:时区(GMT±NN:00),按对应数字从后向前替换对应字母，多余的字母由0补齐，其他字符原样输出,时区仅支持一个字母z,使用时区整个替换字母z，12小时值将在h部分后添加' AM'或‘ PM’;
 */
var FormateDate = function (CurrentDate, Formate) {
    var Result = "";
    var C = null;
    var CI = 0;
    for (var i = Formate.length - 1; i >= 0; i--) {
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
                Result = exports.IndexOfNumberString(C, --CI) + Result;
                break;
            case 's':
                if (C === null) {
                    C = CurrentDate.getSeconds().toString();
                    CI = C.length;
                }
                Result = exports.IndexOfNumberString(C, --CI) + Result;
                break;
            case 'm':
                if (C === null) {
                    C = CurrentDate.getMinutes().toString();
                    CI = C.length;
                }
                Result = exports.IndexOfNumberString(C, --CI) + Result;
                break;
            case 'H':
                if (C === null) {
                    C = CurrentDate.getHours().toString();
                    CI = C.length;
                }
                Result = exports.IndexOfNumberString(C, --CI) + Result;
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
                Result = exports.IndexOfNumberString(C, --CI) + Result;
                break;
            case 'M':
                if (C === null) {
                    C = (CurrentDate.getMonth() + 1).toString();
                    CI = C.length;
                }
                Result = exports.IndexOfNumberString(C, --CI) + Result;
                break;
            case 'y':
                if (C === null) {
                    C = CurrentDate.getFullYear().toString();
                    CI = C.length;
                }
                Result = exports.IndexOfNumberString(C, --CI) + Result;
                break;
            default:
                C = null;
                Result = Formate[i] + Result;
                break;
        }
    }
    return Result;
};
exports.FormateDate = FormateDate;
/**
 * NumberString Index
 * @param Str Source String
 * @param Index <0 : return 0;=length : return .;>length : return 0;
 */
var IndexOfNumberString = function (Str, Index) {
    return Index < 0 ? "0"
        : Index === Str.length ? "."
            : Index > Str.length ? "0"
                : Str[Index];
};
exports.IndexOfNumberString = IndexOfNumberString;
var RemoveBreaks = function (OldString) {
    return OldString.replace(/(\r\n|\n|\r)/gm, "");
};
exports.RemoveBreaks = RemoveBreaks;
var RemoveBreaksAndWriteSpaceBehind = function (OldString) {
    return OldString.replace(/(\r\n|\n|\r)\ */gm, "");
};
exports.RemoveBreaksAndWriteSpaceBehind = RemoveBreaksAndWriteSpaceBehind;
var Insert = function (Source, Index, Value) {
    Source.splice(Index, 0, Value);
};
exports.Insert = Insert;
var InsertChar = function (Source, Index, Value) {
    return Source.slice(0, Index) + Value + Source.slice(Index);
};
exports.InsertChar = InsertChar;
var Index = function () {
    if (!String.prototype.splice) {
        String.prototype.splice = exports.InsertChar;
    }
};
exports.Index = Index;
//# sourceMappingURL=Kits.js.map