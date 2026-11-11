using CinemaBooking.Events;
using CinemaBooking.Read;

namespace CinemaBooking.Tests;

/// <summary>The projections that turn events into read models.</summary>
[TestFixture]
public class ProjectionTests
{
    private static readonly Guid ScreeningId = Guid.NewGuid();

    [Test]
    public void SeatMapProjection_SeatReserved_MarksSeatTaken()
    {
        var store = CreateSeatMapStore();
        var projection = new SeatMapProjection(store);

        projection.Handle(new SeatReserved(ScreeningId, "A2", "anna"));

        var view = store.Get(ScreeningId);
        Assert.That(view.Seats.Single(s => s.SeatId == "A2").Status, Is.EqualTo(SeatStatus.Taken));
        Assert.That(view.FreeSeats, Is.EqualTo(2));
    }

    [Test]
    public void SeatMapProjection_ReservationCancelled_MarksSeatFree()
    {
        var store = CreateSeatMapStore();
        var projection = new SeatMapProjection(store);
        projection.Handle(new SeatReserved(ScreeningId, "A2", "anna"));

        projection.Handle(new ReservationCancelled(ScreeningId, "A2", "anna"));

        var view = store.Get(ScreeningId);
        Assert.That(view.Seats.Single(s => s.SeatId == "A2").Status, Is.EqualTo(SeatStatus.Free));
        Assert.That(view.FreeSeats, Is.EqualTo(3));
    }

    [Test]
    public void CustomerBookingsProjection_SeatReserved_AddsBookingLine()
    {
        var store = new InMemoryCustomerBookingsStore();
        var projection = new CustomerBookingsProjection(store);

        projection.Handle(new SeatReserved(ScreeningId, "A2", "anna"));

        Assert.That(store.Get("anna").Bookings, Is.EqualTo(new[] { new BookingLine(ScreeningId, "A2") }));
    }

    [Test]
    public void CustomerBookingsProjection_ReservationCancelled_RemovesBookingLine()
    {
        var store = new InMemoryCustomerBookingsStore();
        var projection = new CustomerBookingsProjection(store);
        projection.Handle(new SeatReserved(ScreeningId, "A2", "anna"));

        projection.Handle(new ReservationCancelled(ScreeningId, "A2", "anna"));

        Assert.That(store.Get("anna").Bookings, Is.Empty);
    }

    [Test]
    public void Projections_SameEventDeliveredTwice_ReadModelsUnchanged()
    {
        var seatMaps = CreateSeatMapStore();
        var bookings = new InMemoryCustomerBookingsStore();
        var seatMapProjection = new SeatMapProjection(seatMaps);
        var bookingsProjection = new CustomerBookingsProjection(bookings);
        var seatReserved = new SeatReserved(ScreeningId, "A2", "anna");

        // A real message broker may deliver an event more than once.
        for (var i = 0; i < 2; i++)
        {
            seatMapProjection.Handle(seatReserved);
            bookingsProjection.Handle(seatReserved);
        }

        Assert.That(seatMaps.Get(ScreeningId).FreeSeats, Is.EqualTo(2));
        Assert.That(bookings.Get("anna").Bookings, Has.Count.EqualTo(1));
    }

    private static InMemorySeatMapStore CreateSeatMapStore()
    {
        var store = new InMemorySeatMapStore();
        store.Initialise(ScreeningId, "Movie", DateTime.Today, ["A1", "A2", "A3"]);
        return store;
    }
}
