namespace CinemaBooking.Read;

/// <summary>Asks for every seat a customer currently holds.</summary>
public sealed record GetCustomerBookingsQuery(string Customer);
