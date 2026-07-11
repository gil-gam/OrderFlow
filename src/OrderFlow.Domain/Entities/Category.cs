namespace OrderFlow.Domain.Entities;

public sealed class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Category()
    {
        Name = string.Empty;
    }

    private Category(Guid id, string name, string? description, bool isActive, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = createdAt;
    }

    public static Category Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (name.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters.", nameof(name));
        if (description?.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters.", nameof(description));

        return new Category(
            Guid.NewGuid(),
            name.Trim(),
            description?.Trim(),
            true,
            DateTime.UtcNow);
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (name.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters.", nameof(name));
        if (description?.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters.", nameof(description));

        Name = name.Trim();
        Description = description?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
