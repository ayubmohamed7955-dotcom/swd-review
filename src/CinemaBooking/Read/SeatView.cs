namespace CinemaBooking.Read;

/// <summary>One seat as shown on the seat map.</summary>
public sealed record SeatView(string SeatId, SeatStatus Status);
