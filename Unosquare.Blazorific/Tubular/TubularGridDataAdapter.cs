namespace Unosquare.Blazorific.Tubular
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;

    /// <summary>
    /// Provides an adapter implementation for the compact Tubular
    /// grid data structures.
    /// </summary>
    public class TubularGridDataAdapter : IGridDataAdapter
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TubularGridDataAdapter{T}"/> class.
        /// </summary>
        /// <param name="dataItemType">The item type.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="client">The client.</param>
        public TubularGridDataAdapter(Type dataItemType, string requestUrl, HttpClient client)
        {
            Client = client;
            DataItemType = dataItemType;
            RequestUrl = requestUrl;
        }

        /// <summary>
        /// Gets the HTTP Client.
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// Gets the data item type.
        /// </summary>
        public Type DataItemType { get; }

        public string RequestUrl { get; }

        public async Task<GridDataResponse> RetrieveDataAsync(GridDataRequest request)
        {
            using var httpResponse = await RetrieveResponseAsync(RequestUrl, request);
            return await ProcessResponseAsync(httpResponse);
        }

        protected virtual HttpContent SerializeRequest(GridDataRequest request) => request.ToJsonContent();

        protected virtual async Task<HttpResponseMessage> RetrieveResponseAsync(string requestUrl, GridDataRequest request)
        {
            var requestContent = SerializeRequest(request);
            return await Client.PostAsync(requestUrl, requestContent);
        }

        protected virtual async Task<GridDataResponse> ProcessResponseAsync(HttpResponseMessage httpResponse)
        {
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = responseJson.DeserializeJson<TubularGridDataResponse>();
            return CreateGridDataResponse(response);
        }

        private GridDataResponse CreateGridDataResponse(TubularGridDataResponse tubularResponse)
        {
            return new GridDataResponse
            {
                CurrentPage = tubularResponse.CurrentPage,
                DataItems = ParsePayload(tubularResponse),
                AggregateDataItem = ParseAggregateItem(tubularResponse),
                DataItemType = DataItemType,
                FilteredRecordCount = tubularResponse.FilteredRecordCount,
                TotalPages = tubularResponse.TotalPages,
                TotalRecordCount = tubularResponse.TotalRecordCount
            };
        }

        private object ParseAggregateItem(TubularGridDataResponse response)
        {
            var payload = response.AggregationPayload;

            if (payload == null || payload.Count <= 0)
                return null;

            var result = Activator.CreateInstance(DataItemType);
            var proxies = DataItemType.PropertyProxies();

            foreach (var kvp in payload)
            {
                if (kvp.Value is not JsonElement value)
                    continue;

                var property = proxies.ContainsKey(kvp.Key) ? proxies[kvp.Key] : null;
                if (property == null)
                    continue;

                try
                {
                    property.SetValue(result, ParseValue(value, property.PropertyType));
                }
                catch
                {
                    // ignore
                }
            }

            return result;
        }

        private ICollection<object> ParsePayload(TubularGridDataResponse response)
        {
            var result = new List<object>();
            var props = DataItemType.PropertyProxies().Values.Where(t => t.IsFlatType);

            foreach (var itemData in response.Payload)
            {
                var valueIndex = 0;
                var targetItem = Activator.CreateInstance(DataItemType);

                foreach (var targetProperty in props)
                {
                    var v = itemData[valueIndex++];
                    var nullableType = Nullable.GetUnderlyingType(targetProperty.PropertyType);
                    var isNullable = nullableType != null;

                    try
                    {
                        if (v is JsonElement element)
                            v = ParseValue(element, targetProperty.PropertyType);
                        else if (v != null && v.GetType() != (isNullable ? nullableType : targetProperty.PropertyType))
                            v = Convert.ChangeType(v?.ToString(), isNullable ? nullableType : targetProperty.PropertyType);

                        targetProperty.SetValue(targetItem, v);
                    }
                    catch
                    {
                        // ignore
                    }
                }

                result.Add(targetItem);
            }

            return result;
        }

        private static object ParseValue(JsonElement jsonEl, Type type)
        {
            var valueKind = jsonEl.ValueKind;
            var targetType = type;
            var isNullable = false;

            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                targetType = nullableType;
                isNullable = true;
            }

            if (targetType.IsEnum)
                targetType = Enum.GetUnderlyingType(targetType);

            switch (valueKind)
            {
                case JsonValueKind.Array:
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                    return isNullable ? null : targetType.GetDefault();
                case JsonValueKind.False:
                case JsonValueKind.True:
                    return jsonEl.GetBoolean();
                case JsonValueKind.String:
                    return targetType == typeof(string)
                        ? jsonEl.GetString()
                        : targetType == typeof(DateTime)
                        ? jsonEl.GetDateTime()
                        : targetType == typeof(Guid)
                        ? jsonEl.GetGuid()
                        : targetType.GetDefault();
                case JsonValueKind.Number:
                    return targetType == typeof(byte)
                        ? jsonEl.GetByte()
                        : targetType == typeof(decimal)
                        ? jsonEl.GetDecimal()
                        : targetType == typeof(double)
                        ? jsonEl.GetDouble()
                        : targetType == typeof(short)
                        ? jsonEl.GetInt16()
                        : targetType == typeof(int)
                        ? jsonEl.GetInt32()
                        : targetType == typeof(long)
                        ? jsonEl.GetInt64()
                        : targetType == typeof(sbyte)
                        ? jsonEl.GetSByte()
                        : targetType == typeof(float)
                        ? jsonEl.GetSingle()
                        : targetType == typeof(ushort)
                        ? jsonEl.GetUInt16()
                        : targetType == typeof(uint)
                        ? jsonEl.GetUInt32()
                        : targetType == typeof(ulong)
                        ? jsonEl.GetUInt64()
                        : targetType.GetDefault();
                default:
                    return targetType.GetDefault();
            }

        }
    }

    public class TubularGridDataAdapter<T> : TubularGridDataAdapter
    {
        public TubularGridDataAdapter(string requestUrl, HttpClient client)
            : base(typeof(T), requestUrl, client)
        {
        }
    }
}
