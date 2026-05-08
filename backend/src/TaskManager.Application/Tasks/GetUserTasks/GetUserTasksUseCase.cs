using FluentValidation;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Pagination;

namespace TaskManager.Application.Tasks.GetUserTasks;

public sealed class GetUserTasksUseCase : IUseCase<GetUserTasksInput, GetUserTasksOutput>
{
    private readonly IValidator<GetUserTasksInput> _validator;
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly PaginationSettings _paginationSettings;

    public GetUserTasksUseCase(
        IValidator<GetUserTasksInput> validator,
        ITaskRepository repository,
        ICurrentUserService currentUser,
        PaginationSettings paginationSettings)
    {
        _validator = validator;
        _taskRepository = repository;
        _currentUser = currentUser;
        _paginationSettings = paginationSettings;
    }

    public async Task<GetUserTasksOutput> ExecuteAsync(
        GetUserTasksInput input, 
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);

        var result = await _taskRepository.GetByUserIdPagedAsync(
            _currentUser.UserId,
            input.Page,
            _paginationSettings.PageSize,
            cancellationToken);

        var items = result.Items.Select(task =>
            new TaskItemOutput(
                task.Id,
                task.Title,
                task.Description,
                task.Status.ToString(),
                task.DueDate,
                task.UserId)
            ).ToList();

        return new GetUserTasksOutput(items, result.Page, result.PageSize, result.TotalCount);
    }
}
