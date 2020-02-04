namespace Unosquare.Blazorific.Common
{
    using Microsoft.AspNetCore.Components.Web;
    using System;

    public class GridEventArgs : EventArgs
    {
        public GridEventArgs(CandyGrid grid)
        {
            Grid = grid;
        }

        public CandyGrid Grid { get; }
    }

    public class GridStateEventArgs : GridEventArgs
    {
        public GridStateEventArgs(CandyGrid grid, GridState state, bool isReset)
            : base(grid)
        {
            State = state;
            IsReset = isReset;
        }

        public GridState State { get; }

        public bool IsReset { get; }
    }

    public class GridExceptionEventArgs : GridEventArgs
    {
        public GridExceptionEventArgs(CandyGrid grid, Exception exception)
            : base(grid)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }

    public class GridRowMouseEventArgs : GridEventArgs
    {
        public GridRowMouseEventArgs(CandyGridRow row, MouseEventArgs mouse)
            : base(row?.Grid)
        {
            Row = row;
            Mouse = mouse;
        }

        public CandyGridRow Row { get; }

        public MouseEventArgs Mouse { get; }

        public object DataItem => Row?.DataItem;
    }

    public class GridCellCheckboxEventArgs : GridEventArgs
    {
        public GridCellCheckboxEventArgs(CandyGridRow row, CandyGridColumn column, bool isChecked)
            : base(row?.Grid)
        {
            IsChecked = isChecked;
            Row = row;
            Column = column;
        }

        public CandyGridRow Row { get; }

        public CandyGridColumn Column { get; }

        public object DataItem => Row?.DataItem;

        public bool IsChecked { get; }
    }
}
