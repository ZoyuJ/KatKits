/**
   * 
   * <div class="other-checkbox-group">
   *  ...
   *      <input type="checkbox" class="other-checkbox-group-all">
   *      ...
   *      <input type="checkbox" class="other-checkbox-group-one">
   *      <input type="checkbox" class="other-checkbox-group-one">
   *      <input type="checkbox" class="other-checkbox-group-one">
   *      ...
   *  ...
   * </div>
   * 
   * 
   * @param {any} e
   */
function CheckBoxGroupChecking(e) {
  const Self = $(e.target);
  const P = Self.closest(".other-checkbox-group");
  const Feature = P.data("other-checkboxgroup");
  const HasOnChange = Feature?.OnChange;
  if (Self.hasClass("other-checkbox-group-all")) {
    const NeedAll = Self.prop("checked");
    P.find("input.other-checkbox-group-one").off("other-change").prop("checked", NeedAll).on("other-change", CheckBoxGroupChecking);
    Feature.SelectedValues = [];
    if (Self.prop("checked")) {
      P.find("input.other-checkbox-group-one").each((I, E) => Feature.SelectedValues.push($(E).val()));
    }
  }
  else {
    const HasOne = P.find("input.other-checkbox-group-one:not([checked])").length > 0;
    P.find(".other-checkbox-group-all").off("other-change").prop("checked", !HasOne).on("other-change", CheckBoxGroupChecking);
    if (Self.prop("checked")) { Feature.SelectedValues.push(Self.val()); }
    else {
      const I = Feature.SelectedValues.findIndex(E => E == Self.val());
      if (I != -1) {
        Feature.SelectedValues.splice(I, 1);
      }
    }

  }
  HasOnChange && HasOnChange(Feature);
}

function OtherCheckboxGroup(Ele, OnChange) {
  this.OnChange = OnChange;
  this.Element = Ele;
  this.SelectedValues = [];
  if (this.Element.hasClass("other-checkbox-group")) {
    this.Element.find("input.other-checkbox-group-all[type='checkbox'],input.other-checkbox-group-one[type='checkbox']").change(e => $(e.target).trigger("other-change"))
    this.Element.find("input.other-checkbox-group-all[type='checkbox'],input.other-checkbox-group-one[type='checkbox']").off("other-change").on("other-change", CheckBoxGroupChecking);
  }
  else {
    console.error("Element has no declare to ref checkbox-group class")
  }
  this.CheckAll = (TrueOrFalse) => {
    this.Element.find(".other-checkbox-group-one").off("other-change");
    this.Element.find(".other-checkbox-group-all").prop("change", TrueOrFalse).change();
    this.Element.find(".other-checkbox-group-one").on("other-change", CheckBoxGroupChecking);
  }
  this.Element.data("other-checkboxgroup", this);
}


$.fn.extend({
  OtherCheckBoxGroup: function (OnChange) {
    return new OtherCheckboxGroup(this, OnChange);
  }
});