$(function () {

    // ~~~~~~~~~~~~~~~~~ Routing ~~~~~~~~~~~~~~~~~

    function Router(routes) {
        var lastRoute;
        var hasLastRoute;

        $(window).on('hashchange', function() { 
            route(routes); 
            return true;
        });

        route(routes);

        function route(routes) {
            var routeContext = currentRoute();
            var route = routes[routeContext.route];
            if (route) route(routeContext);
            else routes.default(routeContext);
        }

        function currentRoute() 
        {
            var hash = window.location.hash.slice(1);
            var route = hash.split('/', 1)[0];
            var parameter = hash.length > route.length + 1 ?
                hash.slice(route.length + 1) : '';
            var routeContext = {
                route: route,
                lastRoute: lastRoute,
                hasLastRoute: hasLastRoute,
                parameter: parameter
            };
            lastRoute = route;
            hasLastRoute = true;
            return routeContext;
        }

        return {
            currentRoute: currentRoute
        }
    };

    // ~~~~~~~~~~~~~~~~~ Binding ~~~~~~~~~~~~~~~~~

    function Binding() 
    {
        var session = { }
        var enabledCheckboxes = ' input[type="checkbox"]:not(:disabled)';
        var cookieAge = 20*365;

        function bindSessionValues(selector, scopeSelector) {
            bindSession(selector, scopeSelector, 'input:text', 'value');
        }

        function bindSessionPasswords(selector, scopeSelector) {
            bindSession(selector, scopeSelector, 'input:password', 'value');
        }

        function bindSessionText(selector, scopeSelector) {
            bindSession(selector, scopeSelector, 'textarea', 'value');
        }

        function bindSessionCheckboxes(selector, scopeSelector) {
            bindSession(selector, scopeSelector, enabledCheckboxes, 'checked', '-Enabled');
        }

        function bindSession(selector, scopeSelector, element, property, postfix) {
            postfix = postfix || '';
            bindValues({
                property: property,
                scopeSelector: scopeSelector,
                selector: selector + ' ' + element,
                loadValue: function(name) { return session[name + postfix]; },
                saveValue: function(name, value) { session[name + postfix] = value; }
            });
        }

        function bindCookieTabs(selector, cookie, attr) {
            bindOptions({
                selector: selector,
                key: function(el) { return el.attr('data-' + attr); },
                loadKey: function() { return Cookies.get(cookie); },
                saveKey: function(key) { Cookies.set(cookie, key, cookieAge); },
                initSelector: function(key) { return selector + 
                    ' li:not(.hide) a[data-' + attr + '="' + key + '"]'; },
                updateSelector: function(key) { return selector + 
                    ' li a[data-' + attr + '="' + key + '"]'; }
            });
        }

        function bindCookieOptions(selector, cookie) {
            selector = selector + ' input:radio';
            bindOptions({
                selector: selector,
                key: function(el) { return el.attr('value'); },
                loadKey: function() { return Cookies.get(cookie) || 'None'; },
                saveKey: function(key) { Cookies.set(cookie, key, cookieAge); },
                updateSelector: function(key) { return selector + '[value="' + key + '"]'; }
            });        
        }

        function bindCookieValues(selector, cookie) {
            bindValues({
                property: 'value',
                selector: selector + ' input:text',
                loadValue: function(name) { 
                    return Cookies.get(cookie + name); },
                saveValue: function(name, value) { 
                    Cookies.set(cookie + name, value, cookieAge); }
            });
        }

        function bindCookieCheckboxes(selector, cookie) {
            bindValues({
                property: 'checked',
                selector: selector + enabledCheckboxes,
                loadValue: function(name) { return Cookies.get(
                    cookie + name + '-Enabled') == 'true'; },
                saveValue: function(name, value) { Cookies.set(
                    cookie + name + '-Enabled', value, cookieAge); }
            });
        }

        function bindOptions(options) {
            var currentKey;
            var initSelector = options.initSelector || options.updateSelector;
            $(options.selector).click(function (e) {
                var key = options.key($(e.target));
                if (key && key != currentKey) {
                    options.saveKey(currentKey = key);
                    $(options.updateSelector(key)).trigger('click');
                }
            });

            var key = options.loadKey();
            if (key) {
                $(initSelector(key)).trigger('click');
            }
        }

        function bindValues(options) {
            var currentValue = {};
            var getScopeId = function(target) {
                var id = !options.scopeSelector ? '' : target
                    .closest(options.scopeSelector).attr('id');
                return {
                    id: id,
                    prefix: id ? id + '-' : ''
                };
            };
            var getScope = function(target) {
                var scope = getScopeId(target);
                return { 
                    prefix: scope.prefix, 
                    selector: scope.id ? options.scopeSelector + 
                        '[id="' + scope.id + '"] ' : '' 
                };
            };
            $(options.selector).on('change', function (e) {
                var target = $(e.target);
                var name = target.attr('name');
                var value = target.prop(options.property);
                var scope = getScope(target);
                var scopedName = scope.prefix ? scope.prefix + name : name;
                if (value != currentValue[scopedName]) {
                    options.saveValue(scopedName, 
                        currentValue[scopedName] = value);
                    $(scope.selector + options.selector + 
                            '[name="' + name + '"]')
                        .prop(options.property, value);
                }
            });

            $(options.selector).prop(options.property, function() { 
                var target = $(this)
                return options.loadValue(getScopeId(target).prefix + 
                    target.attr('name')); 
            })
        }

        return {
            cookie: {
                bindOptions: bindCookieOptions,
                bindValues: bindCookieValues,
                bindCheckboxes: bindCookieCheckboxes,
                bindTabs: bindCookieTabs
            },
            session: {
                bindValues: bindSessionValues,
                bindCheckboxes: bindSessionCheckboxes,
                bindText: bindSessionText,
                bindPasswords: bindSessionPasswords
            },
            bindOptions: bindOptions
        };
    };

    // ~~~~~~~~~~~~~~~~~ Handlebars ~~~~~~~~~~~~~~~~~

    function TemplateHelpers()
    {
        Handlebars.registerHelper('replace', function (source, find, replace) {
            var findRegex = eval(find.startsWith('/') ? find : '/' + find + '/g');
            return source.replace(findRegex, replace
                .replace(/&apos;/g, '\'').replace(/&amp;/g, '\'&')
                .replace(/&lt;/g, '<').replace(/&gt;/g, '>'));
        });

        Handlebars.registerHelper('htmlEncode', function (source) {
            return source.replace(/\</g, '&lt;').replace(/\>/g, '&gt;');
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

        Handlebars.registerHelper('any', function (source, field, options) {
            var condition = field.substring(0, 1) == '!' ? false : true;
            if (!condition) field = field.slice(1);
            var results = source.filter(function(x) { return x[field] == condition; });
            return results.length > 0 ? options.fn(this) : options.inverse(this);
        });           

        return {
            squash: function(template) {
                return template.split(/[\r\n]/)
                    .map(function (x) { return x.trim(); })
                    .join('');
            }
        }
    };

    // ~~~~~~~~~~~~~~~~~ Clipboard ~~~~~~~~~~~~~~~~~

    function Clipboard() {
        return {          
            init: function(selector, options) {
                $(selector).mouseleave(function(e) {      
                    $(e.target).tooltip('destroy');
                });
                var clipboard = new window.Clipboard(selector, options);
                clipboard.on('success', function(e) {
                    e.clearSelection();
                    $(e.trigger).tooltip('show');
                });
            }
        }
    };

    // ~~~~~~~~~~~~~~~~~ Scrolling ~~~~~~~~~~~~~~~~~

    function SmoothScroll() 
    {
        return {
            scroll: function(hash, scrollTop) {
                hash = (hash.charAt(0) != '#' ? '#' + hash : hash);
                if (!scrollTop && hash != '#' && $(hash).length > 0) {
                    var position = $(hash).offset().top;
                    $('html, body').animate({ scrollTop: position }, "slow");
                }
                else if (scrollTop) {
                    $('html, body').animate({ scrollTop: 0 }, "slow");
                }
            }
        }
    };

    // ~~~~~~~~~~~~~~~~~ Documentation ~~~~~~~~~~~~~~~~~

    function Documentation(templateHelpers, 
        clipboard, binding, smoothScroll) 
    {
        var squash = templateHelpers.squash;
        Handlebars.registerPartial("endpoint", $("#swank-endpoint-template").html());
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

        $('li a[href^="#"]').click(function (e) {
            scrollDocs(this.hash.slice(1).split('/', 1)[0]);
        });

        function scrollDocs(route) {
            smoothScroll.scroll(route, !isHomePage(route));
        }

        function isHomePage(route) {
            var isUnder = function(fragment) {
                return route == fragment || 
                    route.startsWith(fragment + '/');
            };
            return !isUnder('resources') && !isUnder('modules');
        }

        function loadHomePage(routeContext, overview) {
            if (!routeContext.hasLastRoute || !isHomePage(routeContext.lastRoute)) {
                loadMarkdown(overview);
            }
            scrollDocs(routeContext.route);
        }

        function loadMarkdown(markdown) {
            $('#content').html('<div class="markdown-body">' + markdown + '</div>');
            highlightCode('pre code');
        }   

        var resourceTemplate = Handlebars.compile(
            $("#swank-resource-template").html());

        function loadResource(resource) {
            $('#content').html(resourceTemplate(resource));
            initEndpointHeader();
            highlightCode('.swank-code-example code');
            initSampleDataClipboard('.swank-sample-data-copy');
            clipboard.init('.swank-code-example-copy');
            binding.cookie.bindTabs('.swank-data-format-selector',
                'SwankDataFormat', 'format');
            binding.cookie.bindTabs('.swank-code-example-selector',
                'SwankCodeLanguage', 'language');
        }

        function initSampleDataClipboard(selector) {
            clipboard.init(selector, {
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
        };

        function highlightCode(selector) {
            $(selector).each(function (i, block) {
                hljs.highlightBlock(block);
            });
        }

        function initEndpointHeader() {
            var togglePanel = function(element, docs) {
                var panelId = $(element).closest('div').attr('data-target');
                var docPanel = $(panelId + ' .swank-documentation');
                var testDrivePanel = $(panelId + ' .swank-test-drive');
                var docsPanelVisible = docPanel.is(':visible');
                var testDrivePanelVisible = testDrivePanel.is(':visible');

                if (docs)
                {
                    docPanel.show();
                    testDrivePanel.hide();
                }
                else 
                {
                    docPanel.hide();
                    testDrivePanel.show();
                }

                if ((docs && docsPanelVisible) ||
                    (!docs && testDrivePanelVisible) ||
                    (!docsPanelVisible && !testDrivePanelVisible))
                    $(panelId).collapse('toggle');
            }

            $('.swank-test-drive-toggle').on('click', function (e) {
                togglePanel(this, false);
            });

            $('.swank-doc-toggle').on('click', function (e) {
                togglePanel(this, true);
            });

            $('.swank-endpoint').on('sticky', function(e, header, stickyHeader) {
                stickyHeader.find('.swank-test-drive-toggle')
                    .on('click', function() {
                        togglePanel(this, false);
                        e.target.scrollIntoView();
                    });
                stickyHeader.find('.swank-doc-toggle')
                    .on('click', function() {
                        togglePanel(this, true);
                        e.target.scrollIntoView();
                    });
            });
        }

        return {
            loadModule: function(module) {
                loadMarkdown(module);
            },
            loadResource: loadResource,
            loadHomePage: loadHomePage
        };
    };

    // ~~~~~~~~~~~~~~~~~ Test Drive ~~~~~~~~~~~~~~~~~

    function TestDrive(templateHelpers, binding) 
    {
        var squash = templateHelpers.squash;
        Handlebars.registerPartial("testDrive", $("#swank-test-drive-template").html());
        Handlebars.registerPartial("testDriveData", $("#swank-test-drive-data-template").html());
        Handlebars.registerPartial("testDriveJsonData", squash($("#swank-test-drive-json-data-template").html()));
        Handlebars.registerPartial("testDriveXmlData", squash($("#swank-test-drive-xml-data-template").html()));
        Handlebars.registerPartial("testDriveAuthScheme", $("#swank-test-drive-auth-scheme-template").html());
        Handlebars.registerPartial("testDriveHeaders", $("#swank-test-drive-headers-template").html());
        Handlebars.registerPartial("testDriveQuerystring", $("#swank-test-drive-querystring-template").html());
        Handlebars.registerPartial("testDriveUrlParameters", $("#swank-test-drive-url-parameters-template").html());
        Handlebars.registerPartial("testDriveResponse", $("#swank-test-drive-response-template").html());

        function init(authSchemes) {
            binding.cookie.bindOptions('.swank-auth-schemes', 'SwankAuthScheme');
            binding.session.bindValues('.swank-auth-schemes');
            binding.session.bindPasswords('.swank-auth-schemes');
            binding.cookie.bindCheckboxes('.swank-test-drive-headers', 'SwankHeader');
            binding.cookie.bindValues('.swank-test-drive-headers', 'SwankHeader');
            binding.session.bindValues('.swank-auth-schemes');

            binding.session.bindValues('.swank-test-drive-url-parameters', '.swank-endpoint-body');
            binding.session.bindValues('.swank-test-drive-querystring', '.swank-endpoint-body');
            binding.session.bindCheckboxes('.swank-test-drive-querystring', '.swank-endpoint-body');
            binding.session.bindText('.swank-test-drive-json-request', '.swank-endpoint-body');
            binding.session.bindText('.swank-test-drive-xml-request', '.swank-endpoint-body');
            binding.session.bindText('.swank-test-drive-text-request', '.swank-endpoint-body');

            initSelectDropdowns();
            onExecuteClick(authSchemes);
        }

        function initSelectDropdowns() {
            $('.dropdown-menu.select-text a').click(function(e) {
                var item = $(e.target);
                item.closest('.input-group')
                    .find('input[type="text"]')
                    .prop('value', item.text())
                    .trigger('change');
            });        
        }

        function onExecuteClick(authSchemes) {
            $('.swank-test-drive-execute').click(function(e) 
            { 
                var container = $(e.target).closest('.swank-test-drive');
                execute(buildRequest(container, authSchemes), 
                    buildResponse(container));
            });
        }

        function buildRequest(container, authSchemes) {
            var headers = {};
            getEnabledValues(container, '.swank-test-drive-headers').
                forEach(x => headers[x.name] = x.value);
            var querystring = getEnabledValues(container, 
                '.swank-test-drive-querystring');
            var urlParameters = getValues(container, 
                '.swank-test-drive-url-parameters');
            var authScheme = container.find('.swank-auth-schemes ' + 
                'input[type="radio"]:checked')[0];
            if (authScheme) {
                authScheme = $(authScheme);
                var args = authScheme.closest('tr')
                    .find('input[type="text"], input[type="password"]')
                    .map(function() { return $(this).val(); }).get();
                addAuth(authSchemes, authScheme.val(), args, 
                    headers, urlParameters, querystring)
            }
            var bodyContainer = container.find('.swank-test-drive-request-data ' + 
                '.tab-pane.active textarea')
            var executeButton = container.find('.swank-test-drive-execute');
            var request = {
                method: container.attr('data-method'),
                headers: headers,
                url: buildUrl(container.attr('data-url'), urlParameters, querystring),
                dataType: bodyContainer.attr('name'),
                body: bodyContainer.val(),
                busy: function(busy) {
                    if (busy) executeButton.addClass('swank-test-drive-busy');
                    else executeButton.removeClass('swank-test-drive-busy');
                }
            };
            return request;
        }

        function addAuth(authSchemes, scheme, args, headers, urlParameters, querystring) {
            if (!scheme) return;
            var scheme = authSchemes[scheme];
            scheme.components.forEach(c => {
                var value = c.generate.apply(this, args);
                if (c.header) headers[c.name] = value;
                else (c.querystring ? querystring : urlParameters)
                    .push({ name: c.name, value: value });
            });
        }

        function buildUrl(urlTemplate, urlParameters, querystring) {
            var url = urlTemplate.split('?')[0];
            urlParameters.forEach(x => url = url.replace('{' + x.name + '}', x.value));
            if (querystring.length > 0) {
                url = url + '?' + querystring.map(x => 
                    x.name + '=' + x.value).join('&');
            }
            return '/' + url;
        }

        function getEnabledValues(container, selector) {
            return container.find(selector + ' input[type="text"]').toArray()
                .filter(x => container.find(selector + 
                    ' input[type="checkbox"][name="' + x.name + 
                        '"]:checked').length > 0)
                .map(x => ({
                    name: x.name,
                    value: x.value
                }));
        }

        function getValues(container, selector) {
            return container.find(selector + ' input[type="text"]').toArray()
                .map(x => ({
                    name: x.name,
                    value: x.value
                }));
        }

        function buildResponse(container) {
            var responseBody = container.find('.swank-test-drive-response-body');
            return {
                displayHeaders: function(statusType, statusCode, status, headers) {
                    container.find('.swank-test-drive-response-headers')
                    .html('<span class="swank-test-drive-status-' + statusType + '">' + 
                        statusCode + ' ' + status + '</span>\r\n' + headers);
                },
                displayText: function(text) { 
                    responseBody.empty();
                    responseBody.append('<pre><code></code></pre>');
                    responseBody.find('code').text(text);
                },
                displayHtml: function(html) { 
                    responseBody.empty();
                    responseBody.append('<iframe>');
                    responseBody.find('iframe').contents().find('html').html(html);
                }
            };
        }

        function execute(request, response) {
            request.busy(true);
            $.ajax({
                method: request.method,
                url: request.url,
                data: request.dataType == 'base64' ? 
                    base64.decode(request.body) : 
                    request.body,
                headers: request.headers,
                dataType: 'text',
                processData: false
            })
            .done(function(data, textStatus, jqXHR) {
                request.busy(false);
                displayResponse(jqXHR, response);
            })
            .fail(function(jqXHR) {
                request.busy(false);
                displayResponse(jqXHR, response);
            });
        }

        function displayResponse(xhr, response) {
            displayResponseBody(xhr, response);
            displayReponseHeaders(xhr, response);
        }

        function displayReponseHeaders(jqXHR, response) { 
            var status;
            if (jqXHR.status < 200) status = '100';
            else if (jqXHR.status < 300) status = '200';
            else if (jqXHR.status < 400) status = '300';
            else if (jqXHR.status < 500) status = '400';
            else status = '500';
            response.displayHeaders(status, jqXHR.status, 
                jqXHR.statusText, jqXHR.getAllResponseHeaders())
        }

        function displayResponseBody(jqXHR, response) {
            var data = jqXHR.responseText;
            if (!data) {
                response.displayText('');
                return;
            }
            var contentType = jqXHR.getResponseHeader('content-type');
            if (contentType.indexOf('html') > -1)
                response.displayHtml(data);
            else if (contentType.indexOf('json') > -1)
                response.displayText(vkbeautify.json(data, 2));
            else if (contentType.indexOf('xml') > -1)
                response.displayText(vkbeautify.xml(data, 2));
            else if (contentType.indexOf('octet-stream') > -1)
                response.displayText(base64.encode(data));
            else response.displayText(data || '');
        }

        return {
            init: init
        };
    };

    // ~~~~~~~~~~~~~~~~~ Init ~~~~~~~~~~~~~~~~~

    var swank = window.Swank;
    var templateHelpers = TemplateHelpers();
    var binding = Binding();
    var clipboard = Clipboard();
    var smoothScroll = SmoothScroll();
    var documentation = Documentation(templateHelpers, 
        clipboard, binding, smoothScroll);
    var testDrive = TestDrive(templateHelpers, binding);

    Router({
        modules: function(routeContext) { 
            documentation.loadModule(swank.modules[
                routeContext.parameter]); 
        },
        resources: function(routeContext) { 
            $.post('resources', JSON.stringify({ 
                name: routeContext.parameter 
            }), function (resource) {
                documentation.loadResource(resource);
                testDrive.init(swank.authSchemes);
            }, 'json');
        },
        default: function(routeContext) { 
            documentation.loadHomePage(routeContext, swank.overview); 
        }
    });

});