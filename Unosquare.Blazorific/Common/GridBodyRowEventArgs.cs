namespace Unosquare.Blazorific.Common
{
    using System;

    public class GridBodyRowEventArgs : EventArgs
    {
        public GridBodyRowEventArgs(CandyGrid sender, object dataItem)
        {
            Sender = sender;
            DataItem = dataItem;
        }

        public CandyGrid Sender { get; }

        public object DataItem { get; }
    }
}