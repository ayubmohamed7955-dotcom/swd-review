using CinemaBooking.Read;
using CinemaBooking.Write;

namespace CinemaBooking.Tests;

/// <summary>The write side must never sell the same seat twice, even under concurrent commands.</summary>
[TestFixture]
public class ConcurrencyTests
{
    [Test]
    public void ReserveSeat_TwoConcurrentRequestsForSameSeat_ExactlyOneSucceeds()
    {
        // Repeat the race many times so an unsafe implementation would almost surely fail.
        for (var round = 0; round < 200; round++)
        {
            var (cinema, screening) = CreateCinema();
            using var start = new Barrier(2);

            var results = new[] { "anna", "bob" }
                .Select(customer => Task.Run(() =>
                {
                    start.SignalAndWait();
                    return TryReserve(cinema, screening.Id, "F7", customer);
                }))
                .ToArray();
            Task.WaitAll(results);

            Assert.That(results.Count(r => r.Result), Is.EqualTo(1), $"round {round}");
        }
    }

    [Test]
    public void ReserveSeat_ManyCustomersRaceForOneSeat_ReadModelsShowOneBooking()
    {
        var (cinema, screening) = CreateCinema();
        var customers = Enumerable.Range(1, 50).Select(i => $"customer{i}").ToArray();

        var successes = 0;
        Parallel.ForEach(customers, customer =>
        {
            if (TryReserve(cinema, screening.Id, "F7", customer))
                Interlocked.Increment(ref successes);
        });

        var seatMap = cinema.GetSeatMap.Handle(new GetSeatMapQuery(screening.Id));
        var totalBookings = customers.Sum(c => cinema.GetCustomerBookings.Handle(new GetCustomerBookingsQuery(c)).Bookings.Count);

        Assert.That(successes, Is.EqualTo(1));
        Assert.That(seatMap.FreeSeats, Is.EqualTo(59));
        Assert.That(totalBookings, Is.EqualTo(1));
    }

    [Test]
    public void ReserveAndCancel_RacingOnSameSeat_ReadModelsMatchWriteSide()
    {
        // Anna keeps reserving and cancelling F7 while Bob tries to grab it. If the events
        // could reach the read side in another order than the write side decided them,
        // Anna's late "cancelled" would overwrite Bob's "reserved" and show his seat as free.
        for (var round = 0; round < 200; round++)
        {
            var (cinema, screening) = CreateCinema();
            var bobHasSeat = false;

            var anna = Task.Run(() =>
            {
                for (var i = 0; i < 50 && !Volatile.Read(ref bobHasSeat); i++)
                {
                    if (TryReserve(cinema, screening.Id, "F7", "anna"))
                        cinema.CancelReservation.Handle(new CancelReservationCommand(screening.Id, "F7", "anna"));
                }
            });
            var bob = Task.Run(() =>
            {
                while (!TryReserve(cinema, screening.Id, "F7", "bob")) { }
                Volatile.Write(ref bobHasSeat, true);
            });
            Task.WaitAll(anna, bob);

            var seatMap = cinema.GetSeatMap.Handle(new GetSeatMapQuery(screening.Id));
            Assert.That(seatMap.Seats.Single(s => s.SeatId == "F7").Status, Is.EqualTo(SeatStatus.Taken), $"round {round}");
            Assert.That(cinema.GetCustomerBookings.Handle(new GetCustomerBookingsQuery("anna")).Bookings, Is.Empty, $"round {round}");
            Assert.That(cinema.GetCustomerBookings.Handle(new GetCustomerBookingsQuery("bob")).Bookings, Has.Count.EqualTo(1), $"round {round}");
        }
    }

    private static (CinemaBookingSystem, Screening) CreateCinema()
    {
        var cinema = CinemaBookingSetup.Create();
        var screening = new Screening(Guid.NewGuid(), "The Grand Budapest Hotel", DateTime.Today, 6, 10);
        cinema.AddScreening(screening);
        return (cinema, screening);
    }

    private static bool TryReserve(CinemaBookingSystem cinema, Guid screeningId, string seatId, string customer)
    {
        try
        {
            cinema.ReserveSeat.Handle(new ReserveSeatCommand(screeningId, seatId, customer));
            return true;
        }
        catch (SeatAlreadyReservedException)
        {
            return false;
        }
    }
}
