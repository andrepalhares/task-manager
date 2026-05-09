using FluentValidation;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions.Tasks;

namespace TaskManager.Application.Tasks.UpdateTask;

public sealed class UpdateTaskUseCase : IUseCase<UpdateTaskInput, UpdateTaskOutput>
{
    private readonly IValidator<UpdateTaskInput> _validator;
    private readonly ITaskRepository _taskRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateTaskUseCase(
        IValidator<UpdateTaskInput> validator,
        ITaskRepository repository,
        ICurrentUserService currentUser)
    {
        _validator = validator;
        _taskRepository = repository;
        _currentUser = currentUser;
    }

    public async Task<UpdateTaskOutput> ExecuteAsync(
        UpdateTaskInput input, 
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);

        var task = await _taskRepository.GetByIdAsync(input.TaskId, cancellationToken);
        ValidateTask(input, task);

        task!.Update(input.Title, input.Description, input.Status, input.DueDate);
        await _taskRepository.UpdateAsync(task, cancellationToken);

        return new UpdateTaskOutput(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.DueDate,
            task.UserId);
    }

    private void ValidateTask(UpdateTaskInput input, TaskEntity? task)
    {
        if (task is null)
            throw new TaskNotFoundException(input.TaskId);

        if (task.UserId != _currentUser.UserId)
            throw new TaskAccessForbiddenException(input.TaskId);
    }
}
