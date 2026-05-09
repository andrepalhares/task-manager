namespace TaskManager.Domain.Entities;

public sealed class TaskEntity
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Guid UserId { get; }

    private TaskEntity(Guid id, string title, string? description, TaskStatus status, DateTime? dueDate, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));

        Id = id;
        Title = title;
        Description = description;
        Status = status;
        DueDate = dueDate;
        UserId = userId;
    }

    public static TaskEntity Create(string title, string? description, TaskStatus status, DateTime? dueDate, Guid userId)
        => new(Guid.NewGuid(), title, description, status, dueDate, userId);

    public static TaskEntity Rehydrate(Guid id, string title, string? description, TaskStatus status, DateTime? dueDate, Guid userId)
        => new(id, title, description, status, dueDate, userId);

    public void Update(string title, string? description, TaskStatus status, DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
        Description = description;
        Status = status;
        DueDate = dueDate;
    }
}
