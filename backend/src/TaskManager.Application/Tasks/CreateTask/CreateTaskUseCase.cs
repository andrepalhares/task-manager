using FluentValidation;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tasks.CreateTask;

public sealed class CreateTaskUseCase : IUseCase<CreateTaskInput, CreateTaskOutput>
{
    private readonly IValidator<CreateTaskInput> _validator;
    private readonly ITaskRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreateTaskUseCase(
        IValidator<CreateTaskInput> validator,
        ITaskRepository repository,
        ICurrentUserService currentUser)
    {
        _validator = validator;
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<CreateTaskOutput> ExecuteAsync(
        CreateTaskInput input,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(input, cancellationToken);

        var task = TaskEntity.Create(
            input.Title,
            input.Description,
            input.Status,
            input.DueDate,
            _currentUser.UserId);
        await _repository.AddAsync(task, cancellationToken);

        return new CreateTaskOutput(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.DueDate,
            task.UserId);
    }
}

