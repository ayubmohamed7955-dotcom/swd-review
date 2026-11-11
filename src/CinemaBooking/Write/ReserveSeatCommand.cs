namespace CinemaBooking.Write;

/// <summary>Request from a customer to reserve one seat in a screening.</summary>
public sealed record ReserveSeatCommand(Guid ScreeningId, string SeatId, string Customer);
