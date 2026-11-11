namespace CinemaBooking.Read;

/// <summary>Keeps a list of booking lines per customer in memory.</summary>
public sealed class InMemoryCustomerBookingsStore : ICustomerBookingsStore
{
    private readonly Dictionary<string, List<BookingLine>> _bookings = new();
    private readonly object _gate = new();

    public CustomerBookingsView Get(string customer)
    {
        lock (_gate)
        {
            var lines = _bookings.TryGetValue(customer, out var list) ? list.ToList() : [];
            return new CustomerBookingsView(customer, lines);
        }
    }

    public void Add(string customer, BookingLine booking)
    {
        lock (_gate)
        {
            if (!_bookings.TryGetValue(customer, out var list))
                _bookings[customer] = list = [];

            // Ignore a duplicate, so an event delivered twice does not list the seat twice.
            if (!list.Contains(booking))
                list.Add(booking);
        }
    }

    public void Remove(string customer, BookingLine booking)
    {
        lock (_gate)
        {
            if (_bookings.TryGetValue(customer, out var list))
                list.Remove(booking);
        }
    }
}
