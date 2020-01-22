namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using System;
    using System.Collections.Generic;

    public sealed partial class CandyGridRow : IAttachedComponent
    {
        private Dictionary<string, object> m_Attributes;

        [CascadingParameter(Name = nameof(DataItem))]
        public object DataItem { get; private set; }

        public Dictionary<string, object> Attributes
        {
            get => m_Attributes;
            set
            {
                m_Attributes = value;
                StateHasChanged();
            }
        }

        public int Index { get; private set; } = -1;

        public IReadOnlyList<CandyGridCell> Cells { get; } = new List<CandyGridCell>(32);

        public void Dispose()
        {
            Parent.RemoveRow(this);
            Index = -1;
        }

        protected override void OnInitialized()
        {
            Index = Parent.AddRow(this);
            base.OnInitialized();
        }

        internal void NotifyStateChanged() => StateHasChanged();

        internal int AddCell(CandyGridCell cell) => Cells.AddAttachedComponent(cell);

        internal void RemoveCell(CandyGridCell cell) => Cells.RemoveAttachedComponent(cell);

        private void RaiseOnBodyRowDoubleClick(MouseEventArgs e)
        {
            $"EVENT".Log(nameof(CandyGridCell), $"On{nameof(Parent.OnBodyRowDoubleClick)}");
            Parent.OnBodyRowDoubleClick?.Invoke(new GridRowMouseEventArgs(this, e));
        }

        private void RaiseOnBodyRowClick(MouseEventArgs e)
        {
            $"EVENT".Log(nameof(CandyGridCell), $"On{nameof(Parent.OnBodyRowClick)}");
            Parent.OnBodyRowClick?.Invoke(new GridRowMouseEventArgs(this, e));
        }
    }
}
