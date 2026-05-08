using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskStatus = TaskManager.Domain.Entities.TaskStatus;

namespace TaskManager.WebApi;

/// <summary>
/// Seeds a demo user + sample tasks on application startup.
/// Idempotent: if the demo user already exists, nothing is inserted.
/// </summary>
public sealed class DataSeeder
{
    private const string DemoEmail = "admin@taskmanager.com";
    private const string DemoPassword = "Test@1234";
    private const string DemoName = "Admin";

    private readonly IUserRepository _users;
    private readonly ITaskRepository _tasks;
    private readonly IPasswordHasher _hasher;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        IUserRepository users,
        ITaskRepository tasks,
        IPasswordHasher hasher,
        ILogger<DataSeeder> logger)
    {
        _users = users;
        _tasks = tasks;
        _hasher = hasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existing = await _users.GetByEmailAsync(DemoEmail, cancellationToken);
        if (existing is not null)
        {
            _logger.LogInformation("Demo user already exists. Skipping seed.");
            return;
        }

        var user = User.Create(DemoEmail, _hasher.Hash(DemoPassword), DemoName);
        await _users.AddAsync(user, cancellationToken);

        var now = DateTime.UtcNow;
        var samples = new (string Title, string? Description, TaskStatus Status, DateTime? DueDate)[]
        {
            ("Welcome to Task Manager", "Explore the app using this demo account.", TaskStatus.Completed, now.AddDays(-7)),
            ("Read the README", "Setup instructions and project overview.", TaskStatus.Completed, now.AddDays(-5)),
            ("Set up local environment", "Install Docker, run docker compose up.", TaskStatus.Completed, now.AddDays(-3)),
            ("Review architecture diagram", "Understand the layered structure.", TaskStatus.InProgress, now.AddDays(1)),
            ("Implement search filter", "Add full-text search to tasks list.", TaskStatus.InProgress, now.AddDays(2)),
            ("Refactor task validation", "Move validation rules into the domain layer.", TaskStatus.InProgress, now.AddDays(4)),
            ("Write integration tests", "Cover happy-path for tasks endpoints.", TaskStatus.Pending, now.AddDays(5)),
            ("Add dark mode toggle", "Persist user preference in localStorage.", TaskStatus.Pending, now.AddDays(7)),
            ("Configure CI pipeline", "Run tests on every pull request.", TaskStatus.Pending, now.AddDays(10)),
            ("Plan production deployment", "Choose hosting and configure secrets.", TaskStatus.Pending, null),
        };

        foreach (var s in samples)
        {
            var task = TaskItem.Create(s.Title, s.Description, s.Status, s.DueDate, user.Id);
            await _tasks.AddAsync(task, cancellationToken);
        }
    }
}
