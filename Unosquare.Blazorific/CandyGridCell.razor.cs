namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed partial class CandyGridCell : IAttachedComponent
    {
        private IPropertyProxy CheckedProperty;
        private string m_CssClass;

        private enum GridButtonEventType
        {
            DetailsButtonClick,
            EditButtonClick,
            DeleteButtonClick,
        }

        public string CssClass
        {
            get => m_CssClass;
            set
            {
                m_CssClass = value;
                StateHasChanged();
            }
        }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; }

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

        private object DataItem => Row?.DataItem;

        public void Dispose()
        {
            Row?.RemoveCell(this);
        }

        protected override void OnInitialized()
        {
            Index = Row?.AddCell(this) ?? -1;
            var proxy = !string.IsNullOrWhiteSpace(Column.CheckedProperty)
                ? DataItem?.PropertyProxy(Column.CheckedProperty)
                : null;

            if (proxy == null || (proxy.PropertyType != typeof(bool) && proxy.PropertyType != typeof(bool?)))
                CheckedProperty = null;
            else
                CheckedProperty = proxy;
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
