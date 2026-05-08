using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Exceptions.Tasks;

namespace TaskManager.Application.Tasks.GetTaskById;

public sealed class GetTaskByIdUseCase : IUseCase<GetTaskByIdInput, GetTaskByIdOutput>
{
    private readonly ITaskRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetTaskByIdUseCase(ITaskRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<GetTaskByIdOutput> ExecuteAsync(GetTaskByIdInput input, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(input.TaskId, cancellationToken);

        if (task is null)
            throw new TaskNotFoundException(input.TaskId);

        if (task.UserId != _currentUser.UserId)
            throw new TaskAccessForbiddenException(input.TaskId);

        return new GetTaskByIdOutput(task.Id, task.Title, task.Description, task.Status.ToString(), task.DueDate, task.UserId);
    }
}
