function RadioBoxGroupChecking(e) {
  const Self = $(e.target);
  if (Self.data("other-previousvalue")) {
    Self.prop("checked", false);
  }
  else {
    Self.data("other-previousvalue", false);
  }
  Self.data("other-previousvalue", $(e.target).prop("checked"));
  const P = Self.closest(".other-checkbox-group");
  const Feature = P.data("other-checkboxgroup");
  const HasOnChange = Feature?.OnChange;
  const Checked = P.find("input.other-radiobox-group-one[type='radio']:checked");
  if (Checked.length > 0) Feature.SelectedValue = Checked.val();
  else Feature.SelectedValue = null;
  HasOnChange && HasOnChange(Feature);
}
/**
 *      * <div class="other-radiobox-group">
 *  ...
 *      ...
 *      <input type="radio" class="other-radiobox-group-one">
 *      <input type="radio" class="other-radiobox-group-one">
 *      <input type="radio" class="other-radiobox-group-one">
 *      ...
 *  ...
 * </div>
 * @param {any} Ele
 * @param {any} OnChange
 */
function OtherRadioboxGroup(Ele, OnChange) {
  this.OnChange = OnChange;
  this.Element = Ele;
  this.SelectedValue = null;
  if (this.Element.hasClass("other-radiobox-group")) {
    this.Element.find("input.other-radiobox-group-one[type='radio']").change(e => $(e.target).trigger("other-change"))
    this.Element.find("input.other-radiobox-group-one[type='radio']").off("other-change").on("other-change", RadioBoxGroupChecking);
  }
  else {
    console.error("Element has no declare to ref radiobox-group class")
  }
  this.CheckAll = (TrueOrFalse) => {
    this.Element.find(".other-radiobox-group-one").off("other-change");
    this.Element.find(".other-radiobox-group-one").on("other-change", RadioBoxGroupChecking);
  }
  this.Element.data("other-radioboxgroup", this);
}

$.fn.extend({
  OtherRadioBoxGroup: function (OnChange) {
    return new OtherRadioboxGroup(this, OnChange);
  }
});
