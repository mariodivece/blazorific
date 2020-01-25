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

        private CompareOperators FilterOperator;
        private string FilterText1;
        private string FilterText2;
        private ElementReference ColumnFilterElement;

        [Inject]
        private IJSRuntime Js { get; set; }

        [CascadingParameter(Name = nameof(Column))]
        private CandyGridColumn Column { get; set; }

        private bool ShowFilter => (Column?.ShowFilter ?? false) && FilterOperators.Count > 1;

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

        private IPropertyProxy Property => Column?.Property;

        private string FilterInputType
        {
            get
            {
                if (Column is IGridDataColumn col)
                {
                    return col.DataType switch
                    {
                        DataType.Boolean => "checkbox",
                        DataType.Date => "date",
                        DataType.Numeric => "number",
                        DataType.DateTime => "datetime-local",
                        DataType.DateTimeUtc => "datetime-local",
                        _ => "text"
                    };
                }
                
                return "text";
            }
            
        }

        private void OnColumnSortClick(MouseEventArgs e)
        {
            Column.ChangeSortDirection(e.CtrlKey);
        }

        private void OnApplyFilterClick()
        {
            Column?.ApplyFilter(FilterOperator, FilterText1, FilterText2);
            FilterOperator = Column?.Filter.Operator ?? CompareOperators.None;
            FilterText1 = Column?.Filter.Text;
        }

        private void OnClearFilterClick()
        {
            Column?.ApplyFilter(CompareOperators.None);
            FilterOperator = Column?.Filter.Operator ?? CompareOperators.None;
            FilterText1 = Column?.Filter.Text;
        }

        protected override void OnInitialized()
        {
            try
            {
                if (Property == null)
                    return;

                if (Property.PropertyType.IsNumeric())
                {
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
