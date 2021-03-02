if (!window.CandyGrid) {
    window.CandyGrid = {
        constants: {
            renderedEventName: 'candygrid.rendred',
            dataLoadedEventName: 'candygrid.data.loaded',
            dataLoadingEventName: 'candygrid.data.loading',
        },

        state: {
            hasBoundWindowEvents: false,
            filterButtons: new Array(),
        },

        onRendered: function (rootElement, firstRender) {
            CandyGrid.hideDropdowns();
            var event = new CustomEvent(CandyGrid.constants.renderedEventName, { firstRender: firstRender });
            rootElement.dispatchEvent(event);
        },

        onDataLoading: function (rootElement) {
            var overlay = $(rootElement).find('.candygrid-overlay').first();
            if (overlay.css('display') === 'block')
                return;

            overlay.css('display', 'block');
            var event = new CustomEvent(CandyGrid.constants.dataLoadingEventName);
            rootElement.dispatchEvent(event);
        },

        onDataLoaded: function (rootElement) {
            CandyGrid.hideDropdowns();
            $(rootElement).find('.candygrid-overlay').first().css('display', 'none');
            var event = new Event(CandyGrid.constants.dataLoadedEventName);
            rootElement.dispatchEvent(event);
        },

        hideDropdowns: function () {
            $(window).trigger('touchstart');
        },

        bindWindowEvents: function () {

            if (CandyGrid.state.hasBoundWindowEvents !== false)
                return;

            CandyGrid.state.hasBoundWindowEvents = true;
            $(window).on("resize keydown mousedown touchstart", function (e) {
                var isOnPopup = e && e.target && e.target.closest && e.target.closest(".popover");
                var isEscapKey = e && e.which && e.which === 27;
                var isWindowEvent = !e || !e.target || e.target === window;
                var stopPropagation = false;
                var i;
                var buttonEl;

                // hide all popovers
                for (i = 0; i < CandyGrid.state.filterButtons.length; i++) {
                    buttonEl = CandyGrid.state.filterButtons[i];
                    var isOnButton = e && e.target && (e.target === buttonEl[0] || buttonEl.has(e.target).length > 0);

                    if (!isOnButton && (!isOnPopup || isEscapKey || isWindowEvent)) {
                        buttonEl.popover('hide');
                        stopPropagation = true;
                    }
                }

                // remove stale popovers
                for (i = CandyGrid.state.filterButtons.length - 1; i >= 0; i--) {
                    buttonEl = CandyGrid.state.filterButtons[i];
                    if (!CandyGrid.isAttachedToDocument(buttonEl[0]))
                        CandyGrid.state.filterButtons.pop();
                }

                if (stopPropagation === true)
                    e.stopPropagation();
            });
        },

        isAttachedToDocument: function (element) {
            while (element !== document && element.parentNode) {
                element = element.parentNode;
            }

            return element === document;
        },

        bindColumnFilterDropdown: function (columnFilterElement) {
            var filterEl = $(columnFilterElement);
            var buttonEl = filterEl.children("button").first();
            var dialogEl = filterEl.find("div.candygrid-filter-dialog").first();

            if (buttonEl.length <= 0)
                return;

            CandyGrid.state.filterButtons.push(buttonEl);
            CandyGrid.bindWindowEvents();

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
        },
    };
}
