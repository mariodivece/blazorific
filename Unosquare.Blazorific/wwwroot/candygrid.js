window.CandyGrid = {
    initialize: function () {
        // clicking a filter button causes the filter dialog to collapse the dropdown
        $(".candygrid-column-filter.dropdown .dropdown-menu form button").click(function (e) {
            var parentTrigger = $(this).parents('.dropdown-menu').first().parent().children('button').first();
            parentTrigger.trigger('click');
        });

        // Add animation to filter dialog
        $('.candygrid-column-filter.dropdown').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).fadeIn(100);
        });

        // Add animation to filter dialog
        $('.candygrid-column-filter.dropdown').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).fadeOut(100);
        });
    },

    onRendered: function (rootElement) {
        console.info("Rendered Grid: " + rootElement);
        if (document.activeElement !== document.body)
            document.activeElement.blur();
    }
};
