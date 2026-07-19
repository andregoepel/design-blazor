using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class InfoBoxTests : BunitContext
{
    [Fact]
    public void Render_ShowsLabelAndValue()
    {
        var cut = Render<InfoBox>(parameters =>
            parameters
                .Add(p => p.Label, "Next scheduled run")
                .Add(p => p.Value, "Tomorrow at 03:00 UTC")
        );

        Assert.Equal("Next scheduled run", cut.Find(".ag-info-box-label").TextContent);
        Assert.Equal("Tomorrow at 03:00 UTC", cut.Find(".ag-info-box-value").TextContent);
    }

    [Fact]
    public void Render_WithChildContent_TakesPrecedenceOverValue()
    {
        var cut = Render<InfoBox>(parameters =>
            parameters
                .Add(p => p.Label, "Next scheduled run")
                .Add(p => p.Value, "ignored")
                .AddChildContent("<strong>Tomorrow</strong> at 03:00 UTC")
        );

        var value = cut.Find(".ag-info-box-value");
        Assert.DoesNotContain("ignored", value.TextContent);
        Assert.NotNull(value.QuerySelector("strong"));
    }

    [Fact]
    public void Render_UsesAgInfoBoxClass()
    {
        var cut = Render<InfoBox>(parameters => parameters.Add(p => p.Label, "Label"));

        Assert.NotNull(cut.Find(".ag-info-box"));
    }
}
