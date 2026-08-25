# Advent of Code

My personal solutions to [Advent of Code](https://adventofcode.com/) puzzles, written in C#.

## Overview

This repository is a personal archive, published so the code can be **read**. It is not a library to depend on and not a
project seeking collaborators.
Issues and pull requests are not accepted — the repository exists as a record of how I approached each puzzle, and the
code changes whenever I feel like changing it.

If you are here to look at a particular day's solution, the fastest route is `src/AoC.Solutions/YearXXXX/DayNN.cs`.
Everything else in the repository exists to support those files.

Structurally, it is a solution library plus a runner:

| Project | Purpose |
| --- | --- |
| `AoC.Core` | An engine that discovers, loads inputs for, and runs puzzle solutions. |
| `AoC.Solutions` | The puzzle solutions themselves, one class per day. |
| `AoC.App` | A WPF launcher that discovers all solutions by reflection, runs them, and reports results and timings. |

### Why a launcher instead of a console app

Advent of Code accumulates fast — 25 puzzles per year, two parts each. A console app forces you to either recompile with
a different entry point or parse command-line arguments for every run. The launcher removes that friction: it lists
every discovered solution, runs the selected one on demand, colour-codes elapsed time so slow solutions stand out, and
links directly to the puzzle page on adventofcode.com.

The design goal was that a new puzzle costs exactly one attributed class and one input file — no registration lists, no
`switch` statements, no UI changes.

## Requirements

`.NET 10`

## Puzzle inputs

**Puzzle inputs are not part of this repository.** Advent of Code inputs are generated per user, and Eric Wastl
[asks that they not be redistributed](https://adventofcode.com/about) — so `Inputs/` is excluded via `.gitignore`.

This means the launcher has nothing to run against on a fresh clone. That is intentional. The solutions are here to be
read; reproducing my answers is not the point, and my input would not match yours anyway.

## License

This project is licensed under the MIT License.
