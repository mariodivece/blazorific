namespace Unosquare.Blazorific.Common;

/// <summary>
/// Defines a data adapter for a <see cref="CandyGrid"/>.
/// </summary>
public interface IGridDataAdapter
{
    /// <summary>
    /// Gets the type of the data item.
    /// </summary>
    /// <value>
    /// The type of the data item.
    /// </value>
    Type DataItemType { get; }

    /// <summary>
    /// Defines a method that is used to retrieve the data.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    Task<GridDataResponse> RetrieveDataAsync(GridDataRequest request);
}
