window.CandyGrid = {
    initialize: function () {
        // TODO: no initialization
    },

    onRendered: function (rootElement) {
        console.info("Rendered Grid: " + rootElement);
        /*
        if (document.activeElement !== document.body)
            document.activeElement.blur();
        */
    },

    bindColumnFilterDropdown: function (columnFilterElement) {
        var buttonEl = $(columnFilterElement).children("button").first();
        var dialogEl = $(columnFilterElement).find("div.candygrid-filter-dialog").first();
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

        buttonEl.data('isopen.bs.popover', false);

        buttonEl.on('show.bs.popover', function () {
            buttonEl.data('isopen.bs.popover', true);
        });

        buttonEl.on('hide.bs.popover', function () {
            buttonEl.data('isopen.bs.popover', false);
        });

        buttonEl.on('click', function (event) {
            // event.preventDefault();
            var popoverId = buttonEl.attr('aria-describedby');
            buttonEl.popover('toggle');
        });

        $(document).on('click', function () {
            // if (buttonEl.data('isopen.bs.popover') === true)
            //    buttonEl.popover('hide');
        });

        // clicking a filter button causes the filter dialog to collapse the dropdown
        /*
        filterEl.find($(".dropdown-menu form button")).click(function (e) {
            var parentTrigger = $(this).parents('.dropdown-menu').first().parent().children('button').first();
            parentTrigger.trigger('click');
        });
        */
        /*
        // Add animation to filter dialog
        filterEl.on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).fadeIn(100);
        });

        // Add animation to filter dialog
        filterEl.on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).fadeOut(100);
        });
        */
    }
};
