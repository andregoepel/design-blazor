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
    <FormField Label="Email" For="Email" Required="true" Hint="We'll only use this to contact you.">
        <RadzenTextBox @bind-Value="Input.Email" Name="Email" Placeholder="name@example.com" Style="width:100%" />
        <RadzenRequiredValidator Component="Email" Text="Email is required" />
        <RadzenEmailValidator Component="Email" Text="That is not a valid email" />
    </FormField>
</CardForm>
```

`Submit` fires only when validation passes; while `IsBusy` is true the primary
button shows `BusyText` and both buttons disable. `Cancel` is optional — omit it and
no cancel button is shown. `FormField`'s `Required` is purely the visual red
asterisk — it doesn't validate anything, so pair it with a
`RadzenRequiredValidator` as above.

For a form whose primary action is destructive (e.g. "Permanently delete my
account"), set `Danger="true"` to render the submit button `ButtonStyle.Danger`
instead of `Primary`. Everything else about `CardForm` — validation, `IsBusy`,
the optional `Light` cancel — behaves the same.

Two fields side by side (collapses to one column on narrow screens): wrap them in
`<div class="ag-form-grid">…</div>` inside the `CardForm`.

A field whose label row needs a trailing action (e.g. "Forgot password?" next
to "Password") — pass `LabelActions`; it renders right-aligned next to the
label instead of a plain label:

```razor
<FormField Label="Password" For="Password">
    <LabelActions>
        <RadzenLink Path="forgot-password" Text="Forgot password?" class="ag-field-head-link" />
    </LabelActions>
    <ChildContent>
        <RadzenPassword @bind-Value="Input.Password" Name="Password" Style="width:100%" />
        <RadzenRequiredValidator Component="Password" Text="Password is required" />
    </ChildContent>
</FormField>
```

(When `LabelActions` is used alongside field content, Razor requires the
content to be wrapped in an explicit `<ChildContent>` tag too — see RZ9996.)

Omit `LabelActions` for the plain-label case (the default).

Read-only fields render greyed automatically (`.rz-textbox[readonly]`). Just set
`ReadOnly="true"`; you do not need extra inline styles.

### Settings toggles

A stack of on/off feature toggles (e.g. an admin settings page) — use
`SettingToggleRow` inside a `RadzenCard`, one per setting. Each row is label +
description on the left, a `RadzenSwitch` on the right, with a bottom border
that the last row drops automatically:

```razor
<RadzenCard Style="padding: 8px 30px;">
    <SettingToggleRow Label="Self-service registration"
                       Description="Lets visitors create their own account."
                       @bind-Value="Input.EnableUserRegistration" />
    <SettingToggleRow Label="Passkeys"
                       Description="WebAuthn passkey registration and sign-in."
                       @bind-Value="Input.EnablePasskey" />
</RadzenCard>
```

### Buttons

Sentence case, dark-on-accent primaries. Order secondary/cancel **before** the
primary in a right-aligned footer.

| Style | `ButtonStyle` | Look |
|---|---|---|
| Primary action | `Primary` | emerald fill, dark on-accent text, 800 |
| Secondary / cancel | `Light` | transparent, hairline border, muted-to-text |
| Destructive | `Danger` | danger-soft tint + red text (not a solid red block) |

A button that navigates instead of raising a `Click` handler — use `LinkButton`:

```razor
<LinkButton Text="View passkeys" Path="account/manage/passkeys" ButtonStyle="ButtonStyle.Light" Icon="chevron_right" />
```

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

### Info box

An inline soft-tinted pill for a single labelled fact, e.g. "Next scheduled
run" — use `InfoBox`. Pass `Value` for plain text, or `ChildContent` for
richer content (a mono span, a link, …):

```razor
<InfoBox Label="Next scheduled run">
    Tomorrow at 03:00 — <span class="ag-mono">0 3 * * *</span>
</InfoBox>

