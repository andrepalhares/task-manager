using TaskManager.Application.Common.Pagination;

namespace TaskManager.Application.Tasks.GetUserTasks;

public sealed record TaskItemOutput(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? DueDate,
    Guid UserId);

public sealed record GetUserTasksOutput(
    List<TaskItemOutput> Items,
    int Page,
    int PageSize,
    long TotalCount);
