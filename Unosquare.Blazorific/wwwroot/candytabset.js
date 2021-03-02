if (!window.CandyTabSet) {
    window.CandyTabSet = {
        show: function (el) {
            $(el).trigger('click');
        },
    };
}
