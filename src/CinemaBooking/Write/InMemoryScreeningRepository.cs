using System.Collections.Concurrent;

namespace CinemaBooking.Write;

/// <summary>Write-side storage kept in memory. A real system would use a database here.</summary>
public sealed class InMemoryScreeningRepository : IScreeningRepository
{
    private readonly ConcurrentDictionary<Guid, Screening> _screenings = new();

    public Screening Get(Guid id) =>
        _screenings.TryGetValue(id, out var screening)
            ? screening
            : throw new ScreeningNotFoundException(id);

    public void Save(Screening screening)
    {
        ArgumentNullException.ThrowIfNull(screening);
        _screenings[screening.Id] = screening;
    }
}
