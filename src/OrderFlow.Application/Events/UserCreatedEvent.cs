using MediatR;

namespace OrderFlow.Application.Events;

public sealed record UserCreatedEvent(
    Guid UserId,
    string Name,
    string Email) : INotification;