if (!window.CandyModal) {
    window.CandyModal = {
        show: function (el) {
            $(el).modal('show');
        },

        hide: function (el) {
            $(el).modal('hide');
        }
    };
}
