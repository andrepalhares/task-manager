# Task Manager - GenAI Tools

## Approach

The tools used were **GitHub Copilot** (in Visual Studio and VS Code) to handle code generation, codebase analysis, filling in repetitive boilerplate code and tests; and **Claude AI** for architectural decisions, design discussions, and reviewing AI-generated code before it landed in the repository. Splitting allowed me to extract the best of each tool: GitHub Copilot for quick, in-context code completions, while Claude for more thoughtful, slower responses that could lead to careful interactions.

Some of the habits I like to follow when working with AI:

- **Round-tabling for design decisions**: Since there are usually multiple ways to do the same thing, I like to use the Round Table technique to weigh pros and cons before making a decision. It consists of pointing out the multiple options (or asking for a brainstorm) and their pros/cons and then asking AI to make a round table to decide what's the best approach and the explanation on why.
- **Divide and conquer**: To split a big task into smaller ones is helpful when dealing with AI. It allows me to verify each step, refine the prompt if needed, and validate the code generated. Since the outputs are smaller chunks of code, it's also easier to review and harder to get lost in it.
- **Pushing back on AI confidence**: When an answer doesn't sound credible or I know it's not what I'm looking for, I like to always ask it to double-check or share references to documentation where the correct information can be found.
- **Cross-tool review pipeline**: I used Copilot to produce drafts including documentation in markdown files, design notes, prompt artifacts and sent them to Claude for critique and refinement before acting on them.

## The Prompt

The prompt below serves as a structured scaffold for the project. Some details are intentionally left unspecified so the AI's decisions in those areas could be evaluated separately.

```
You should work on the TaskManager project. It is a small web application focused on task management built for a technical interview. Scaffold the solution following Clean Architecture and you're not allowed to use Entity Framework, Dapper or Mediator.

## Tech stack
- .NET 10, C# 13
- ASP.NET Core Web API
- MongoDB 7, using `MongoDB.Driver` NuGet package
- JWT bearer authentication
- Password hashing
- FluentValidation for simple input validation
- xUnit + NSubstitute + Shouldly for tests
- Docker / docker-compose for local orchestration

The common solution architecture I use is separated by 4 different layers with distinct responsibilities.

- TaskManager.Domain for business rules, containing entities, domain exceptions.
- TaskManager.Application for business flow, containing use cases, DTOs, validators and port interfaces.
- TaskManager.Infrastructure for Data layer, containing MongoDB repositories, BCrypt and JWT issuer.
- TaskManager.WebApi for API layer, containing controllers, middleware and DI composition.

Also each of these projects should have an associated unit test project.

## Dependency rule between projects in the solution:
- Domain depends on nothing.
- Application depends only on Domain. It declares repository and service interfaces, that will be implemented in the Infrastructure project.
- Infrastructure depends on Application and Domain. It contains the actual implementations for MongoDB repositories, password hasher and token issuer.
- WebApi is the composition root. It depends on Application and Infrastructure.


## Domain
Two aggregates:
- User: Id (Guid), Email, Name, PasswordHash
- Task: Id (Guid), Title, Description, Status (as enum), DueDate, UserId

Both use private setters, static factory methods and smaller methods for individual state changes.

## Persistence
I need you to map the 2 MongoDB collections: users and tasks. The Infrastructure project implements the interfaces declared in Application, so the persistence layer can be swapped without touching Application or Domain. Map between domain entities and Mongo documents inside the repository.

Finally, I need you to generate the 4 main projects under a /src folder and the 4 tests projects under a /tests folder.
```

## Sample Output

This code provides a representative sample of the AI-generated output, including the project tree, the `UpdateTask` use case, and the related controller.

### Project structure

```
TaskManager/
├── src/
│   ├── TaskManager.Domain/
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   └── TaskItem.cs
│   │   └── Exceptions/
│   ├── TaskManager.Application/
│   │   ├── Common/
│   │   │   └── Result.cs
│   │   ├── UseCases/
│   │   │   ├── Auth/
│   │   │   └── Tasks/
│   │   │       ├── CreateTaskUseCase.cs
│   │   │       ├── UpdateTaskUseCase.cs
│   │   │       ├── DeleteTaskUseCase.cs
│   │   │       └── GetTaskUseCase.cs
│   │   ├── DTOs/
│   │   ├── Validators/
│   │   └── Interfaces/
│   │       ├── IUserRepository.cs
│   │       ├── ITaskRepository.cs
│   │       ├── IPasswordHasher.cs
│   │       └── ITokenService.cs
│   ├── TaskManager.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── MongoUserRepository.cs
│   │   │   └── MongoTaskRepository.cs
│   │   └── Security/
│   └── TaskManager.WebApi/
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   └── TasksController.cs
│       └── Program.cs
└── tests/
    ├── TaskManager.Domain.Tests/
    ├── TaskManager.Application.Tests/
    ├── TaskManager.Infrastructure.Tests/
    └── TaskManager.WebApi.Tests/
```

