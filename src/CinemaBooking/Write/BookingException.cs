namespace CinemaBooking.Write;

/// <summary>Base class for every business rule the write side can reject a command with.</summary>
public abstract class BookingException : Exception
{
    protected BookingException(string message) : base(message) { }
}
