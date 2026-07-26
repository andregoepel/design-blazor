using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class DialogFormTests : DesignBlazorTestContext
{
    public sealed class Model
    {
        public string? Name { get; set; }
    }

    public DialogFormTests()
    {
        // RadzenTemplateForm / RadzenButton / DialogService resolve Radzen services.
        Services.AddRadzenComponents();
    }

    private IRenderedComponent<DialogForm<Model>> RenderForm(
        Model model,
        Action<ComponentParameterCollectionBuilder<DialogForm<Model>>>? extra = null
    ) =>
        Render<DialogForm<Model>>(parameters =>
        {
            parameters.Add(p => p.Data, model);
            extra?.Invoke(parameters);
        });

    [Fact]
    public void Render_ShowsSubmitButtonWithDefaultText()
    {
        var cut = RenderForm(new Model());

        var buttons = cut.FindAll(".ag-card-actions button");
        Assert.Contains(buttons, b => b.TextContent.Contains("Save changes"));
    }

    [Fact]
    public void Render_WithCustomSubmitText_UsesIt()
    {
        var cut = RenderForm(new Model(), p => p.Add(c => c.SubmitText, "Create customer"));

        Assert.Contains(
            cut.FindAll(".ag-card-actions button"),
            b => b.TextContent.Contains("Create customer")
        );
    }

    [Fact]
    public void Render_AlwaysShowsCancelButton()
    {
        // Unlike CardForm, DialogForm always shows Cancel — it has a default
        // close-the-dialog action even without an explicit handler.
        var cut = RenderForm(new Model());

        var buttons = cut.FindAll(".ag-card-actions button");
        Assert.Equal(2, buttons.Count);
        Assert.Contains(buttons, b => b.TextContent.Contains("Cancel"));
    }

    [Fact]
    public void Render_DoesNotRenderRadzenCardWrapper()
    {
        var cut = RenderForm(new Model());

        Assert.Empty(cut.FindAll(".rz-card"));
    }

    [Fact]
    public void Render_WhenBusy_SubmitShowsBusyText()
    {
        var cut = RenderForm(
            new Model(),
            p => p.Add(c => c.IsBusy, true).Add(c => c.BusyText, "Saving…")
        );

        Assert.Contains("Saving…", cut.Markup);
    }

    [Fact]
    public void Render_WhenBusy_CancelButtonIsDisabled()
    {
        var cut = RenderForm(new Model(), p => p.Add(c => c.IsBusy, true));

        var cancel = cut.Find(".ag-card-actions button[type=button]");
        Assert.True(cancel.HasAttribute("disabled"));
    }

    [Fact]
    public void Render_Default_SubmitButtonIsPrimary()
    {
        var cut = RenderForm(new Model());

        var submit = cut.Find(".ag-card-actions button[type=submit]");
        Assert.Contains("rz-primary", submit.ClassList);
        Assert.DoesNotContain("rz-danger", submit.ClassList);
    }

    [Fact]
    public void Render_WithDanger_SubmitButtonIsDanger()
    {
        var cut = RenderForm(new Model(), p => p.Add(c => c.Danger, true));

        var submit = cut.Find(".ag-card-actions button[type=submit]");
        Assert.Contains("rz-danger", submit.ClassList);
        Assert.DoesNotContain("rz-primary", submit.ClassList);
    }

    [Fact]
    public void Submit_WhenFormSubmitted_RaisesSubmitWithData()
    {
        var model = new Model { Name = "Ada" };
        Model? received = null;
        var cut = RenderForm(
            model,
            p => p.Add(c => c.Submit, EventCallback.Factory.Create<Model>(this, m => received = m))
        );

        cut.Find("form").Submit();

        Assert.Same(model, received);
    }

    // DialogService.Close only takes effect through the internal state a real open
    // dialog sets up — calling it on the plain service bUnit registers (with no
    // dialog actually opened via OpenAsync) never raises OnClose. Close is virtual,
    // so substitute it and assert the call instead, matching ConfirmServiceTests'
    // pattern for the same service. The substitute is built inside the registration
    // factory (not eagerly beforehand) because bUnit locks its service collection
    // against further registrations as soon as any service has been resolved, and
    // resolving NavigationManager up front to build the substitute would trip that.
    private void RegisterMockDialog() =>
        Services.AddScoped(sp =>
            Substitute.For<DialogService>(
                sp.GetRequiredService<NavigationManager>(),
                JSInterop.JSRuntime
            )
        );

    [Fact]
    public void Cancel_WithoutCustomHandler_ClosesDialogWithNull()
    {
        // Arrange
        RegisterMockDialog();
        var cut = RenderForm(new Model());
        var dialogService = Services.GetRequiredService<DialogService>();

        // Act
        cut.Find(".ag-card-actions button[type=button]").Click();

        // Assert
        dialogService.Received(1).Close(null);
    }

    [Fact]
    public void Cancel_WithCustomHandler_InvokesItInsteadOfClosingDialog()
    {
        // Arrange
        RegisterMockDialog();
        var cancelled = false;
        var cut = RenderForm(
            new Model(),
            p => p.Add(c => c.Cancel, EventCallback.Factory.Create(this, () => cancelled = true))
        );
        var dialogService = Services.GetRequiredService<DialogService>();

        // Act
        cut.Find(".ag-card-actions button[type=button]").Click();

        // Assert
        Assert.True(cancelled);
        dialogService.DidNotReceive().Close(Arg.Any<object?>());
    }

    [Fact]
    public void Render_InGerman_ShowsGermanSubmitAndCancelText()
    {
        UseCulture("de");

        var cut = RenderForm(new Model());

        var buttons = cut.FindAll(".ag-card-actions button");
        Assert.Contains(buttons, b => b.TextContent.Contains("Änderungen speichern"));
        Assert.Contains(buttons, b => b.TextContent.Contains("Abbrechen"));
    }
}
