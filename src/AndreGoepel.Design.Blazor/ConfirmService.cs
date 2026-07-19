using Radzen;

namespace AndreGoepel.Design.Blazor;

/// <summary>
/// Thin wrapper over Radzen's <see cref="DialogService"/> that standardises the
/// confirm-dialog boilerplate (title, OK/Cancel button text) and collapses the
/// nullable <c>bool?</c> result — <c>null</c> (dismissed) counts as "no". Register
/// via <c>services.AddDesignBlazor()</c>; requires <c>AddRadzenComponents()</c>.
/// </summary>
public sealed class ConfirmService(DialogService dialogService)
{
    /// <summary>
    /// Shows a confirm dialog. Returns <c>true</c> only when the user explicitly
    /// confirms; cancelling or dismissing returns <c>false</c>.
    /// </summary>
    public async Task<bool> ConfirmAsync(
        string message,
        string title = "Confirm",
        string okText = "OK",
        string cancelText = "Cancel"
    )
    {
        var result = await dialogService.Confirm(
            message,
            title,
            new ConfirmOptions { OkButtonText = okText, CancelButtonText = cancelText }
        );
        return result == true;
    }

    /// <summary>
    /// Shows a standard destructive-delete confirmation for <paramref name="name"/>
    /// ("Delete {name}? This cannot be undone." with a "Delete" primary button).
    /// </summary>
    public Task<bool> ConfirmDeleteAsync(string name, string title = "Delete") =>
        ConfirmAsync($"Delete {name}? This cannot be undone.", title, okText: "Delete");
}
