using AndreGoepel.Design.Blazor;
using AndreGoepel.Design.Blazor.Demo.Components;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Razor components with interactive server rendering.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Radzen services: registers the Dialog / Notification / Tooltip / ContextMenu
// hosts that the design-system components rely on (rendered once in MainLayout).
builder.Services.AddRadzenComponents();

// Design-system services (ConfirmService, …) — must follow AddRadzenComponents.
// BrandName is appended to every page's document title by AppPageTitle. Cultures
// default to en/de, which is what this demo (and the design system) ships.
builder.Services.AddDesignBlazor(o => o.BrandName = "Design Blazor");

// The demo's own strings (nav, page copy, …), separate from the library's.
builder.Services.AddLocalization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Must run before MapRazorComponents: it sets CurrentUICulture on the request
// that establishes the Blazor Server circuit, and maps the endpoint LanguageSwitcher
// links to.
app.UseDesignBlazorLocalization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
