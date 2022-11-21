if (!window.CandyModal) {
    window.CandyModal = {
        bindEvents: function (el, dotNetInstance) {
            if (!el) return;
            "shown.bs.modal".split(" ").forEach((eventName) => {
                el.addEventListener(eventName, event => {
                    dotNetInstance.invokeMethodAsync('JsHandleShownEvent').then();
                });
            });

            "hidden.bs.modal".split(" ").forEach((eventName) => {
                el.addEventListener(eventName, event => {
                    dotNetInstance.invokeMethodAsync('JsHandleHiddenEvent').then();
                });
            });
        },

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
