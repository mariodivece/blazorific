window.CandyGrid = {
    constants: {
        renderedEventName: 'candygrid.rendred',
        dataLoadedEventName: 'candygrid.data.loaded',
    },

    onRendered: function (rootElement, firstRender) {
        CandyGrid.hideDropdowns();
        var event = new CustomEvent(CandyGrid.constants.renderedEventName, { firstRender: firstRender });
        rootElement.dispatchEvent(event);
    },

    onDataLoaded: function (rootElement) {
        CandyGrid.hideDropdowns();
        var event = new Event(CandyGrid.constants.dataLoadedEventName);
        rootElement.dispatchEvent(event);
    },

    hideDropdowns: function () {
        $(window).trigger('touchstart');
    },

    bindColumnFilterDropdown: function (columnFilterElement) {
        var filterEl = $(columnFilterElement);
        var buttonEl = filterEl.children("button").first();
        var dialogEl = filterEl.find("div.candygrid-filter-dialog").first();

        buttonEl.popover({
            content: dialogEl,
            html: true,
            placement: 'bottom',
            trigger: 'manual',
            boundary: 'viewport',
            container: 'body',
            offset: 20000,
            template:
                '<div class="popover" role="tooltip">' +
                '  <div class="arrow"></div>' +
                '  <h3 class="popover-header"></h3>' +
                '  <div class="popover-body"></div>' +
                '</div>'
        });

        dialogEl.find("button.candygrid-column-filter-apply").on('click', function (e) {
            // buttonEl.popover('hide');
        });

        buttonEl.on('click', function (e) {
            buttonEl.popover('toggle');
            e.stopPropagation();
        });

        $(window).on("resize keydown mousedown touchstart", function (e) {
            var isOnPopup = e && e.target && e.target.closest && e.target.closest(".popover");
            var isEscapKey = e && e.which && e.which === 27;
            var isWindowEvent = !e || !e.target || e.target === window;
            var isOnButton = e && e.target && (e.target === buttonEl || $(buttonEl).has(e.target).length > 0);

            if (!isOnButton && (!isOnPopup || isEscapKey || isWindowEvent)) {
                buttonEl.popover('hide');
                e.stopPropagation();
            }
        });
    },
};
