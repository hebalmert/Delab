using CurrieTechnologies.Razor.SweetAlert2;
using Delab.AccessService.Repositories;
using Delab.Front.Helpers;
using Delab.Shared.EntitesSoftSec;
using Microsoft.AspNetCore.Components;

namespace Delab.Front.Pages.EntitesSoftSecView;

public partial class CreateUsuario
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private Usuario Usuario = new();

    private string BaseUrl = "/api/usuarios";
    private string BaseView = "/usuarios";

    private async Task Create()
    {
        var responseHttp = await _repository.PostAsync($"{BaseUrl}", Usuario);
        bool errorHandler = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandler)
        {
            _navigationManager.NavigateTo($"{BaseView}");
            return;
        }
        _navigationManager.NavigateTo($"{BaseView}");
    }

    private void Return()
    {
        _navigationManager.NavigateTo($"{BaseView}");
    }
}