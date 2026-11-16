
using MudBlazor.Services;
using Nexa.Front.Components;

var builder = WebApplication.CreateBuilder(args);

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
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
