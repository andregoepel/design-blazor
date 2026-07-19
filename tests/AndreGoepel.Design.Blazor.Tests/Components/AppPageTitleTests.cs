using AndreGoepel.Design.Blazor;
using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class AppPageTitleTests : BunitContext
{
    public AppPageTitleTests() => JSInterop.Mode = JSRuntimeMode.Loose; // <PageTitle> pokes JS

    // --- Document-title composition (the Suffix-vs-BrandName precedence) ---

    [Fact]
    public void Compose_WithNeitherSuffixNorBrand_IsJustTitle() =>
        Assert.Equal("Profile", AppPageTitle.Compose("Profile", suffix: null, brandName: null));

    [Fact]
    public void Compose_WithBrandOnly_AppendsBrand() =>
        Assert.Equal(
            "Profile – Acme",
            AppPageTitle.Compose("Profile", suffix: null, brandName: "Acme")
        );

    [Fact]
    public void Compose_WithExplicitSuffix_AppendsSuffix() =>
        Assert.Equal(
            "Profile – Login",
            AppPageTitle.Compose("Profile", suffix: "Login", brandName: null)
        );

    [Fact]
    public void Compose_ExplicitSuffix_WinsOverBrand() =>
        Assert.Equal(
            "Profile – Login",
            AppPageTitle.Compose("Profile", suffix: "Login", brandName: "Acme")
        );

    [Fact]
    public void Compose_WhitespaceSuffix_FallsBackToBrand() =>
        Assert.Equal(
            "Profile – Acme",
            AppPageTitle.Compose("Profile", suffix: "  ", brandName: "Acme")
        );

    // --- Breadcrumb behaviour (observable via the cascaded BreadcrumbState) ---

    [Fact]
    public void Render_SetsBreadcrumbFromParameter()
    {
        var crumb = new BreadcrumbState();

        Render<AppPageTitle>(parameters =>
            parameters
                .AddCascadingValue(crumb)
                .Add(p => p.Title, "Profile")
                .Add(p => p.Breadcrumb, "Account / Profile")
        );

        Assert.Equal("Account / Profile", crumb.Value);
    }

    [Fact]
    public void Render_WithoutBreadcrumb_FallsBackToTitle()
    {
        var crumb = new BreadcrumbState();

        Render<AppPageTitle>(parameters =>
            parameters.AddCascadingValue(crumb).Add(p => p.Title, "Profile")
        );

        Assert.Equal("Profile", crumb.Value);
    }
}
