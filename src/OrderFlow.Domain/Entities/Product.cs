using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money UnitPrice { get; private set; }
    public Guid CategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        UnitPrice = null!;
    }

    private Product(
        Guid id, string name, string description,
        Money unitPrice, Guid categoryId,
        bool isActive, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        CategoryId = categoryId;
        IsActive = isActive;
        CreatedAt = createdAt;
    }

    public static Product Create(
        string name, string description,
        Money unitPrice, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (name.Length > 200)
            throw new ArgumentException("Name cannot exceed 200 characters.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));
        if (unitPrice is null)
            throw new ArgumentException("Unit price is required.", nameof(unitPrice));
        if (unitPrice.Amount < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category is required.", nameof(categoryId));

        return new Product(
            id: Guid.NewGuid(),
            name: name.Trim(),
            description: description.Trim(),
            unitPrice: unitPrice,
            categoryId: categoryId,
            isActive: true,
            createdAt: DateTime.UtcNow);
    }

    public void Update(
        string name, string description,
        Money unitPrice, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (name.Length > 200)
            throw new ArgumentException("Name cannot exceed 200 characters.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));
        if (unitPrice is null)
            throw new ArgumentException("Unit price is required.", nameof(unitPrice));
        if (unitPrice.Amount < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category is required.", nameof(categoryId));

        Name = name.Trim();
        Description = description.Trim();
        UnitPrice = unitPrice;
        CategoryId = categoryId;
    }

    public void Deactivate() => IsActive = false;
}