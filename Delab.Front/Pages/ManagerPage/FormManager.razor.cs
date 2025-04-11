using CurrieTechnologies.Razor.SweetAlert2;
using Delab.AccessService.Repositories;
using Delab.Front.Helpers;
using Delab.Shared.Entities;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Delab.Front.Pages.ManagerPage;

public partial class FormManager
{
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Corporation? SelectedCorporation;
    private List<Corporation>? Corporations;
    private string? ImageUrl;
    private string BaseView = "/managers";
    private string BaseComboCompany = "/api/corporations/loadCombo";

    [Parameter, EditorRequired] public Manager Manager { get; set; } = null!;
    [Parameter, EditorRequired] public EventCallback OnSubmit { get; set; }
    [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }
    [Parameter, EditorRequired] public bool IsEditControl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadCorporation();
        if (IsEditControl)
        {
            ImageUrl = Manager.ImageFullPath;
        }
    }

    private async Task LoadCorporation()
    {
        var responseHttp = await _repository.GetAsync<List<Corporation>>($"{BaseComboCompany}");
        bool errorHandler = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandler)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        Corporations = responseHttp.Response;
        if (IsEditControl)
        {
            SelectedCorporation = Corporations!.Where(x => x.CorporationId == Manager.CorporationId)
                .Select(x => new Corporation { CorporationId = x.CorporationId, Name = x.Name })
                .FirstOrDefault();
        }
    }

    private void CorporationChanged(Corporation modelo)
    {
        Manager.CorporationId = modelo.CorporationId;
        SelectedCorporation = modelo;
    }

    private void ImageSelected(string imagenBase64)
    {
        Manager.ImgBase64 = imagenBase64;
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