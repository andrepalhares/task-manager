using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Tasks.CreateTask;
using TaskManager.Application.Tasks.DeleteTask;
using TaskManager.Application.Tasks.GetTaskById;
using TaskManager.Application.Tasks.GetUserTasks;
using TaskManager.Application.Tasks.UpdateTask;
using TaskManager.Application.Users.LoginUser;
using TaskManager.Application.Users.RegisterUser;

namespace TaskManager.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<IUseCase<RegisterUserInput, RegisterUserOutput>, RegisterUserUseCase>();
        services.AddScoped<LoginUserUseCase>();
        services.AddScoped<IUseCase<LoginUserInput, LoginUserOutput>, LoginUserUseCase>();
        services.AddScoped<CreateTaskUseCase>();
        services.AddScoped<IUseCase<CreateTaskInput, CreateTaskOutput>, CreateTaskUseCase>();
        services.AddScoped<GetTaskByIdUseCase>();
        services.AddScoped<IUseCase<GetTaskByIdInput, GetTaskByIdOutput>, GetTaskByIdUseCase>();
        services.AddScoped<GetUserTasksUseCase>();
        services.AddScoped<IUseCase<GetUserTasksInput, GetUserTasksOutput>, GetUserTasksUseCase>();
        services.AddScoped<UpdateTaskUseCase>();
        services.AddScoped<IUseCase<UpdateTaskInput, UpdateTaskOutput>, UpdateTaskUseCase>();
        services.AddScoped<DeleteTaskUseCase>();
        services.AddScoped<IUseCase<DeleteTaskInput, DeleteTaskOutput>, DeleteTaskUseCase>();
        return services;
    }
}
