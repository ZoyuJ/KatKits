"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.AsyncEvent8 = exports.AsyncEvent7 = exports.AsyncEvent6 = exports.AsyncEvent5 = exports.AsyncEvent4 = exports.AsyncEvent3 = exports.AsyncEvent2 = exports.AsyncEvent1 = exports.AsyncEvent0 = exports.Event8 = exports.Event7 = exports.Event6 = exports.Event5 = exports.Event4 = exports.Event3 = exports.Event2 = exports.Event1 = exports.Event0 = void 0;
var Event0 = /** @class */ (function (_super) {
    __extends(Event0, _super);
    function Event0() {
        return _super.call(this) || this;
    }
    Event0.prototype.Invoke = function () {
        if (this.length > 0) {
            this.forEach(function (E) { return E(); });
        }
    };
    Event0.prototype.Add = function (Action) {
        this.push(Action);
    };
    Event0.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return Event0;
}(Array));
exports.Event0 = Event0;
var Event1 = /** @class */ (function (_super) {
    __extends(Event1, _super);
    function Event1() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Event1.prototype.Invoke = function (Arg) {
        if (this.length > 0) {
            this.forEach(function (E) { return E(Arg); });
        }
    };
    Event1.prototype.Add = function (Action) {
        this.push(Action);
    };
    Event1.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return Event1;
}(Array));
exports.Event1 = Event1;
var Event2 = /** @class */ (function (_super) {
    __extends(Event2, _super);
    function Event2() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Event2.prototype.Invoke = function (Arg1, Arg2) {
        if (this.length > 0) {
            this.forEach(function (E) { return E(Arg1, Arg2); });
        }
    };
    Event2.prototype.Add = function (Action) {
        this.push(Action);
    };
    Event2.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return Event2;
}(Array));
exports.Event2 = Event2;
var Event3 = /** @class */ (function (_super) {
    __extends(Event3, _super);
    function Event3() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Event3.prototype.Invoke = function (Arg1, Arg2, Arg3) {
        if (this.length > 0) {
            this.forEach(function (E) { return E(Arg1, Arg2, Arg3); });
        }
    };
    Event3.prototype.Add = function (Action) {
        this.push(Action);
    };
    Event3.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return Event3;
}(Array));
exports.Event3 = Event3;
var Event4 = /** @class */ (function (_super) {
    __extends(Event4, _super);
    function Event4() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Event4.prototype.Invoke = function (Arg1, Arg2, Arg3, Arg4) {
        if (this.length > 0) {
            this.forEach(function (E) { return E(Arg1, Arg2, Arg3, Arg4); });
        }
    };
    Event4.prototype.Add = function (Action) {
        this.push(Action);
    };
    Event4.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return Event4;
}(Array));
exports.Event4 = Event4;
var Event5 = /** @class */ (function (_super) {
    __extends(Event5, _super);
    function Event5() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Event5.prototype.Invoke = function (Arg1, Arg2, Arg3, Arg4, Arg5) {
        if (this.length > 0) {
            this.forEach(function (E) { return E(Arg1, Arg2, Arg3, Arg4, Arg5); });
        }
    };
    Event5.prototype.Add = function (Action) {
        this.push(Action);
    };
    Event5.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return Event5;
}(Array));
exports.Event5 = Event5;
var Event6 = /** @class */ (function (_super) {
    __extends(Event6, _super);
    function Event6() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Event6.prototype.Invoke = function (Arg1, Arg2, Arg3, Arg4, Arg5, Arg6) {
        if (this.length > 0) {
            this.forEach(function (E) { return E(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6); });
        }
    };
    Event6.prototype.Add = function (Action) {
        this.push(Action);
    };
    Event6.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return Event6;
}(Array));
exports.Event6 = Event6;
var Event7 = /** @class */ (function (_super) {
    __extends(Event7, _super);
    function Event7() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Event7.prototype.Invoke = function (Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7) {
        if (this.length > 0) {
            this.forEach(function (E) { return E(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7); });
        }
    };
    Event7.prototype.Add = function (Action) {
        this.push(Action);
    };
    Event7.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return Event7;
}(Array));
exports.Event7 = Event7;
var Event8 = /** @class */ (function (_super) {
    __extends(Event8, _super);
    function Event8() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Event8.prototype.Invoke = function (Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8) {
        if (this.length > 0) {
            this.forEach(function (E) { return E(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8); });
        }
    };
    Event8.prototype.Add = function (Action) {
        this.push(Action);
    };
    Event8.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return Event8;
}(Array));
exports.Event8 = Event8;
var AsyncEvent0 = /** @class */ (function (_super) {
    __extends(AsyncEvent0, _super);
    function AsyncEvent0() {
        return _super.call(this) || this;
    }
    AsyncEvent0.prototype.Invoke = function () {
        return __awaiter(this, void 0, void 0, function () {
            var _i, _a, Delegate;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.length <= 0)
                            return [2 /*return*/];
                        _i = 0, _a = this;
                        _b.label = 1;
                    case 1:
                        if (!(_i < _a.length)) return [3 /*break*/, 4];
                        Delegate = _a[_i];
                        return [4 /*yield*/, Delegate()];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        _i++;
                        return [3 /*break*/, 1];
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    AsyncEvent0.prototype.Add = function (Action) {
        this.push(Action);
    };
    AsyncEvent0.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return AsyncEvent0;
}(Array));
exports.AsyncEvent0 = AsyncEvent0;
var AsyncEvent1 = /** @class */ (function (_super) {
    __extends(AsyncEvent1, _super);
    function AsyncEvent1() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AsyncEvent1.prototype.Invoke = function (Arg) {
        return __awaiter(this, void 0, void 0, function () {
            var _i, _a, Delegate;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.length <= 0)
                            return [2 /*return*/];
                        _i = 0, _a = this;
                        _b.label = 1;
                    case 1:
                        if (!(_i < _a.length)) return [3 /*break*/, 4];
                        Delegate = _a[_i];
                        return [4 /*yield*/, Delegate(Arg)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        _i++;
                        return [3 /*break*/, 1];
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    AsyncEvent1.prototype.Add = function (Action) {
        this.push(Action);
    };
    AsyncEvent1.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return AsyncEvent1;
}(Array));
exports.AsyncEvent1 = AsyncEvent1;
var AsyncEvent2 = /** @class */ (function (_super) {
    __extends(AsyncEvent2, _super);
    function AsyncEvent2() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AsyncEvent2.prototype.Invoke = function (Arg1, Arg2) {
        return __awaiter(this, void 0, void 0, function () {
            var _i, _a, Delegate;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.length <= 0)
                            return [2 /*return*/];
                        _i = 0, _a = this;
                        _b.label = 1;
                    case 1:
                        if (!(_i < _a.length)) return [3 /*break*/, 4];
                        Delegate = _a[_i];
                        return [4 /*yield*/, Delegate(Arg1, Arg2)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        _i++;
                        return [3 /*break*/, 1];
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    AsyncEvent2.prototype.Add = function (Action) {
        this.push(Action);
    };
    AsyncEvent2.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return AsyncEvent2;
}(Array));
exports.AsyncEvent2 = AsyncEvent2;
var AsyncEvent3 = /** @class */ (function (_super) {
    __extends(AsyncEvent3, _super);
    function AsyncEvent3() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AsyncEvent3.prototype.Invoke = function (Arg1, Arg2, Arg3) {
        return __awaiter(this, void 0, void 0, function () {
            var _i, _a, Delegate;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.length <= 0)
                            return [2 /*return*/];
                        _i = 0, _a = this;
                        _b.label = 1;
                    case 1:
                        if (!(_i < _a.length)) return [3 /*break*/, 4];
                        Delegate = _a[_i];
                        return [4 /*yield*/, Delegate(Arg1, Arg2, Arg3)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        _i++;
                        return [3 /*break*/, 1];
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    AsyncEvent3.prototype.Add = function (Action) {
        this.push(Action);
    };
    AsyncEvent3.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return AsyncEvent3;
}(Array));
exports.AsyncEvent3 = AsyncEvent3;
var AsyncEvent4 = /** @class */ (function (_super) {
    __extends(AsyncEvent4, _super);
    function AsyncEvent4() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AsyncEvent4.prototype.Invoke = function (Arg1, Arg2, Arg3, Arg4) {
        return __awaiter(this, void 0, void 0, function () {
            var _i, _a, Delegate;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.length <= 0)
                            return [2 /*return*/];
                        _i = 0, _a = this;
                        _b.label = 1;
                    case 1:
                        if (!(_i < _a.length)) return [3 /*break*/, 4];
                        Delegate = _a[_i];
                        return [4 /*yield*/, Delegate(Arg1, Arg2, Arg3, Arg4)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        _i++;
                        return [3 /*break*/, 1];
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    AsyncEvent4.prototype.Add = function (Action) {
        this.push(Action);
    };
    AsyncEvent4.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return AsyncEvent4;
}(Array));
exports.AsyncEvent4 = AsyncEvent4;
var AsyncEvent5 = /** @class */ (function (_super) {
    __extends(AsyncEvent5, _super);
    function AsyncEvent5() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AsyncEvent5.prototype.Invoke = function (Arg1, Arg2, Arg3, Arg4, Arg5) {
        return __awaiter(this, void 0, void 0, function () {
            var _i, _a, Delegate;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.length <= 0)
                            return [2 /*return*/];
                        _i = 0, _a = this;
                        _b.label = 1;
                    case 1:
                        if (!(_i < _a.length)) return [3 /*break*/, 4];
                        Delegate = _a[_i];
                        return [4 /*yield*/, Delegate(Arg1, Arg2, Arg3, Arg4, Arg5)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        _i++;
                        return [3 /*break*/, 1];
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    AsyncEvent5.prototype.Add = function (Action) {
        this.push(Action);
    };
    AsyncEvent5.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return AsyncEvent5;
}(Array));
exports.AsyncEvent5 = AsyncEvent5;
var AsyncEvent6 = /** @class */ (function (_super) {
    __extends(AsyncEvent6, _super);
    function AsyncEvent6() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AsyncEvent6.prototype.Invoke = function (Arg1, Arg2, Arg3, Arg4, Arg5, Arg6) {
        return __awaiter(this, void 0, void 0, function () {
            var _i, _a, Delegate;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.length <= 0)
                            return [2 /*return*/];
                        _i = 0, _a = this;
                        _b.label = 1;
                    case 1:
                        if (!(_i < _a.length)) return [3 /*break*/, 4];
                        Delegate = _a[_i];
                        return [4 /*yield*/, Delegate(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        _i++;
                        return [3 /*break*/, 1];
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    AsyncEvent6.prototype.Add = function (Action) {
        this.push(Action);
    };
    AsyncEvent6.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return AsyncEvent6;
}(Array));
exports.AsyncEvent6 = AsyncEvent6;
var AsyncEvent7 = /** @class */ (function (_super) {
    __extends(AsyncEvent7, _super);
    function AsyncEvent7() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AsyncEvent7.prototype.Invoke = function (Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7) {
        return __awaiter(this, void 0, void 0, function () {
            var _i, _a, Delegate;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.length <= 0)
                            return [2 /*return*/];
                        _i = 0, _a = this;
                        _b.label = 1;
                    case 1:
                        if (!(_i < _a.length)) return [3 /*break*/, 4];
                        Delegate = _a[_i];
                        return [4 /*yield*/, Delegate(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        _i++;
                        return [3 /*break*/, 1];
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    AsyncEvent7.prototype.Add = function (Action) {
        this.push(Action);
    };
    AsyncEvent7.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return AsyncEvent7;
}(Array));
exports.AsyncEvent7 = AsyncEvent7;
var AsyncEvent8 = /** @class */ (function (_super) {
    __extends(AsyncEvent8, _super);
    function AsyncEvent8() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AsyncEvent8.prototype.Invoke = function (Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8) {
        return __awaiter(this, void 0, void 0, function () {
            var _i, _a, Delegate;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (this.length <= 0)
                            return [2 /*return*/];
                        _i = 0, _a = this;
                        _b.label = 1;
                    case 1:
                        if (!(_i < _a.length)) return [3 /*break*/, 4];
                        Delegate = _a[_i];
                        return [4 /*yield*/, Delegate(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        _i++;
                        return [3 /*break*/, 1];
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    AsyncEvent8.prototype.Add = function (Action) {
        this.push(Action);
    };
    AsyncEvent8.prototype.Remove = function (Action) {
        this.splice(this.findIndex(function (E) { return E === Action; }), 1);
    };
    return AsyncEvent8;
}(Array));
exports.AsyncEvent8 = AsyncEvent8;
//# sourceMappingURL=Event.js.map