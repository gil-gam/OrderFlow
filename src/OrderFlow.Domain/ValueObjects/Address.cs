namespace OrderFlow.Domain.ValueObjects;

public sealed record Address
{
    public string Street { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string ZipCode { get; init; }
    public string Country { get; init; }

    private Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public static Address Create(
        string street, string city, string state, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street is required.");
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.");
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State is required.");
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode is required.");
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country is required.");

        return new Address(street, city, state, zipCode, country);
    }
}