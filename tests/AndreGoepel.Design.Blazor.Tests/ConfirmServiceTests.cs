using AndreGoepel.Design.Blazor;
using AndreGoepel.Design.Blazor.Resources;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests;

public class ConfirmServiceTests : DesignBlazorTestContext
{
    private IStringLocalizer<DesignStrings> Localizer =>
        Services.GetRequiredService<IStringLocalizer<DesignStrings>>();

    // Radzen's DialogService.Confirm/Alert are virtual, so NSubstitute can override
    // them. Its ctor needs a NavigationManager + IJSRuntime — bUnit supplies fakes.
    private DialogService MockDialog(bool? confirmResult)
    {
        var dialog = Substitute.For<DialogService>(
            Services.GetRequiredService<NavigationManager>(),
            JSInterop.JSRuntime
        );
        dialog
            .Confirm(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ConfirmOptions>())
            .Returns(Task.FromResult(confirmResult));
        return dialog;
    }

    private DialogService MockAlertDialog()
    {
        var dialog = Substitute.For<DialogService>(
            Services.GetRequiredService<NavigationManager>(),
            JSInterop.JSRuntime
        );
        dialog
            .Alert(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<AlertOptions>())
            .Returns(Task.FromResult<bool?>(true));
        return dialog;
    }

    [Fact]
    public async Task ConfirmAsync_WhenConfirmed_ReturnsTrue()
    {
        var svc = new ConfirmService(MockDialog(true));

        Assert.True(await svc.ConfirmAsync("Delete?"));
    }

    [Fact]
    public async Task ConfirmAsync_WhenCancelled_ReturnsFalse()
    {
        var svc = new ConfirmService(MockDialog(false));

        Assert.False(await svc.ConfirmAsync("Delete?"));
    }

    [Fact]
    public async Task ConfirmAsync_WhenDismissed_ReturnsFalse()
    {
        // Radzen returns null when the dialog is dismissed without a choice.
        var svc = new ConfirmService(MockDialog(null));

        Assert.False(await svc.ConfirmAsync("Delete?"));
    }

    [Fact]
    public async Task ConfirmAsync_PassesMessageTitleAndButtonTextThrough()
    {
        var dialog = MockDialog(true);
        var svc = new ConfirmService(dialog);

        await svc.ConfirmAsync("Really?", "My title", okText: "Yes", cancelText: "No");

        await dialog
            .Received(1)
            .Confirm(
                "Really?",
                "My title",
                Arg.Is<ConfirmOptions>(o =>
                    o != null && o.OkButtonText == "Yes" && o.CancelButtonText == "No"
                )
            );
    }

    [Fact]
    public async Task ConfirmDeleteAsync_UsesStandardMessageAndDeleteButton()
    {
        var dialog = MockDialog(true);
        var svc = new ConfirmService(dialog);

        var result = await svc.ConfirmDeleteAsync("Ada Lovelace");

        Assert.True(result);
        await dialog
            .Received(1)
            .Confirm(
                "Delete Ada Lovelace? This cannot be undone.",
                "Delete",
                Arg.Is<ConfirmOptions>(o => o != null && o.OkButtonText == "Delete")
            );
    }

    [Fact]
    public async Task ConfirmAsync_WithLocalizer_UsesTheCurrentCultureForTitleAndButtons()
    {
        UseCulture("de");
        var dialog = MockDialog(true);
        var svc = new ConfirmService(dialog, Localizer);

        await svc.ConfirmAsync("Wirklich?");

        await dialog
            .Received(1)
            .Confirm(
                "Wirklich?",
                "Bestätigen",
                Arg.Is<ConfirmOptions>(o =>
                    o != null && o.OkButtonText == "OK" && o.CancelButtonText == "Abbrechen"
                )
            );
    }

    [Fact]
    public async Task ConfirmDeleteAsync_WithLocalizer_FormatsTheGermanMessage()
    {
        UseCulture("de");
        var dialog = MockDialog(true);
        var svc = new ConfirmService(dialog, Localizer);

        await svc.ConfirmDeleteAsync("Ada Lovelace");

        await dialog
            .Received(1)
            .Confirm(
                "Ada Lovelace löschen? Das kann nicht rückgängig gemacht werden.",
                "Löschen",
                Arg.Is<ConfirmOptions>(o => o != null && o.OkButtonText == "Löschen")
            );
    }

    [Fact]
    public async Task ConfirmAsync_WithLocalizer_StillHonoursExplicitText()
    {
        UseCulture("de");
        var dialog = MockDialog(true);
        var svc = new ConfirmService(dialog, Localizer);

        await svc.ConfirmAsync("Really?", "My title", okText: "Yes", cancelText: "No");

        await dialog
            .Received(1)
            .Confirm(
                "Really?",
                "My title",
                Arg.Is<ConfirmOptions>(o =>
                    o != null && o.OkButtonText == "Yes" && o.CancelButtonText == "No"
                )
            );
    }

    [Fact]
    public async Task ConfirmAsync_WithoutLocalizer_FallsBackToEnglish()
    {
        // Hosts that construct the service directly get no localizer; that must not
        // throw or produce empty dialog text.
        UseCulture("de");
        var dialog = MockDialog(true);
        var svc = new ConfirmService(dialog);

        await svc.ConfirmAsync("Really?");

        await dialog
            .Received(1)
            .Confirm(
                "Really?",
                "Confirm",
                Arg.Is<ConfirmOptions>(o =>
                    o != null && o.OkButtonText == "OK" && o.CancelButtonText == "Cancel"
                )
            );
    }

    [Fact]
    public async Task AlertAsync_UsesDefaultTitleAndOkButton()
    {
        var dialog = MockAlertDialog();
        var svc = new ConfirmService(dialog);

        await svc.AlertAsync("Something happened.");

        await dialog
            .Received(1)
            .Alert(
                "Something happened.",
                "Alert",
                Arg.Is<AlertOptions>(o => o != null && o.OkButtonText == "OK")
            );
    }

    [Fact]
    public async Task AlertAsync_PassesTitleAndOkTextThrough()
    {
        var dialog = MockAlertDialog();
        var svc = new ConfirmService(dialog);

        await svc.AlertAsync("You're all set.", "Done", okText: "Got it");

        await dialog
            .Received(1)
            .Alert(
                "You're all set.",
                "Done",
                Arg.Is<AlertOptions>(o => o != null && o.OkButtonText == "Got it")
            );
    }

    [Fact]
    public async Task AlertAsync_WithLocalizer_UsesTheCurrentCultureForTitleAndButton()
    {
        UseCulture("de");
        var dialog = MockAlertDialog();
        var svc = new ConfirmService(dialog, Localizer);

        await svc.AlertAsync("Etwas ist passiert.");

        await dialog
            .Received(1)
            .Alert(
                "Etwas ist passiert.",
                "Hinweis",
                Arg.Is<AlertOptions>(o => o != null && o.OkButtonText == "OK")
            );
    }

    [Fact]
    public async Task AlertAsync_WithoutLocalizer_FallsBackToEnglish()
    {
        UseCulture("de");
        var dialog = MockAlertDialog();
        var svc = new ConfirmService(dialog);

        await svc.AlertAsync("Something happened.");

        await dialog
            .Received(1)
            .Alert(
                "Something happened.",
                "Alert",
                Arg.Is<AlertOptions>(o => o != null && o.OkButtonText == "OK")
            );
    }
}
