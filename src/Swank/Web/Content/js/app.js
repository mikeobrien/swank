$(function () {

    var getHash = GetHash();

    var resourceTemplate = Handlebars.compile(
        $("#swank-resource-template").html());

    $(window).on('hashchange', route);

    initHandlebars()
    initSmoothScroll();

    route();

    function route(e) {
        var hash = getHash();
        switch (hash.route) {
            case 'modules': loadModule(hash.parameter); break;
            case 'resources': loadResource(hash.parameter); break;
            default: loadOverview(hash);
        }
        return true;
    }

    function loadModule(module) {
        loadMarkdown(window.Swank.modules[module]);
    }

    function loadResource(resource) {
        $.post('resources', JSON.stringify({ name: resource }),
            function (data) {
                $('#content').html(resourceTemplate(data));
                highlightCode('.swank-code-example code');
                initSampleDataClipboard('.swank-sample-data-copy');
                initClipboard('.swank-code-example-copy');
                initTabPreferences('SampleDataFormat', 'format',
                    '.swank-sample-data-selector');
                initTabPreferences('CodeExampleLanguage', 'language',
                    '.swank-code-example-selector');
            }, 'json');
    }

    function loadOverview(hash) {
        if (!hash.lastRoute || !isOverview(hash.lastRoute)) {
            loadMarkdown(window.Swank.overview);
        }
        if (hash.route) smoothScroll(hash.route);
    }

    function loadMarkdown(markdown) {
        $('#content').html('<div class="markdown-body">' + markdown + '</div>');
        highlightCode('pre code');
    }

    function isOverview(hash) {
        return !hash || !hashStartsWith(hash, 'resources', 'modules');
    }

    // ~~~~~~~~~~~~~~~~~ Handlebars ~~~~~~~~~~~~~~~~~

    function initHandlebars() {

        Handlebars.registerHelper('replace', function (source, find, replace) {
            var findRegex = eval(find.startsWith('/') ? find : '/' + find + '/g');
            return source.replace(findRegex, replace
                .replace(/&apos;/g, '\'').replace(/&amp;/g, '\'&')
                .replace(/&lt;/g, '<').replace(/&gt;/g, '>'));
        });

        Handlebars.registerHelper('equals',
            function (actual, expected, options) {
                return actual == expected ?
                    options.fn(this) :
                    options.inverse(this);
            });

        Handlebars.registerHelper('either', function () {
            var hasValue = function (value) { return value != null && value != false; }
            var options = arguments[arguments.length - 1];
            for (var index = 0; index < arguments.length - 1; index++) {
                if (hasValue(arguments[index])) {
                    return options.fn(this);
                }
            }
            return options.inverse(this);
        });

        function squash(template) {
            return template.split(/[\r\n]/).map(function (x) { return x.trim(); }).join('');
        }

        Handlebars.registerPartial("urlParameters", $("#swank-url-parameters-template").html());
        Handlebars.registerPartial("querystringParameters", $("#swank-querystring-template").html());
        Handlebars.registerPartial("headers", $("#swank-headers-template").html());
        Handlebars.registerPartial("jsonMessage", squash($("#swank-json-message-template").html()));
        Handlebars.registerPartial("xmlMessage", squash($("#swank-xml-message-template").html()));
        Handlebars.registerPartial("messageDescription", $("#swank-message-description-template").html());
        Handlebars.registerPartial("message", $("#swank-message-template").html());
        Handlebars.registerPartial("codeExamples", $("#swank-code-examples-template").html());
        Handlebars.registerPartial("statusCodes", $("#swank-status-codes-template").html());
        Handlebars.registerPartial("options", $("#swank-options-template").html());
    }

    // ~~~~~~~~~~~~~~~~~ Scrolling ~~~~~~~~~~~~~~~~~

    function initSmoothScroll() {
        $('li a[href^="#"]').click(function () {
            smoothScroll(this.hash);
        });
    }

    function smoothScroll(hash) {
        var overview = isOverview(hash);
        hash = qualifyHash(hash);
        if (overview && hash != '#' && $(hash).length > 0) {
            var position = $(hash).offset().top;
            $('html, body').animate({ scrollTop: position }, "slow");
        }
        else if (!overview) {
            $('html, body').animate({ scrollTop: 0 }, "slow");
        }
    }

    // ~~~~~~~~~~~~~~~~~ Tabs ~~~~~~~~~~~~~~~~~

    function initTabPreferences(cookie, data, selector) {
        var current = {};
        $(selector + ' a').click(function (e) {
            var key = $(e.target).data(data);
            if (key && key != current[data]) {
                current[data] = key;
                Cookies.set(cookie, key, 20*365);
                $(selector + ' li a[data-' + data + '="' + 
                    key + '"]').trigger('click');
            }
        });

        var key = Cookies.get(cookie);
        if (key) {
            $(selector + ' li:not(.hide) a[data-' + data + '="' + 
                key + '"]').trigger('click');
        }
    }

    // ~~~~~~~~~~~~~~~~~ Clipboard ~~~~~~~~~~~~~~~~~

    function initSampleDataClipboard(selector) {
        initClipboard(selector, {
            text: function(trigger) {
                var $trigger = $(trigger);
                var column = $trigger.closest('table')
                    .find('> thead ul li.active a')
                    .attr("href")
                    .replace('#', '');
                return $trigger.closest('table')
                    .children('tbody').children('tr')
                    .children('td:nth-child(' + column + ')')
                    .map(function() { return $(this).text(); })
                    .get().join('\n');
            }
        });
    }

    function initClipboard(selector, options) {
        $(selector).mouseleave(function(e) {      
            $(e.target).tooltip('destroy');
        });
        var clipboard = new Clipboard(selector, options);
        clipboard.on('success', function(e) {
            e.clearSelection();
            $(e.trigger).tooltip('show');
        });
    }

    // ~~~~~~~~~~~~~~~~~ Highlighting ~~~~~~~~~~~~~~~~~

    function highlightCode(selector) {
        $(selector).each(function (i, block) {
            hljs.highlightBlock(block);
        });
    }

    // ~~~~~~~~~~~~~~~~~ Hash ~~~~~~~~~~~~~~~~~

    function qualifyHash(hash) {
        return hash.charAt(0) != '#' ? '#' + hash : hash;
    }

    function hashStartsWith(hash) {
        hash = hash.replace('#', '');
        return Array.prototype.slice.call(arguments, 1)
            .filter(function (resource) {
                return hash.startsWith(resource + '/') || hash == resource;
            }).length > 0;
    }

    function GetHash() {
        var lastRoute;
        return function () {
            var hash = window.location.hash.slice(1);
            var route = hash.split('/', 1)[0];
            var hash = {
                route: route,
                parameter: hash.length > route.length + 1 ?
                    hash.slice(route.length + 1) : '',
                lastRoute: lastRoute
            };
            lastRoute = route;
            return hash;
        }
    };
});