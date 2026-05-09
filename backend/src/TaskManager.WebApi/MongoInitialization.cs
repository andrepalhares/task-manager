using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.WebApi;

[ExcludeFromCodeCoverage]
public static class MongoInitialization
{
    public static async Task InitializeMongoIndexesAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using (var scope = app.Services.CreateScope())
        {
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
            await MongoIndexInitializer.EnsureIndexesAsync(database, cancellationToken);
        }
    }
}
