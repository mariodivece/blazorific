namespace Unosquare.Blazorific.Common
{
    using Microsoft.AspNetCore.Components.Web;
    using System;

    public class GridEventArgs : EventArgs
    {
        public GridEventArgs(CandyGrid sender)
        {
            Sender = sender;
        }

        public CandyGrid Sender { get; }
    }

    public class GridExceptionEventArgs : GridEventArgs
    {
        public GridExceptionEventArgs(CandyGrid sender, Exception exception)
            : base(sender)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }

    public class GridDataEventArgs : GridEventArgs
    {
        public GridDataEventArgs(CandyGrid sender, object dataItem)
            : base(sender)
        {
            DataItem = dataItem;
        }

        public object DataItem { get; }
    }

    public class GridInputEventArgs : GridEventArgs
    {
        public GridInputEventArgs(CandyGrid sender, MouseEventArgs input)
            : base(sender)
        {
            Input = input;
        }

        public MouseEventArgs Input { get; }
    }

    public class GridInputDataEventArgs : GridInputEventArgs
    {
        public GridInputDataEventArgs(CandyGrid sender, MouseEventArgs input, object dataItem)
            : base(sender, input)
        {
            DataItem = dataItem;
        }

        public object DataItem { get; }
    }

    public class GridCellCheckedEventArgs : GridDataEventArgs
    {
        public GridCellCheckedEventArgs(CandyGrid sender, CandyGridColumn column, object dataItem, bool isChecked)
            : base(sender, dataItem)
        {
            IsChecked = isChecked;
            Column = column;
        }

        public bool IsChecked { get; }

        public CandyGridColumn Column { get; }
    }
}
