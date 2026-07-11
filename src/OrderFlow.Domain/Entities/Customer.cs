namespace OrderFlow.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; private set; }
    public Guid UserExternalId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Customer() { }

    public Customer(Guid userExternalId, string name, string email)
    {
        Id = Guid.NewGuid();
        UserExternalId = userExternalId;
        Name = name;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string email, string phone)
    {
        Name = name;
        Email = email;
        Phone = phone;
    }
}