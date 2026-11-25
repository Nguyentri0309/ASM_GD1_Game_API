using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using GameClient_WinForms.Models;

namespace GameClient_WinForms.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService()
    {
        string baseUrl = "https://localhost:7272/";
        try
        {
            string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (File.Exists(path))
            {
                using var doc = JsonDocument.Parse(File.ReadAllText(path));
                if (doc.RootElement.TryGetProperty("ApiBaseUrl", out var url) &&
                    Uri.TryCreate(url.GetString(), UriKind.Absolute, out var uri))
                {
                    baseUrl = uri.ToString();
                }
            }
        }
        catch
        {
            // Giữ URL mặc định khi file config hỏng
        }

        _http = new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(10) };
    }

    public string BaseUrl => _http.BaseAddress?.ToString() ?? string.Empty;

    private HttpRequestMessage CreateRequest(HttpMethod method, string url, object? body, bool authorize)
    {
        var request = new HttpRequestMessage(method, url);
        if (body != null)
        {
            request.Content = JsonContent.Create(body);
        }
        if (authorize && Session.Token != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token);
        }
        return request;
    }

    private static string ReadMessage(string body, string fallback)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("message", out var msg))
            {
                return msg.GetString() ?? fallback;
            }
        }
        catch
        {
            // JSON khong hop le: dung thong bao mac dinh
        }
        return fallback;
    }

    private static async Task<(bool Ok, string Error, T? Data)> SendAsync<T>(
        ApiService api, HttpMethod method, string url, object? body, bool authorize,
        CancellationToken ct = default) where T : class
    {
        try
        {
            using var request = api.CreateRequest(method, url, body, authorize);
            using var response = await api._http.SendAsync(request, ct);
            string responseBody = await response.Content.ReadAsStringAsync(ct);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return (true, string.Empty, data);
                }
                catch
                {
                    return (false, "Phản hồi từ server không đúng định dạng.", null);
                }
            }

            string error = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => ReadMessage(responseBody, "Dữ liệu không hợp lệ (400)."),
                HttpStatusCode.Unauthorized => "Token hết hạn hoặc không có quyền truy cập (401).",
                HttpStatusCode.NotFound => ReadMessage(responseBody, "Không tìm thấy nhân vật hoặc không thuộc tài khoản này (404)."),
                HttpStatusCode.Conflict => ReadMessage(responseBody, "Tên đăng nhập đã tồn tại (409)."),
                HttpStatusCode.InternalServerError => "Lỗi máy chủ (500).",
                _ => $"Lỗi từ server: {(int)response.StatusCode}."
            };
            return (false, error, null);
        }
        catch (TaskCanceledException)
        {
            return (false, "Hết thời gian chờ kết nối API. Kiểm tra server và thử lại.", null);
        }
        catch (HttpRequestException)
        {
            return (false, "Không kết nối được API. Kiểm tra server và kết nối mạng.", null);
        }
        catch (JsonException)
        {
            return (false, "Phản hồi từ server không đúng định dạng.", null);
        }
    }

    public Task<(bool Ok, string Error, AccountResponse? Data)> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        => SendAsync<AccountResponse>(this, HttpMethod.Post, "api/auth/register", request, false, ct);

    public async Task<(bool Ok, string Error, LoginResponse? Data)> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var result = await SendAsync<LoginResponse>(this, HttpMethod.Post, "api/auth/login", request, false, ct);
        if (result.Ok && result.Data != null)
        {
            Session.SetLogin(result.Data.Token, result.Data.Username);
        }
        return result;
    }

    public Task<(bool Ok, string Error, CharacterResponse? Data)> CreateCharacterAsync(CreateCharacterRequest request, CancellationToken ct = default)
        => SendAsync<CharacterResponse>(this, HttpMethod.Post, "api/characters", request, true, ct);

    public Task<(bool Ok, string Error, CharacterResponse? Data)> UpdateCharacterAsync(int id, UpdateCharacterRequest request, CancellationToken ct = default)
        => SendAsync<CharacterResponse>(this, HttpMethod.Put, $"api/characters/{id}", request, true, ct);

    public async Task<(bool Ok, string Error, List<CharacterResponse>? Data)> GetMyCharactersAsync(CancellationToken ct = default)
    {
        var result = await SendAsync<List<CharacterResponse>>(this, HttpMethod.Get, "api/characters/mine", null, true, ct);
        return result;
    }
}
