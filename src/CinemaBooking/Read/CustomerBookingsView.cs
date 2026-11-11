namespace CinemaBooking.Read;

/// <summary>
/// Second read model, built from the same events as the seat map but shaped for a
/// "my bookings" screen. One write model, many views.
/// </summary>
public sealed record CustomerBookingsView(string Customer, IReadOnlyList<BookingLine> Bookings);
