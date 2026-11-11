using CinemaBooking.Events;

namespace CinemaBooking.Write;

/// <summary>Handles <see cref="CancelReservationCommand"/>: load, let the aggregate decide, save, publish.</summary>
public sealed class CancelReservationHandler(IScreeningRepository screenings, IEventPublisher events)
    : ICommandHandler<CancelReservationCommand>
{
    public void Handle(CancelReservationCommand command)
    {
        var screening = screenings.Get(command.ScreeningId);

        // One command per screening at a time, so the events reach the read side
        // in the same order as the decisions were made on the write side.
        lock (screening)
        {
            var reservationCancelled = screening.Cancel(command.SeatId, command.Customer);
            screenings.Save(screening);
            events.Publish(reservationCancelled);
        }
    }
}
