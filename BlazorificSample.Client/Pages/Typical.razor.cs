namespace BlazorificSample.Client.Pages
{
    using BlazorificSample.Shared;
    using Microsoft.AspNetCore.Components.Web;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unosquare.Blazorific;
    using Unosquare.Blazorific.Common;

    public partial class Typical
    {
        private CandyModal TestDialog { get; set; }
        
        private CandyModal EditDialog { get; set; }

        private Product EditorItem { get; set; }

        private void CheckAll(CandyGrid sender)
        {
            foreach (var row in sender.Rows)
            {
                UpdateSelected(row);
            }
        }

        private void OnBodyRowDoubleClick(GridRowMouseEventArgs e)
        {
            var product = e.DataItem as Product;
            if (product == null) return;

            product.IsChecked = !product.IsChecked;
            product.Description = $"Item was {product.ProductId} double clicked!";
            UpdateSelected(e.Row);
        }

        private void OnDetailsButtonClick(GridRowMouseEventArgs e)
        {
            var product = e.DataItem as Product;
            if (product == null) return;
            product.Description = $"Details for item {product.ProductId} clicked. Show a modal or something!";
        }

        private async void OnEditButtonClick(GridRowMouseEventArgs e)
        {
            var product = e.DataItem as Product;
            if (product == null) return;
            product.Description = $"Product with ID = {product.ProductId} editing . . .";
            EditorItem = product;
            await EditDialog.Show();
        }

        private async Task OnModalButtonClick(MouseEventArgs e)
        {
            await TestDialog.Show();
        }

        private void OnCellCheckChanged(GridCellCheckboxEventArgs e)
        {
            var product = e.DataItem as Product;
            if (product == null) return;
            product.Description = $"Product with ID = {product.ProductId}, on column {e.Column.Field} is now {(e.IsChecked ? "checked" : "unchecked")}.";
            UpdateSelected(e.Row);
        }

        private void UpdateSelected(CandyGridRow row)
        {
            var product = row.DataItem as Product;
            if (product == null) return;

            row.Attributes.CssClass = product.IsChecked ? "table-active" : null;
            row.Cells[1].Attributes.Style = product.IsChecked ? "font-weight: bold" : "font-weight: normal";
        }
    }
}
