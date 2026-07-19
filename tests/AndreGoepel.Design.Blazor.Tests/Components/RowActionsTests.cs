using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class RowActionsTests : BunitContext
{
    [Fact]
    public void Render_UsesAgRowActionsClass()
    {
        // Act
        var cut = Render<RowActions>(parameters =>
            parameters.AddChildContent("<button>Edit</button>")
        );

        // Assert
        Assert.NotNull(cut.Find(".ag-row-actions"));
    }

    [Fact]
    public void Render_RendersChildContent()
    {
        // Act
        var cut = Render<RowActions>(parameters =>
            parameters.AddChildContent("<button>Edit</button>")
        );

        // Assert
        Assert.Equal("Edit", cut.Find("button").TextContent);
    }
}
