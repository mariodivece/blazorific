if (!window.CandyTabSet) {
    window.CandyTabSet = {
        bindEvents: function (el, dotNetInstance) {
            if (!el) return;
            "shown.bs.tab shown.bs.collapse".split(" ").forEach((eventName) => {
                el.addEventListener(eventName, event => {
                    dotNetInstance.invokeMethodAsync('JsHandleShownEvent').then();
                });
            });
        },

        show: function (el) {
            if (!el) return;

            const activator = el.classList.contains('nav-link')
                ? bootstrap.Tab.getOrCreateInstance(el)
                : bootstrap.Collapse.getOrCreateInstance(el);

            activator.show();
        },
    };
}
