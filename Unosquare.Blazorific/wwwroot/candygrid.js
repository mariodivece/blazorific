window.CandyGrid = {
    constants: {
        filterOpenAttribute: 'data-cg-filter-isopen',
        renderedEventName: 'candygrid.rendred',
        dataLoadedEventName: 'candygrid.data.loaded',
    },

    onRendered: function (rootElement, firstRender) {
        CandyGrid.hideColumnFilterDropdowns(rootElement);
        var event = new CustomEvent(CandyGrid.constants.renderedEventName, { firstRender: firstRender });
        rootElement.dispatchEvent(event);
    },

    onDataLoaded: function (rootElement) {
        CandyGrid.hideColumnFilterDropdowns(rootElement);
        var event = new Event(CandyGrid.constants.dataLoadedEventName);
        rootElement.dispatchEvent(event);
    },

    bindColumnFilterDropdown: function (columnFilterElement) {
        var buttonEl = $(columnFilterElement).children("button").first();
        var dialogEl = $(columnFilterElement).find("div.candygrid-filter-dialog").first();
        var gridEl = $(columnFilterElement).parents(".candygrid-container").first();

        buttonEl.popover({
            content: dialogEl,
            html: true,
            placement: 'bottom',
            trigger: 'manual',
            template:
                '<div class="popover" role="tooltip">' +
                '  <div class="arrow"></div>' +
                '  <h3 class="popover-header"></h3>' +
                '  <div class="popover-body"></div>' +
                '</div>'
        });

        dialogEl.find("button").on('click', function (e) {
            buttonEl.popover('hide');
        });

        var isOpenAttr = CandyGrid.constants.filterOpenAttribute;
        buttonEl.attr(isOpenAttr, 'false');

        buttonEl.on('show.bs.popover', function () {
            buttonEl.attr(isOpenAttr, 'true');
        });

        buttonEl.on('hide.bs.popover', function () {
            buttonEl.attr(isOpenAttr, 'false');
        });

        buttonEl.on('click', function (event) {
            var isOpen = buttonEl.attr(isOpenAttr) === 'true';
            CandyGrid.hideColumnFilterDropdowns(gridEl);
            buttonEl.popover(isOpen ? 'hide' : 'show');
        });
    },

    hideColumnFilterDropdowns: function (rootElement) {
        $(rootElement).find("[" + CandyGrid.constants.filterOpenAttribute + "]").popover('hide');
    }
};
