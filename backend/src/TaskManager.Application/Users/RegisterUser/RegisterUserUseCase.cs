using FluentValidation;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Users.RegisterUser;

public sealed class RegisterUserUseCase : IUseCase<RegisterUserInput, RegisterUserOutput>
{
    private readonly IUserRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterUserInput> _validator;

    public RegisterUserUseCase(
        IUserRepository usersRepository,
        IPasswordHasher passwordHasher,
        IValidator<RegisterUserInput> validator)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<RegisterUserOutput> ExecuteAsync(RegisterUserInput command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var hash = _passwordHasher.Hash(command.Password);
        var user = User.Create(command.Email, hash, command.Name);
        await _usersRepository.AddAsync(user, cancellationToken);

        return new RegisterUserOutput(user.Id, user.Email, user.Name);
    }
}
