const RemoveSpacesAndCRLF = (Source: string): string => {
  return Source.replace(/(\r\n|\n|\r)\ */gm, "");
}
const ReplaceMarks = (Template: string, Data: any): string => {
  for (const Key in Data) {
    const Regx = new RegExp("({{" + Key + "}})", "g");
    Template = Template.replace(Regx, Data[Key]);
  }
  return Template;
}
const ReplaceIndexes = (Template: string, Data: string[]): string => {
  for (let i = 0; i < Data.length; i++) {
    const Regx = new RegExp("([[" + i + "]])", "g");
    Template = Template.replace(Regx, Data[i]);
  }
  return Template;
}
const LoopMarks = (Template: string, Data: Array<any>): string => {
  let Result = "";
  const RegxI = new RegExp("({{\.index}})", "g");
  const RegxE = new RegExp("({{\.item}})", "g");
  for (let i = 0; i < Data.length; i++) {
    Result = Result + Template.replace(RegxI, i.toString()).replace(RegxE, Data[i]);
  }
  return Result;
}
const LoopMapMarks = (Template: string, Data: any): string => {
  let Result = "";
  for (let K in Data) {
    const RegxK = new RegExp("({{\.key}})", "g");
    const RegxV = new RegExp("({{\.value}})", "g");
    Result = Result + Template.replace(RegxK, K.toString()).replace(RegxV, Data[K].toString());
  }
  return Result;
};
