namespace Unosquare.Blazorific.Tubular
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;
    using Microsoft.AspNetCore.Components;
    using System.Collections;
    using System.Text.Json;

    public class TubularGridDataAdapter : IGridDataAdapter
    {
        public TubularGridDataAdapter(Type dataItemType, string requestUrl)
        {
            DataItemType = dataItemType;
            RequestUrl = requestUrl;
        }

        public Type DataItemType { get; }

        public string RequestUrl { get; }

        public async Task<GridDataResponse> RetrieveDataAsync(GridDataRequest request)
        {
            using var client = new HttpClient();
            var response = await client.PostJsonAsync<TubularGridDataResponse>(RequestUrl, request);
            return CreateGridDataResponse(response);
        }

        private GridDataResponse CreateGridDataResponse(TubularGridDataResponse response)
        {
            return new GridDataResponse
            {
                CurrentPage = response.CurrentPage,
                DataItems = ParsePayload(response),
                DataItemType = DataItemType,
                FilteredRecordCount = response.FilteredRecordCount,
                TotalPages = response.TotalPages,
                TotalRecordCount = response.TotalRecordCount
            };
        }

        private IList ParsePayload(TubularGridDataResponse response)
        {
            var result = DataItemType.CreateGenericList();
            var props = DataItemType.GetPropertyProxies();

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

            {
                var nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null)
                {
                    targetType = nullableType;
                    isNullable = true;
                }
            }

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
