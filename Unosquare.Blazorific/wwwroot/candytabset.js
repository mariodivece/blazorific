if (!window.CandyTabSet) {
    window.CandyTabSet = {
        show: function (el) {
            bootstrap.Tab.getOrCreateInstance(el).show();
        },
    };
}
