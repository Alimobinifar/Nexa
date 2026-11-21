
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using Nexa.Front.Components;
using Nexa.Front.Services.Auth;
using Nexa.Front.Services.Products;
using Nexa.Front.Services.Products.Interfaces;

var builder = WebApplication.CreateBuilder(args);

#region JwtToken
var apiBase = new Uri(builder.Configuration["ApiBaseUrl"]!);

builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = false;
    });

builder.Services.AddAuthorization();

// کلاینت بدون توکن (لاگین، رفرش، لاگ‌اوت)
builder.Services.AddHttpClient<AuthApiClient>(c => c.BaseAddress = apiBase);

// کلاینت با توکن برای بقیه‌ی API
builder.Services.AddTransient<TokenRefreshHandler>();
builder.Services.AddHttpClient("Api", c => c.BaseAddress = apiBase)
    .AddHttpMessageHandler<TokenRefreshHandler>();
#endregion




// ثبت سرویس‌های Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ثبت سرویس‌های MudBlazor
builder.Services.AddMudServices();

var app = builder.Build();

// تنظیمات محیط اجرا
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}


app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true
);

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.MapPost("/auth/login", async (HttpContext ctx, AuthApiClient api,
    [Microsoft.AspNetCore.Mvc.FromForm] string userName,
    [Microsoft.AspNetCore.Mvc.FromForm] string password,
    [Microsoft.AspNetCore.Mvc.FromForm] bool rememberMe) =>
{
    var tokens = await api.LoginAsync(userName, password);
    if (tokens is null)
        return Results.Redirect("/login?error=1");

    var principal = JwtHelper.ToPrincipal(tokens.AccessToken,
        CookieAuthenticationDefaults.AuthenticationScheme);

    var props = new AuthenticationProperties
    {
        IsPersistent = rememberMe,
        ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
    };
    props.StoreTokens(new[]
    {
        new AuthenticationToken { Name = "access_token",  Value = tokens.AccessToken },
        new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken }
    });

    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapPost("/auth/logout", async (HttpContext ctx, AuthApiClient api) =>
{
    var refresh = await ctx.GetTokenAsync("refresh_token");
    if (!string.IsNullOrEmpty(refresh))
        await api.LogoutAsync(refresh);

    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).DisableAntiforgery();


app.Run();
