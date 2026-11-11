namespace CinemaBooking.Write;

/// <summary>
/// Handles a command: a request to change state. A command returns nothing;
/// it either succeeds or throws a <see cref="BookingException"/>.
/// </summary>
public interface ICommandHandler<in TCommand>
{
    void Handle(TCommand command);
}
