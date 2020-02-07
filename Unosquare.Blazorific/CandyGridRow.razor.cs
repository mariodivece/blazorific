namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using System.Collections.Generic;

    public sealed partial class CandyGridRow : CandyGridChildComponent, IAttachedComponent
    {
        public CandyGridRow()
        {
            Attributes = new AttributeDictionary(StateHasChanged);
        }

        [CascadingParameter(Name = nameof(DataItem))]
        public object DataItem { get; private set; }

        [Parameter]
        public bool IsFooter { get; set; }

        public AttributeDictionary Attributes { get; }

        public int Index { get; private set; } = -1;

        public IReadOnlyList<CandyGridCell> Cells { get; } = new List<CandyGridCell>(32);

        public void Dispose()
        {
            Parent.RemoveRow(this);
            Index = -1;
        }

        protected override void OnInitialized()
        {
            Index = Parent?.AddRow(this) ?? -1;
            base.OnInitialized();
        }

        internal void NotifyStateChanged() => StateHasChanged();

        internal int AddCell(CandyGridCell cell) => Cells.AddAttachedComponent(cell);

        internal void RemoveCell(CandyGridCell cell) => Cells.RemoveAttachedComponent(cell);

        private void RaiseOnBodyRowDoubleClick(MouseEventArgs e)
        {
            if (IsFooter) return;
            $"EVENT".Log(nameof(CandyGridCell), $"On{nameof(Parent.OnBodyRowDoubleClick)}");
            Parent.OnBodyRowDoubleClick?.Invoke(new GridRowMouseEventArgs(this, e));
        }

        private void RaiseOnBodyRowClick(MouseEventArgs e)
        {
            if (IsFooter) return;
            $"EVENT".Log(nameof(CandyGridCell), $"On{nameof(Parent.OnBodyRowClick)}");
            Parent.OnBodyRowClick?.Invoke(new GridRowMouseEventArgs(this, e));
        }
    }
}