<InfoBox Label="Environment" Value="Production" />
```

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

### Stat tiles

Use `StatTile` for a summary number (a dashboard row of Income/Expenses/Net,
planned-vs-actual, …): a muted caption over a big value, optionally tinted with
the same `BadgeVariant` as `StatusBadge`, and an optional `AppIcon` glyph next
to the caption. `Value` is a pre-formatted string — `StatTile` doesn't format
numbers or currency itself.

```razor
<RadzenRow Gap="1rem">
    <RadzenColumn Size="12" SizeMd="4">
        <StatTile Label="Income" Value="€4,250.00" Variant="BadgeVariant.Success" />
    </RadzenColumn>
    <RadzenColumn Size="12" SizeMd="4">
        <StatTile Label="Expenses" Value="€1,890.40" Variant="BadgeVariant.Danger" />
    </RadzenColumn>
    <RadzenColumn Size="12" SizeMd="4">
        <StatTile Label="Net" Value="€2,359.60" />   @* Neutral: inherits the default heading colour *@
    </RadzenColumn>
</RadzenRow>
```

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

### Filter bar

For structured filters above a grid (status, date range, owner, …), use
`FilterBar` — a card holding a horizontal, wrapping row of fields with an Apply
button. It's for *committed* filters (Apply-on-click); for live text search
inside the grid card, use `GridToolbar` instead — the two compose. Put
`FormField`s in `ChildContent`, handle `OnApply`, and give controls a `min-width`
so they don't collapse:

```razor
<FilterBar OnApply="ApplyFilters">
    <ChildContent>
        <FormField Label="Status" For="StatusFilter">
            <RadzenDropDown @bind-Value="_pendingStatus" Data="_statuses" Name="StatusFilter" Style="min-width: 12rem;" />
        </FormField>
    </ChildContent>
    <Actions>
        <RadzenButton Text="Reset" Icon="restart_alt" ButtonStyle="ButtonStyle.Light" Click="ResetFilters" />
    </Actions>
</FilterBar>
```

Note: because `FilterBar` has **both** `ChildContent` and an optional `Actions`
slot, once you use `Actions` you must wrap the fields in an explicit
`<ChildContent>` tag (a Blazor rule — you can't mix implicit child content with a
named fragment). With no `Actions`, the fields can sit directly inside.

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

- **No rows:** `RadzenDataGrid`'s own `EmptyText` is styled (padding, muted colour,
  centred) for a plain "no rows" message. For a richer empty state — an icon, a
  call-to-action — swap the grid for `EmptyState` in your empty-data branch instead
  (see the Data grid gallery page for both side by side).

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

### Dialogs

`DialogService.OpenAsync(string title, ...)` only takes a plain title string —
fine for a one-liner, but a form dialog that needs a subtitle (and a border
separating the header from the body) should pass `DialogHeader` as the
`titleContent` RenderFragment instead. Radzen still renders its own close button
and wrapper around whatever `titleContent` returns:

```razor
var result = await Dialogs.OpenAsync<TItem>(
    titleContent: _ =>
        @<DialogHeader Title="New customer" Subtitle="Add a customer to the studio's portal." />,
    childContent: _ => @<NewCustomerForm />
);
```

Inside the dialog body, use `CardForm`/`FormField` as usual — a `FormField`
that's required visually should set `Required="true"` for the red asterisk
(pair it with a `RadzenRequiredValidator` for the actual validation), and pair
naturally-related fields (e.g. email + mobile) in an `ag-form-grid` as normal.

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

For a single-button acknowledgement (no choice, just "OK") — e.g. surfacing an
error a `RadzenAlert`/`Notification` doesn't fit — don't call `DialogService.Alert`
directly either; use `ConfirmService.AlertAsync` the same way:

```csharp
await Confirm.AlertAsync("That invite link has already been used.", "Invite expired");
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

Fields still use `FormField` as usual — a password field with a "Forgot
password?" link is `FormField`'s `LabelActions` slot (see §5 "Forms").

---

## 6. `ag-*` class reference

