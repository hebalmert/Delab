using CurrieTechnologies.Razor.SweetAlert2;
using Delab.AccessService.Repositories;
using Delab.Front.Helpers;
using Delab.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Delab.Front.Pages.CorporationPage;

public partial class FormCorporation
{
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Country? SelectedCountry;
    private List<Country>? Countries;
    private SoftPlan? SelectedSoftplan;
    private SoftPlan? SoftplanDays;
    private List<SoftPlan>? SoftPlans;
    private DateTime? DateMin = new DateTime(2025, 1, 1);
    private string? ImageUrl;
    private string BaseView = "/corporations";
    private string BaseComboSoftplan = "/api/softplans/loadCombo";
    private string BaseComboCountry = "/api/countries/loadCombo";

    [Parameter, EditorRequired] public Corporation Corporation { get; set; } = null!;
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadSoftPlan();
        await LoadCountry();
        if (IsEditControl)
        {
            ImageUrl = Corporation.ImageFullPath;
        }
        else
        {
            Corporation.DateStart = DateTime.Now;
            Corporation.DateEnd = DateTime.Now;
        }
    }

    private async Task LoadSoftPlan()
    {
        var responseHttp = await _repository.GetAsync<List<SoftPlan>>($"{BaseComboSoftplan}");
        bool errorHandler = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandler)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        SoftPlans = responseHttp.Response;
        if (IsEditControl)
        {
            SelectedSoftplan = SoftPlans!.Where(x => x.SoftPlanId == Corporation.SoftPlanId)
                .Select(x => new SoftPlan { SoftPlanId = x.SoftPlanId, Name = x.Name })
                .FirstOrDefault();
            SoftplanDays = SoftPlans!.FirstOrDefault(x => x.SoftPlanId == Corporation.SoftPlanId);
        }
    }

    private async Task LoadCountry()
    {
        var responseHttp = await _repository.GetAsync<List<Country>>($"{BaseComboCountry}");
        bool errorHandler = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandler)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        Countries = responseHttp.Response;
        if (IsEditControl)
        {
            SelectedCountry = Countries!.Where(x => x.CountryId == Corporation.CountryId)
                .Select(x => new Country { CountryId = x.CountryId, Name = x.Name })
                .FirstOrDefault();
        }
    }

    private void ImageSelected(string imagenBase64)
    {
        Corporation.ImgBase64 = imagenBase64;
        ImageUrl = null;
    }

    private void CountryChanged(Country modelo)
    {
        Corporation.CountryId = modelo.CountryId;
        SelectedCountry = modelo;
    }

    private void SoftPlanChanged(SoftPlan modelo)
    {
        Corporation.SoftPlanId = modelo.SoftPlanId;
        SelectedSoftplan = modelo;

        SoftplanDays = SoftPlans!.FirstOrDefault(x => x.SoftPlanId == modelo.SoftPlanId);
        Corporation.DateStart = Convert.ToDateTime(DateTime.Now);
        DateTime nuevaFecha = Corporation.DateStart.AddMonths(SoftplanDays!.Meses);
        var ndate = nuevaFecha.ToString("yyyy-MM-dd");
        Corporation.DateEnd = Convert.ToDateTime(ndate);
    }

    private void DateInicioChanged(DateTime? newDate)
    {
        if (SoftplanDays == null) return;

        Corporation.DateStart = Convert.ToDateTime(newDate);
        DateTime nuevafecha = Corporation.DateStart.AddMonths(SoftplanDays!.Meses);
        var ndate = nuevafecha.ToString("yyyy-MM-dd");
        Corporation.DateEnd = Convert.ToDateTime(ndate);
    }

    private void DateFinalChanged(DateTime? newDate)
    {
        Corporation.DateEnd = Convert.ToDateTime(newDate);
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