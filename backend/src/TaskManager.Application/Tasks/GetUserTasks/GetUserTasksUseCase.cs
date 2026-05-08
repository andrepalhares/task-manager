using FluentValidation;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.Application.Tasks.GetUserTasks;

public sealed class GetUserTasksUseCase : IUseCase<GetUserTasksInput, GetUserTasksOutput>
{
    private const int PAGE_SIZE = 10;

    private readonly IValidator<GetUserTasksInput> _validator;
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUser;

    public GetUserTasksUseCase(
        IValidator<GetUserTasksInput> validator,
        ITaskRepository repository,
        ICurrentUserService currentUser)
    {
        _validator = validator;
        _taskRepository = repository;
        _currentUser = currentUser;
    }

    public async Task<GetUserTasksOutput> ExecuteAsync(
        GetUserTasksInput input, 
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);

        var result = await _taskRepository.GetByUserIdPagedAsync(
            _currentUser.UserId,
            input.Page,
            PAGE_SIZE,
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
