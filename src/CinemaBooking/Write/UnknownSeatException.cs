namespace CinemaBooking.Write;

/// <summary>Thrown when a seat id does not exist in the hall of the screening.</summary>
public sealed class UnknownSeatException : BookingException
{
    public UnknownSeatException(Guid screeningId, string seatId)
        : base($"Seat {seatId} does not exist in this screening.")
    {
        ScreeningId = screeningId;
        SeatId = seatId;
    }

    public Guid ScreeningId { get; }
    public string SeatId { get; }
}
