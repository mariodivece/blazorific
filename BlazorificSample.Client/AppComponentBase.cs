namespace BlazorificSample.Client
{
    using Microsoft.AspNetCore.Components;
    using Unosquare.Blazorific;

    public class AppComponentBase : CandyComponentBase
    {
        [Inject]
        protected ApplicationState AppState { get; set; }

        [Inject]
        protected CandyModalService Modal { get; set; }

        [Inject]
        protected DataAccessService DataAccess { get; set; }
    }
}
