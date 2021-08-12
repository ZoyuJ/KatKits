namespace KatKits {
  export const RemoveSpacesAndCRLF = (Source: string): string => {
    return Source.replace(/(\r\n|\n|\r)\ */gm, "");
  }
  const _ReplaceMarks = (Template: string, Regs: Map<string, RegExp>, Data: any) => {
    Regs.forEach((R, K) => {
      Template = Template.replace(R, Data[K]);
    });
    return Template;
  }
  export const ReplaceMarks = (Template: string, Data: any): string => {
    const Regs = new Map<string, RegExp>();
    for (const Key in Data) { Regs.set(Key, new RegExp("({{" + Key + "}})", "g")); }
    return _ReplaceMarks(Template, Regs, Data);
  }
  export const ReplaceIndexes = (Template: string, Data: string[]): string => {
    for (let i = 0; i < Data.length; i++) {
      const Regx = new RegExp("([[" + i + "]])", "g");
      Template = Template.replace(Regx, Data[i]);
    }
    return Template;
  }
  export const LoopMarks = (Template: string, Data: Array<any>): string => {
    let Result = "";
    const RegxI = new RegExp("({{\.index}})", "g");
    const RegxE = new RegExp("({{\.item}})", "g");
    for (let i = 0; i < Data.length; i++) {
      Result = Result + Template.replace(RegxI, i.toString()).replace(RegxE, Data[i]);
    }
    return Result;
  }
  export const LoopMapMarks = (Template: string, Data: any): string => {
    let Result = "";
    for (let K in Data) {
      const RegxK = new RegExp("({{\.key}})", "g");
      /*    const RegxV = new RegExp("({{\.value}})", "g");*/
      Result = Result + Template.replace(RegxK, K.toString())/*.replace(RegxV, Data[K].toString())*/;
    }
    return Result;
  };
}