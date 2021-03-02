namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.JSInterop;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public sealed partial class CandyGridColumnHeader
    {
        private const string DateTimeFormatFilter = "yyyy-MM-dd HH:mm:ss";
        private const string DateTimeFormatUI = "yyyy-MM-ddTHH:mm:ss";

        private static readonly IReadOnlyDictionary<CompareOperators, string> EmptyOperators = new Dictionary<CompareOperators, string>(0);

        private static readonly IReadOnlyDictionary<CompareOperators, string> BooleanOperators = new Dictionary<CompareOperators, string>
        {
            { CompareOperators.None, "(Select)" },
            { CompareOperators.Equals, "Equals" }
        };

        private static readonly IReadOnlyDictionary<CompareOperators, string> StringOperators = new Dictionary<CompareOperators, string>
        {
            { CompareOperators.None, "(Select)" },
            { CompareOperators.Equals, "Equals" },
            { CompareOperators.Contains, "Contains" },
            { CompareOperators.StartsWith, "Starts With" },
            { CompareOperators.EndsWith, "Ends With" },
            { CompareOperators.NotContains, "Not Contains" },
            { CompareOperators.NotEndsWith, "Not Ends With" },
            { CompareOperators.NotStartsWith, "Not Starts With" }
        };

        private static readonly IReadOnlyDictionary<CompareOperators, string> NumericOperators = new Dictionary<CompareOperators, string>
        {
            { CompareOperators.None, "(Select)" },
            { CompareOperators.Equals, "Equals" },
            { CompareOperators.Between, "Between" },
            { CompareOperators.Gte, "Greater or Equal" },
            { CompareOperators.Lte, "Less or Equal" },
            { CompareOperators.Gt, "Greater Than" },
            { CompareOperators.Lt, "Less Than" },
            { CompareOperators.NotEquals, "Not Equals" }
        };

        private readonly Dictionary<string, bool> CheckedFilterOptions = new Dictionary<string, bool>(32);
        private CompareOperators FilterOperator;
        private string FilterArg1;
        private string FilterArg2;

        [Inject]
        private IJSRuntime Js { get; set; }

        [CascadingParameter(Name = nameof(Column))]
        private CandyGridColumn Column { get; set; }

        private ElementReference ColumnFilterElement { get; set; }

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

        private IReadOnlyDictionary<CompareOperators, string> FilterOperators
        {
            get
            {
                if (Property == null)
                    return EmptyOperators;

                if (HasFilterOptions)
                    return BooleanOperators;

                var dataType = (Column as IGridDataColumn)?.DataType ?? DataType.String;
                return dataType switch
                {
                    DataType.Boolean => BooleanOperators,
                    DataType.String => StringOperators,
                    _ => NumericOperators
                };
            }
        }

        private IReadOnlyDictionary<string, string> FilterOptions { get; set; } = new Dictionary<string, string>(0);

        private bool BooleanFilterArg
        {
            get => (FilterArg1 ?? "false") == "true";
            set => FilterArg1 = value ? "true" : "false";
        }

        private string DateFilterArg1
        {
            get => DateTime.TryParse(FilterArg1, out var result) ? result.ToString(DateTimeFormatUI) : null;
            set => FilterArg1 = DateTime.TryParse(value, out var result) ? result.ToString(DateTimeFormatFilter) : null;
        }

        private string DateFilterArg2
        {
            get => DateTime.TryParse(FilterArg2, out var result) ? result.ToString(DateTimeFormatUI) : null;
            set => FilterArg2 = DateTime.TryParse(value, out var result) ? result.ToString(DateTimeFormatFilter) : null;
        }

        private string FilterInputType
        {
            get
            {
                if (Column is IGridDataColumn col)
                {
                    return col.DataType switch
                    {
                        DataType.Boolean => "checkbox",
                        DataType.Numeric => "number",
                        DataType.Date => "date",
                        DataType.DateTime => "datetime-local",
                        DataType.DateTimeUtc => "datetime-local",
                        _ => "text"
                    };
                }

                return "text";
            }

        }

        private bool IsBooleanInputType => FilterInputType == "checkbox";

        private bool IsDateInputType => FilterInputType == "date" || FilterInputType == "datetime-local";

        private bool HasFilterOptions => FilterOptions != null && FilterOptions.Count > 0;

        private void OnColumnSortClick(MouseEventArgs e)
        {
            Column.ChangeSortDirection(e.CtrlKey);
        }

        private void OnApplyFilterClick()
        {
            FilterOperator = HasFilterOptions
                ? CompareOperators.Multiple
                : IsBooleanInputType
                ? CompareOperators.Equals
                : FilterOperator;

            if (FilterOperator == CompareOperators.Multiple)
                Column?.ApplyFilter(FilterOperator, CheckedFilterOptions.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray());
            else
                Column?.ApplyFilter(FilterOperator, FilterArg1, FilterArg2);

            CoerceFilterState();
        }

        private void OnClearFilterClick()
        {
            Column?.ApplyFilter(CompareOperators.None);
            CoerceFilterState();
        }

        private void OnFilterOptionCheckedChanged(ChangeEventArgs e, string key)
        {
            var isChecked = (bool)e.Value;
            CheckedFilterOptions[key] = isChecked;
        }

        private void OnFilterOptionCheckTool(bool checkAll)
        {
            CheckedFilterOptions.Clear();
            if (!checkAll) return;

            foreach (var kvp in FilterOptions)
                CheckedFilterOptions[kvp.Key] = true;
        }

        private void CoerceFilterState()
        {
            FilterOperator = Column?.FilterOperator ?? CompareOperators.None;
            FilterArg1 = Column?.FilterText;
            FilterArg2 = Column?.FilterArgument != null && Column?.FilterArgument.Length > 0
                ? Column?.FilterArgument[0]
                : null;

            CheckedFilterOptions.Clear();

            if (FilterOperator != CompareOperators.Multiple)
                return;

            var checkedOptions = Column?.FilterArgument ?? Array.Empty<string>();
            foreach (var key in checkedOptions)
            {
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                CheckedFilterOptions[key] = true;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            await Js.InvokeVoidAsync($"{nameof(CandyGrid)}.bindColumnFilterDropdown", ColumnFilterElement);
        }

        protected override async Task OnInitializedAsync()
        {
            Grid.StateLoaded += (s, e) => CoerceFilterState();

            try
            {
                if (Column?.FilterOptionsProvider != null)
                    FilterOptions = await Column.FilterOptionsProvider.Invoke();
            }
            catch
            {
                // empty
            }
        }
    }
}
