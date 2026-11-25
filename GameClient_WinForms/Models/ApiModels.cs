namespace GameClient_WinForms.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

public class AccountResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
}

public class CreateCharacterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
}

public class UpdateCharacterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Hp { get; set; }
}

public class CharacterResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Hp { get; set; }
    public int AccountId { get; set; }
}
