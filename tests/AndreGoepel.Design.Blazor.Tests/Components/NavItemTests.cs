using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class NavItemTests : BunitContext
{
    [Fact]
    public void Render_ShowsTextAsLinkContent()
    {
        // Act
        var cut = Render<NavItem>(parameters =>
            parameters.Add(p => p.Href, "account/profile").Add(p => p.Text, "Profile")
        );

        // Assert
        Assert.Contains("Profile", cut.Find("a").TextContent);
    }

    [Fact]
    public void Render_SetsHrefAttribute()
    {
        // Act
        var cut = Render<NavItem>(parameters =>
            parameters.Add(p => p.Href, "account/profile").Add(p => p.Text, "Profile")
        );

        // Assert
        Assert.EndsWith("account/profile", cut.Find("a").GetAttribute("href"));
    }

    [Fact]
    public void Render_UsesAgNavItemClass()
    {
        // Act
        var cut = Render<NavItem>(parameters =>
            parameters.Add(p => p.Href, "account/profile").Add(p => p.Text, "Profile")
        );

        // Assert
        Assert.Contains("ag-nav-item", cut.Find("a").ClassList);
    }

    [Fact]
    public void Render_WithoutIcon_RendersNoSvg()
    {
        // Act
        var cut = Render<NavItem>(parameters =>
            parameters.Add(p => p.Href, "account/profile").Add(p => p.Text, "Profile")
        );

        // Assert
        Assert.Empty(cut.FindAll("svg"));
    }

    [Fact]
    public void Render_WithIcon_RendersAppIconSvg()
    {
        // Act
        var cut = Render<NavItem>(parameters =>
            parameters
                .Add(p => p.Href, "account/profile")
                .Add(p => p.Text, "Profile")
                .Add(p => p.Icon, "key")
        );

        // Assert
        Assert.NotNull(cut.Find("a svg"));
    }

    [Fact]
    public void Render_DefaultMatch_IsActiveOnDeeperPath()
    {
        // Prefix (NavItem's default) matches any path starting with Href, same as NavLink's own default.
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("account/profile/edit");

        // Act
        var cut = Render<NavItem>(parameters =>
            parameters.Add(p => p.Href, "account/profile").Add(p => p.Text, "Profile")
        );

        // Assert
        Assert.Contains("active", cut.Find("a").ClassList);
    }

    [Fact]
    public void Render_MatchAll_IsNotActiveOnDeeperPath()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("account/profile/edit");

        // Act
        var cut = Render<NavItem>(parameters =>
            parameters
                .Add(p => p.Href, "account/profile")
                .Add(p => p.Text, "Profile")
                .Add(p => p.Match, NavLinkMatch.All)
        );

        // Assert
        Assert.DoesNotContain("active", cut.Find("a").ClassList);
    }

    [Fact]
    public void Render_MatchAll_IsActiveOnExactPath()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("account/profile");

        // Act
        var cut = Render<NavItem>(parameters =>
            parameters
                .Add(p => p.Href, "account/profile")
                .Add(p => p.Text, "Profile")
                .Add(p => p.Match, NavLinkMatch.All)
        );

        // Assert
        Assert.Contains("active", cut.Find("a").ClassList);
    }
}
