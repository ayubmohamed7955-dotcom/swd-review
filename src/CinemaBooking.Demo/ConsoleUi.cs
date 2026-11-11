using CinemaBooking.Read;

namespace CinemaBooking.Demo;

/// <summary>
/// Console output for the demo. Colours follow the slides:
/// red for the write side (commands), cyan for the read side (queries).
/// </summary>
internal static class ConsoleUi
{
    public static bool StepMode { get; set; }

    public static void Title(string text)
    {
        var line = new string('=', text.Length + 4);
        Write(ConsoleColor.White, $"{line}\n  {text}\n{line}");
    }

    public static void Step(int number, string text)
    {
        Console.WriteLine();
        Write(ConsoleColor.White, $"--- Step {number}: {text} ---");
    }

    public static void Info(string text) => Write(ConsoleColor.Gray, $"  {text}");

    public static void Command(string text) => Write(ConsoleColor.Red, $"  [COMMAND] {text}");

    public static void Query(string text) => Write(ConsoleColor.Cyan, $"  [QUERY]   {text}");

    public static void Event(string text) => Write(ConsoleColor.DarkGray, $"  [EVENT]   {text}");

    public static void Accepted(string text) => Write(ConsoleColor.Green, $"     OK     {text}");

    public static void Rejected(string text) => Write(ConsoleColor.Yellow, $"     REJECTED  {text}");

    public static void Result(string text) => Write(ConsoleColor.Cyan, $"     {text}");

    /// <summary>Draws the seat map as a grid: X = taken, . = free.</summary>
    public static void SeatMap(SeatMapView map)
    {
        var rows = map.Seats.GroupBy(seat => seat.SeatId[0]).ToList();
        var seatsPerRow = rows[0].Count();

        Result($"{map.Movie}, {map.StartsAt:ddd d MMM HH:mm}");
        Result($"      {"SCREEN".PadLeft(seatsPerRow * 3 / 2 + 3)}");
        Result("     " + string.Concat(Enumerable.Range(1, seatsPerRow).Select(n => $"{n,3}")));
        foreach (var row in rows)
        {
            var cells = row.Select(seat => seat.Status == SeatStatus.Taken ? "  X" : "  .");
            Result($"  {row.Key}  " + string.Concat(cells));
        }
        Result($"Free seats: {map.FreeSeats} of {map.Seats.Count}   (X = taken, . = free)");
    }

    /// <summary>With --step, waits for a key so the presenter can talk over each step.</summary>
    public static void Pause()
    {
        if (!StepMode || Console.IsInputRedirected) return;
        Write(ConsoleColor.DarkGray, "  (press any key)");
        Console.ReadKey(intercept: true);
    }

    private static void Write(ConsoleColor colour, string text)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
