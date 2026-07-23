using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class LoginShellTests : DesignBlazorTestContext
{
    [Fact]
    public void Render_ShowsBrandNameAndDerivedInitial()
    {
        var cut = Render<LoginShell>(parameters => parameters.Add(p => p.BrandName, "Acme"));

        Assert.Equal("Acme", cut.Find(".ag-brand-name").TextContent);
        Assert.Equal("A", cut.Find(".ag-brand-mark").TextContent);
    }

    [Fact]
    public void Render_ShowsThemeToggleByDefault()
    {
        // Every app's auth pages get a light/dark control out of the box,
        // matching the AppShell topbar.
        var cut = Render<LoginShell>(parameters => parameters.Add(p => p.BrandName, "Acme"));

        Assert.NotNull(cut.Find(".ag-login-topright .ag-theme-toggle"));
    }

    [Fact]
    public void Render_WithShowThemeToggleFalse_HidesTheToggleAndItsContainer()
    {
        var cut = Render<LoginShell>(parameters =>
            parameters.Add(p => p.BrandName, "Acme").Add(p => p.ShowThemeToggle, false)
        );

        Assert.Empty(cut.FindAll(".ag-login-topright"));
    }

    [Fact]
    public void Render_TopRightActionsRenderBeforeTheThemeToggle()
    {
        var cut = Render<LoginShell>(parameters =>
            parameters
                .Add(p => p.BrandName, "Acme")
                .Add(p => p.TopRightActions, "<span id=\"extra\">x</span>")
        );

        var topRight = cut.Find(".ag-login-topright");
        Assert.NotNull(topRight.QuerySelector("#extra"));
        var children = topRight.Children;
        Assert.Equal("extra", children[0].Id);
        Assert.Contains("ag-theme-toggle", children[^1].ClassName);
    }

    [Fact]
    public void Render_WithOnlyTopRightActions_StillRendersTheContainer()
    {
        var cut = Render<LoginShell>(parameters =>
            parameters
                .Add(p => p.BrandName, "Acme")
                .Add(p => p.ShowThemeToggle, false)
                .Add(p => p.TopRightActions, "<span id=\"extra\">x</span>")
        );

        Assert.NotNull(cut.Find(".ag-login-topright #extra"));
        Assert.Empty(cut.FindAll(".ag-theme-toggle"));
    }
}
