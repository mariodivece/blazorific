namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public partial class CandyThemeManager
    {
        [Inject]
        protected IJSRuntime Js { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string ThemeStorageKey { get; set; } = "Unosquare.Blazorific.Theme";

        public ICollection<string> ThemeNames { get; private set; } = new string[0];

        public string CurrentThemeName { get; set; } = string.Empty;

        public async Task ApplyThemeAsync(string themeName)
        {
            await Js.ApplyThemeAsync(themeName);
            CurrentThemeName = await Js.GetCurrentThemeNameAsync();
            await Js.StorageSetItemAsync(ThemeStorageKey, CurrentThemeName);

            $"Current theme changed to '{CurrentThemeName}'"
                .Log(nameof(CandyThemeManager), nameof(OnInitializedAsync));

            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            ThemeNames = await Js.GetThemeNamesAsync();
            CurrentThemeName = await Js.StorageGetItemAsync(ThemeStorageKey);
            if (string.IsNullOrWhiteSpace(CurrentThemeName))
                CurrentThemeName = await Js.GetCurrentThemeNameAsync();
            
            await Js.StorageSetItemAsync(ThemeStorageKey, CurrentThemeName);
            await Js.ApplyThemeAsync(CurrentThemeName);

            $"Loaded {ThemeNames?.Count} themes. Current Theme: {CurrentThemeName}"
                .Log(nameof(CandyThemeManager), nameof(OnInitializedAsync));
        }
    }
}
