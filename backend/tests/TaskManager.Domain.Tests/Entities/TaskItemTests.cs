using Shouldly;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Tests.Entities;

public class TaskEntityTests
{
    [Fact]
    public void Create_WithValidArgs_SetsAllProperties_AndGeneratesNewId()
    {
        var userId = Guid.NewGuid();
        var title = "Test Task";
        var description = "Test Description";
        var status = DomainTaskStatus.Pending;
        var dueDate = DateTime.UtcNow.AddDays(1);

        var task = TaskEntity.Create(title, description, status, dueDate, userId);

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
        var userId = Guid.NewGuid();

        Should.Throw<ArgumentException>(() => TaskEntity.Create("", null, DomainTaskStatus.Pending, null, userId));
    }

    [Fact]
    public void Create_WithWhitespaceTitle_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();

        Should.Throw<ArgumentException>(() => TaskEntity.Create("   ", null, DomainTaskStatus.Pending, null, userId));
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => TaskEntity.Create("Title", null, DomainTaskStatus.Pending, null, Guid.Empty));
    }

    [Fact]
    public void Create_WithNullDescription_AllowsNull()
    {
        var userId = Guid.NewGuid();

        var task = TaskEntity.Create("Title", null, DomainTaskStatus.Pending, null, userId);

        task.Description.ShouldBeNull();
    }

    [Fact]
    public void Create_DefaultsToNoDueDate_WhenNullPassed()
    {
        var userId = Guid.NewGuid();

        var task = TaskEntity.Create("Title", null, DomainTaskStatus.Pending, null, userId);

        task.DueDate.ShouldBeNull();
    }

    [Fact]
    public void Update_ChangesAllMutableFields()
    {
        var task = TaskEntity.Create("Original Title", "Original Description", DomainTaskStatus.Pending, null, Guid.NewGuid());
        var newTitle = "Updated Title";
        var newDescription = "Updated Description";
        var newStatus = DomainTaskStatus.InProgress;
        var newDueDate = DateTime.UtcNow.AddDays(1);

        task.Update(newTitle, newDescription, newStatus, newDueDate);

        task.Title.ShouldBe(newTitle);
        task.Description.ShouldBe(newDescription);
        task.Status.ShouldBe(newStatus);
        task.DueDate.ShouldBe(newDueDate);
    }

    [Fact]
    public void Update_WithEmptyTitle_ThrowsArgumentException()
    {
        var task = TaskEntity.Create("Title", null, DomainTaskStatus.Pending, null, Guid.NewGuid());

        Should.Throw<ArgumentException>(() => task.Update("", null, DomainTaskStatus.Pending, null));
    }

    [Fact]
    public void Rehydrate_PreservesId_AndDoesNotGenerateNewOne()
    {
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var title = "Test Task";

        var task = TaskEntity.Rehydrate(taskId, title, null, DomainTaskStatus.Pending, null, userId);

        task.Id.ShouldBe(taskId);
    }
}
