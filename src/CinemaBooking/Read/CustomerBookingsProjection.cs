using CinemaBooking.Events;

namespace CinemaBooking.Read;

/// <summary>Builds the customer bookings read model from the same events as <see cref="SeatMapProjection"/>.</summary>
public sealed class CustomerBookingsProjection(ICustomerBookingsStore store)
    : IEventHandler<SeatReserved>, IEventHandler<ReservationCancelled>
{
    public void Handle(SeatReserved domainEvent) =>
        store.Add(domainEvent.Customer, new BookingLine(domainEvent.ScreeningId, domainEvent.SeatId));

    public void Handle(ReservationCancelled domainEvent) =>
        store.Remove(domainEvent.Customer, new BookingLine(domainEvent.ScreeningId, domainEvent.SeatId));
}