| Class | Purpose |
|---|---|
| `ag-page-head` | header row: heading left, action right (rendered by `PageHeader`) |
| `ag-dialog-header` | bordered dialog title + optional subtitle (rendered by `DialogHeader`) |
| `ag-card-actions` | bordered card footer, right-aligned (add `ag-start` for left; rendered by `CardForm`) |
| `ag-actions-inline` | inline button group that doesn't stretch |
| `ag-form-grid` | two-column field grid (collapses ≤640px) |
| `ag-required` | red asterisk after a required field's label (rendered by `FormField`) |
| `ag-field-head`, `ag-field-head-link` | label row + trailing action, e.g. "Forgot password?" (rendered by `FormField` when `LabelActions` is set) |
| `ag-toggle-row`, `ag-toggle-row-text`, `ag-toggle-row-label`, `ag-toggle-row-description` | settings toggle row: label + description + switch (rendered by `SettingToggleRow`) |
| `ag-badge` + `ag-badge-success` / `-danger` / `-warn` / `-info` / `-neutral` | status pills (rendered by `StatusBadge`) |
| `ag-grid-toolbar`, `ag-search`, `ag-search-icon`, `ag-search-input`, `ag-grid-count` | in-card grid toolbar: filter box + row count (rendered by `GridToolbar`, inside `DataCard`) |
| `ag-info-box`, `ag-info-box-label`, `ag-info-box-value` | inline soft-tinted info pill (rendered by `InfoBox`) |
| `ag-empty`, `ag-empty-icon`, `ag-empty-title`, `ag-empty-text` | dashed empty state (rendered by `EmptyState`) |
| `ag-row-actions`, `ag-icon-btn` | grid row actions (rendered by `RowActions`) + compact `⋯` button (rendered by `IconButton`) |
| `ag-cell-name`, `ag-cell-id` | name/email + truncated mono id in a grid cell |
| `ag-login-*` | login-card building blocks (provided by `LoginLayout`) |
| `ag-shell`, `ag-sidebar`, `ag-topbar`, `ag-topbar-left`, `ag-nav-item`, `ag-theme-toggle`, `ag-hamburger`, `ag-backdrop`, … | app shell (rendered by `AppShell`) |
| `ag-lang-toggle`, `ag-lang-btn` | language switch pill (rendered by `LanguageSwitcher`, §9) |

### App shell

The host layout renders an **`AppShell`** rather than hand-writing the shell markup.
`AppShell` provides the sidebar / topbar / content structure and the responsive
off-canvas drawer, and owns the breadcrumb state; the host supplies branding and
fills the slots:

```razor
<AppShell BrandName="@AppName">
    <Sidebar>@* NavLinks + ag-nav-section groups *@</Sidebar>
    <TopbarActions>@* language switcher, theme toggle, user chip, sign-in/out *@</TopbarActions>
    <SidebarFooter>@* optional *@</SidebarFooter>
    <ChildContent>@Body</ChildContent>
</AppShell>
```

