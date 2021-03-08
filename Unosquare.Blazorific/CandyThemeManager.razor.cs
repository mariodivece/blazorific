namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public partial class CandyThemeManager
    {
        public const string DefaultThemeStorageKey = "Unosquare.Blazorific.Theme";

        [Inject]
        protected IJSRuntime Js { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string ThemeStorageKey { get; set; }

        [Parameter]
        public string DefaultThemeName { get; set; } = "Default";

        public ICollection<string> ThemeNames { get; private set; } = Array.Empty<string>();

        public string CurrentThemeName { get; set; } = string.Empty;

        private bool IsStorageEnabled => !string.IsNullOrWhiteSpace(ThemeStorageKey);

        public async Task ApplyThemeAsync(string themeName)
        {
            await Js.ApplyThemeAsync(themeName);
            CurrentThemeName = await Js.GetCurrentThemeNameAsync();

            if (IsStorageEnabled)
                await Js.StorageSetItemAsync(ThemeStorageKey, CurrentThemeName);

            $"Current theme changed to '{CurrentThemeName}'"
                .Log(nameof(CandyThemeManager), nameof(OnInitializedAsync));

            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            ThemeNames = await Js.GetThemeNamesAsync();
            CurrentThemeName = IsStorageEnabled
                ? await Js.StorageGetItemAsync(ThemeStorageKey)
                : string.Empty;
            
            if (string.IsNullOrWhiteSpace(CurrentThemeName))
                CurrentThemeName = DefaultThemeName;

            if (IsStorageEnabled)
                await Js.StorageSetItemAsync(ThemeStorageKey, CurrentThemeName);
            
            await Js.ApplyThemeAsync(CurrentThemeName);

            $"Loaded {ThemeNames?.Count} themes. Current Theme: {CurrentThemeName}"
                .Log(nameof(CandyThemeManager), nameof(OnInitializedAsync));
        }
    }
}
