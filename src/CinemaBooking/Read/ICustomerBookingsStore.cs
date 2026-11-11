namespace CinemaBooking.Read;

/// <summary>Storage for the customer bookings read model.</summary>
public interface ICustomerBookingsStore
{
    /// <summary>Returns the customer's bookings; an empty list if they have none.</summary>
    CustomerBookingsView Get(string customer);

    void Add(string customer, BookingLine booking);
    void Remove(string customer, BookingLine booking);
}
