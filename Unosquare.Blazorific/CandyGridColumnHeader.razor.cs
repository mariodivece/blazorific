namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.JSInterop;
    using System.Threading.Tasks;

    public sealed partial class CandyGridColumnHeader
    {
        private ElementReference ColumnFilterElement;

        [Inject]
        private IJSRuntime Js { get; set; }

        [CascadingParameter(Name = nameof(Column))]
        private CandyGridColumn Column { get; set; }

        private bool ShowFilter => Column.Filter.Operator != CompareOperators.None && Column.Filter.Operator != CompareOperators.Auto;

        private string CaretCssClass
        {
            get
            {
                return Column.SortDirection == SortDirection.Ascending
                    ? "fa-caret-up"
                    : Column.SortDirection == SortDirection.Descending
                    ? "fa-caret-down"
                    : string.Empty;
            }
        }

        private void OnColumnSortClick(MouseEventArgs e)
        {
            Column.ChangeSortDirection(e.CtrlKey);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
                return;

            await Js.InvokeVoidAsync($"{nameof(CandyGrid)}.bindColumnFilterDropdown", ColumnFilterElement);
        }
    }
}
