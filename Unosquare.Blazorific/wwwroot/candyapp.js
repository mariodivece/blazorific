if (!window.CandyAppConstants) {

    window.CandyAppConstants = {
        contentBaseUrl: '/_content/Unosquare.Blazorific/',
        bootstrapBaseUrl: 'https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.6.0/',
        bootswatchBaseUrl: 'https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.6.0/',
    };

    window.CandyAppLoader = {
        themeElement: null,

        scriptFiles: [
            "https://cdnjs.cloudflare.com/ajax/libs/jquery/3.5.1/jquery.min.js",
            CandyAppConstants.bootstrapBaseUrl + "js/bootstrap.bundle.min.js",
            CandyAppConstants.contentBaseUrl + "candygrid.js",
            CandyAppConstants.contentBaseUrl + "candymodal.js",
            CandyAppConstants.contentBaseUrl + "candytabset.js",
            "/_framework/blazor.webassembly.js"
        ],

        styleFiles: [
            CandyAppConstants.bootstrapBaseUrl + "css/bootstrap.min.css",
            "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.2/css/all.min.css",
            CandyAppConstants.contentBaseUrl + "candygrid.css",
            CandyAppConstants.contentBaseUrl + "candymodal.css",
        ],

        themeFiles: [
            { name: 'Default', url: CandyAppConstants.bootstrapBaseUrl + 'css/bootstrap.min.css' },
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

        __loadResources: function () {
            window.removeEventListener('DOMContentLoaded', CandyAppLoader.__loadResources);
            CandyAppLoader.__loadScripts();
            CandyAppLoader.__loadStyles();
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

            var themeFile = null;
            for (var i = 0; i < CandyAppLoader.themeFiles.length; i++) {
                if (CandyAppLoader.themeFiles[i].name.toLowerCase() === themeName.toLowerCase())
                    themeFile = CandyAppLoader.themeFiles[i];
            }

            if (themeFile === null)
                return false;

            if (themeFile.url === '') {
                themeFile.url = CandyAppConstants.bootswatchBaseUrl
                    + themeFile.name.toLowerCase()
                    + '/bootstrap.min.css';
            }

            CandyAppLoader.themeElement.setAttribute('href', themeFile.url);
            return true;
        },

        bindTooltip: function (targetEl) {
            $(targetEl).tooltip();
        },        
    };

    CandyAppLoader.initialize();
}