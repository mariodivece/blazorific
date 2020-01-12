namespace Unosquare.Blazorific.Common
{
    using System;
    using System.Threading.Tasks;

    public interface IGridDataAdapter
    {
        Type DataItemType { get; }

        Task<GridDataResponse> RetrieveDataAsync(GridDataRequest request);
    }
}
