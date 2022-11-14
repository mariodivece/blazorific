namespace Unosquare.Blazorific.Tubular;

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

    /// <summary>
    /// Gets the request URL.
    /// </summary>
    /// <value>
    /// The request URL.
    /// </value>
    public string RequestUrl { get; }

    /// <summary>
    /// Retrieves the data asynchronously.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public async Task<GridDataResponse> RetrieveDataAsync(GridDataRequest request)
    {
        using var httpResponse = await RetrieveResponseAsync(RequestUrl, request);
        return await ProcessResponseAsync(httpResponse);
    }

    /// <summary>
    /// Serializes the request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    protected virtual HttpContent SerializeRequest(GridDataRequest request) => request.ToJsonContent();

    /// <summary>
    /// Retrieves the response asynchronous.
    /// </summary>
    /// <param name="requestUrl">The request URL.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    protected virtual async Task<HttpResponseMessage> RetrieveResponseAsync(string requestUrl, GridDataRequest request)
    {
        var requestContent = SerializeRequest(request);
        return await Client.PostAsync(requestUrl, requestContent);
    }

    /// <summary>
    /// Processes the response asynchronous.
    /// </summary>
    /// <param name="httpResponse">The HTTP response.</param>
    /// <returns></returns>
    protected virtual async Task<GridDataResponse> ProcessResponseAsync(HttpResponseMessage httpResponse)
    {
        var responseJson = await httpResponse.Content.ReadAsStringAsync();
        var response = responseJson.DeserializeJson<TubularGridDataResponse>();
        return CreateGridDataResponse(response);
    }

    /// <summary>
    /// Creates the grid data response.
    /// </summary>
    /// <param name="tubularResponse">The tubular response.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Parses the aggregate item.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <returns></returns>
    private object ParseAggregateItem(TubularGridDataResponse response)
    {
        var payload = response.AggregationPayload;

        if (payload is null || payload.Count <= 0)
            return null;

        var result = DataItemType.CreateInstance();
        var proxies = DataItemType.Properties();

        foreach (var kvp in payload)
        {
            if (kvp.Value is not JsonElement value)
                continue;

            var property = proxies.FirstOrDefault(p => p.PropertyName == kvp.Key);
            if (property is null)
                continue;

            property.TryWrite(result, ParseValue(value, property.PropertyType.BackingType.NativeType));
        }

        return result;
    }

    /// <summary>
    /// Parses the payload.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <returns></returns>
    private ICollection<object> ParsePayload(TubularGridDataResponse response)
    {
        var result = new List<object>();
        var props = DataItemType.Properties().Where(t => t.IsFlatType());

        foreach (var itemData in response.Payload)
        {
            var valueIndex = 0;
            var targetItem = DataItemType.CreateInstance();

            foreach (var targetProperty in props)
            {
                var v = itemData[valueIndex++];
                try
                {
                    if (v is JsonElement element)
                        v = ParseValue(element, targetProperty.PropertyType.BackingType.NativeType);

                    targetProperty.TryWrite(targetItem, v);
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

    /// <summary>
    /// Parses the value.
    /// </summary>
    /// <param name="jsonEl">The json el.</param>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    private static object? ParseValue(JsonElement jsonEl, Type type)
    {
        var valueKind = jsonEl.ValueKind;
        var targetType = type;
        var isNullable = false;

        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType is not null)
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
                var r = targetType == typeof(string)
                    ? jsonEl.GetString()
                    : targetType == typeof(DateTime)
                    ? jsonEl.GetDateTime()
                    : targetType == typeof(Guid)
                    ? jsonEl.GetGuid()
                    : targetType.GetDefault();
                return r;
            case JsonValueKind.Number:
                var r2 = targetType == typeof(byte)
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
                return r2;
            default:
                return targetType.GetDefault();
        }

    }
}

/// <summary>
/// PRovides a strongly-typed <see cref="TubularGridDataAdapter"/>.
/// </summary>
/// <typeparam name="T">The data item type.</typeparam>
/// <seealso cref="IGridDataAdapter" />
public class TubularGridDataAdapter<T> : TubularGridDataAdapter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TubularGridDataAdapter{T}"/> class.
    /// </summary>
    /// <param name="requestUrl">The request URL.</param>
    /// <param name="client">The client.</param>
    public TubularGridDataAdapter(string requestUrl, HttpClient client)
        : base(typeof(T), requestUrl, client)
    {
    }
}
