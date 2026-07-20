using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class GridToolbarTests : DesignBlazorTestContext
{
    [Fact]
    public void Render_ShowsSearchValueAndDefaultPlaceholder()
    {
        // Act
        var cut = Render<GridToolbar>(parameters => parameters.Add(p => p.Search, "ada"));

        // Assert
        var input = cut.Find("input.ag-search-input");
        Assert.Equal("ada", input.GetAttribute("value"));
        Assert.Equal("Search…", input.GetAttribute("placeholder"));
    }

    [Fact]
    public void Render_WithCustomPlaceholder_UsesIt()
    {
        // Act
        var cut = Render<GridToolbar>(parameters =>
            parameters.Add(p => p.SearchPlaceholder, "Search users…")
        );

        // Assert
        Assert.Equal(
            "Search users…",
            cut.Find("input.ag-search-input").GetAttribute("placeholder")
        );
    }

    [Fact]
    public void Typing_RaisesSearchChangedWithNewValue()
    {
        // Arrange
        string? raised = null;
        var cut = Render<GridToolbar>(parameters =>
            parameters
                .Add(p => p.Search, "")
                .Add(
                    p => p.SearchChanged,
                    EventCallback.Factory.Create<string>(this, v => raised = v)
                )
        );

        // Act
        cut.Find("input.ag-search-input").Input("grace");

        // Assert
        Assert.Equal("grace", raised);
    }

    [Fact]
    public void Render_WithCount_ShowsCountSpan()
    {
        // Act
        var cut = Render<GridToolbar>(parameters => parameters.Add(p => p.Count, "9 of 12"));

        // Assert
        Assert.Equal("9 of 12", cut.Find(".ag-grid-count").TextContent);
    }

    [Fact]
    public void Render_WithoutCount_OmitsCountSpan()
    {
        // Act
        var cut = Render<GridToolbar>(parameters => parameters.Add(p => p.Search, ""));

        // Assert
        Assert.Empty(cut.FindAll(".ag-grid-count"));
    }

    [Fact]
    public void Render_InGerman_UsesGermanPlaceholder()
    {
        // Arrange
        UseCulture("de");

        // Act
        var cut = Render<GridToolbar>(parameters => parameters.Add(p => p.Search, ""));

        // Assert
        Assert.Equal("Suchen…", cut.Find("input.ag-search-input").GetAttribute("placeholder"));
    }

    [Fact]
    public void Render_InGerman_WithCustomPlaceholder_KeepsTheHostsText()
    {
        // Arrange — an explicit parameter has to win over the localizer, otherwise
        // hosts that already pass their own text would silently lose it.
        UseCulture("de");

        // Act
        var cut = Render<GridToolbar>(parameters =>
            parameters.Add(p => p.SearchPlaceholder, "Search users…")
        );

        // Assert
        Assert.Equal(
            "Search users…",
            cut.Find("input.ag-search-input").GetAttribute("placeholder")
        );
    }

    [Fact]
    public void Render_WithActions_RendersActionContent()
    {
        // Act
        var cut = Render<GridToolbar>(parameters =>
            parameters.Add(p => p.Actions, "<button>Filter</button>")
        );

        // Assert
        Assert.Equal("Filter", cut.Find("button").TextContent);
    }
}
