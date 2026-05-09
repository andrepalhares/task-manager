using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Diagnostics.CodeAnalysis;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence.Models;

namespace TaskManager.Infrastructure.Persistence;

[ExcludeFromCodeCoverage]
public static class MongoBsonConfiguration
{
    private static bool _registered;
    private static readonly object _lock = new();

    public static void Register()
    {
        if (_registered) return;
        lock (_lock)
        {
            if (_registered) return;

            BsonClassMap.RegisterClassMap<UserEntity>(cm =>
            {
                cm.MapIdProperty(u => u.Id).SetSerializer(new GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));
                cm.MapProperty(u => u.Email);
                cm.MapProperty(u => u.PasswordHash);
                cm.MapProperty(u => u.Name);
                cm.MapCreator(u => UserEntity.Rehydrate(u.Id, u.Email, u.PasswordHash, u.Name));
            });

            BsonClassMap.RegisterClassMap<TaskDocument>(cm =>
            {
                cm.MapIdProperty(t => t.Id).SetSerializer(new GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));
                cm.MapProperty(t => t.Title);
                cm.MapProperty(t => t.Description);
                cm.MapProperty(t => t.Status);
                cm.MapProperty(t => t.DueDate);
                cm.MapProperty(t => t.UserId).SetSerializer(new GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));
                cm.MapProperty(t => t.CreatedAt);
            });

            _registered = true;
        }
    }
}
