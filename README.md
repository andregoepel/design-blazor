# AndreGoepel.Design.Blazor

An **app-neutral design system for Blazor**: an emerald-accented light/dark token
palette, a [Radzen](https://www.radzen.com/) reskin, self-hosted fonts, and a small
set of shell / login building blocks. It has **no dependency on any application** —
consume it from any Blazor app or library to get a consistent look without pulling
in unrelated stacks.

> Extracted from the [marten-identity](https://github.com/andregoepel/marten-identity)
> project, where it originated as the "AppFoundation" UI.

## Install

```bash
dotnet add package AndreGoepel.Design.Blazor
```

## Use

Reference the stylesheets **after** Radzen's `material-base.css`, and load the
theme + nav scripts, in your host page (`App.razor`):

```html
<link rel="stylesheet" href="_content/AndreGoepel.Design.Blazor/css/fonts.css" />
<link rel="stylesheet" href="_content/Radzen.Blazor/css/material-base.css" />
<link rel="stylesheet" href="_content/AndreGoepel.Design.Blazor/css/design.css" />
<script src="_content/AndreGoepel.Design.Blazor/js/theme.js"></script>
<script src="_content/AndreGoepel.Design.Blazor/js/nav.js"></script>
```

Then build with Radzen components and the `ag-*` helper classes, and use the shell:

```razor
<AppShell BrandName="Acme">
    <Sidebar>@* NavLinks + ag-nav-section groups *@</Sidebar>
    <TopbarActions>
        <LanguageSwitcher />
        <ThemeToggle />
    </TopbarActions>
    <ChildContent>@Body</ChildContent>
</AppShell>
```

## What's inside

- **Design tokens** (`--ag-*`) for light + dark, and a remap of Radzen's `--rz-*`
  variables onto them.
- **`ag-*` helper classes** — cards, buttons, badges, forms, grids, empty states,
  the app shell, and the login card.
- **Components** — `AppShell`, `AppPageTitle`, `LoginShell`, `AppIcon`,
  `ThemeToggle`, `LanguageSwitcher`, `PageHeader`, `StatusBadge`, `EmptyState`,
  `RowActions`, `IconButton`, `DataCard`, `GridToolbar`, `FilterBar`, `CardForm`,
  `FormField`, `SettingToggleRow`, `StatTile`, `LinkButton`, `InfoBox`, and the
  cascading `BreadcrumbState`.
- **Services & options** — `ConfirmService` (standardised confirm/delete dialogs)
  and `DesignBlazorOptions` (e.g. the `BrandName` `AppPageTitle` appends to document
  titles), registered via `services.AddDesignBlazor(o => o.BrandName = "Acme")`
  (call after `AddRadzenComponents()`).
- **Self-hosted fonts** (Manrope / Space Grotesk / JetBrains Mono) — nothing is
  fetched from Google at runtime. Regenerate with `scripts/fetch-fonts.py`.

## Localization

English and German ship out of the box — every string a component renders on its
own falls back to a localized default, overridable per-parameter as always:

```csharp
builder.Services.AddDesignBlazor(o => o.BrandName = "Acme"); // en/de by default
...
app.UseDesignBlazorLocalization(); // before MapRazorComponents
```

`LanguageSwitcher` (above) links to the culture-switch endpoint this sets up. See
[DESIGN.md §9](src/AndreGoepel.Design.Blazor/DESIGN.md#9-localization) for how a
host app localizes its own pages the same way — the
[demo app](samples/AndreGoepel.Design.Blazor.Demo) is a fully bilingual reference.

See [`DESIGN.md`](src/AndreGoepel.Design.Blazor/DESIGN.md) for the full guidelines.

## License

MIT © André Göpel
