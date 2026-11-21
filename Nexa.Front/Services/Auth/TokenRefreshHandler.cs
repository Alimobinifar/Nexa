using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Http.Headers;

namespace Nexa.Front.Services.Auth
{
    public class TokenRefreshHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly AuthApiClient _auth;

        public TokenRefreshHandler(IHttpContextAccessor accessor, AuthApiClient auth)
        {
            _accessor = accessor;
            _auth = auth;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            var ctx = _accessor.HttpContext!;
            var token = await ctx.GetTokenAsync("access_token");

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, ct);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            // Access منقضی شده: رفرش
            var refresh = await ctx.GetTokenAsync("refresh_token");
            if (string.IsNullOrEmpty(refresh)) return response;

            var fresh = await _auth.RefreshAsync(refresh);
            if (fresh is null) return response;

            // توکن‌های جدید را در کوکی جایگزین کن
            var result = await ctx.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                result.Properties!.UpdateTokenValue("access_token", fresh.AccessToken);
                result.Properties.UpdateTokenValue("refresh_token", fresh.RefreshToken);
                try
                {
                    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        result.Principal!, result.Properties);
                }
                catch (InvalidOperationException) { /* پاسخ شروع شده؛ کوکی در درخواست بعد به‌روز می‌شود */ }
            }

            var retry = await CloneAsync(request);
            retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fresh.AccessToken);
            response.Dispose();
            return await base.SendAsync(retry, ct);
        }
        private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri);
            if (req.Content != null)
            {
                var bytes = await req.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(bytes);
                foreach (var h in req.Content.Headers)
                    clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }
            foreach (var h in req.Headers)
                clone.Headers.TryAddWithoutValidation(h.Key, h.Value);
            return clone;
        }
    }
}