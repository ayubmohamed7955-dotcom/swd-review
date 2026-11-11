namespace CinemaBooking.Events;

/// <summary>Reacts to one type of domain event, for example by updating a read model.</summary>
public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
{
    void Handle(TEvent domainEvent);
}
