using CurrieTechnologies.Razor.SweetAlert2;
using Delab.AccessService.Repositories;
using Delab.Front.Helpers;
using Delab.Shared.EntitesSoftSec;
using Microsoft.AspNetCore.Components;

namespace Delab.Front.Pages.EntitesSoftSecView;

public partial class CreateUsuarioRole
{
    [Inject] private IRepository _repository { get; set; } = null!;
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService _sweetAlert { get; set; } = null!;
    [Inject] private HttpResponseHandler _responseHandler { get; set; } = null!;

    private UsuarioRole UsuarioRole = new();

    private string BaseUrl = "/api/usuarioRoles";
    private string BaseView = "/usuarios/details";

    [Parameter] public int Id { get; set; }  //UsuarioId

    private async Task Create()
    {
        UsuarioRole.UsuarioId = Id;
        var responseHttp = await _repository.PostAsync($"{BaseUrl}", UsuarioRole);
        bool errorHandler = await _responseHandler.HandleErrorAsync(responseHttp);
        if (errorHandler)
        {
            _navigationManager.NavigateTo($"/usuarios");
            return;
        }
        _navigationManager.NavigateTo($"{BaseView}/{Id}");
    }

    private void Return()
    {
        _navigationManager.NavigateTo($"{BaseView}/{Id}");
    }
}