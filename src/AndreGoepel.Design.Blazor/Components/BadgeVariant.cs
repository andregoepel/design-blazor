namespace AndreGoepel.Design.Blazor.Components;

/// <summary>Semantic colour for a <see cref="StatusBadge"/>, mapped to an ag-badge-* class.</summary>
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
