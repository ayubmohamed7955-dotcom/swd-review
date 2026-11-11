namespace CinemaBooking.Events;

/// <summary>A seat in a screening was reserved by a customer.</summary>
public sealed record SeatReserved(Guid ScreeningId, string SeatId, string Customer) : IDomainEvent;
