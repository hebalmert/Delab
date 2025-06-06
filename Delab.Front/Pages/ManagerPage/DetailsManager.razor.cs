using CurrieTechnologies.Razor.SweetAlert2;
using Delab.AccessService.Repositories;
using Delab.Front.Helpers;
using Delab.Shared.Entities;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Delab.Front.Pages.ManagerPage;

public partial class DetailsManager
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Manager? Manager;

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadManager();
    }

    private async Task LoadManager()
    {
        var responseHTTP = await _repository.GetAsync<Manager>($"/api/managers/{Id}");
        bool errorHandler = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandler)
        {
            _navigationManager.NavigateTo($"/corporations");
            return;
        }
        Manager = responseHTTP.Response;
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