using CinemaBooking.Events;

namespace CinemaBooking.Write;

/// <summary>
/// Handles <see cref="ReserveSeatCommand"/>: load, let the aggregate decide, save, publish.
/// The handler holds no business rules; those are in <see cref="Screening"/>.
/// </summary>
public sealed class ReserveSeatHandler(IScreeningRepository screenings, IEventPublisher events)
    : ICommandHandler<ReserveSeatCommand>
{
    public void Handle(ReserveSeatCommand command)
    {
        var screening = screenings.Get(command.ScreeningId);

        // One command per screening at a time, so the events reach the read side
        // in the same order as the decisions were made on the write side.
        lock (screening)
        {
            var seatReserved = screening.Reserve(command.SeatId, command.Customer);
            screenings.Save(screening);
            events.Publish(seatReserved);
        }
    }
}
