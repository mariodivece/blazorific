if (!window.CandyAssets) {

    window.CandyAssetsConstants = {
        contentBaseUrl: '/_content/Unosquare.Blazorific/',
        bootstrapBaseUrl: 'https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/4.6.0/',
        bootswatchBaseUrl: 'https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.6.0/',
    };

    window.CandyAssets = {
        themeElement: null,

        scriptFiles: [
            "https://cdnjs.cloudflare.com/ajax/libs/jquery/3.5.1/jquery.min.js",
            CandyAssetsConstants.bootstrapBaseUrl + "js/bootstrap.bundle.min.js",
            CandyAssetsConstants.contentBaseUrl + "candygrid.js",
            CandyAssetsConstants.contentBaseUrl + "candymodal.js",
            CandyAssetsConstants.contentBaseUrl + "candytabset.js",
            "/_framework/blazor.webassembly.js"
        ],

        styleFiles: [
            CandyAssetsConstants.bootstrapBaseUrl + "css/bootstrap.min.css",
            "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.2/css/all.min.css",
            CandyAssetsConstants.contentBaseUrl + "candygrid.css",
            CandyAssetsConstants.contentBaseUrl + "candymodal.css",
        ],

        themeFiles: [
            { name: 'Default', url: CandyAssetsConstants.bootstrapBaseUrl + 'css/bootstrap.min.css' },
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
            window.addEventListener('DOMContentLoaded', CandyAssets.__loadResources);
        },

        applyTheme: function (themeName) {

            if (CandyAssets.themeElement === null)
                return false;

            var themeFile = null;
            for (var i = 0; i < CandyAssets.themeFiles.length; i++) {
                if (CandyAssets.themeFiles[i].name.toLowerCase() === themeName.toLowerCase())
                    themeFile = CandyAssets.themeFiles[i];
            }

            if (themeFile === null)
                return false;

            if (themeFile.url === '') {
                themeFile.url = CandyAssetsConstants.bootswatchBaseUrl
                    + themeFile.name.toLowerCase()
                    + '/bootstrap.min.css';
            }

            CandyAssets.themeElement.setAttribute('href', themeFile.url);
            return true;
        },

        bindTooltip: function (targetEl) {
            $(targetEl).tooltip();
        },

        __loadResources: function () {
            window.removeEventListener('DOMContentLoaded', CandyAssets.__loadResources);
            CandyAssets.__loadScripts();
            CandyAssets.__loadStyles();
        },

        __loadScripts: function () {
            CandyAssets.__loadScriptChain(0);
        },

        __loadStyles: function () {
            for (i = CandyAssets.styleFiles.length - 1; i >= 0; i--) {
                CandyAssets.__loadStyle(CandyAssets.styleFiles[i]);
            }
        },

        __loadScriptChain: function (scriptIndex) {
            var scriptEl = document.createElement('script');
            scriptEl.setAttribute('src', CandyAssets.scriptFiles[scriptIndex]);
            scriptEl.setAttribute('type', 'text/javascript');

            if (scriptIndex + 1 < CandyAssets.scriptFiles.length) {
                scriptEl.onload = function () { CandyAssets.__loadScriptChain(scriptIndex + 1); };
            } else {
                scriptEl.onload = function () { CandyAssets.hasLoaded = true; };
            }

            document.body.appendChild(scriptEl);
        },

        __loadStyle: function (styleFile) {
            var styleEl = document.createElement('link');
            styleEl.setAttribute('href', styleFile);
            styleEl.setAttribute('rel', 'stylesheet');
            if (styleFile.endsWith('bootstrap.min.css'))
                CandyAssets.themeElement = styleEl;

            document.head.prepend(styleEl);
        },
    };

    CandyAssets.initialize();
}