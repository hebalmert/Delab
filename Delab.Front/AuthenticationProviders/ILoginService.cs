namespace Delab.Front.AuthenticationProviders;

public interface ILoginService
{
    Task LoginAsync(string token);

    Task LogoutAsync();
}