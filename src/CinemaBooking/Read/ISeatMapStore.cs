namespace CinemaBooking.Read;

/// <summary>Storage for seat map read models. Only the projection writes to it; queries only read.</summary>
public interface ISeatMapStore
{
    /// <exception cref="KeyNotFoundException">No seat map has been initialised for the screening.</exception>
    SeatMapView Get(Guid screeningId);
    void Initialise(Guid screeningId, string movie, DateTime startsAt, IEnumerable<string> seatIds);
    void MarkTaken(Guid screeningId, string seatId);
    void MarkFree(Guid screeningId, string seatId);
}
