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

    public class GridEventArgs<T> : GridEventArgs
    {
        public GridEventArgs(CandyGrid sender, T argument)
            : base(sender)
        {
            Argument = argument;
        }

        public T Argument { get; }
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

    public class GridInputEventArgs<T> : GridInputEventArgs
    {
        public GridInputEventArgs(CandyGrid sender, MouseEventArgs input, T argument)
            : base(sender, input)
        {
            Argument = argument;
        }

        public T Argument { get; }
    }
}
