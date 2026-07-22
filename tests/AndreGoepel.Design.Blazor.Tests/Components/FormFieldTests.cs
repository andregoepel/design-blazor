using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class FormFieldTests : BunitContext
{
    [Fact]
    public void Render_ShowsLabel()
    {
        // Act
        var cut = Render<FormField>(parameters => parameters.Add(p => p.Label, "Email"));

        // Assert
        Assert.Equal("Email", cut.Find("label").TextContent);
    }

    [Fact]
    public void Render_WithFor_LinksLabelToControlName()
    {
        // Act
        var cut = Render<FormField>(parameters =>
            parameters.Add(p => p.Label, "Email").Add(p => p.For, "Email")
        );

        // Assert
        Assert.Equal("Email", cut.Find("label").GetAttribute("for"));
    }

    [Fact]
    public void Render_RendersChildContent()
    {
        // Act
        var cut = Render<FormField>(parameters =>
            parameters.Add(p => p.Label, "Email").AddChildContent("<input name=\"Email\" />")
        );

        // Assert
        Assert.NotNull(cut.Find("input[name=Email]"));
    }

    [Fact]
    public void Render_WithHint_ShowsHintCaption()
    {
        // Act
        var cut = Render<FormField>(parameters =>
            parameters
                .Add(p => p.Label, "Email")
                .Add(p => p.Hint, "We'll only use this to contact you.")
        );

        // Assert
        Assert.Contains("We'll only use this to contact you.", cut.Markup);
    }

    [Fact]
    public void Render_WithoutHint_OmitsCaption()
    {
        // Act
        var cut = Render<FormField>(parameters => parameters.Add(p => p.Label, "Email"));

        // Assert
        Assert.Empty(cut.FindAll(".rz-text-caption"));
    }

    [Fact]
    public void Render_Required_ShowsAsteriskAfterLabel()
    {
        // Act
        var cut = Render<FormField>(parameters =>
            parameters.Add(p => p.Label, "Email").Add(p => p.Required, true)
        );

        // Assert
        Assert.Contains("Email", cut.Find("label").TextContent);
        Assert.NotNull(cut.Find("label .ag-required"));
    }

    [Fact]
    public void Render_NotRequired_OmitsAsterisk()
    {
        // Act
        var cut = Render<FormField>(parameters => parameters.Add(p => p.Label, "Email"));

        // Assert
        Assert.Empty(cut.FindAll(".ag-required"));
    }
}
