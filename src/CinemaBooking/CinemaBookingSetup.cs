using CinemaBooking.Events;
using CinemaBooking.Read;
using CinemaBooking.Write;

namespace CinemaBooking;

/// <summary>Composition root: wires the write side, the read side and the event bus by hand.</summary>
public static class CinemaBookingSetup
{
    /// <param name="deferDelivery">
    /// When true, events wait on the bus until <see cref="InMemoryEventBus.ProcessPending"/>
    /// is called, so the read side is eventually consistent instead of immediately consistent.
    /// </param>
    public static CinemaBookingSystem Create(bool deferDelivery = false)
    {
        var screenings = new InMemoryScreeningRepository();
        var seatMaps = new InMemorySeatMapStore();
        var customerBookings = new InMemoryCustomerBookingsStore();
        var events = new InMemoryEventBus(deferDelivery);

        // Each projection listens to the events and keeps its own read model up to date.
        var seatMapProjection = new SeatMapProjection(seatMaps);
        events.Subscribe<SeatReserved>(seatMapProjection);
        events.Subscribe<ReservationCancelled>(seatMapProjection);

        var customerBookingsProjection = new CustomerBookingsProjection(customerBookings);
        events.Subscribe<SeatReserved>(customerBookingsProjection);
        events.Subscribe<ReservationCancelled>(customerBookingsProjection);

        return new CinemaBookingSystem(screenings, seatMaps, customerBookings, events);
    }
}