The host only needs to load `nav.js` (see the Aspire sample's `App.razor`);
`AppShell` emits the hamburger, backdrop and `data-nav-open` itself.

### Page title & brand

Every routed page uses `AppPageTitle` to set the document `<title>`. Register the
brand once at startup and pages don't repeat it:

```csharp
builder.Services.AddDesignBlazor(o => o.BrandName = "Acme");
```

```razor
<AppPageTitle Title="Profile" Breadcrumb="Account / Profile" />
@* document title → "Profile – Acme" *@
```

The title is `"{Title} – {brand}"`, where the brand is an explicit `Suffix` on the
page if given, otherwise the configured `BrandName`; with neither, it's just
`Title`. Pass `Suffix` only to override the brand for one page. Options are
resolved optionally, so a page still renders (without a suffix) when no brand is
configured.

> **Migration.** `AppPageTitle` now reads the brand from `DesignBlazorOptions`, so
> the per-app wrapper components (`AppFoundationPageTitle`,
> `IdentityPageTitle`) are redundant: register the brand via
> `AddDesignBlazor(o => o.BrandName = …)` (mapping the existing
> `AppFoundationLayoutOptions.BrandName` / `MartenIdentityBlazorOptions.ApplicationName`),
> replace `<AppFoundationPageTitle …>` / `<IdentityPageTitle …>` with
> `<AppPageTitle …>`, and delete the wrapper. Tracked in each repo's adoption issue.

### Topbar breadcrumb

The topbar crumb is **defined per page, not by the layout**. Each shell page sets
it through `AppPageTitle`'s optional `Breadcrumb` parameter, in sentence case with
the section prefix:

```razor
<AppPageTitle Title="Profile" Breadcrumb="Account / Profile" />
```

`AppPageTitle` pushes the value into a cascading `BreadcrumbState` that `AppShell`
owns and renders in `ag-topbar-title`; the shell re-renders when it changes. Omit
`Breadcrumb` and the document `Title` is used as a fallback. Pages outside the
shell (the `LoginLayout` auth pages) have no `BreadcrumbState` cascaded, so they
simply ignore the parameter.

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
- **Radzen's neutral ramp and grid stripes have their own light defaults** that
  `--rz-text-color`/`--rz-border-color` don't cover: `--rz-base` / `--rz-on-base`
  (backs `ButtonStyle.Base`, which `DialogService.Confirm()` uses for its Cancel
  button — unmapped, every confirm dialog showed a white button), `--rz-base-200`
  (backs `BadgeStyle.Light` — unmapped, measured 1.2:1 contrast), and
  `--rz-grid-stripe-background-color` / `--rz-grid-stripe-odd-background-color`
  (alternating grid rows — unmapped, every second row stayed white). All three
  are remapped onto the token palette.
- **A `RadzenDataGrid`'s `EmptyText` renders an unstyled `<td class="rz-datatable-emptymessage">`**
  — nothing upstream styles it, so it's given deliberate padding/colour/centring.
  Because `.rz-density-compact .rz-grid-table td` (2 classes + a type selector) has
  higher specificity, a compact grid needs its own override
  (`.rz-density-compact .rz-grid-table td.rz-datatable-emptymessage`) or the
  compact padding wins and crushes the empty state back down.
- **`RadzenDropDown`'s panel-item hover/selected colours are their own variables**
  (`--rz-dropdown-item-*`), not `--rz-primary` — same class of leak as the switch/
  checkbox checked state below. Unmapped, the selected item's text stayed Radzen's
  default indigo and hover stayed white.
- **A closed dropdown's inner value label carries `.rz-inputtext`**, so the
  generic input-border rule drew a second border *inside* the dropdown's own
  frame. Stripped via `.rz-dropdown .rz-dropdown-label.rz-inputtext`.
- **Switches/checkboxes have their own "checked" colour variables** (not
  `--rz-primary`), so the on-state stays Radzen's default indigo unless
  `--rz-switch-checked-*` / `--rz-checkbox-checked-*` are mapped onto the accent.
- **Blazor's enhanced navigation morphs `<html>` to the server response**, which
  has no `data-theme` — wiping the theme on every page change. `theme.js` runs a
  `MutationObserver` on `data-theme` that re-applies the resolved preference
  whenever the attribute is removed.

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
- [ ] Any text the component renders on its own comes from the design-system
      resources via `Services.DesignText("Key")` (§9), with a nullable parameter so
      a host can still pass its own text. Do **not** `@inject IStringLocalizer`
      directly — that is a required injection and breaks hosts and tests that never
      registered localization.

---

## 9. Localization

The library ships English and German out of the box: every string a component
renders on its own (`CardForm`'s submit/cancel/busy text, `FilterBar`'s apply
button, `GridToolbar`'s search placeholder, `ThemeToggle`'s labels, `AppShell`'s
hamburger `aria-label`, `ConfirmService`'s dialog text) comes from
the design-system resources — see
[`Resources/DesignStrings.resx`](Resources/DesignStrings.resx) /
[`DesignStrings.de.resx`](Resources/DesignStrings.de.resx).

Components read them through `Services.DesignText("Key")`, which prefers a registered
`IStringLocalizer<DesignStrings>` (so a host can substitute one) and otherwise falls
back to the embedded resources directly, keyed off `CultureInfo.CurrentUICulture`.
That indirection is deliberate: injecting `IStringLocalizer` into a component makes it
a *required* service, so a host — or a consuming repo's bUnit test — that never
registered localization would throw merely from rendering a `CardForm`. A host that
never touches localization at all still gets correct English, and a host that sets up
request localization gets German, in both cases without registering anything extra.

### Setup

```csharp
builder.Services.AddDesignBlazor(o =>
{
    o.DefaultCulture = "en";          // the default; only set if you want another
    o.SupportedCultures = ["en", "de"]; // the default; list the cultures you offer
});
```

```csharp
// Before MapRazorComponents — the request that establishes a Blazor Server
// circuit has to already carry the resolved culture.
app.UseDesignBlazorLocalization();
```

`UseDesignBlazorLocalization` turns on `RequestLocalizationMiddleware` and maps
the culture-switch endpoint (`GET /ag-culture?c={culture}&redirect={url}`) that
`LanguageSwitcher` links to. The resolved culture, in order, is: a culture cookie
the visitor previously chose, then the browser's `Accept-Language` header, then
`DefaultCulture`. An explicit choice always wins over the browser's preference —
otherwise a visitor who deliberately picked English would be switched back on
every request.

### `LanguageSwitcher`

Drop it in `TopbarActions` next to `ThemeToggle`:

```razor
<AppShell BrandName="@AppName">
    <TopbarActions>
        <LanguageSwitcher />
        <ThemeToggle />
    </TopbarActions>
    ...
</AppShell>
```

It renders one item per configured culture, labelled with the culture's own
native name (its *endonym* — "Deutsch", not "German" translated into whatever's
currently active), so adding a culture to `SupportedCultures` needs no resource
entry. The items are plain anchors, not buttons: the switch has to work without a
Blazor circuit (e.g. on statically rendered pages), and it survives Blazor's
enhanced navigation only because each link carries `data-enhance-nav="false"` —
without it, enhanced navigation would morph in the response without rebuilding the
circuit, leaving the UI rendered in the old culture despite the cookie being set.

Because switching culture only takes effect on the request that (re-)builds the
circuit, this is a full page load, not a live update — by design, not a
limitation to work around.

### A host app's own strings

`DesignStrings` covers only the library's own built-in text. A host localizes its
*own* pages the same way, with its *own* resource pair, separate from the
library's — an app's translation work shouldn't be entangled with the design
system's:

```csharp
// Resources/Strings.cs
namespace YourApp.Resources;
public sealed class Strings;
```

```
Resources/Strings.resx      (English, neutral — the source of truth)
Resources/Strings.de.resx
```

```razor
@inject IStringLocalizer<Strings> L
<PageHeader Title="@L["Profile.Title"]" Subtitle="@L["Profile.Subtitle"]" />
```

Key convention: `{Area}.{Purpose}` (e.g. `DataGrid.SearchUsers`,
`Confirm.DeleteMessage`). The sample app
([`samples/AndreGoepel.Design.Blazor.Demo`](../../samples/AndreGoepel.Design.Blazor.Demo))
is translated end-to-end this way and doubles as the reference implementation —
copy its `Program.cs`, `App.razor`, and `Resources/` wiring.

> **Gotcha.** Don't compare against a *displayed* (localized) string to drive
> logic — e.g. a status filter or a bound dropdown value. Once the display text
> changes with the culture, an English literal comparison silently stops
> matching in German. Bind a stable, culture-invariant key (`"active"`, not
> `L["DataGrid.Active"]`) and only use the localized string for the label; see
> `DataGrid.razor`'s `StatusOptions` in the sample app.
