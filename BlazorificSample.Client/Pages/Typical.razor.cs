﻿namespace BlazorificSample.Client.Pages;

using BlazorificSample.Shared;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using Unosquare.Blazorific;
using Unosquare.Blazorific.Common;

public partial class Typical
{
    private CandyModal TestDialog { get; set; }
    
    private CandyModal EditDialog { get; set; }

    private Product EditorItem { get; set; }

    private static void ToggleCheckedForAll(CandyGrid sender)
    {
        foreach (var row in sender.Rows)
        {
            if (row.DataItem is null)
                continue;

            if (row.DataItem is not Product product)
                return;

            product.IsChecked = !product.IsChecked;
            UpdateSelected(row);
        }
    }

    private static void OnBodyRowDoubleClick(GridRowMouseEventArgs e)
    {
        if (e.DataItem is not Product product)
            return;

        product.IsChecked = !product.IsChecked;
        product.Description = $"Item was {product.ProductId} double clicked!";
        UpdateSelected(e.Row);
    }

    private static void OnDetailsButtonClick(GridRowMouseEventArgs e)
    {
        if (e.DataItem is not Product product)
            return;

        product.Description = $"Details for item {product.ProductId} clicked. Show a modal or something!";
    }

    private async void OnEditButtonClick(GridRowMouseEventArgs e)
    {
        if (e.DataItem is not Product product)
            return;

        product.Description = $"Product with ID = {product.ProductId} editing . . .";
        EditorItem = product;
        await EditDialog.Show();
    }

    private async Task OnModalButtonClick(MouseEventArgs e)
    {
        await TestDialog.Show();
    }

    private static void OnCellCheckChanged(GridCellCheckboxEventArgs e)
    {
        if (e.DataItem is not Product product)
            return;
        
        product.Description = $"Product with ID = {product.ProductId}, on column {e.Column.Field} is now {(e.IsChecked ? "checked" : "unchecked")}.";
        UpdateSelected(e.Row);
    }

    private static void UpdateSelected(CandyGridRow row)
    {
        if (row.DataItem is not Product product)
            return;

        row.Attributes.CssClass = product.IsChecked ? "table-active" : null;
        row.Cells[1].Attributes.Style = product.IsChecked ? "font-weight: bold" : "font-weight: normal";
    }
}
