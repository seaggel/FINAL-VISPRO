namespace CrossPlatformApp.Services;

public sealed record AuthenticatedUser(string Username, string Role, int? MahasiswaId = null, int? KonselorId = null);

public static class AuthSession
{
    public static AuthenticatedUser? CurrentUser { get; private set; }

    public static bool IsAuthenticated => CurrentUser != null;

    public static void SignIn(AuthenticatedUser user)
    {
        CurrentUser = user;
    }

    public static void SignOut()
    {
        CurrentUser = null;
    }
}
