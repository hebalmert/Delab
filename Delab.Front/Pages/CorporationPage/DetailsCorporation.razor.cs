using CurrieTechnologies.Razor.SweetAlert2;
using Delab.AccessService.Repositories;
using Delab.Front.Helpers;
using Delab.Shared.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Delab.Front.Pages.CorporationPage;

public partial class DetailsCorporation
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Corporation? Corporation;
    private SoftPlan? SoftPlan;
    private Country? Country;

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadCorporation();
        await LoadSoftPlans();
        await LoadCountry();
    }

    private async Task LoadCorporation()
    {
        var responseHTTP = await _repository.GetAsync<Corporation>($"/api/corporations/{Id}");
        bool errorHandler = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandler)
        {
            _navigationManager.NavigateTo($"/corporations");
            return;
        }
        Corporation = responseHTTP.Response;
    }

    private async Task LoadSoftPlans()
    {
        var responseHTTP = await _repository.GetAsync<SoftPlan>($"/api/softplans/{Corporation!.SoftPlanId}");
        bool errorHandler = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandler)
        {
            _navigationManager.NavigateTo($"/corporations");
            return;
        }
        SoftPlan = responseHTTP.Response;
    }

    private async Task LoadCountry()
    {
        var responseHTTP = await _repository.GetAsync<Country>($"/api/countries/{Corporation!.CountryId}");
        bool errorHandler = await _responseHandler.HandleErrorAsync(responseHTTP);
        if (errorHandler)
        {
            _navigationManager.NavigateTo($"/corporations");
            return;
        }
        Country = responseHTTP.Response;
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