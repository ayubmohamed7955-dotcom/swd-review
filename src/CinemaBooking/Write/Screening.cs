using CinemaBooking.Events;

namespace CinemaBooking.Write;

/// <summary>
/// The write model: one showing of a movie, and who holds which seat.
/// <para>
/// All business rules live here. Every change returns the domain event that describes it,
/// which the command handler then publishes to the read side.
/// </para>
/// </summary>
public sealed class Screening
{
    private const int MaxRows = 26; // one letter per row: A to Z

    private readonly List<string> _seatIds;
    private readonly Dictionary<string, string> _reservations = new(); // seat id -> customer
    private readonly object _gate = new();

    public Screening(Guid id, string movie, DateTime startsAt, int rows, int seatsPerRow)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(movie);
        ArgumentOutOfRangeException.ThrowIfLessThan(rows, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rows, MaxRows);
        ArgumentOutOfRangeException.ThrowIfLessThan(seatsPerRow, 1);

        Id = id;
        Movie = movie;
        StartsAt = startsAt;
        _seatIds = CreateSeatIds(rows, seatsPerRow);
    }

    public Guid Id { get; }
    public string Movie { get; }
    public DateTime StartsAt { get; }

    /// <summary>Every seat in the hall, row by row: "A1", "A2", ..., "F10".</summary>
    public IReadOnlyCollection<string> AllSeatIds => _seatIds.AsReadOnly();

    /// <summary>Reserves a free seat. A seat can never be held by two customers.</summary>
    /// <exception cref="UnknownSeatException">The seat does not exist.</exception>
    /// <exception cref="SeatAlreadyReservedException">The seat is already taken.</exception>
    public SeatReserved Reserve(string seatId, string customer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customer);

        // The lock makes "check, then reserve" atomic, so two concurrent
        // commands for the same seat can never both succeed.
        lock (_gate)
        {
            EnsureSeatExists(seatId);
            if (_reservations.ContainsKey(seatId))
                throw new SeatAlreadyReservedException(Id, seatId);

            _reservations[seatId] = customer;
        }

        return new SeatReserved(Id, seatId, customer);
    }

    /// <summary>Cancels a reservation. Only the customer who holds the seat can cancel it.</summary>
    /// <exception cref="UnknownSeatException">The seat does not exist.</exception>
    /// <exception cref="ReservationNotFoundException">The customer does not hold the seat.</exception>
    public ReservationCancelled Cancel(string seatId, string customer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customer);

        lock (_gate)
        {
            EnsureSeatExists(seatId);
            if (!_reservations.TryGetValue(seatId, out var holder) || holder != customer)
                throw new ReservationNotFoundException(Id, seatId, customer);

            _reservations.Remove(seatId);
        }

        return new ReservationCancelled(Id, seatId, customer);
    }

    private void EnsureSeatExists(string seatId)
    {
        if (!_seatIds.Contains(seatId))
            throw new UnknownSeatException(Id, seatId);
    }

    private static List<string> CreateSeatIds(int rows, int seatsPerRow) =>
        (from row in Enumerable.Range(0, rows)
         from number in Enumerable.Range(1, seatsPerRow)
         select $"{(char)('A' + row)}{number}").ToList();
}
