namespace CinemaBooking.Write;

/// <summary>Thrown when a customer tries to reserve a seat that someone already holds.</summary>
public sealed class SeatAlreadyReservedException : BookingException
{
    public SeatAlreadyReservedException(Guid screeningId, string seatId)
        : base($"Seat {seatId} is already reserved.")
    {
        ScreeningId = screeningId;
        SeatId = seatId;
    }

    public Guid ScreeningId { get; }
    public string SeatId { get; }
}
