using AndreGoepel.Design.Blazor.Resources;
using Microsoft.Extensions.Localization;
using Radzen;

namespace AndreGoepel.Design.Blazor;

/// <summary>
/// Thin wrapper over Radzen's <see cref="DialogService"/> that standardises the
/// confirm-dialog boilerplate (title, OK/Cancel button text) and collapses the
/// nullable <c>bool?</c> result — <c>null</c> (dismissed) counts as "no". Register
/// via <c>services.AddDesignBlazor()</c>; requires <c>AddRadzenComponents()</c>.
/// </summary>
/// <remarks>
/// The localizer is optional so hosts that construct the service directly, rather
/// than through <c>AddDesignBlazor</c>, keep working — those fall back to English.
/// </remarks>
public sealed class ConfirmService(
    DialogService dialogService,
    IStringLocalizer<DesignStrings>? localizer = null
)
{
    /// <summary>
    /// Shows a confirm dialog. Returns <c>true</c> only when the user explicitly
    /// confirms; cancelling or dismissing returns <c>false</c>. Title and button
    /// text default to their localized equivalents.
    /// </summary>
    public async Task<bool> ConfirmAsync(
        string message,
        string? title = null,
        string? okText = null,
        string? cancelText = null
    )
    {
        var result = await dialogService.Confirm(
            message,
            title ?? Localized("Confirm.Title", "Confirm"),
            new ConfirmOptions
            {
                OkButtonText = okText ?? Localized("Confirm.Ok", "OK"),
                CancelButtonText = cancelText ?? Localized("Confirm.Cancel", "Cancel"),
            }
        );
        return result == true;
    }

    /// <summary>
    /// Shows a standard destructive-delete confirmation for <paramref name="name"/>
    /// ("Delete {name}? This cannot be undone." with a "Delete" primary button), in
    /// the current culture.
    /// </summary>
    public Task<bool> ConfirmDeleteAsync(string name, string? title = null) =>
        ConfirmAsync(
            Localized("Confirm.DeleteMessage", "Delete {0}? This cannot be undone.", name),
            title ?? Localized("Confirm.DeleteTitle", "Delete"),
            okText: Localized("Confirm.DeleteAction", "Delete")
        );

    private string Localized(string key, string fallback) =>
        localizer is null ? fallback : localizer[key];

    private string Localized(string key, string fallback, params object[] arguments) =>
        localizer is null ? string.Format(fallback, arguments) : localizer[key, arguments];
}
