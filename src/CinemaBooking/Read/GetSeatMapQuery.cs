namespace CinemaBooking.Read;

/// <summary>Asks for the seat map of one screening.</summary>
public sealed record GetSeatMapQuery(Guid ScreeningId);
