using AndreGoepel.Design.Blazor;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests;

public class ConfirmServiceTests : BunitContext
{
    // Radzen's DialogService.Confirm is virtual, so NSubstitute can override it.
    // Its ctor needs a NavigationManager + IJSRuntime — bUnit supplies fakes.
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
}
