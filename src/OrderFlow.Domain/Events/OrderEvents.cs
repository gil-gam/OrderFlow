using MediatR;

namespace OrderFlow.Domain.Events;

public sealed record OrderCreatedDomainEvent(
    Guid OrderId, Guid CustomerId, DateTime OccurredOn) : INotification;

public sealed record OrderConfirmedDomainEvent(Guid OrderId) : INotification;

public sealed record OrderCancelledDomainEvent(Guid OrderId) : INotification;