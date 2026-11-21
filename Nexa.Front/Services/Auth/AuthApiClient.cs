using Contracts.Cmmon.Dtos.Auth;

namespace Nexa.Front.Services.Auth
{
    public class AuthApiClient
    {
        private readonly HttpClient _http;
        public AuthApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<AuthResponse?> LoginAsync(string username, string password)
        {
            var response = await _http.PostAsJsonAsync("auth/login", new  { username, password });
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<AuthResponse>() : null;
        }

        public async Task<AuthResponse?> RefreshAsync(string refreshToken)
        {
            var res = await _http.PostAsJsonAsync("api/Auth/Refresh", new { refreshToken });
            return res.IsSuccessStatusCode
                ? await res.Content.ReadFromJsonAsync<AuthResponse>()
                : null;
        }

        public async Task LogoutAsync(string refreshToken)
            => await _http.PostAsJsonAsync("api/Auth/Logout", new { refreshToken });
    }
}
