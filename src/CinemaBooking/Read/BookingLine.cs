namespace CinemaBooking.Read;

/// <summary>One reserved seat in a customer's list of bookings.</summary>
public sealed record BookingLine(Guid ScreeningId, string SeatId);
