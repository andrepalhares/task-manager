using System.Diagnostics.CodeAnalysis;

namespace TaskManager.Infrastructure.Persistence;

[ExcludeFromCodeCoverage]
public sealed class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
