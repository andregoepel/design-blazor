# AndreGoepel.Design.Blazor design guidelines

How to build Blazor components that fit the design-system look used across the
Identity UI. Follow this whenever you add a page, form, table, or control so it
matches the rest of the app in both light and dark themes.

The single source of truth is
[`wwwroot/css/design.css`](wwwroot/css/design.css). It defines the
design tokens, remaps Radzen's variables onto them, and provides the `ag-*`
helper classes referenced below. A host app must reference that stylesheet
**after** Radzen's `material-base.css` (see the Aspire sample's `App.razor`).

---

## 1. Principles

- **Token-first.** Never hard-code a colour. Use a CSS variable so the component
  adapts to light/dark automatically. Prefer the `--ag-*` tokens; the `--rz-*`
  Radzen variables are already mapped onto them.
- **Radzen components, design-system skin.** Build with Radzen components
  (`RadzenButton`, `RadzenTextBox`, `RadzenCard`, `RadzenDataGrid`, …). The
  stylesheet reskins them — you rarely need custom control markup.
- **Flat surfaces.** Cards and inputs are flat: a solid fill, a 1px hairline
  border, small radius. No gradients, no drop shadows, no glows.
- **Sentence case everywhere.** "Save changes", not "Save Changes" or "SAVE
  CHANGES". Applies to buttons, headings, labels, table headers (the header CSS
  upper-cases them for you — write them in sentence case).
- **Both themes always.** Mentally test every colour on a near-black *and* a
  near-white background. If it only works on one, you hard-coded something.

---

## 2. Design tokens

Defined on `:root` (dark, the default) and `:root[data-theme="light"]`. Use
these — do not invent new hex values.

| Token | Role | Dark | Light |
|---|---|---|---|
| `--ag-bg` | page background | `#101619` | `#f7faf9` |
| `--ag-bg-sidebar` | sidebar / app rail | `#0c1114` | `#ffffff` |
| `--ag-surface` | card / panel fill | `#131a1e` | `#ffffff` |
| `--ag-input` | input fill | `#151c20` | `#ffffff` |
| `--ag-hover` | hover / disabled fill | `#182226` | `#eef4f1` |
| `--ag-border` | hairline border | `#1e262b` | `#e0e8e4` |
| `--ag-border2` | stronger border (inputs) | `#263137` | `#d0dcd6` |
| `--ag-text` | primary text / headings | `#eef2f3` | `#141d19` |
| `--ag-text2` | body text | `#d3dadd` | `#37413e` |
| `--ag-muted` | labels, secondary text | `#7d8a91` | `#68766f` |
| `--ag-faint` | captions, ids, table headers | `#536066` | `#9aa6a1` |
| `--ag-accent` | brand emerald (buttons, active) | `#10b981` | `#0e9f6e` |
| `--ag-on-accent` | text **on** the accent | `#06281c` | `#ffffff` |
| `--ag-accent-text` | accent-coloured text/links | `#5eead4` | `#0b8a60` |
| `--ag-accent-soft` | accent tint (badges, active bg) | `rgba(16,185,129,.12)` | `rgba(14,159,110,.1)` |
| `--ag-warn` / `--ag-warn-soft` | warning | amber | amber |
| `--ag-danger` / `--ag-danger-soft` | danger | `#f47171` | `#dc2626` |
| `--ag-info` / `--ag-info-soft` | info | `#4cb8e8` | `#0369a1` |

> **Note the on-accent split:** in dark mode text on the emerald button is a
> *dark* green (`#06281c`); in light mode it's white. Always pair `--ag-accent`
> backgrounds with `--ag-on-accent` text — never plain white/black.

---

## 3. Typography

Three families, **self-hosted** by the RCL and loaded via a single
`<link rel="stylesheet" href="_content/AndreGoepel.Design.Blazor/css/fonts.css" />`
in the host `App.razor`. The `.woff2` files live in `wwwroot/fonts` and are
vendored from Google Fonts (latin + latin-ext subsets) — the app never calls
`fonts.googleapis.com` at runtime (GDPR / offline / reliability). Regenerate with
`scripts/fetch-fonts.py`.

- **Space Grotesk** — headings and the topbar/page titles.
- **Manrope** — body text, labels, buttons (the default `--rz-text-font-family`).
- **JetBrains Mono** — ids, codes, cron expressions.

Sizes that matter:

