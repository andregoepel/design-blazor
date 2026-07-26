using Radzen;

namespace AndreGoepel.Design.Blazor;

/// <summary>
/// Extension methods over Radzen's <see cref="NotificationService"/> for the dominant
/// idiom across consuming apps: a plain success toast, or an error/warning toast with an
/// optional detail line (e.g. a <c>Result&lt;T&gt;</c> error message).
/// </summary>
/// <remarks>
/// Deliberately additive — plain extension methods, nothing added to <see cref="ConfirmService"/>.
/// A <c>Result</c>-aware overload (e.g. <c>notifications.Notify(result)</c>) is intentionally out of
/// scope for now: the <c>AndreGoepel.Core.Result</c> migration is still in flight in consuming repos,
/// so that overload is deferred to a follow-up issue once the migration lands everywhere.
/// </remarks>
public static class NotifyExtensions
{
    /// <summary>Shows a success toast. Duration matches <see cref="NotificationService"/>'s own default.</summary>
    public static void Success(
        this NotificationService notifications,
        string title,
        string? detail = null
    ) => notifications.Notify(NotificationSeverity.Success, title, detail ?? "");

    /// <summary>Shows an error toast. Duration matches <see cref="NotificationService"/>'s own default.</summary>
    public static void Error(
        this NotificationService notifications,
        string title,
        string? detail = null
    ) => notifications.Notify(NotificationSeverity.Error, title, detail ?? "");

    /// <summary>Shows a warning toast. Duration matches <see cref="NotificationService"/>'s own default.</summary>
    public static void Warning(
        this NotificationService notifications,
        string title,
        string? detail = null
    ) => notifications.Notify(NotificationSeverity.Warning, title, detail ?? "");
}
