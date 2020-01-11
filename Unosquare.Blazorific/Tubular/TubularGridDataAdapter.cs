namespace Unosquare.Blazorific.Tubular
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;
    using Microsoft.AspNetCore.Components;

    public class TubularGridDataAdapter : IGridDataAdapter
    {
        public TubularGridDataAdapter(Type dataItemType, string requestUrl)
        {
            DataItemType = dataItemType;
            RequestUrl = requestUrl;
        }

        public Type DataItemType { get; }

        public string RequestUrl { get; }

        public async Task<GridDataResponse> RetrieveDataAsync(CandyGrid grid)
        {
            using var client = new HttpClient();
            var request = grid.CreateTubularGridDataRequest(DataItemType);
            var response = await client.PostJsonAsync<TubularGridDataResponse>(RequestUrl, request);
            return response.ToGridDataResponse(DataItemType);
        }
    }

    public class TubularGridDataAdapter<T> : TubularGridDataAdapter
    {
        public TubularGridDataAdapter(string requestUrl)
            : base(typeof(T), requestUrl)
        {
        }

        public new async Task<GridDataResponse> RetrieveDataAsync(CandyGrid grid)
        {
            using var client = new HttpClient();
            var request = grid.CreateTubularGridDataRequest<T>();
            var response = await client.PostJsonAsync<TubularGridDataResponse>(RequestUrl, request);
            return response.ToGridDataResponse<T>();
        }
    }
}
