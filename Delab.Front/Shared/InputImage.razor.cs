using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Delab.Front.Shared;

public partial class InputImage
{
    private string? ImageBase64;
    private string? FileName;

    [Parameter] public string? Label { get; set; }
    [Parameter] public string? ImageUrl { get; set; }
    [Parameter] public EventCallback<string> ImageSelected { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrWhiteSpace(Label))
        {
            Label = "Imagen";
        }
    }

    private async Task OnChange(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file != null)
        {
            FileName = file.Name;

            var arrBytes = new byte[file.Size];
            await file.OpenReadStream().ReadAsync(arrBytes);
            ImageBase64 = Convert.ToBase64String(arrBytes);
            ImageUrl = null;
            await ImageSelected.InvokeAsync(ImageBase64);
            StateHasChanged();
        }
    }
}