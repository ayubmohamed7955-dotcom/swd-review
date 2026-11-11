namespace CinemaBooking.Write;

/// <summary>Thrown when a command refers to a screening that does not exist.</summary>
public sealed class ScreeningNotFoundException : BookingException
{
    public ScreeningNotFoundException(Guid screeningId)
        : base($"Screening {screeningId} does not exist.")
    {
        ScreeningId = screeningId;
    }

    public Guid ScreeningId { get; }
}
