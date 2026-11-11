namespace CinemaBooking.Write;

/// <summary>Request from a customer to cancel their reservation of a seat.</summary>
public sealed record CancelReservationCommand(Guid ScreeningId, string SeatId, string Customer);