| Use | Style |
|---|---|
| Page heading (`h1`) | Space Grotesk, 600, ~26px, `color: var(--ag-text)` |
| Page subtitle | 13.5px, `color: var(--ag-muted)` |
| Field label | Manrope, **600, 12.5px**, `color: var(--ag-muted)` |
| Body | Manrope, 400 |
| Button | Manrope, primary 800 / secondary 600, ~13px |
| Table header | Manrope, 700, 11px, upper-cased, `letter-spacing: .07em`, `--ag-faint` |
| Id / mono | JetBrains Mono, 10.5px, `--ag-faint` |

Labels and headings are styled globally (`.rz-label`, heading rules) — use
`RadzenLabel` / `RadzenText` and they inherit the right size automatically.

**Body vs. secondary text.** The default body colour is `--ag-text2` (bright).
Descriptive/secondary paragraphs — a subtitle, or the blurb under a card
sub-heading — should be **muted**: add `Style="color: var(--rz-text-secondary-color);"`
(which resolves to `--ag-muted`). Don't leave a description at the default body
colour or it reads too bright.

---

## 4. Page structure

Every content page follows the same skeleton. The heading lives **outside** the
card; the card holds the form/content; actions sit in a bordered footer.

```razor
<div class="rz-p-4 rz-p-md-6">
    <RadzenStack Gap="1.5rem">

        <PageHeader Title="Page title" Subtitle="Short description." />

        @* Status banners (if any) go here — OUTSIDE the card *@

        @* A form page: CardForm supplies the card + template form + action footer. *@
        <CardForm TItem="InputModel" Data="Input" Submit="SaveAsync">
            @* FormFields … (see §5) *@
        </CardForm>

    </RadzenStack>
</div>
```

