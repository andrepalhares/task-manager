using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Persistence.Users;
using TaskManager.Infrastructure.Security;

namespace TaskManager.Infrastructure;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        MongoBsonConfiguration.Register();

        var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt configuration section is missing.");
        services.AddSingleton(jwtSettings);

        var mongoSettings = config.GetSection("MongoDb").Get<MongoDbSettings>()
            ?? throw new InvalidOperationException("MongoDb configuration section is missing.");
        services.AddSingleton(mongoSettings);

        services.AddSingleton<IMongoClient>(sp =>
        {
            return new MongoClient(mongoSettings.ConnectionString);
        });

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoSettings.DatabaseName);
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<ITokenIssuer, JwtTokenIssuer>();

        return services;
    }
}
