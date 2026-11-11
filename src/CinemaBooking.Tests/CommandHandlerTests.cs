using CinemaBooking.Events;
using CinemaBooking.Write;

namespace CinemaBooking.Tests;

/// <summary>The command handlers: load, decide, save, publish.</summary>
[TestFixture]
public class CommandHandlerTests
{
    private InMemoryScreeningRepository _repository = null!;
    private RecordingPublisher _publisher = null!;
    private Screening _screening = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new InMemoryScreeningRepository();
        _publisher = new RecordingPublisher();
        _screening = new Screening(Guid.NewGuid(), "The Grand Budapest Hotel", DateTime.Today, 6, 10);
        _repository.Save(_screening);
    }

    [Test]
    public void ReserveSeat_FreeSeat_PublishesSeatReserved()
    {
        var handler = new ReserveSeatHandler(_repository, _publisher);

        handler.Handle(new ReserveSeatCommand(_screening.Id, "F7", "anna"));

        Assert.That(_publisher.Published, Is.EqualTo(new[] { new SeatReserved(_screening.Id, "F7", "anna") }));
    }

    [Test]
    public void ReserveSeat_SeatAlreadyTaken_ThrowsAndPublishesNothingNew()
    {
        var handler = new ReserveSeatHandler(_repository, _publisher);
        handler.Handle(new ReserveSeatCommand(_screening.Id, "F7", "anna"));

        Assert.Throws<SeatAlreadyReservedException>(() =>
            handler.Handle(new ReserveSeatCommand(_screening.Id, "F7", "bob")));
        Assert.That(_publisher.Published, Has.Count.EqualTo(1));
    }

    [Test]
    public void ReserveSeat_UnknownScreening_Throws()
    {
        var handler = new ReserveSeatHandler(_repository, _publisher);

        Assert.Throws<ScreeningNotFoundException>(() =>
            handler.Handle(new ReserveSeatCommand(Guid.NewGuid(), "F7", "anna")));
    }

    [Test]
    public void CancelReservation_ExistingReservation_PublishesReservationCancelled()
    {
        new ReserveSeatHandler(_repository, _publisher).Handle(new ReserveSeatCommand(_screening.Id, "F7", "anna"));
        var handler = new CancelReservationHandler(_repository, _publisher);

        handler.Handle(new CancelReservationCommand(_screening.Id, "F7", "anna"));

        Assert.That(_publisher.Published[^1], Is.EqualTo(new ReservationCancelled(_screening.Id, "F7", "anna")));
    }

    [Test]
    public void CancelReservation_NoReservation_ThrowsAndPublishesNothing()
    {
        var handler = new CancelReservationHandler(_repository, _publisher);

        Assert.Throws<ReservationNotFoundException>(() =>
            handler.Handle(new CancelReservationCommand(_screening.Id, "F7", "anna")));
        Assert.That(_publisher.Published, Is.Empty);
    }
}
