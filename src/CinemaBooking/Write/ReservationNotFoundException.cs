namespace CinemaBooking.Write;

/// <summary>Thrown when a customer cancels a seat they have not reserved.</summary>
public sealed class ReservationNotFoundException : BookingException
{
    public ReservationNotFoundException(Guid screeningId, string seatId, string customer)
        : base($"{customer} has no reservation for seat {seatId}.")
    {
        ScreeningId = screeningId;
        SeatId = seatId;
        Customer = customer;
    }

    public Guid ScreeningId { get; }
    public string SeatId { get; }
    public string Customer { get; }
}
