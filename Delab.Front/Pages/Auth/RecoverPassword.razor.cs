using Delab.AccessService.Repositories;
using Delab.Shared.ResponsesSec;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Delab.Front.Pages.Auth;

public partial class RecoverPassword
{
    [Inject] private NavigationManager _navigationManager { get; set; } = null!;
    [Inject] private ISnackbar _snackbar { get; set; } = null!;
    [Inject] private IRepository _repository { get; set; } = null!;

    private EmailDTO EmailDTO = new();

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    private async Task SendRecoverPasswordEmailTokenAsync()
    {
        var responseHttp = await _repository.PostAsync("/api/accounts/RecoverPassword", EmailDTO);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            _snackbar.Add("Error en la Recuperacion de la Clave", Severity.Error);
            return;
        }

        MudDialog.Cancel();
        _navigationManager.NavigateTo("/");
        _snackbar.Add("Se ha enviado un Correo con el Cambio de Clave", Severity.Success);
    }

    private void CloseModal()
    {
        MudDialog.Cancel();
    }
}