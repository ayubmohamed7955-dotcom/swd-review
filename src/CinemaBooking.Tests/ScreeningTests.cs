using CinemaBooking.Events;
using CinemaBooking.Write;

namespace CinemaBooking.Tests;

/// <summary>The business rules of the write model.</summary>
[TestFixture]
public class ScreeningTests
{
    private Screening _screening = null!;

    [SetUp]
    public void SetUp() =>
        _screening = new Screening(Guid.NewGuid(), "The Grand Budapest Hotel", new DateTime(2026, 11, 6, 19, 30, 0), 6, 10);

    [Test]
    public void Constructor_CreatesSeatIdsRowByRow()
    {
        var small = new Screening(Guid.NewGuid(), "Movie", DateTime.Today, rows: 2, seatsPerRow: 3);

        Assert.That(small.AllSeatIds, Is.EqualTo(new[] { "A1", "A2", "A3", "B1", "B2", "B3" }));
    }

    [Test]
    public void Constructor_MoreThan26Rows_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Screening(Guid.NewGuid(), "Movie", DateTime.Today, rows: 27, seatsPerRow: 10));
    }

    [Test]
    public void Reserve_FreeSeat_ReturnsSeatReservedEvent()
    {
        var domainEvent = _screening.Reserve("F7", "anna");

        Assert.That(domainEvent, Is.EqualTo(new SeatReserved(_screening.Id, "F7", "anna")));
    }

    [Test]
    public void Reserve_SeatAlreadyTaken_Throws()
    {
        _screening.Reserve("F7", "anna");

        Assert.Throws<SeatAlreadyReservedException>(() => _screening.Reserve("F7", "bob"));
    }

    [Test]
    public void Reserve_UnknownSeat_Throws()
    {
        Assert.Throws<UnknownSeatException>(() => _screening.Reserve("G1", "anna"));
    }

    [Test]
    public void Cancel_OwnReservation_ReturnsReservationCancelledEvent()
    {
        _screening.Reserve("F7", "anna");

        var domainEvent = _screening.Cancel("F7", "anna");

        Assert.That(domainEvent, Is.EqualTo(new ReservationCancelled(_screening.Id, "F7", "anna")));
    }

    [Test]
    public void Cancel_SeatHeldByAnotherCustomer_Throws()
    {
        _screening.Reserve("F7", "anna");

        Assert.Throws<ReservationNotFoundException>(() => _screening.Cancel("F7", "bob"));
    }

    [Test]
    public void Reserve_AfterCancel_Succeeds()
    {
        _screening.Reserve("F7", "anna");
        _screening.Cancel("F7", "anna");

        Assert.DoesNotThrow(() => _screening.Reserve("F7", "bob"));
    }
}
