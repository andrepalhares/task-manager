using FluentValidation;
using Shouldly;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.WebApi.Tasks;

namespace TaskManager.WebApi.Tests.Tasks;

public class UpdateTaskRequestTests
{
    [Fact]
    public void ToInput_WithValidStatus_MapsAllFieldsAndAttachesTaskId()
    {
        var taskId = Guid.NewGuid();
        var due = DateTime.UtcNow.AddDays(3);
        var request = new UpdateTaskRequest("Title", "Desc", "Completed", due);

        var input = request.ToInput(taskId);

        input.TaskId.ShouldBe(taskId);
        input.Title.ShouldBe("Title");
        input.Description.ShouldBe("Desc");
        input.Status.ShouldBe(DomainTaskStatus.Completed);
        input.DueDate.ShouldBe(due);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid")]
    [InlineData("pending")]
    public void ToInput_WhenStatusMissingOrInvalid_ThrowsValidationException(string? status)
    {
        var request = new UpdateTaskRequest("Title", null, status, null);

        Should.Throw<ValidationException>(() => request.ToInput(Guid.NewGuid()));
    }
}
