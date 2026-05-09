using FluentValidation;
using Shouldly;
using TaskManager.WebApi.Tasks;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;

namespace TaskManager.WebApi.Tests.Tasks;

public class CreateTaskRequestTests
{
    [Fact]
    public void ToInput_WithValidStatus_ParsesEnum()
    {
        var due = DateTime.UtcNow.AddDays(1);
        var request = new CreateTaskRequest("Title", "Desc", "InProgress", due);

        var input = request.ToInput();

        input.Title.ShouldBe("Title");
        input.Description.ShouldBe("Desc");
        input.Status.ShouldBe(DomainTaskStatus.InProgress);
        input.DueDate.ShouldBe(due);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToInput_WhenStatusOmitted_DefaultsToPending(string? status)
    {
        var request = new CreateTaskRequest("Title", null, status, null);

        var input = request.ToInput();

        input.Status.ShouldBe(DomainTaskStatus.Pending);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("pending")]
    [InlineData("inprogress")]
    public void ToInput_WhenStatusUnknownOrWrongCase_ThrowsValidationException(string status)
    {
        var request = new CreateTaskRequest("Title", null, status, null);

        Should.Throw<ValidationException>(() => request.ToInput());
    }
}
