namespace GameClient_WinForms.Services;

// Token va ten dang nhap chi ton trong bo nho ung dung
public static class Session
{
    public static string? Token { get; private set; }
    public static string Username { get; private set; } = string.Empty;

    public static bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    public static void SetLogin(string token, string username)
    {
        Token = token;
        Username = username;
    }

    public static void Clear()
    {
        Token = null;
        Username = string.Empty;
    }
}
