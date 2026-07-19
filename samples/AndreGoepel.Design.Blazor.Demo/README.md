# AndreGoepel.Design.Blazor.Demo

A standalone Blazor Server host that renders the `AndreGoepel.Design.Blazor`
component library as a live gallery — **no Aspire, no database, no auth**. It is
the surface for visually reviewing the design system in light **and** dark.

## Run

```bash
dotnet run --project samples/AndreGoepel.Design.Blazor.Demo
```

Then open the printed `https://localhost:<port>` URL. Toggle light / dark with
the switch in the top bar.

## What it shows

The chrome (sidebar, top bar, responsive drawer, theme toggle) is the library's
own `AppShell` + `ThemeToggle`. The sidebar links to one page per surface:

- **Overview** — the shell and page-header pattern.
- **Tokens & typography** — the `--ag-*` colour tokens and the type ramp.
- **Icons** — the `AppIcon` glyph set.
- **Buttons & badges** — Radzen buttons reskinned by the tokens; `ag-badge` pills.
- **Forms** — the label-above field recipe, `ag-form-grid`, validators, card actions.
- **Data grid** — the in-card grid recipe (toolbar, name/id cells, badges, row-action
  overflow menu, confirm dialog, empty state).
- **Feedback & empty states** — alerts, toasts, info box, empty state.
- **Login shell** — the `LoginShell` auth-card pattern.

## Note

This host is the visual-verification target for the design-system component
epic. Sections currently use the documented `ag-*` CSS recipes; as the typed
components land (`PageHeader`, `StatusBadge`, `EmptyState`, `CardForm`, …) each
section is swapped over to the real component.
