using Shouldly;
using TaskManager.Application.Tasks.Common;
using TaskManager.Domain.Entities;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;

namespace TaskManager.Application.Tests.Tasks.Common;

public class TaskDtoTests
{
    [Fact]
    public void FromEntity_MapsAllFields()
    {
        var userId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(2);
        var task = TaskEntity.Create("Title", "Description", DomainTaskStatus.InProgress, dueDate, userId);

        var dto = TaskDto.FromEntity(task);

        dto.Id.ShouldBe(task.Id);
        dto.Title.ShouldBe("Title");
        dto.Description.ShouldBe("Description");
        dto.Status.ShouldBe(nameof(DomainTaskStatus.InProgress));
        dto.DueDate.ShouldBe(dueDate);
        dto.UserId.ShouldBe(userId);
    }

    [Fact]
    public void FromEntity_WithNullDescriptionAndDueDate_PreservesNulls()
    {
        var task = TaskEntity.Create("Title", null, DomainTaskStatus.Pending, null, Guid.NewGuid());

        var dto = TaskDto.FromEntity(task);

        dto.Description.ShouldBeNull();
        dto.DueDate.ShouldBeNull();
    }
}
