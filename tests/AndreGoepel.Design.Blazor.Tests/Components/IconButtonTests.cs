using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class IconButtonTests : BunitContext
{
    [Fact]
    public void Render_UsesAgIconBtnClassAndIcon()
    {
        // Act
        var cut = Render<IconButton>(parameters =>
            parameters.Add(p => p.Icon, "more_horiz").Add(p => p.Title, "More actions")
        );

        // Assert
        var button = cut.Find("button");
        Assert.Contains("ag-icon-btn", button.ClassList);
        Assert.Equal("more_horiz", cut.Find(".rzi").TextContent);
    }

    [Fact]
    public void Render_SetsTitleAndAriaLabelFromTitleParameter()
    {
        // Act
        var cut = Render<IconButton>(parameters =>
            parameters.Add(p => p.Icon, "delete").Add(p => p.Title, "Delete role")
        );

        // Assert
        var button = cut.Find("button");
        Assert.Equal("Delete role", button.GetAttribute("title"));
        Assert.Equal("Delete role", button.GetAttribute("aria-label"));
    }

    [Fact]
    public void Render_DefaultsToLightStyle()
    {
        // Act
        var cut = Render<IconButton>(parameters =>
            parameters.Add(p => p.Icon, "edit").Add(p => p.Title, "Edit")
        );

        // Assert
        Assert.Contains("rz-light", cut.Find("button").ClassList);
    }

    [Fact]
    public void Render_DangerTrue_UsesDangerStyle()
    {
        // Act
        var cut = Render<IconButton>(parameters =>
            parameters
                .Add(p => p.Icon, "delete")
                .Add(p => p.Title, "Delete")
                .Add(p => p.Danger, true)
        );

        // Assert
        var classes = cut.Find("button").ClassList;
        Assert.Contains("rz-danger", classes);
        Assert.DoesNotContain("rz-light", classes);
    }

    [Fact]
    public void Click_InvokesCallbackWithMouseEventArgs()
    {
        // Arrange
        MouseEventArgs? received = null;
        var cut = Render<IconButton>(parameters =>
            parameters
                .Add(p => p.Icon, "more_horiz")
                .Add(p => p.Title, "More actions")
                .Add(
                    p => p.Click,
                    EventCallback.Factory.Create<MouseEventArgs>(this, args => received = args)
                )
        );

        // Act
        cut.Find("button").Click();

        // Assert
        Assert.NotNull(received);
    }
}
