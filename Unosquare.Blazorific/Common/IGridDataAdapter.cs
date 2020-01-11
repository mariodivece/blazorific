namespace Unosquare.Blazorific.Common
{
    using System.Threading.Tasks;

    public interface IGridDataAdapter
    {
        Task<GridDataResponse> RetrieveDataAsync(CandyGrid grid);
    }
}
