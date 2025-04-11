using Delab.Front.Pages.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Delab.Front.Shared;

public partial class AuthLinks
{
    [Inject] private NavigationManager _navigation { get; set; } = null!;
    [Inject] private IDialogService _dialogService { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    private string? photoUser;
    private string? LogoCorp;
    private string? NameCorp;

    protected override async Task OnParametersSetAsync()
    {
        var authenticationState = await AuthenticationStateTask;
        var claims = authenticationState.User.Claims.ToList();
        var photoClaim = claims.FirstOrDefault(x => x.Type == "Photo");
        var LogoCorpClaim = claims.FirstOrDefault(x => x.Type == "LogoCorp");
        var NameCorpClaim = claims.FirstOrDefault(x => x.Type == "CorpName");
        if (photoClaim is not null)
        {
            photoUser = photoClaim.Value;
        }
        if (LogoCorpClaim is not null)
        {
            LogoCorp = LogoCorpClaim.Value;
        }
        if (NameCorpClaim is not null)
        {
            NameCorp = NameCorpClaim.Value;
        }
    }

    private async Task ShowModalLogIn()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        await _dialogService.ShowAsync<Login>("Iniciar Sesion", closeOnEscapeKey);
    }

    private async Task ShowModalLogOut()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        await _dialogService.ShowAsync<Logout>("Abandonar Sesion", closeOnEscapeKey);
    }

    private async Task ShowModalRecoverPassword()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraLarge };
        await _dialogService.ShowAsync<RecoverPassword>("Recuperar su Clave", closeOnEscapeKey);
    }

    private async Task ShowModalCambiarClave()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraLarge };
        await _dialogService.ShowAsync<ChangePassword>("Cambiar Clave", closeOnEscapeKey);
    }
}