namespace Unosquare.Blazorific.Tubular
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.Json;
    using Unosquare.Blazorific.Common;

    public static class TubularExtensions
    {
        private static readonly object SyncLock = new object();
        private static int GridDateRequestIndex;

        public static TubularGridDataRequest CreateTubularGridDataRequest(this CandyGrid grid, Type dataItemType)
        {
            lock (SyncLock)
            {
                var request = new TubularGridDataRequest
                {
                    Counter = GridDateRequestIndex++,
                    Skip = grid.PageIndex * grid.PageSize,
                    Take = grid.PageSize,
                    TimezoneOffset = (int)Math.Round(DateTime.UtcNow.Subtract(DateTime.Now).TotalMinutes, 0),
                    Search = new TubularDataFilter(),
                    Columns = dataItemType.GetRequestColumns()
                };

                return request;
            }
        }

        public static void ApplyGridDataResponse(this CandyGrid grid, Type dataItemType, TubularGridDataResponse response)
        {
            var result = response.ParsePayload(dataItemType);
            grid.UpdateData(result);
        }

        public static DataType ToTubularDataType(this Type t)
        {
            var type = Nullable.GetUnderlyingType(t) ?? t;

            if (type.IsValueType)
            {
                var value = Activator.CreateInstance(type);
                if (type == typeof(bool)) return DataType.Boolean;
                if (type == typeof(DateTime)) return DataType.DateTime;
                if (type.IsEnum || value.IsNumeric()) return DataType.Numeric;
            }

            return DataType.String;
        }

        private static bool IsNumeric(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        private static object GetDefault(this Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;

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
                    return isNullable ? null : GetDefault(targetType);
                case JsonValueKind.False:
                    return jsonEl.GetBoolean();
                case JsonValueKind.String:
                    return targetType == typeof(string)
                        ? jsonEl.GetString()
                        : targetType == typeof(DateTime)
                        ? jsonEl.GetDateTime()
                        : targetType == typeof(Guid)
                        ? jsonEl.GetGuid()
                        : GetDefault(targetType);
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
                        : GetDefault(targetType);
                default:
                    return GetDefault(targetType);
            }

        }

        private static IList CreateGenericList(this Type t)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(t);
            return Activator.CreateInstance(constructedListType) as IList;
        }

        private static TubularColumn[] GetRequestColumns(this Type t)
        {
            var properties = t.GetPropertyProxies();
            var result = new List<TubularColumn>(properties.Length);

            foreach (var property in properties)
            {
                result.Add(new TubularColumn
                {
                    DataType = property.PropertyType.ToTubularDataType(),
                    Name = property.Name,
                });
            }

            return result.ToArray();
        }

        private static IList ParsePayload(this TubularGridDataResponse response, Type t)
        {
            var result = t.CreateGenericList();
            var props = t.GetPropertyProxies();

            foreach (var itemData in response.Payload)
            {
                var valueIndex = 0;
                var targetItem = Activator.CreateInstance(t);
                
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
    }
}
