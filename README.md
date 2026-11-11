# CinemaBooking: CQRS

SW4SWD Design Patterns Assignment, E26, Group 2 (Brightspace `PATT_2`)

An investigation of the **CQRS** pattern (Command Query Responsibility Segregation), with a cinema booking system as the C# example. Approved by Henrik Bitsch Kirk.

## Group members

| Name | auID | Contribution |
|---|---|---|
| Alexander Drexel Hougaard | au766270 | Report: introduction, background and motivation. Code: events and event bus. Presentation: slides 1 to 4 |
| Mads Kvisgaard Schnell | au766237 | Report: structure, dynamics, variations, consequences, limitations. Code: write side. Presentation: slides 5 to 7 |
| Morten Rode Enghausen | au529147 | Report: the example's design, implementation, diagrams and concurrency. Code: read side and setup. Presentation: slides 8 to 14, joining the video |
| Oskar Jentzsch Seeberg | au776273 | Report: eventual consistency in practice, tests, related patterns, analysis, conclusion. Code: demo and tests. Presentation: slides 15 to 18, demonstration video |

The assignment requires that each member's participation is visible. The report's chapter 7 has the same split in more detail.

## Structure

| Path | Content |
|---|---|
| `src/CinemaBooking/` | The CQRS example: write side and read side |
| `src/CinemaBooking.Demo/` | Console app used in the demonstration video |
| `src/CinemaBooking.Tests/` | NUnit tests |
| `report/` | The report in LaTeX: `main.tex`, one file per section in `sections/`, figures in `figures/` |
| `presentation/` | Slides and the final presentation and demonstration videos |
| `assets/` | Diagrams (draw.io, SVG, PNG) used in the report and slides |

## Running

Prerequisite: .NET 10 SDK.

```bash
dotnet build
dotnet test
dotnet run --project src/CinemaBooking.Demo
dotnet run --project src/CinemaBooking.Demo -- --step   # pause between steps when recording
```

## Hand-in

All five files go into one zip. Every file must contain the group number, names and auIDs.

| File | Requirement |
|---|---|
| `E26-SW4SWD-DesignPatternsAssignment-Group2-CQRS-report.pdf` | The report |
| `E26-SW4SWD-DesignPatternsAssignment-Group2-CQRS-presentation.mp4` | 7 to 10 minutes, under 100 MB, mkv or mp4 |
| `E26-SW4SWD-DesignPatternsAssignment-Group2-CQRS-presentation.pdf` | Slides from the presentation |
| `E26-SW4SWD-DesignPatternsAssignment-Group2-CQRS-demonstration.mp4` | 2 to 5 minutes, under 50 MB, mkv or mp4 |
| `E26-SW4SWD-DesignPatternsAssignment-Group2-CQRS-vs-project.zip` | This solution, standard zip (not 7z, rar or tgz) |
| `E26-SW4SWD-DesignPatternsAssignment-Group2-CQRS.zip` | The five files above |

- Hand-in is in **FeedbackFruits**, and **every member must have logged in** to FeedbackFruits before the group can hand in
- If generative AI is used, a **GAI declaration** must be handed in, see [AU's guide](https://studerende.au.dk/gai)
- The final videos are committed in `presentation/`; other videos and zip files are excluded by `.gitignore`

## Deadlines

| When | What |
|---|---|
| Fri 6/11 13:50 | Hand-in in FeedbackFruits |
| Fri 13/11 13:50 | Two individual peer reviews, about one hour each |

## Conventions

- **Everything in English**: report, slides, videos, code, comments and commit messages
- Commit messages in the imperative, for example "Add seat reservation command"
- The report is built with `latexmk -pdf main.tex` from `report/`, and the output goes to `report/build/main.pdf`. Copy it to `report/E26-SW4SWD-DesignPatternsAssignment-Group2-CQRS-report.pdf` when it is final. It uses minted, so Python with Pygments must be installed; `report/latexmkrc` turns on shell escape. The code listings are read directly from `src/` by line numbers, so update the numbers in `sections/03-example.tex` if the code moves
- A built copy of the report is committed as `report/E26-SW4SWD-DesignPatternsAssignment-Group2-CQRS-report.pdf`, so you can read it without building. Rebuild and replace it when the report changes
