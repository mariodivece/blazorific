if (!window.CandyTabSet) {
    window.CandyTabSet = {
        show: function (el) {
            if (!el) return;

            const activator = el.classList.contains('nav-link')
                ? bootstrap.Tab.getOrCreateInstance(el)
                : bootstrap.Collapse.getOrCreateInstance(el);

            activator.show();
        },
    };
}
