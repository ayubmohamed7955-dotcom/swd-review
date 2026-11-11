namespace CinemaBooking.Events;

/// <summary>
/// Marker for something that has happened on the write side.
/// Events are the only thing the read side learns from the write side.
/// </summary>
public interface IDomainEvent { }
