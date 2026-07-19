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
builder.Services.AddDesignBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
