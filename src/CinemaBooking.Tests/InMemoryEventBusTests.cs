using CinemaBooking.Events;
using CinemaBooking.Read;
using CinemaBooking.Write;

namespace CinemaBooking.Tests;

/// <summary>Immediate and deferred delivery of events.</summary>
[TestFixture]
public class InMemoryEventBusTests
{
    private static readonly SeatReserved AnySeatReserved = new(Guid.NewGuid(), "A1", "anna");

    [Test]
    public void Publish_Immediate_DeliversToSubscriberAtOnce()
    {
        var bus = new InMemoryEventBus();
        var handler = new RecordingHandler<SeatReserved>();
        bus.Subscribe(handler);

        bus.Publish(AnySeatReserved);

        Assert.That(handler.Received, Is.EqualTo(new[] { AnySeatReserved }));
        Assert.That(bus.PendingCount, Is.Zero);
    }

    [Test]
    public void Publish_OnlyDeliversToSubscribersOfThatEventType()
    {
        var bus = new InMemoryEventBus();
        var cancelledHandler = new RecordingHandler<ReservationCancelled>();
        bus.Subscribe(cancelledHandler);

        bus.Publish(AnySeatReserved);

        Assert.That(cancelledHandler.Received, Is.Empty);
    }

    [Test]
    public void Publish_Deferred_QueuesUntilProcessPending()
    {
        var bus = new InMemoryEventBus(deferDelivery: true);
        var handler = new RecordingHandler<SeatReserved>();
        bus.Subscribe(handler);

        bus.Publish(AnySeatReserved);
        bus.Publish(AnySeatReserved with { SeatId = "A2" });

        Assert.That(handler.Received, Is.Empty);
        Assert.That(bus.PendingCount, Is.EqualTo(2));

        var delivered = bus.ProcessPending();

        Assert.That(delivered, Is.EqualTo(2));
        Assert.That(handler.Received.Select(e => e.SeatId), Is.EqualTo(new[] { "A1", "A2" }));
        Assert.That(bus.PendingCount, Is.Zero);
    }

    [Test]
    public void DeferredDelivery_ReadModelIsStaleUntilEventsAreProcessed()
    {
        var cinema = CinemaBookingSetup.Create(deferDelivery: true);
        var screening = new Screening(Guid.NewGuid(), "Movie", DateTime.Today, 2, 2);
        cinema.AddScreening(screening);

        cinema.ReserveSeat.Handle(new ReserveSeatCommand(screening.Id, "A1", "anna"));
        var before = cinema.GetSeatMap.Handle(new GetSeatMapQuery(screening.Id));
        cinema.Events.ProcessPending();
        var after = cinema.GetSeatMap.Handle(new GetSeatMapQuery(screening.Id));

        Assert.That(before.FreeSeats, Is.EqualTo(4), "stale view before delivery");
        Assert.That(after.FreeSeats, Is.EqualTo(3), "consistent view after delivery");
    }

    private sealed class RecordingHandler<TEvent> : IEventHandler<TEvent> where TEvent : IDomainEvent
    {
        public List<TEvent> Received { get; } = [];

        public void Handle(TEvent domainEvent) => Received.Add(domainEvent);
    }
}
