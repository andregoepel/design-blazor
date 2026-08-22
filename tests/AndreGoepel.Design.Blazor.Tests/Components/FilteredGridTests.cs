using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class FilteredGridTests : DesignBlazorTestContext
{
    public sealed record Item(string Name);

    public FilteredGridTests()
    {
        // RadzenDataGrid resolves Radzen services even when the grid branch doesn't render,
        // and calls into JS (Radzen.createDataGrid, ResizeObserver hookup, ...) on render —
        // loose mode auto-answers those instead of requiring every call configured by hand.
        Services.AddRadzenComponents();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<FilteredGrid<Item>> Render(
        Action<ComponentParameterCollectionBuilder<FilteredGrid<Item>>> parameters
    ) =>
        Render<FilteredGrid<Item>>(p =>
        {
            p.Add(c => c.EmptyTitle, "Nothing here yet");
            p.Add(
                c => c.Columns,
                (RenderFragment)(
                    builder =>
                    {
                        builder.OpenComponent<Radzen.Blazor.RadzenDataGridColumn<Item>>(0);
                        builder.AddAttribute(1, "Property", "Name");
                        builder.AddAttribute(2, "Title", "Name");
                        builder.CloseComponent();
                    }
                )
            );
            parameters(p);
        });

    [Fact]
    public void Render_WithNoItemsAndZeroTotal_ShowsEmptyTitle()
    {
        // Act
        var cut = Render(p => p.Add(c => c.Items, []).Add(c => c.TotalCount, 0));

        // Assert
        Assert.Equal("Nothing here yet", cut.Find(".ag-empty-title").TextContent);
        Assert.Empty(cut.FindAll(".rz-data-grid"));
    }

    [Fact]
    public void Render_WithNoItemsButNonZeroTotal_ShowsDefaultNoMatchTitle()
    {
        // Act — a search matched nothing, but items exist
        var cut = Render(p => p.Add(c => c.Items, []).Add(c => c.TotalCount, 5));

        // Assert
        Assert.Equal("No matches", cut.Find(".ag-empty-title").TextContent);
    }

    [Fact]
    public void Render_WithCustomNoMatchTitle_UsesIt()
    {
        // Act
        var cut = Render(p =>
            p.Add(c => c.Items, [])
                .Add(c => c.TotalCount, 5)
                .Add(c => c.NoMatchTitle, "No customers match your search")
        );

        // Assert
        Assert.Equal("No customers match your search", cut.Find(".ag-empty-title").TextContent);
    }

    [Fact]
    public void Render_WithItems_RendersGridInsteadOfEmptyState()
    {
        // Act
        var cut = Render(p => p.Add(c => c.Items, [new Item("Ada")]).Add(c => c.TotalCount, 1));

        // Assert
        Assert.Empty(cut.FindAll(".ag-empty"));
        Assert.NotEmpty(cut.FindAll(".rz-data-grid"));
    }

    [Fact]
    public void Render_DefaultCount_FormatsFilteredOfTotal()
    {
        // Act
        var cut = Render(p =>
            p.Add(c => c.Items, [new Item("Ada"), new Item("Grace")]).Add(c => c.TotalCount, 5)
        );

        // Assert
        Assert.Equal("2 of 5", cut.Find(".ag-grid-count").TextContent);
    }

    [Fact]
    public void Render_WithExplicitCount_OverridesDefault()
    {
        // Act
        var cut = Render(p =>
            p.Add(c => c.Items, [new Item("Ada")])
                .Add(c => c.TotalCount, 1)
                .Add(c => c.Count, "1 result")
        );

        // Assert
        Assert.Equal("1 result", cut.Find(".ag-grid-count").TextContent);
    }

    [Fact]
    public void Render_InGerman_UsesGermanDefaults()
    {
        // Arrange
        using var _ = new CultureScope("de");

        // Act
        var cut = Render(p => p.Add(c => c.Items, []).Add(c => c.TotalCount, 5));

        // Assert
        Assert.Equal("Keine Treffer", cut.Find(".ag-empty-title").TextContent);
    }

    [Fact]
    public void Typing_RaisesSearchChanged()
    {
        // Arrange
        string? raised = null;
        var cut = Render(p =>
            p.Add(c => c.Items, [])
                .Add(c => c.TotalCount, 0)
                .Add(
                    c => c.SearchChanged,
                    EventCallback.Factory.Create<string>(this, v => raised = v)
                )
        );

        // Act
        cut.Find("input.ag-search-input").Input("ada");

        // Assert
        Assert.Equal("ada", raised);
    }

    [Fact]
    public void Render_WithToolbarActions_RendersThem()
    {
        // Act
        var cut = Render(p =>
            p.Add(c => c.Items, [])
                .Add(c => c.TotalCount, 0)
                .Add(c => c.ToolbarActions, "<button>Filter</button>")
        );

        // Assert
        Assert.Equal("Filter", cut.Find(".ag-grid-toolbar button").TextContent);
    }
}
