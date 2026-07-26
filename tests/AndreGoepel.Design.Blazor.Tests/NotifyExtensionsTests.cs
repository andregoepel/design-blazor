using AndreGoepel.Design.Blazor;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests;

public class NotifyExtensionsTests
{
    [Fact]
    public void Success_AddsSuccessNotificationWithTitleAndDetail()
    {
        var notifications = new NotificationService();

        // Act
        notifications.Success("Saved", "Your changes were saved.");

        // Assert
        var message = Assert.Single(notifications.Messages);
        Assert.Equal(NotificationSeverity.Success, message.Severity);
        Assert.Equal("Saved", message.Summary);
        Assert.Equal("Your changes were saved.", message.Detail);
    }

    [Fact]
    public void Error_AddsErrorNotificationWithTitleAndDetail()
    {
        var notifications = new NotificationService();

        // Act
        notifications.Error("Save failed", "The server rejected the request.");

        // Assert
        var message = Assert.Single(notifications.Messages);
        Assert.Equal(NotificationSeverity.Error, message.Severity);
        Assert.Equal("Save failed", message.Summary);
        Assert.Equal("The server rejected the request.", message.Detail);
    }

    [Fact]
    public void Warning_AddsWarningNotificationWithTitleAndDetail()
    {
        var notifications = new NotificationService();

        // Act
        notifications.Warning("Missing category", "Pick a category first.");

        // Assert
        var message = Assert.Single(notifications.Messages);
        Assert.Equal(NotificationSeverity.Warning, message.Severity);
        Assert.Equal("Missing category", message.Summary);
        Assert.Equal("Pick a category first.", message.Detail);
    }

    [Theory]
    [MemberData(nameof(TitleOnlyCalls))]
    public void TitleOnlyOverload_OmitsDetail(Action<NotificationService> call)
    {
        var notifications = new NotificationService();

        // Act
        call(notifications);

        // Assert
        var message = Assert.Single(notifications.Messages);
        Assert.True(string.IsNullOrEmpty(message.Detail));
    }

    public static TheoryData<Action<NotificationService>> TitleOnlyCalls =>
        new() { n => n.Success("Saved"), n => n.Error("Failed"), n => n.Warning("Careful") };
}
