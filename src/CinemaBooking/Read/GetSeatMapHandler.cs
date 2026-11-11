namespace CinemaBooking.Read;

/// <summary>Returns the seat map read model. No business rules, no calculations: just a lookup.</summary>
public sealed class GetSeatMapHandler(ISeatMapStore store) : IQueryHandler<GetSeatMapQuery, SeatMapView>
{
    public SeatMapView Handle(GetSeatMapQuery query) => store.Get(query.ScreeningId);
}
