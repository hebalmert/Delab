using Delab.Shared.EntitesSoftSec;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Delab.Front.Pages.EntitesSoftSecView;

public partial class FormUsuario
{
    private string? ImageUrl;

    [Parameter, EditorRequired] public Usuario Usuario { get; set; } = null!;
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }

    protected override void OnInitialized()
    {
        if (IsEditControl)
        {
            ImageUrl = Usuario.ImageFullPath;
        }
    }

    private void ImageSelected(string imagenBase64)
    {
        Usuario.ImgBase64 = imagenBase64;
        ImageUrl = null;
    }

    private string GetDisplayName<T>(Expression<Func<T>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            var property = memberExpression.Member as PropertyInfo;
            if (property != null)
            {
                var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null)
                {
                    return displayAttribute.Name!;
                }
            }
        }
        return "Texto no definido";
    }
}