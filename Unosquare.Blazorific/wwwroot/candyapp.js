if (!window.CandyAppConstants) {

    window.CandyAppConstants = {
        contentBaseUrl: '/_content/Unosquare.Blazorific/',
    };

    window.CandyAppLoader = {
        themeElement: null,

        scriptFiles: [
            CandyAppConstants.contentBaseUrl + "jquery/jquery-3.6.0.min.js",
            CandyAppConstants.contentBaseUrl + "bootstrap/bootstrap.bundle.min.js",
            CandyAppConstants.contentBaseUrl + "candygrid.js",
            CandyAppConstants.contentBaseUrl + "candymodal.js",
            CandyAppConstants.contentBaseUrl + "candytabset.js",
            "/_framework/blazor.webassembly.js"
        ],

        styleFiles: [
            CandyAppConstants.contentBaseUrl + "bootstrap/bootstrap.min.css",
            CandyAppConstants.contentBaseUrl + "fontawesome/css/all.min.css",
            CandyAppConstants.contentBaseUrl + "candygrid.css",
            CandyAppConstants.contentBaseUrl + "candymodal.css",
        ],

        themeFiles: [
            { name: 'Default', url: CandyAppConstants.contentBaseUrl + 'bootstrap/bootstrap.min.css' },
            { name: 'Cerulean', url: '' },
            { name: 'Cosmo', url: '' },
            { name: 'Cyborg', url: '' },
            { name: 'Darkly', url: '' },
            { name: 'Flatly', url: '' },
            { name: 'Journal', url: '' },
            { name: 'Litera', url: '' },
            { name: 'Lumen', url: '' },
            { name: 'Lux', url: '' },
            { name: 'Materia', url: '' },
            { name: 'Minty', url: '' },
            { name: 'Pulse', url: '' },
            { name: 'Sandstone', url: '' },
            { name: 'Simplex', url: '' },
            { name: 'Sketchy', url: '' },
            { name: 'Slate', url: '' },
            { name: 'Solar', url: '' },
            { name: 'Spacelab', url: '' },
            { name: 'Superhero', url: '' },
            { name: 'United', url: '' },
            { name: 'Yeti', url: '' },
        ],

        hasLoaded: false,

        initialize: function () {
            window.addEventListener('DOMContentLoaded', CandyAppLoader.__loadResources);
        },

        hideApp: function () {
            var appElement = document.getElementById("app");
            appElement.style.display = "none";
        },

        showApp: function () {
            var appElement = document.getElementById("app");
            appElement.style.display = "";
        },

        __loadResources: function () {
            window.removeEventListener('DOMContentLoaded', CandyAppLoader.__loadResources);
            CandyAppLoader.hideApp();
            CandyAppLoader.__loadStyles();
            CandyAppLoader.showApp();
            CandyAppLoader.__loadScripts();
        },

        __loadScripts: function () {
            CandyAppLoader.__loadScriptChain(0);
        },

        __loadStyles: function () {
            for (i = CandyAppLoader.styleFiles.length - 1; i >= 0; i--) {
                CandyAppLoader.__loadStyle(CandyAppLoader.styleFiles[i]);
            }
        },

        __loadScriptChain: function (scriptIndex) {
            var scriptEl = document.createElement('script');
            scriptEl.setAttribute('src', CandyAppLoader.scriptFiles[scriptIndex]);
            scriptEl.setAttribute('type', 'text/javascript');

            if (scriptIndex + 1 < CandyAppLoader.scriptFiles.length) {
                scriptEl.onload = function () { CandyAppLoader.__loadScriptChain(scriptIndex + 1); };
            } else {
                scriptEl.onload = function () { CandyAppLoader.hasLoaded = true; };
            }

            document.body.appendChild(scriptEl);
        },

        __loadStyle: function (styleFile) {
            var styleEl = document.createElement('link');
            styleEl.setAttribute('href', styleFile);
            styleEl.setAttribute('rel', 'stylesheet');
            if (styleFile.endsWith('bootstrap.min.css'))
                CandyAppLoader.themeElement = styleEl;

            document.head.prepend(styleEl);
        },
    }

    window.CandyApp = {
        applyTheme: function (themeName) {

            if (CandyAppLoader.themeElement === null)
                return false;

            CandyAppLoader.hideApp();
            var themeFile = null;
            for (var i = 0; i < CandyAppLoader.themeFiles.length; i++) {
                if (CandyAppLoader.themeFiles[i].name.toLowerCase() === themeName.toLowerCase()) {
                    if (CandyAppLoader.themeFiles[i].url === '') {
                        CandyAppLoader.themeFiles[i].url = CandyAppConstants.contentBaseUrl
                            + 'bootstrap/'
                            + CandyAppLoader.themeFiles[i].name.toLowerCase()
                            + '/bootstrap.min.css';
                    }

                    themeFile = CandyAppLoader.themeFiles[i];
                }

            }

            if (themeFile === null) {
                CandyAppLoader.showApp();
                return false;
            }

            CandyAppLoader.themeElement.setAttribute('href', themeFile.url);
            CandyAppLoader.showApp();
            return true;
        },

        getThemeNames: function () {
            var names = [];
            for (var i = 0; i < CandyAppLoader.themeFiles.length; i++) {
                names.push(CandyAppLoader.themeFiles[i].name);
            }

            return names;
        },

        getCurrentThemeName: function () {
            var themeName = '';
            var themeUrl = CandyAppLoader.themeElement.getAttribute('href');

            for (var i = 0; i < CandyAppLoader.themeFiles.length; i++) {
                if (CandyAppLoader.themeFiles[i].url === themeUrl)
                    return CandyAppLoader.themeFiles[i].name;
            }

            return themeName;
        },

        bindTooltip: function (targetEl) {
            $(targetEl).tooltip();
        },

        focusElement: function (element) {
            $(element).focus();
        },
    };

    CandyAppLoader.initialize();
}