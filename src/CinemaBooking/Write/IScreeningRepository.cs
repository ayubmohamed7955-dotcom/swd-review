namespace CinemaBooking.Write;

/// <summary>Loads and stores screenings for the write side.</summary>
public interface IScreeningRepository
{
    /// <exception cref="ScreeningNotFoundException">No screening has that id.</exception>
    Screening Get(Guid id);

    void Save(Screening screening);
}
