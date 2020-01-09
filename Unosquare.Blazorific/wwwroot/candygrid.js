window.CandyGrid = {
    initialize: function () {
        // clicking a filter button causes the filter dialog to collapse the dropdown
        $(".candygrid-column-header .dropdown .dropdown-menu form button").click(function (e) {
            var parentTrigger = $(this).parents('.dropdown-menu').first().parent().children('button').first();
            parentTrigger.trigger('click');
        });

        // Add animation to filter dialog
        $('.candygrid-column-header .dropdown').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).fadeIn(100);
        });

        // Add animation to filter dialog
        $('.candygrid-column-header .dropdown').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).fadeOut(100);
        });
    }
};
