using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class DialogHeaderTests : BunitContext
{
    [Fact]
    public void Render_TitleOnly_ShowsHeadingAndNoSubtitle()
    {
        // Act
        var cut = Render<DialogHeader>(parameters => parameters.Add(p => p.Title, "New customer"));

        // Assert
        Assert.Equal("New customer", cut.Find("h2").TextContent);
        Assert.Empty(cut.FindAll("p.rz-text-body2"));
    }

    [Fact]
    public void Render_WithSubtitle_ShowsSubtitleText()
    {
        // Act
        var cut = Render<DialogHeader>(parameters =>
            parameters
                .Add(p => p.Title, "New customer")
                .Add(p => p.Subtitle, "Add a customer to the studio's portal.")
        );

        // Assert
        Assert.Contains("Add a customer to the studio's portal.", cut.Markup);
    }

    [Fact]
    public void Render_WithoutSubtitle_OmitsSubtitleParagraph()
    {
        // Act
        var cut = Render<DialogHeader>(parameters => parameters.Add(p => p.Title, "New customer"));

        // Assert
        Assert.DoesNotContain("rz-text-body2", cut.Markup);
    }

    [Fact]
    public void Render_UsesAgDialogHeaderClass()
    {
        // Act
        var cut = Render<DialogHeader>(parameters => parameters.Add(p => p.Title, "New customer"));

        // Assert
        Assert.NotNull(cut.Find(".ag-dialog-header"));
    }
}
