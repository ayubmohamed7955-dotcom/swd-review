using CinemaBooking.Events;

namespace CinemaBooking.Read;

/// <summary>Keeps the seat map read model in step with the write side by listening to its events.</summary>
public sealed class SeatMapProjection(ISeatMapStore store)
    : IEventHandler<SeatReserved>, IEventHandler<ReservationCancelled>
{
    public void Handle(SeatReserved domainEvent) =>
        store.MarkTaken(domainEvent.ScreeningId, domainEvent.SeatId);

    public void Handle(ReservationCancelled domainEvent) =>
        store.MarkFree(domainEvent.ScreeningId, domainEvent.SeatId);
}
