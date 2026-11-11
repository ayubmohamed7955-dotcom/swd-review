using CinemaBooking.Events;

namespace CinemaBooking.Demo;

/// <summary>
/// Extra subscriber that prints every event when the bus delivers it.
/// It shows that new listeners can be added without touching the write side.
/// </summary>
internal sealed class EventLog : IEventHandler<SeatReserved>, IEventHandler<ReservationCancelled>
{
    public void Handle(SeatReserved domainEvent) =>
        ConsoleUi.Event($"SeatReserved({domainEvent.SeatId}, {domainEvent.Customer}) delivered to the read side");

    public void Handle(ReservationCancelled domainEvent) =>
        ConsoleUi.Event($"ReservationCancelled({domainEvent.SeatId}, {domainEvent.Customer}) delivered to the read side");

    public static void AttachTo(InMemoryEventBus bus)
    {
        var log = new EventLog();
        bus.Subscribe<SeatReserved>(log);
        bus.Subscribe<ReservationCancelled>(log);
    }
}
