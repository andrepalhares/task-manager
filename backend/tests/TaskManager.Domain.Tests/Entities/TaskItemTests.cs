using Shouldly;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Tests.Entities;

public class TaskItemTests
{
    [Fact]
    public void Create_WithValidArgs_SetsAllProperties_AndGeneratesNewId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var title = "Test Task";
        var description = "Test Description";
        var status = DomainTaskStatus.Pending;
        var dueDate = DateTime.UtcNow.AddDays(1);

        // Act
        var task = TaskItem.Create(title, description, status, dueDate, userId);

        // Assert
        task.Id.ShouldNotBe(Guid.Empty);
        task.Title.ShouldBe(title);
        task.Description.ShouldBe(description);
        task.Status.ShouldBe(status);
        task.DueDate.ShouldBe(dueDate);
        task.UserId.ShouldBe(userId);
    }

    [Fact]
    public void Create_WithEmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        Should.Throw<ArgumentException>(() => TaskItem.Create("", null, DomainTaskStatus.Pending, null, userId));
    }

    [Fact]
    public void Create_WithWhitespaceTitle_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        Should.Throw<ArgumentException>(() => TaskItem.Create("   ", null, DomainTaskStatus.Pending, null, userId));
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsArgumentException()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => TaskItem.Create("Title", null, DomainTaskStatus.Pending, null, Guid.Empty));
    }

    [Fact]
    public void Create_WithNullDescription_AllowsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var task = TaskItem.Create("Title", null, DomainTaskStatus.Pending, null, userId);

        // Assert
        task.Description.ShouldBeNull();
    }

    [Fact]
    public void Create_DefaultsToNoDueDate_WhenNullPassed()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var task = TaskItem.Create("Title", null, DomainTaskStatus.Pending, null, userId);

        // Assert
        task.DueDate.ShouldBeNull();
    }

    [Fact]
    public void Update_ChangesAllMutableFields()
    {
        // Arrange
        var task = TaskItem.Create("Original Title", "Original Description", DomainTaskStatus.Pending, null, Guid.NewGuid());
        var newTitle = "Updated Title";
        var newDescription = "Updated Description";
        var newStatus = DomainTaskStatus.InProgress;
        var newDueDate = DateTime.UtcNow.AddDays(1);

        // Act
        task.Update(newTitle, newDescription, newStatus, newDueDate);

        // Assert
        task.Title.ShouldBe(newTitle);
        task.Description.ShouldBe(newDescription);
        task.Status.ShouldBe(newStatus);
        task.DueDate.ShouldBe(newDueDate);
    }

    [Fact]
    public void Update_WithEmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        var task = TaskItem.Create("Title", null, DomainTaskStatus.Pending, null, Guid.NewGuid());

        // Act & Assert
        Should.Throw<ArgumentException>(() => task.Update("", null, DomainTaskStatus.Pending, null));
    }

    [Fact]
    public void Rehydrate_PreservesId_AndDoesNotGenerateNewOne()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var title = "Test Task";

        // Act
        var task = TaskItem.Rehydrate(taskId, title, null, DomainTaskStatus.Pending, null, userId);

        // Assert
        task.Id.ShouldBe(taskId);
    }
}
