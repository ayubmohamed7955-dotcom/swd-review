using CinemaBooking.Events;

namespace CinemaBooking.Tests;

/// <summary>Test double that remembers published events instead of delivering them.</summary>
internal sealed class RecordingPublisher : IEventPublisher
{
    public List<IDomainEvent> Published { get; } = [];

    public void Publish<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent => Published.Add(domainEvent);
}
