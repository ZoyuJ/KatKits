
//call callback on popover displayed

var showPopover = $.fn.popover.Constructor.prototype.show;
$.fn.popover.Constructor.prototype.show = function () {
  showPopover.call(this);
  if (this.options.showCallback) {
    this.options.showCallback.call(this);
  }
}
// end

$(function () {
  OtherUtil = {
    DateTextIgnoreTimezone: function (Date) {
      return {
        YYYY: Date.getFullYear(),
        MM: Date.getMonth() + 1,
        DD: Date.getDate(),
        HH: Date.getHours(),
        mm: Date.getMinutes(),
        ss: Date.getSeconds(),
      }
    },
    DateTimeTickToDate: function (DateTickStr) {
      const Ms = /^\/Date\((-?\d+)\)\/$/g.exec(DateTickStr);
      if (Ms && Ms.length > 1) {
        return new Date(Ms[1] * 1).toLocaleDateString();
      }
      return null;
    },
    DateTimeTickToDateInputValue: function (DateTickStr) {
      const Ms = /^\/Date\((-?\d+)\)\/$/g.exec(DateTickStr);
      if (Ms && Ms.length > 1) {
        const D = new Date(Ms[1] * 1);
        const day = ("0" + D.getDate()).slice(-2);
        const month = ("0" + (D.getMonth() + 1)).slice(-2);
        const val = D.getFullYear() + "-" + (month) + "-" + (day);
        return val;
      }
      return null;
    },
    NearestSunday: function (SelectedDate) {
      const D = SelectedDate.getDay();
      if (D == 0) return SelectedDate;
      return new Date(SelectedDate.setDate(SelectedDate.getDate() - D));
    },
    NearestSaturday: function (SelectedDate) {
      const D = SelectedDate.getDay();
      if (D == 6) return SelectedDate;
      return new Date(SelectedDate.setDate(SelectedDate.getDate() + (6 - D)));
    },
    /**
     * 1 -> 1st , 2 -> 2nd , 3 -> 3rd , 4 -> 4th
     * @param {any} d
     */
    OridinalDayOfMonth: function (d) {
      if (d > 3 && d < 21) return d.toString() + 'th';
      switch (d % 10) {
        case 1: return d.toString() + "st";
        case 2: return d.toString() + "nd";
        case 3: return d.toString() + "rd";
        default: return d.toString() + "th";
      }
    },
    MonthTo3Chars: function (m) {
      return ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][m].substring(0, 3);
    }
  }
  window.OtherUtil = { ...window.OtherUtil, ...OtherUtil };
});