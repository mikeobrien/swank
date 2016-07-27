+function ($) {

    function getSelector(element) {
        var $element = $(element);
        var selector = $element.data('target');

        if (selector) selector = selector.replace('#', '');
        else
        {
          selector = $element.attr('href');
          selector = selector && selector
            .replace(/.*#(?=[^\s]*$)/, ''); // strip for ie7
        }

        if (!selector) return;

        var parts = selector.split('.');

        if (parts.length > 2) return;

        var column = parts.pop();

        if (!$.isNumeric(column)) return;

        var table = parts.length == 1
            ? $('#' + parts[0])
            : $element.closest('table');

        return table.children('tr')
            .add(table.children('tbody').children('tr'))
            .children('td:nth-child(' + column + ')');
    }

    var clickHandler = function (e) {
        e.preventDefault();

        var $this = $(this);

        var thisSelector = getSelector($this);
        if (thisSelector) {
            thisSelector.removeClass('hide').show();

            $.makeArray($this
                .closest('ul:not(.dropdown-menu)')
                .find('li:not(.active) a'))
                .map(getSelector)
                .forEach(function(x) { if (x) x.hide(); });
        }
    }

    $(document).on('click.bs.tab.data-api', 
        '[data-toggle="tab"]', clickHandler);

}(jQuery);