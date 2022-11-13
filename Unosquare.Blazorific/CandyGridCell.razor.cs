namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Swan.Reflection;
    using System;

    public sealed partial class CandyGridCell : IAttachedComponent, IDisposable
    {
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

        public IPropertyProxy Property => Column?.Property;

        private bool IsChecked
        {
            get
            {
                return (bool)(Column.CheckedPropertyProxy?.Read(DataItem) ?? false);
            }
            set
            {
                if (value == IsChecked) return;
                Column.CheckedPropertyProxy?.Write(DataItem, value);
                RaiseOnCellCheckedChanged(value);
            }
        }

        private string TextAlignCssClass { get; set; }

        private object DataItem => Row?.DataItem;

        private bool HasAutomaticButtons =>
            Column.OnDeleteButtonClick != null ||
            Column.OnDetailsButtonClick != null ||
            Column.OnEditButtonClick != null;

        private bool HasAutomaticCheckbox => Column.CheckedPropertyProxy != null;

        public void Dispose()
        {
            Row?.RemoveCell(this);
        }

        protected override void OnInitialized()
        {
            Index = Row?.AddCell(this) ?? -1;
            TextAlignCssClass = GetTextAlignCssClass();
        }

        private string? GetTextAlignCssClass()
        {
            if (Column.Alignment != TextAlignment.Auto)
            {
                return Column.Alignment switch
                {
                    TextAlignment.Center => "text-center",
                    TextAlignment.Left => "text-start",
                    TextAlignment.Right => "text-end",
                    TextAlignment.Justify => "text-start",
                    _ => null
                };
            }

            if (Property == null) return null;

            var t = Property.PropertyType;
            return t.IsNumeric || t.BackingType.NativeType == typeof(DateTime)
                ? "text-end"
                : t.BackingType.NativeType == typeof(bool)
                ? "text-center"
                : null;
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
            Row?.NotifyStateChanged();
        }

        private void RaiseOnCellCheckedChanged(bool isChecked)
        {
            $"EVENT".Log(nameof(CandyGridCell), nameof(CandyGridColumn.OnCellCheckedChanged));
            Column.OnCellCheckedChanged?.Invoke(new GridCellCheckboxEventArgs(Row, Column, isChecked));
            Row?.NotifyStateChanged();
        }
    }
}
