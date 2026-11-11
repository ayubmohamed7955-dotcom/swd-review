// Demo application for the demonstration video.
// Shows the write side (commands, red) and the read side (queries, cyan) of the CQRS example.
// Run with --step to pause between steps.

using System.Globalization;
using CinemaBooking;
using CinemaBooking.Demo;
using CinemaBooking.Read;
using CinemaBooking.Write;
using static CinemaBooking.Demo.ConsoleUi;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture; // English dates on every machine
StepMode = args.Contains("--step");
var startsAt = new DateTime(2026, 11, 6, 19, 30, 0);
const string Anna = "anna@example.com";
const string Bob = "bob@example.com";

Title("CinemaBooking: a CQRS demo");

// 1. Set up a screening and fill a few seats.
Step(1, "Create a screening and pre-book some seats");
var cinema = CinemaBookingSetup.Create();
var screening = new Screening(Guid.NewGuid(), "The Grand Budapest Hotel", startsAt, rows: 6, seatsPerRow: 10);
cinema.AddScreening(screening);
Info($"{screening.Movie}, Hall 2, {screening.StartsAt:ddd d MMM HH:mm}, {screening.AllSeatIds.Count} seats");

string[] preBooked = ["C4", "C5", "D5", "D6", "E3"];
foreach (var seat in preBooked)
    cinema.ReserveSeat.Handle(new ReserveSeatCommand(screening.Id, seat, "guest@example.com"));
Info($"Pre-booked: {string.Join(", ", preBooked)}");

EventLog.AttachTo(cinema.Events); // from now on, print events as they reach the read side
Pause();

// 2. A command that the write side accepts.
Step(2, "Anna reserves seat F7");
Command($"ReserveSeatCommand(F7, {Anna})");
cinema.ReserveSeat.Handle(new ReserveSeatCommand(screening.Id, "F7", Anna));
Accepted("Seat F7 reserved");
Pause();

// 3. A command that breaks a business rule. The write side rejects it.
Step(3, "Bob tries to reserve the same seat");
Command($"ReserveSeatCommand(F7, {Bob})");
try
{
    cinema.ReserveSeat.Handle(new ReserveSeatCommand(screening.Id, "F7", Bob));
}
catch (BookingException ex)
{
    Rejected($"{ex.GetType().Name}: {ex.Message}");
}
Pause();

// 4. A query served by the seat map read model.
Step(4, "Query the seat map");
Query("GetSeatMapQuery(screening)");
SeatMap(cinema.GetSeatMap.Handle(new GetSeatMapQuery(screening.Id)));
Pause();

// 5. A second read model, built from the same events.
Step(5, "Query Anna's bookings (a second read model)");
Query($"GetCustomerBookingsQuery({Anna})");
ShowBookings(cinema.GetCustomerBookings.Handle(new GetCustomerBookingsQuery(Anna)));
Pause();

// 6. Eventual consistency: events wait on the bus before they reach the read side.
Step(6, "Eventual consistency: the read side lags behind");
var delayed = CinemaBookingSetup.Create(deferDelivery: true);
var lateShow = new Screening(Guid.NewGuid(), "The Grand Budapest Hotel", startsAt.AddHours(3), rows: 6, seatsPerRow: 10);
delayed.AddScreening(lateShow);
EventLog.AttachTo(delayed.Events);
Info($"Late show at {lateShow.StartsAt:HH:mm}. Events now wait on the bus, like a slow message broker.");

Command($"ReserveSeatCommand(A1, {Bob})");
delayed.ReserveSeat.Handle(new ReserveSeatCommand(lateShow.Id, "A1", Bob));
Accepted($"Seat A1 reserved on the write side. Events waiting on the bus: {delayed.Events.PendingCount}");

Query("GetSeatMapQuery(late show)");
ShowSeat(delayed.GetSeatMap.Handle(new GetSeatMapQuery(lateShow.Id)), "A1", note: "(stale)");

Info("Bus delivers the pending events...");
var delivered = delayed.Events.ProcessPending();
Info($"ProcessPending() delivered {delivered} event(s)");

Query("GetSeatMapQuery(late show)");
ShowSeat(delayed.GetSeatMap.Handle(new GetSeatMapQuery(lateShow.Id)), "A1", note: "(consistent)");
Pause();

// 7. Cancelling is a command too; both read models follow.
Step(7, "Anna cancels F7");
Command($"CancelReservationCommand(F7, {Anna})");
cinema.CancelReservation.Handle(new CancelReservationCommand(screening.Id, "F7", Anna));
Accepted("Reservation cancelled");

Query("GetSeatMapQuery(screening)");
SeatMap(cinema.GetSeatMap.Handle(new GetSeatMapQuery(screening.Id)));
Query($"GetCustomerBookingsQuery({Anna})");
ShowBookings(cinema.GetCustomerBookings.Handle(new GetCustomerBookingsQuery(Anna)));

Console.WriteLine();
Info("Done.");

static void ShowBookings(CustomerBookingsView view)
{
    if (view.Bookings.Count == 0)
        Result($"{view.Customer} has no bookings");

    foreach (var booking in view.Bookings)
        Result($"{view.Customer}: seat {booking.SeatId} (screening {booking.ScreeningId.ToString()[..8]})");
}

static void ShowSeat(SeatMapView map, string seatId, string note)
{
    var seat = map.Seats.Single(s => s.SeatId == seatId);
    Result($"Seat {seatId}: {seat.Status} {note}   Free seats: {map.FreeSeats} of {map.Seats.Count}");
}
