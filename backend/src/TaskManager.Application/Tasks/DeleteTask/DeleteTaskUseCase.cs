using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions.Tasks;

namespace TaskManager.Application.Tasks.DeleteTask;

public sealed record DeleteTaskInput(Guid TaskId);

public sealed class DeleteTaskUseCase : IUseCase<DeleteTaskInput, DeleteTaskOutput>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUser;

    public DeleteTaskUseCase(ITaskRepository repository, ICurrentUserService currentUser)
    {
        _taskRepository = repository;
        _currentUser = currentUser;
    }

    public async Task<DeleteTaskOutput> ExecuteAsync(
        DeleteTaskInput input,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(input.TaskId, cancellationToken);
        ValidateTask(input, task);

        await _taskRepository.DeleteAsync(input.TaskId, cancellationToken);

        return new DeleteTaskOutput();
    }

    private void ValidateTask(DeleteTaskInput input, TaskEntity? task)
    {
        if (task is null)
            throw new TaskNotFoundException(input.TaskId);

        if (task.UserId != _currentUser.UserId)
            throw new TaskAccessForbiddenException(input.TaskId);
    }
}

