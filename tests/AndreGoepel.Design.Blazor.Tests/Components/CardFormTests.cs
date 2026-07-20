using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class CardFormTests : DesignBlazorTestContext
{
    public sealed class Model
    {
        public string? Name { get; set; }
    }

    public CardFormTests()
    {
        // RadzenTemplateForm / RadzenButton resolve Radzen services.
        Services.AddRadzenComponents();
    }

    private IRenderedComponent<CardForm<Model>> RenderForm(
        Model model,
        Action<ComponentParameterCollectionBuilder<CardForm<Model>>>? extra = null
    ) =>
        Render<CardForm<Model>>(parameters =>
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
        var cut = RenderForm(new Model(), p => p.Add(c => c.SubmitText, "Create user"));

        Assert.Contains(
            cut.FindAll(".ag-card-actions button"),
            b => b.TextContent.Contains("Create user")
        );
    }

    [Fact]
    public void Render_WithoutCancel_ShowsOnlyOneAction()
    {
        var cut = RenderForm(new Model());

        Assert.Single(cut.FindAll(".ag-card-actions button"));
    }

    [Fact]
    public void Render_WithCancel_ShowsCancelButton()
    {
        var cut = RenderForm(
            new Model(),
            p => p.Add(c => c.Cancel, EventCallback.Factory.Create(this, () => { }))
        );

        var buttons = cut.FindAll(".ag-card-actions button");
        Assert.Equal(2, buttons.Count);
        Assert.Contains(buttons, b => b.TextContent.Contains("Cancel"));
    }

    [Fact]
    public void Render_WithTitle_ShowsHeading()
    {
        var cut = RenderForm(new Model(), p => p.Add(c => c.Title, "Profile"));

        Assert.Contains("Profile", cut.Find(".rz-card h2").TextContent);
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
    public void Render_InGerman_ShowsGermanSubmitAndCancelText()
    {
        UseCulture("de");

        var cut = RenderForm(
            new Model(),
            p => p.Add(c => c.Cancel, EventCallback.Factory.Create(this, () => { }))
        );

        var buttons = cut.FindAll(".ag-card-actions button");
        Assert.Contains(buttons, b => b.TextContent.Contains("Änderungen speichern"));
        Assert.Contains(buttons, b => b.TextContent.Contains("Abbrechen"));
    }

    [Fact]
    public void Render_InGerman_WhenBusy_ShowsGermanBusyText()
    {
        UseCulture("de");

        var cut = RenderForm(new Model(), p => p.Add(c => c.IsBusy, true));

        Assert.Contains("Wird gespeichert…", cut.Markup);
    }

    [Fact]
    public void Render_InGerman_WithCustomSubmitText_KeepsTheHostsText()
    {
        UseCulture("de");

        var cut = RenderForm(new Model(), p => p.Add(c => c.SubmitText, "Create user"));

        var buttons = cut.FindAll(".ag-card-actions button");
        Assert.Contains(buttons, b => b.TextContent.Contains("Create user"));
        Assert.DoesNotContain(buttons, b => b.TextContent.Contains("Änderungen speichern"));
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
}
