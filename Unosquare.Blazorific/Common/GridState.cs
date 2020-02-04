namespace Unosquare.Blazorific.Common
{
    using System.Collections.Generic;

    public class GridState
    {
        public string SearchText { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public IReadOnlyCollection<GridColumnState> Columns { get; set; }

        public string Serialize() => this.SerializeJson();

        internal static GridState FromGrid(CandyGrid grid, GridDataRequest request)
        {
            var result = new GridState
            {
                PageNumber = grid.PageNumber,
                PageSize = grid.PageSize,
                SearchText = grid.SearchText,
            };

            var columns = new List<GridColumnState>(request.Columns.Count);
            foreach (var col in request.Columns)
                columns.Add(GridColumnState.FromOther(col));

            result.Columns = columns;
            return result;
        }

        internal static GridState CreateDefault(CandyGrid grid)
        {
            var result = new GridState
            {
                PageNumber = 1,
                PageSize = 20,
                SearchText = string.Empty,
            };

            var columns = new List<GridColumnState>(grid.Columns.Count);
            foreach (var col in grid.Columns)
            {
                columns.Add(new GridColumnState
                {
                    Name = col.Field,
                    Filter = new GridDataFilter(),
                });
            }

            result.Columns = columns;
            return result;
        }

        internal static GridState Deserialize(string json) => json.DeserializeJson<GridState>();
    }

    public class GridColumnState : IGridDataColumn
    {
        public string Name { get; set; }

        public SortDirection SortDirection { get; set; }

        public int SortOrder { get; set; }

        public GridDataFilter Filter { get; set; }

        public bool Sortable { get; set; }

        public DataType DataType { get; set; }

        public bool Searchable { get; set; }

        public AggregationFunction Aggregate { get; set; }

        internal static GridColumnState FromOther(IGridDataColumn other)
        {
            return new GridColumnState
            {
                Name = other.Name,
                SortDirection = other.SortDirection,
                SortOrder = other.SortOrder,
                Filter = other.Filter,
                Sortable = other.Sortable,
                DataType = other.DataType,
                Searchable = other.Searchable,
                Aggregate = other.Aggregate,
            };
        }
    }
}
