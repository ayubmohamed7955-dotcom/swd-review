using CinemaBooking.Events;
using CinemaBooking.Read;
using CinemaBooking.Write;

namespace CinemaBooking;

/// <summary>
/// The wired-up system, as seen from the outside: command handlers for the write side,
/// query handlers for the read side, and the event bus between them.
/// Create one with <see cref="CinemaBookingSetup.Create"/>.
/// </summary>
public sealed class CinemaBookingSystem
{
    private readonly IScreeningRepository _screenings;
    private readonly ISeatMapStore _seatMaps;

    internal CinemaBookingSystem(
        IScreeningRepository screenings,
        ISeatMapStore seatMaps,
        ICustomerBookingsStore customerBookings,
        InMemoryEventBus events)
    {
        _screenings = screenings;
        _seatMaps = seatMaps;
        Events = events;

        ReserveSeat = new ReserveSeatHandler(screenings, events);
        CancelReservation = new CancelReservationHandler(screenings, events);
        GetSeatMap = new GetSeatMapHandler(seatMaps);
        GetCustomerBookings = new GetCustomerBookingsHandler(customerBookings);
    }

    // Write side
    public ICommandHandler<ReserveSeatCommand> ReserveSeat { get; }
    public ICommandHandler<CancelReservationCommand> CancelReservation { get; }

    // Read side
    public IQueryHandler<GetSeatMapQuery, SeatMapView> GetSeatMap { get; }
    public IQueryHandler<GetCustomerBookingsQuery, CustomerBookingsView> GetCustomerBookings { get; }

    /// <summary>The bus that carries events from the write side to the projections.</summary>
    public InMemoryEventBus Events { get; }

    /// <summary>
    /// Schedules a new screening: saves it on the write side and creates its empty seat map
    /// on the read side. (A larger system would publish a ScreeningScheduled event instead.)
    /// </summary>
    public void AddScreening(Screening screening)
    {
        _screenings.Save(screening);
        _seatMaps.Initialise(screening.Id, screening.Movie, screening.StartsAt, screening.AllSeatIds);
    }
}
