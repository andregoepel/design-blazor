using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class LinkButtonTests : BunitContext
{
    public LinkButtonTests() => Services.AddRadzenComponents();

    [Fact]
    public void Render_ShowsText()
    {
        var cut = Render<LinkButton>(parameters =>
            parameters.Add(p => p.Text, "Back to overview").Add(p => p.Path, "/")
        );

        Assert.Contains("Back to overview", cut.Find("button").TextContent);
    }

    [Fact]
    public void Click_NavigatesToPath()
    {
        var cut = Render<LinkButton>(parameters =>
            parameters.Add(p => p.Text, "Roles").Add(p => p.Path, "administration/roles")
        );

        cut.Find("button").Click();

        var nav = Services.GetRequiredService<NavigationManager>();
        Assert.EndsWith("administration/roles", nav.Uri);
    }

    [Fact]
    public void Render_DefaultButtonStyle_IsPrimary()
    {
        var cut = Render<LinkButton>(parameters =>
            parameters.Add(p => p.Text, "Roles").Add(p => p.Path, "/roles")
        );

        Assert.Contains("rz-primary", cut.Find("button").ClassList);
    }

    [Fact]
    public void Render_WithCustomButtonStyle_UsesIt()
    {
        var cut = Render<LinkButton>(parameters =>
            parameters
                .Add(p => p.Text, "Cancel")
                .Add(p => p.Path, "/")
                .Add(p => p.ButtonStyle, ButtonStyle.Light)
        );

        Assert.Contains("rz-light", cut.Find("button").ClassList);
    }
}
