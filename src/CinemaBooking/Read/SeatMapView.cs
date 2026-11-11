namespace CinemaBooking.Read;

/// <summary>
/// Read model for the seat map screen. It is denormalised: movie, start time, every seat
/// and the free-seat count are stored together, so a query is a single lookup.
/// </summary>
public sealed record SeatMapView(
    Guid ScreeningId,
    string Movie,
    DateTime StartsAt,
    IReadOnlyList<SeatView> Seats,
    int FreeSeats);
