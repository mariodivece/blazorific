namespace Unosquare.Blazorific.Common
{
    /// <summary>
    /// Represents a Tubular's filter (by column).
    /// 
    /// This object is only used to be serialized/deserialized between
    /// the API and Tubular.
    /// </summary>
    public class GridDataFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Filter"/> class.
        /// </summary>
        public GridDataFilter()
        {
            Operator = CompareOperators.None;
            Text = string.Empty;
        }

        /// <summary>
        /// Filter search text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Filter search params.
        /// </summary>
        public string[] Argument { get; set; }

        /// <summary>
        /// Filter's operator.
        /// </summary>
        public CompareOperators Operator { get; set; }
    }
}
