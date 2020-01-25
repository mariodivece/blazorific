namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.JSInterop;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    public sealed partial class CandyGridColumnHeader
    {
        private readonly Dictionary<CompareOperators, string> FilterOperators = new Dictionary<CompareOperators, string>(16);
        private string FilterInputType = "text";

        private CompareOperators FilterOperator;
        private string FilterText1;
        private string FilterText2;
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

        private IPropertyProxy Property { get; set; }

        private void OnColumnSortClick(MouseEventArgs e)
        {
            Column.ChangeSortDirection(e.CtrlKey);
        }

        private void OnApplyFilterClick(MouseEventArgs e)
        {
            Column.Filter.Operator = FilterOperator;
            Column.Filter.Text = FilterText1;
            Grid.QueueDataUpdate();
        }

        private void OnClearFilterClick(MouseEventArgs e)
        {
            FilterOperator = CompareOperators.None;
            FilterText1 = null;
            FilterText2 = null;

            OnApplyFilterClick(e);
        }

        protected override void OnInitialized()
        {
            try
            {
                Property = !string.IsNullOrWhiteSpace(Column?.Field)
                    ? Grid?.DataAdapter?.DataItemType?.PropertyProxy(Column.Field) : null;

                if (Property == null)
                    return;

                if (Property.PropertyType.IsNumeric())
                {
                    FilterInputType = "number";
                    FilterOperators[CompareOperators.None] = "(Select)";
                    FilterOperators[CompareOperators.Equals] = "Equals";
                    FilterOperators[CompareOperators.Between] = "Between";
                    FilterOperators[CompareOperators.Gte] = "Greater or Equal";
                    FilterOperators[CompareOperators.Lte] = "Less or Equal";
                    FilterOperators[CompareOperators.Gt] = "Greater Than";
                    FilterOperators[CompareOperators.Lt] = "Less Than";
                    FilterOperators[CompareOperators.NotEquals] = "Not Equals";
                    return;
                }

                if (Property.PropertyType == typeof(string))
                {
                    FilterInputType = "text";
                    FilterOperators[CompareOperators.None] = "(Select)";
                    FilterOperators[CompareOperators.Equals] = "Equals";
                    FilterOperators[CompareOperators.Contains] = "Contains";
                    FilterOperators[CompareOperators.StartsWith] = "Starts With";
                    FilterOperators[CompareOperators.EndsWith] = "Ends With";
                    FilterOperators[CompareOperators.NotContains] = "Not Contains";
                    FilterOperators[CompareOperators.NotEndsWith] = "Not Ends With";
                    FilterOperators[CompareOperators.NotStartsWith] = "Not Starts With";
                    return;
                }
            }
            finally
            {
                base.OnInitialized();
            }
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
