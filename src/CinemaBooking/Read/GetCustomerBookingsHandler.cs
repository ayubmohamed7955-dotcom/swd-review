namespace CinemaBooking.Read;

/// <summary>Returns the customer bookings read model.</summary>
public sealed class GetCustomerBookingsHandler(ICustomerBookingsStore store)
    : IQueryHandler<GetCustomerBookingsQuery, CustomerBookingsView>
{
    public CustomerBookingsView Handle(GetCustomerBookingsQuery query) => store.Get(query.Customer);
}