### `UpdateTaskUseCase.cs`

```csharp
namespace TaskManager.Application.UseCases.Tasks;

public class UpdateTaskUseCase
{
    private readonly ITaskRepository _taskRepository;
    private readonly IValidator<UpdateTaskInput> _validator;

    public UpdateTaskUseCase(
        ITaskRepository taskRepository,
        IValidator<UpdateTaskInput> validator)
    {
        _taskRepository = taskRepository;
        _validator = validator;
    }

    public async Task<Result<TaskDto>> ExecuteAsync(
        UpdateTaskInput input,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
            return Result<TaskDto>.ValidationFailure(validationResult.Errors);

        var task = await _taskRepository.GetByIdAsync(input.TaskId, cancellationToken);
        if (task is null)
            return Result<TaskDto>.NotFound($"Task '{input.TaskId}' was not found.");

        if (task.UserId != input.UserId)
            return Result<TaskDto>.Forbidden("You cannot modify a task you do not own.");

        task.Update(input.Title, input.Description, input.Status, input.DueDate);
        await _taskRepository.UpdateAsync(task, cancellationToken);

        return Result<TaskDto>.Success(TaskDto.FromEntity(task));
    }
}
```

### `TasksController.cs`

```csharp
[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly UpdateTaskUseCase _updateTaskUseCase;
    // [...]

    public TasksController(UpdateTaskUseCase updateTaskUseCase)
    {
        _updateTaskUseCase = updateTaskUseCase;
        // [...]
    }

    // [...]

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var input = new UpdateTaskInput(id, userId, request.Title, request.Description,
                                        request.Status, request.DueDate);

        var result = await _updateTaskUseCase.ExecuteAsync(input, cancellationToken);

        if (result.IsValidationFailure)
            return BadRequest(result.Errors);
        if (result.IsNotFound)
            return NotFound(new { message = result.ErrorMessage });
        if (result.IsForbidden)
            return StatusCode(StatusCodes.Status403Forbidden, new { message = result.ErrorMessage });

        return Ok(result.Value);
    }
}
```

## Critical Evaluation

The AI demonstrated strong judgment in handling the overall architecture, including project layout, layer dependencies, MongoDB integration, and test scaffolding, ensuring a solid foundation.

The shortcomings were relatively minor in scope but still important to fix, as addressing them would help preserve the integrity of the Clean Architecture principles and ensure the project remains robust and maintainable over time.

### Concrete classes injected into controllers

The controller class generated by AI depended on concrete use case classes, breaking the Dependency Inversion Principle. It would be a better approach to create interfaces for use cases, so that the external layer (WebApi) depends on interfaces, rather than actual implementations (from the Application project). That approach would create several interfaces, one for each use case. But another improvement is to introduce a single generic `IUseCase<TInput, TOutput>` interface, allowing controllers to depend on a stable abstraction, facilitating implementation swapping, and avoiding interface explosion, resulting in more flexible and maintainable code.

### HTTP semantics leaking into the Application layer

Also, the AI's use cases returned `Result<T>` wrappers, which in itself is not a problem at all. But it generated them with HTTP-specific variants like `NotFound` (when the task doesn't exist) and `Forbidden` (when the task belongs to another user), which can be considered a layering violation, since these are WebApi concerns, not domain concepts. A better approach would be to use domain-specific exceptions, such as `TaskNotFoundException` and `UnauthorizedTaskAccessException` for those scenarios respectively, leaving the HTTP mapping to a centralized filter in `GlobalExceptionHandler` that translates the exceptions into proper `ProblemDetails` responses, maintaining separation of concerns and better adhering to layered architecture principles.

### A transitive dependency mismatch the AI couldn't see

Another issue surfaced while testing Swagger. The AI's package version suggestion caused a compatibility issue when introducing Swagger because `Microsoft.OpenApi` 1.6.23 required a newer `swagger-ui`, which `Swashbuckle.AspNetCore` 7.2.0 did not bundle, leading to an error when trying to open the Swagger page. Diagnosing this mismatch involved inspecting browser errors, tracing package versions, and consulting release notes.

## Closing Thoughts

My experience with AI in this project reinforced a key principle: AI serves as a powerful accelerator, providing useful drafts and insights, helping the learning process and enriching discussions, but it can't replace human judgment and contextual understanding. Careful review and critical assessment of AI-generated suggestions are essential to maintain the integrity, coherence and quality of the codebase.
