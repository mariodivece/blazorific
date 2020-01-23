namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;

    public partial class CandyGridPaginator
    {
        private int m_ButtonCount = 5;

        [Parameter]
        public bool ShowFirstButton { get; set; } = true;

        [Parameter]
        public bool ShowLastButton { get; set; } = true;

        [Parameter]
        public bool ShowPreviousButton { get; set; } = true;

        [Parameter]
        public bool ShowNextButton { get; set; } = true;

        [Parameter]
        public int ButtonCount
        {
            get => m_ButtonCount < 1 ? 1 : m_ButtonCount;
            set => m_ButtonCount = value;
        }

        public int PageNumber
        {
            get => Parent.PageNumber;
            set => Parent.ChangePageNumber(value);
        }
    }
}
