namespace Unosquare.Blazorific.Common
{
    public class GridCellData
    {

        internal GridCellData(CandyGridRow row, CandyGridColumn column, object dataItem)
        {
            Row = row;
            Column = column;
            DataItem = dataItem;
        }

        public CandyGridRow Row { get; }

        public CandyGridColumn Column { get; }

        public object DataItem { get; }

        public bool IsFooter => Row?.IsFooter ?? false;

        public T Item<T>() where T : class => DataItem as T;
    }
}
