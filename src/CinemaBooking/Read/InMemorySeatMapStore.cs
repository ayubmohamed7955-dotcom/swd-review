namespace CinemaBooking.Read;

/// <summary>
/// Keeps one ready-made <see cref="SeatMapView"/> per screening. The views are immutable,
/// so every update stores a new view and a query always gets a consistent snapshot.
/// </summary>
public sealed class InMemorySeatMapStore : ISeatMapStore
{
    private readonly Dictionary<Guid, SeatMapView> _views = new();
    private readonly object _gate = new();

    public SeatMapView Get(Guid screeningId)
    {
        lock (_gate)
        {
            return _views.TryGetValue(screeningId, out var view)
                ? view
                : throw new KeyNotFoundException($"No seat map for screening {screeningId}.");
        }
    }

    public void Initialise(Guid screeningId, string movie, DateTime startsAt, IEnumerable<string> seatIds)
    {
        var seats = seatIds.Select(seatId => new SeatView(seatId, SeatStatus.Free)).ToList();
        lock (_gate)
            _views[screeningId] = new SeatMapView(screeningId, movie, startsAt, seats, seats.Count);
    }

    public void MarkTaken(Guid screeningId, string seatId) => SetStatus(screeningId, seatId, SeatStatus.Taken);

    public void MarkFree(Guid screeningId, string seatId) => SetStatus(screeningId, seatId, SeatStatus.Free);

    private void SetStatus(Guid screeningId, string seatId, SeatStatus status)
    {
        lock (_gate)
        {
            var view = Get(screeningId);
            var seats = view.Seats
                .Select(seat => seat.SeatId == seatId ? seat with { Status = status } : seat)
                .ToList();

            _views[screeningId] = view with
            {
                Seats = seats,
                FreeSeats = seats.Count(seat => seat.Status == SeatStatus.Free),
            };
        }
    }
}