**Page header with a right-aligned action** (e.g. "+ New role", "Register
passkey") — pass `Actions`:

```razor
<PageHeader Title="Page title" Subtitle="Description.">
    <Actions>
        <RadzenButton Text="New role" Icon="add" ButtonStyle="ButtonStyle.Primary" Click="@New" />
    </Actions>
</PageHeader>
```

`PageHeader` is a thin wrapper over `.ag-page-head` (heading left, actions right,
bottom-aligned; wraps below ~640px). Omit `Subtitle` for a title-only heading and
`Actions` for a plain heading with no right-hand slot.

---

## 5. Component recipes

### Forms

Use `CardForm` (the card + `RadzenTemplateForm` + action footer) with `FormField`s
(label **above** the field, full width). Never use `RadzenFormField` — its floating
label doesn't match the design. Put the control + any validators in each
`FormField`'s content; set `For` to the control's `Name` so the label links to it.

```razor
<CardForm TItem="InputModel" Data="Input" Submit="SaveAsync" Cancel="Cancel" IsBusy="_saving">
    <FormField Label="Email" For="Email" Hint="We'll only use this to contact you.">
        <RadzenTextBox @bind-Value="Input.Email" Name="Email" Placeholder="name@example.com" Style="width:100%" />
        <RadzenRequiredValidator Component="Email" Text="Email is required" />
        <RadzenEmailValidator Component="Email" Text="That is not a valid email" />
    </FormField>
</CardForm>
```

`Submit` fires only when validation passes; while `IsBusy` is true the primary
button shows `BusyText` and both buttons disable. `Cancel` is optional — omit it and
no cancel button is shown.

Two fields side by side (collapses to one column on narrow screens): wrap them in
`<div class="ag-form-grid">…</div>` inside the `CardForm`.

Read-only fields render greyed automatically (`.rz-textbox[readonly]`). Just set
`ReadOnly="true"`; you do not need extra inline styles.

### Buttons

Sentence case, dark-on-accent primaries. Order secondary/cancel **before** the
primary in a right-aligned footer.

| Style | `ButtonStyle` | Look |
|---|---|---|
| Primary action | `Primary` | emerald fill, dark on-accent text, 800 |
| Secondary / cancel | `Light` | transparent, hairline border, muted-to-text |
| Destructive | `Danger` | danger-soft tint + red text (not a solid red block) |

### Icons

Two separate icon systems are in play:

- **`RadzenButton`'s `Icon="…"`** uses Radzen's bundled Material Symbols font
  (ligature names like `add`, `delete`, `search`, `more_horiz`) — the default
  for anything inside a Radzen button.
- **`AppIcon`** is a small hand-drawn inline-SVG set (stroke-based, inherits
  `currentColor`, 24×24 viewBox) for icon needs *outside* a Radzen button slot
  — e.g. `ThemeToggle`'s sun/monitor/moon switch, or a bespoke icon next to
  plain text.

```razor
<AppIcon Name="search" />           @* 16px, default *@
<AppIcon Name="chevron-left" Size="24" />
```

Available glyphs: `sun`, `monitor`, `moon`, `edit`, `delete`, `search`,
`more-horizontal`, `plus`, `chevron-left`, `chevron-right`, `key`, `check`,
`x`. Icons are decorative by default (`aria-hidden`) — give the surrounding
control its own accessible label. Add a new glyph by dropping its 24×24
geometry into `AppIcon`'s `Glyphs` dictionary.

### Card action row

```razor
<div class="ag-card-actions">            @* right-aligned, top border *@
    <RadzenButton Text="Cancel" ButtonStyle="ButtonStyle.Light" … />
    <RadzenButton Text="Save changes" ButtonStyle="ButtonStyle.Primary" … />
</div>
```

Variants: add `ag-start` for left-aligned actions (e.g. Personal Data). For a
button that isn't a card footer (sits under a paragraph), use
`<div class="ag-actions-inline">…</div>` so it keeps its natural width instead of
stretching.

### Status badges

Use `StatusBadge` with a semantic `Variant`; it maps to the matching
`ag-badge-*` class. Five variants ship: `Success`, `Danger`, `Warning`, `Info`,
`Neutral` (the default).

```razor
<StatusBadge Text="Active" Variant="BadgeVariant.Success" />
<StatusBadge Text="Deleted" Variant="BadgeVariant.Danger" />
<StatusBadge Text="Invited" Variant="BadgeVariant.Warning" />
```

The underlying classes are `ag-badge` + `ag-badge-success` / `-danger` / `-warn`
/ `-info` / `-neutral`, all tinted from the `--ag-*` tokens.

### Alerts / status banners

Use `RadzenAlert`; the stylesheet softens Radzen's filled variants into tinted
banners (`Info`, `Warning`, `Danger`, `Success`) — 13px `--ag-text2` body text,
with the icon automatically tinted to the status colour. A page-level status
banner (e.g. "2FA is disabled") belongs **outside/above** the card, as a sibling
of the heading — not inside it.

### Empty state

Use `EmptyState` over the `ag-empty` recipe: `Icon` (optional glyph), `Title`,
`Text` (optional), `Actions` (optional call-to-action).

```razor
<EmptyState Icon="@("🔑︎")" Title="No passkeys registered yet"
            Text="Passkeys let you sign in without a password.">
    <Actions>
        <RadzenButton Text="Register passkey" Icon="add" ButtonStyle="ButtonStyle.Primary" Size="ButtonSize.Small" />
    </Actions>
</EmptyState>
```

`Icon` is rendered as-is inside `.ag-empty-icon` — pass a plain emoji glyph
with a trailing U+FE0E variation selector so it renders as the flat text-style
glyph the `ag-empty-icon` colour token expects, not a full-colour emoji.

### Data grids (`RadzenDataGrid`)

- Wrap in `DataCard` (a `RadzenCard` with `Style="padding: 0; overflow: hidden;"`
  so a `GridToolbar` + grid sit flush against its edges).
- `AllowFiltering="false"`, `AllowColumnResize="false"`, no `SelectionMode` —
  rows are display-only. Column headers are upper-cased by the stylesheet; write
  titles in sentence case.
- Put an entity's **id under its name/email** in one column, muted + monospace:

```razor
<RadzenDataGridColumn Property="@nameof(Role.Name)" Title="Role name">
    <Template Context="role">
        <div style="display:flex; flex-direction:column; gap:2px;">
            <span class="ag-cell-name">@role.Name</span>
            <span class="ag-cell-id">@role.Id</span>
        </div>
    </Template>
</RadzenDataGridColumn>
```

- **Toolbar (search + count):** use `GridToolbar` inside `DataCard`, above the
  grid. Bind `Search` two-way and filter your own data off it — the component
  only renders the box, filtering is the page's job:

```razor
<DataCard>
    <GridToolbar @bind-Search="_search" SearchPlaceholder="Search roles…"
                 Count="@($"{Filtered.Count} of {_roles.Count}")" />

    @if (Filtered.Count == 0)
    {
        <div class="rz-p-4 rz-p-md-6">
            <EmptyState Title="No roles match your search" />
        </div>
    }
    else
    {
        <RadzenDataGrid TItem="Role" Data="Filtered" … />
    }
</DataCard>
```

  Keep the toolbar mounted even when the grid is replaced by an `EmptyState` —
  otherwise the user has no way to clear a search that matched nothing.

- Status columns use `.ag-badge` (see above).
- **Row actions:** use `RowActions` + `IconButton` — keep the primary action
  visible and hide the rest behind a compact `⋯` overflow menu via
  `ContextMenuService`. `IconButton`'s `Icon` is a Radzen Material icon
  ligature name (like `RadzenButton`'s own `Icon`), not an `AppIcon` glyph —
  `Title` is required since the button has no visible text:

```razor
<RowActions>
    <RadzenButton Text="Users" ButtonStyle="ButtonStyle.Light" Size="ButtonSize.Small"
                  Disabled="@data.Deleted" Click="@(() => ShowUsersAsync(data))" />
    @if (RowHasMenu(data))
    {
        <IconButton Icon="more_horiz" Title="More actions" Click="@(args => OpenRowMenu(args, data))" />
    }
</RowActions>
```

```csharp
@inject ContextMenuService ContextMenuService

private void OpenRowMenu(MouseEventArgs args, Role role)
{
    var items = new List<ContextMenuItem>();
    if (role.Deleted)
        items.Add(new() { Text = "Restore", Value = "restore", Icon = "restore" });
    else if (role.Deletable)
        // A destructive item: give it the trash icon and the danger colour. The
        // inline IconColor also lets the stylesheet tint the whole row red.
        items.Add(new() { Text = "Delete", Value = "delete", Icon = "delete", IconColor = "var(--ag-danger)" });

    ContextMenuService.Open(args, items, async e =>
    {
        ContextMenuService.Close();
        switch (e.Value as string)
        {
            case "restore": await RestoreRoleAsync(role); break;
            case "delete":  await DeleteRoleAsync(role);  break;
        }
    });
}
```

The host layout must render `<RadzenContextMenu />` once (the Aspire sample does).

### Confirmations

Don't call `DialogService.Confirm` directly — inject `ConfirmService` and use it,
so every confirm dialog gets the same title, OK/Cancel wording, and the
`bool?`→`bool` collapse (a dismissed dialog counts as "no"). Register it once with
`services.AddDesignBlazor()` (after `AddRadzenComponents()`).

```csharp
@inject ConfirmService Confirm

// standard destructive delete: "Delete {name}? This cannot be undone." + a "Delete" button
if (await Confirm.ConfirmDeleteAsync(role.Name))
    await DeleteRoleAsync(role);

// or a custom confirm
if (await Confirm.ConfirmAsync("Sign everyone out?", "End sessions", okText: "Sign out"))
    await EndSessionsAsync();
```

### Auth / login pages

Auth pages use `LoginLayout`, which supplies the centred card, brand, and footer.
A page just provides the heading block + form + a centred footer link:

```razor
<div class="ag-login-head">
    <RadzenText TagName="TagName.H1" TextStyle="TextStyle.H4" Text="Welcome back" />
    <RadzenText TextStyle="TextStyle.Body2" class="ag-login-sub" Text="Fill in your login credentials to proceed." />
</div>

@* form … *@

<div class="ag-login-actions">   @* full-width stacked buttons *@ </div>

<div class="ag-login-footer-links">
    <span class="ag-login-resend">Already have an account? <RadzenLink Path="Account/Login" Text="Log in" /></span>
</div>
```

---

## 6. `ag-*` class reference

| Class | Purpose |
|---|---|
| `ag-page-head` | header row: heading left, action right (rendered by `PageHeader`) |
| `ag-card-actions` | bordered card footer, right-aligned (add `ag-start` for left; rendered by `CardForm`) |
| `ag-actions-inline` | inline button group that doesn't stretch |
| `ag-form-grid` | two-column field grid (collapses ≤640px) |
| `ag-badge` + `ag-badge-success` / `-danger` / `-warn` / `-info` / `-neutral` | status pills (rendered by `StatusBadge`) |
| `ag-grid-toolbar`, `ag-search`, `ag-search-icon`, `ag-search-input`, `ag-grid-count` | in-card grid toolbar: filter box + row count (rendered by `GridToolbar`, inside `DataCard`) |
| `ag-info-box`, `ag-info-box-label`, `ag-info-box-value` | inline soft-tinted info pill (e.g. "Next scheduled run …") |
| `ag-empty`, `ag-empty-icon`, `ag-empty-title`, `ag-empty-text` | dashed empty state (rendered by `EmptyState`) |
| `ag-row-actions`, `ag-icon-btn` | grid row actions (rendered by `RowActions`) + compact `⋯` button (rendered by `IconButton`) |
| `ag-cell-name`, `ag-cell-id` | name/email + truncated mono id in a grid cell |
| `ag-login-*` | login-card building blocks (provided by `LoginLayout`) |
| `ag-shell`, `ag-sidebar`, `ag-topbar`, `ag-topbar-left`, `ag-nav-item`, `ag-theme-toggle`, `ag-hamburger`, `ag-backdrop`, … | app shell (rendered by `AppShell`) |

### App shell

The host layout renders an **`AppShell`** rather than hand-writing the shell markup.
`AppShell` provides the sidebar / topbar / content structure and the responsive
off-canvas drawer, and owns the breadcrumb state; the host supplies branding and
fills the slots:

```razor
<AppShell BrandName="@AppName">
    <Sidebar>@* NavLinks + ag-nav-section groups *@</Sidebar>
    <TopbarActions>@* theme toggle, user chip, sign-in/out *@</TopbarActions>
    <SidebarFooter>@* optional *@</SidebarFooter>
    <ChildContent>@Body</ChildContent>
</AppShell>
```

The host only needs to load `nav.js` (see the Aspire sample's `App.razor`);
`AppShell` emits the hamburger, backdrop and `data-nav-open` itself.

### Topbar breadcrumb

The topbar crumb is **defined per page, not by the layout**. Each shell page sets
it through `IdentityPageTitle`'s optional `Breadcrumb` parameter, in sentence case
with the section prefix:

```razor
<IdentityPageTitle Title="Profile" Breadcrumb="Account / Profile" />
```

`IdentityPageTitle` pushes the value into a cascading `BreadcrumbState` that
`AppShell` owns and renders in `ag-topbar-title`; the shell re-renders when it
changes. Omit `Breadcrumb` and the document `Title` is used as a fallback. Pages
outside the shell (the `LoginLayout` auth pages) have no `BreadcrumbState` cascaded,
so they simply ignore the parameter.

### Responsive

The shell is responsive via two breakpoints (no JS layout logic — just CSS media
queries plus a tiny toggle script):

- **≤ 1000px** — the sidebar becomes a fixed off-canvas drawer. The topbar shows
  a `ag-hamburger` button (`onclick="agNav.toggle()"`); an `ag-backdrop`
  (`onclick="agNav.close()"`) dims the content while it's open. The drawer state
  is `data-nav-open` on `.ag-shell`, flipped by `nav.js`, which
  also closes it on Escape and after a sidebar link is followed.
- **≤ 700px** — tighter page/topbar padding, the user email is hidden (avatar
  only), `ag-form-grid` collapses to one column, and data grids get a
  `min-width` with horizontal scroll so columns don't crush.

A host that renders the shell must include `nav.js` and give
`.ag-shell` a `data-nav-open="false"` plus the `ag-backdrop` element (see the
Aspire sample's `MainLayout`).

---

## 7. Radzen gotchas (why the stylesheet does what it does)

Radzen's Material theme ships opinionated rules that fight the design. These are
already handled in `design.css`; keep them in mind if you add new
component types:

- **Buttons force a white contrast colour** on filled variants. Primary and
  danger text colours are set with `!important` to win. If you introduce another
  coloured button variant, expect the same and override its `color`.
- **Filled alerts / danger buttons** use a solid status colour; we soften them to
  the `*-soft` tint via `.rz-alert.rz-{info,warning,danger,success}` and
  `.rz-button.rz-danger`.
- **`RadzenLabel` defaults to 16px/400/body colour** — the global `.rz-label`
  rule makes it 12.5px/600/muted.
- **`FocusOnNavigate` focuses the page `h1`**, which the browser paints a focus
  ring on. Suppressed globally (`h1:focus{outline:none}`); reuse that if you add
  focusable non-interactive elements.
- **`RadzenDataGrid` uses white grid variables** (`--rz-grid-*`) and a fixed
  table width equal to the sum of column widths. We remap the grid vars to
  `--ag-*` and force `table-layout: auto; width: 100%` so rows hover dark and the
  table fills the card without horizontal overflow.
- **Read-only inputs** need `--rz-input-disabled-background` defined (Radzen
  leaves it empty → transparent).

---

## 8. Checklist for a new component

- [ ] No hard-coded colours — only `--ag-*` (or `--rz-*`) variables.
- [ ] Looks correct in **both** light and dark (`data-theme` on `<html>`).
- [ ] Sentence case on every label, button, heading, and column title.
- [ ] Labels sit **above** fields; no `RadzenFormField`.
- [ ] Primary buttons pair `--ag-accent` with `--ag-on-accent`; destructive
      actions use the danger-soft style.
- [ ] Card actions in an `ag-card-actions` footer; page-level banners outside the
      card.
- [ ] Reuse `ag-*` helpers instead of new one-off inline styles; if you need a
      new pattern, add a token-based rule to `design.css` rather than a
      literal hex value.
- [ ] Builds clean and renders without a horizontal scrollbar at desktop widths.
