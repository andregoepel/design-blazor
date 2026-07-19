namespace AndreGoepel.Design.Blazor.Components;

/// <summary>
/// Semantic colour shared by status-oriented components (<see cref="StatusBadge"/>
/// maps it to an ag-badge-* class; <see cref="StatTile"/> maps it to a text colour).
/// </summary>
public enum BadgeVariant
{
    /// <summary>Neutral / muted (default) — no strong status meaning.</summary>
    Neutral,

    /// <summary>Positive / active — accent-tinted.</summary>
    Success,

    /// <summary>Negative / deleted / failed — danger-tinted.</summary>
    Danger,

    /// <summary>Caution / pending — warning-tinted.</summary>
    Warning,

    /// <summary>Informational — info-tinted.</summary>
    Info,
}
