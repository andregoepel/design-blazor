using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class DesignBlazorHeadTests : BunitContext
{
    [Fact]
    public void Render_EmitsStylesheetsAndThemeScriptInOrder()
    {
        // Act
        var cut = Render<DesignBlazorHead>(parameters =>
            parameters.Add(p => p.AppStylesheetHref, "app.css")
        );

        // Assert
        var hrefs = cut.FindAll("link").Select(link => link.GetAttribute("href")).ToList();
        Assert.Equal(
            [
                "_content/AndreGoepel.Design.Blazor/css/fonts.css",
                "_content/Radzen.Blazor/css/material-base.css",
                "_content/AndreGoepel.Design.Blazor/css/design.css",
                "app.css",
            ],
            hrefs
        );
        Assert.Equal(
            "_content/AndreGoepel.Design.Blazor/js/theme.js",
            cut.Find("script").GetAttribute("src")
        );
    }

    [Fact]
    public void Render_UsesGivenAppStylesheetHref()
    {
        // Act
        var cut = Render<DesignBlazorHead>(parameters =>
            parameters.Add(p => p.AppStylesheetHref, "_content/MyApp/app.css")
        );

        // Assert
        Assert.Equal("_content/MyApp/app.css", cut.FindAll("link").Last().GetAttribute("href"));
    }
}
