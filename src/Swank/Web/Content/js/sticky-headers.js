+function ($) {

    var currentHeader = null;
    var animating = false;

    $(document).scroll(function() {
        if (animating) return;
        animating = true;
        requestAnimationFrame(animateHeader);
        animating = false;
    });

    function animateHeader() {   
        if (currentHeader) {

            var stickyHeader = getHeaderDescriptor(currentHeader);
 
            if (stickyHeader.parent.top >= 0 ||
                stickyHeader.parent.bottom < 0)
            {
                stickyHeader.element.remove();
                currentHeader = null;
                // Animate once more in case another header 
                // is now active otherwise it will be skipped.
                animateHeader();
            } 
            else if (stickyHeader.bottom >= stickyHeader.height && 
                stickyHeader.element.css('position') == 'absolute')
            {
                stickyHeader.element.css({
                    top: 0, 
                    position: 'fixed',
                    'margin-top': '0'
                });              
            }
            else if (stickyHeader.bottom >= stickyHeader.parent.bottom)
            {
                stickyHeader.element.css({
                    position:'absolute',
                    top: $('body').scrollTop() -
                       (stickyHeader.height -
                        stickyHeader.parent.bottom)
                });            
            }

            return;
        } 

        var header = findHeader();

        if (!header || header.height == header.parent.height) return;

        if (header.parent.bottom <= header.height)
        {   
            var top = $('body').scrollTop() -
               (header.height - header.parent.bottom);
            currentHeader = createStickyHeader(header, top, 'absolute');       
        }
        else
        {
            currentHeader = createStickyHeader(header, 0, 'fixed');
        } 
    }

    function createStickyHeader(header, top, position) {
        var stickyHeader = header.element.clone();
        stickyHeader.hide();
        header.parent.element.append(stickyHeader);
        stickyHeader.css({
            top: top, 
            left: header.left,
            right: header.viewport.width - header.right,
            height: header.height,
            position: position,
            'margin-top': '0',
            'z-index': 999
        });
        header.element.trigger("sticky", [$(header.element), $(stickyHeader[0])]);
        stickyHeader.show();
        return stickyHeader;
    }

    function findHeader() {
        return $.makeArray($('div.sticky'))
            .map(getHeaderDescriptor)
            .find(function(x) { 
                return x.top <= 0 && x.parent.bottom > 0; 
            });
    }

    function getHeaderDescriptor(element) {
        var header = element instanceof jQuery ? element : $(element);
        element = element instanceof jQuery ? element[0] : element;
        var parent = header.parent();
        var coords = element.getBoundingClientRect();
        var parentCoords = element.parentElement.getBoundingClientRect();
        return {
            parent: {
                left: parentCoords.left,
                right: parentCoords.right,
                top: parentCoords.top,
                bottom: parentCoords.bottom,
                width: parent.width(),
                height: parent.height(),
                element: parent
            },
            viewport: {
                width: document.documentElement.clientWidth,
                height: document.documentElement.clientHeight
            },
            left: coords.left,
            right: coords.right,
            top: coords.top,
            bottom: coords.bottom,
            width: header.width(),
            height: header.height(),
            element: header
        };
    }

}(jQuery);
