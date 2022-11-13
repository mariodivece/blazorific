if (!window.CandyModal) {
    window.CandyModal = {
        show: function (el) {
            bootstrap.Modal.getOrCreateInstance(el).show();
        },

        hide: function (el) {
            bootstrap.Modal.getOrCreateInstance(el).hide();
        },

        toggle: function (el) {
            bootstrap.Modal.getOrCreateInstance(el).toggle();
        },
    };
}
