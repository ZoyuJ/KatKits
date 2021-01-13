"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.RemoveBreaksAndWriteSpaceBehind = exports.RemoveBreaks = exports.IndexOfNumberString = exports.FormateDate = exports.UTCToLocalDateTime = void 0;
var UTCToLocalDateTime = function (UTCDate) {
    return new Date(UTCDate.toLocaleString());
};
exports.UTCToLocalDateTime = UTCToLocalDateTime;
/**
 * formate datetime toString like C#(but not look like).
 * @param CurrentDate JS Date
 * @param Formate y��,M��,d��,H:ʱ(24),h:ʱ(12),m:��,s:��,z:ʱ��(GMT��NN:00),����Ӧ���ִӺ���ǰ�滻��Ӧ��ĸ���������ĸ��0���룬�����ַ�ԭ�����,ʱ����֧��һ����ĸz,ʹ��ʱ�������滻��ĸz��12Сʱֵ����h���ֺ�����' AM'�� PM��;
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
//# sourceMappingURL=Kits.js.map