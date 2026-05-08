namespace TaskManager.Application.Common.Interfaces;

public interface IUseCase<in TCommand, TResult>
{
    Task<TResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
}
