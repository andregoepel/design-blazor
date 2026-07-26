using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class NavSectionTests : BunitContext
{
    [Fact]
    public void Render_ShowsTitleInSectionDiv()
    {
        // Act
        var cut = Render<NavSection>(parameters => parameters.Add(p => p.Title, "Account"));

        // Assert
        Assert.Equal("Account", cut.Find(".ag-nav-section").TextContent);
    }

    [Fact]
    public void Render_WrapsChildContentInNavGroupDiv()
    {
        // Act
        var cut = Render<NavSection>(parameters =>
            parameters.Add(p => p.Title, "Account").AddChildContent("<a>Profile</a>")
        );

        // Assert
        var group = cut.Find(".ag-nav-group");
        Assert.Equal("Profile", group.QuerySelector("a")?.TextContent);
    }

    [Fact]
    public void Render_WithoutChildContent_StillRendersEmptyGroupDiv()
    {
        // Act
        var cut = Render<NavSection>(parameters => parameters.Add(p => p.Title, "Account"));

        // Assert
        Assert.NotNull(cut.Find(".ag-nav-group"));
    }
}
