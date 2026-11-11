using CinemaBooking.Read;
using CinemaBooking.Write;

namespace CinemaBooking.Tests;

/// <summary>The query handlers, used through the wired-up system: command in, query out.</summary>
[TestFixture]
public class QueryHandlerTests
{
    private CinemaBookingSystem _cinema = null!;
    private Screening _screening = null!;

    [SetUp]
    public void SetUp()
    {
        _cinema = CinemaBookingSetup.Create();
        _screening = new Screening(Guid.NewGuid(), "The Grand Budapest Hotel", new DateTime(2026, 11, 6, 19, 30, 0), 6, 10);
        _cinema.AddScreening(_screening);
    }

    [Test]
    public void GetSeatMap_NewScreening_AllSeatsFree()
    {
        var view = _cinema.GetSeatMap.Handle(new GetSeatMapQuery(_screening.Id));

        Assert.That(view.Movie, Is.EqualTo("The Grand Budapest Hotel"));
        Assert.That(view.Seats, Has.Count.EqualTo(60));
        Assert.That(view.FreeSeats, Is.EqualTo(60));
    }

    [Test]
    public void GetSeatMap_AfterReservation_ShowsSeatTaken()
    {
        _cinema.ReserveSeat.Handle(new ReserveSeatCommand(_screening.Id, "F7", "anna"));

        var view = _cinema.GetSeatMap.Handle(new GetSeatMapQuery(_screening.Id));

        Assert.That(view.Seats.Single(s => s.SeatId == "F7").Status, Is.EqualTo(SeatStatus.Taken));
        Assert.That(view.FreeSeats, Is.EqualTo(59));
    }

    [Test]
    public void GetCustomerBookings_AfterReservations_ListsOnlyThatCustomersSeats()
    {
        _cinema.ReserveSeat.Handle(new ReserveSeatCommand(_screening.Id, "F7", "anna"));
        _cinema.ReserveSeat.Handle(new ReserveSeatCommand(_screening.Id, "F8", "anna"));
        _cinema.ReserveSeat.Handle(new ReserveSeatCommand(_screening.Id, "A1", "bob"));

        var view = _cinema.GetCustomerBookings.Handle(new GetCustomerBookingsQuery("anna"));

        Assert.That(view.Bookings.Select(b => b.SeatId), Is.EqualTo(new[] { "F7", "F8" }));
    }

    [Test]
    public void GetCustomerBookings_UnknownCustomer_ReturnsEmptyView()
    {
        var view = _cinema.GetCustomerBookings.Handle(new GetCustomerBookingsQuery("nobody"));

        Assert.That(view.Customer, Is.EqualTo("nobody"));
        Assert.That(view.Bookings, Is.Empty);
    }
}
