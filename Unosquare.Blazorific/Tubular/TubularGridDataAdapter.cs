namespace Unosquare.Blazorific.Tubular
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;

    public class TubularGridDataAdapter : IGridDataAdapter
    {
        protected static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };

        public TubularGridDataAdapter(Type dataItemType, string requestUrl)
        {
            DataItemType = dataItemType;
            RequestUrl = requestUrl;
        }

        public Type DataItemType { get; }

        public string RequestUrl { get; }

        public async Task<GridDataResponse> RetrieveDataAsync(GridDataRequest request)
        {
            using var httpResponse = await RetrieveResponseAsync(RequestUrl, request);
            return await ProcessResponseAsync(httpResponse);
        }

        protected virtual HttpContent SerializeRequest(GridDataRequest request) =>
            new StringContent(JsonSerializer.Serialize(request, JsonOptions), Encoding.UTF8, "application/json");

        protected virtual async Task<HttpResponseMessage> RetrieveResponseAsync(string requestUrl, GridDataRequest request)
        {
            var requestContent = SerializeRequest(request);
            using var client = new HttpClient();
            return await client.PostAsync(requestUrl, requestContent);
        }

        protected virtual async Task<GridDataResponse> ProcessResponseAsync(HttpResponseMessage httpResponse)
        {
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<TubularGridDataResponse>(responseJson, JsonOptions);
            return CreateGridDataResponse(response);
        }

        private GridDataResponse CreateGridDataResponse(TubularGridDataResponse tubularResponse)
        {
            return new GridDataResponse
            {
                CurrentPage = tubularResponse.CurrentPage,
                DataItems = ParsePayload(tubularResponse),
                DataItemType = DataItemType,
                FilteredRecordCount = tubularResponse.FilteredRecordCount,
                TotalPages = tubularResponse.TotalPages,
                TotalRecordCount = tubularResponse.TotalRecordCount
            };
        }

        private IReadOnlyList<object> ParsePayload(TubularGridDataResponse response)
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
                        if (v is JsonElement)
                            v = ParseValue((JsonElement)v, targetProperty.PropertyType);
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
        public TubularGridDataAdapter(string requestUrl)
            : base(typeof(T), requestUrl)
        {
        }
    }
}
