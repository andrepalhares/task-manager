namespace TaskManager.Domain.Entities;

public sealed class User
{
    public Guid Id { get; }
    public string Email { get; }
    public string PasswordHash { get; }
    public string Name { get; }

    private User(Guid id, string email, string passwordHash, string name)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        Id = id;
        Email = email.Trim().ToLower();
        PasswordHash = passwordHash;
        Name = name.Trim();
    }

    public static User Create(string email, string passwordHash, string name)
        => new(Guid.NewGuid(), email, passwordHash, name);

    public static User Rehydrate(Guid id, string email, string passwordHash, string name)
        => new(id, email, passwordHash, name);
}
