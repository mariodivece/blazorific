namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using System;
    using System.Reflection;

    public sealed partial class CandyGridCell : IAttachedComponent, IDisposable
    {
        private IPropertyProxy CheckedProperty;

        public CandyGridCell()
        {
            Attributes = new AttributeDictionary(StateHasChanged);
        }

        private enum GridButtonEventType
        {
            DetailsButtonClick,
            EditButtonClick,
            DeleteButtonClick,
        }

        public AttributeDictionary Attributes { get; }

        public int Index { get; private set; } = -1;

        [CascadingParameter(Name = nameof(Column))]
        public CandyGridColumn Column { get; private set; }

        [CascadingParameter(Name = nameof(Row))]
        public CandyGridRow Row { get; private set; }

        internal CandyGridRow Container => Row;

        private bool IsChecked
        {
            get
            {
                return (bool)(CheckedProperty?.GetValue(DataItem) ?? false);
            }
            set
            {
                if (value == IsChecked) return;
                CheckedProperty?.SetValue(DataItem, value);
                RaiseOnCellCheckedChanged(value);
            }
        }

        private string RightAlignCssClass { get; set; }

        private object DataItem => Row?.DataItem;

        private bool HasAutomaticButtons => 
            Column.OnDeleteButtonClick != null ||
            Column.OnDetailsButtonClick != null ||
            Column.OnEditButtonClick != null;

        private bool HasAutomaticCheckbox => CheckedProperty != null;

        public void Dispose()
        {
            Row?.RemoveCell(this);
        }

        protected override void OnInitialized()
        {
            Index = Row?.AddCell(this) ?? -1;

            var fieldProxy = string.IsNullOrWhiteSpace(Column?.Field) ? null : Row?.DataItem?.PropertyProxy(Column.Field);
            RightAlignCssClass = fieldProxy == null || !fieldProxy.PropertyType.IsNumeric() ? null : "text-right";

            var checkedProxy = !string.IsNullOrWhiteSpace(Column.CheckedProperty)
                ? DataItem?.PropertyProxy(Column.CheckedProperty)
                : null;

            if (checkedProxy == null || (checkedProxy.PropertyType != typeof(bool) && checkedProxy.PropertyType != typeof(bool?)))
                CheckedProperty = null;
            else
                CheckedProperty = checkedProxy;
        }

        private void RaiseOnRowButtonClick(MouseEventArgs e, GridButtonEventType eventType)
        {
            var callback = eventType switch
            {
                GridButtonEventType.EditButtonClick => Column.OnEditButtonClick,
                GridButtonEventType.DeleteButtonClick => Column.OnDeleteButtonClick,
                _ => Column.OnDetailsButtonClick
            };

            if (callback == null) return;

            $"EVENT".Log(nameof(CandyGridCell), $"On{eventType}");
            callback?.Invoke(new GridRowMouseEventArgs(Row, e));
            Row.NotifyStateChanged();
        }

        private void RaiseOnCellCheckedChanged(bool isChecked)
        {
            $"EVENT".Log(nameof(CandyGridCell), nameof(CandyGridColumn.OnCellCheckedChanged));
            Column.OnCellCheckedChanged?.Invoke(new GridCellCheckboxEventArgs(Row, Column, isChecked));
            Row.NotifyStateChanged();
        }
    }
}
