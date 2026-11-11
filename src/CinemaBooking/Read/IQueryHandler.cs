namespace CinemaBooking.Read;

/// <summary>
/// Handles a query: a request for data. A query never changes state,
/// so it can be served from a read model shaped exactly for the screen that needs it.
/// </summary>
public interface IQueryHandler<in TQuery, out TResult>
{
    TResult Handle(TQuery query);
}
