namespace CinemaBooking.Events;

/// <summary>A customer cancelled their reservation of a seat, so the seat is free again.</summary>
public sealed record ReservationCancelled(Guid ScreeningId, string SeatId, string Customer) : IDomainEvent;
