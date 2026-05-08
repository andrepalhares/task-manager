using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Tasks.CreateTask;
using TaskManager.Application.Tasks.DeleteTask;
using TaskManager.Application.Tasks.GetTaskById;
using TaskManager.Application.Tasks.GetUserTasks;
using TaskManager.Application.Tasks.UpdateTask;
using TaskManager.WebApi.Common;

namespace TaskManager.WebApi.Tasks;

[ApiController]
[Authorize]
[Route("api/tasks")]
public sealed class TasksController : ApiControllerBase
{
    private readonly IUseCase<CreateTaskInput, CreateTaskOutput> _createTask;
    private readonly IUseCase<GetTaskByIdInput, GetTaskByIdOutput> _getTaskById;
    private readonly IUseCase<GetUserTasksInput, GetUserTasksOutput> _getUserTasks;
    private readonly IUseCase<UpdateTaskInput, UpdateTaskOutput> _updateTask;
    private readonly IUseCase<DeleteTaskInput, DeleteTaskOutput> _deleteTask;

    public TasksController(
        IUseCase<CreateTaskInput, CreateTaskOutput> createTask,
        IUseCase<GetTaskByIdInput, GetTaskByIdOutput> getTaskById,
        IUseCase<GetUserTasksInput, GetUserTasksOutput> getUserTasks,
        IUseCase<UpdateTaskInput, UpdateTaskOutput> updateTask,
        IUseCase<DeleteTaskInput, DeleteTaskOutput> deleteTask)
    {
        _createTask = createTask;
        _getTaskById = getTaskById;
        _getUserTasks = getUserTasks;
        _updateTask = updateTask;
        _deleteTask = deleteTask;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateTaskOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await _createTask.ExecuteAsync(request.ToInput(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetTaskByIdOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _getTaskById.ExecuteAsync(new GetTaskByIdInput(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetUserTasksOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] int page = 1, 
        CancellationToken cancellationToken = default)
    {
        var result = await _getUserTasks.ExecuteAsync(new GetUserTasksInput(page), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateTaskOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] Guid id, 
        [FromBody] UpdateTaskRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await _updateTask.ExecuteAsync(request.ToInput(id), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        await _deleteTask.ExecuteAsync(new DeleteTaskInput(id), cancellationToken);
        return NoContent();
    }
}

