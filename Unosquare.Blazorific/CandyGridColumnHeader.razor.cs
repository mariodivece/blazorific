namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;

    public sealed partial class CandyGridColumnHeader
    {
        [Parameter]
        public string CssClass { get; set; } = "candygrid-column-header";

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
    }
}
