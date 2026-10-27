using System;
using System.Collections.Generic;

public class Match3Board
{
    public const int Empty = -1;
    public int Rows { get; }
    public int Cols { get; }
    public readonly int[] ClearedCounts = new int[4];   // per rarity, for biasing the slot later

    readonly int[,] cells;
    readonly Func<Rarity> roll;

    public Match3Board(int rows, int cols, Func<Rarity> roll)
    {
        Rows = rows; Cols = cols; this.roll = roll;
        cells = new int[rows, cols];
        Fill();
    }

    public Rarity Get(int r, int c) => (Rarity)cells[r, c];

    // Fills the board with no starting matches and at least one possible move.
    public void Fill()
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                {
                    int v, tries = 0;
                    do { v = (int)roll(); tries++; }
                    while (tries < 50 && MakesRun(r, c, v));
                    cells[r, c] = v;
                }
            if (FindMatches().Count == 0 && HasPossibleMove()) return;
        }
    }

    bool MakesRun(int r, int c, int v) =>
        (c >= 2 && cells[r, c - 1] == v && cells[r, c - 2] == v) ||
        (r >= 2 && cells[r - 1, c] == v && cells[r - 2, c] == v);

    public bool AreAdjacent(int r1, int c1, int r2, int c2) =>
        Math.Abs(r1 - r2) + Math.Abs(c1 - c2) == 1;

    // Swaps and keeps the swap only if it creates a match.
    public bool TrySwap(int r1, int c1, int r2, int c2)
    {
        if (!AreAdjacent(r1, c1, r2, c2)) return false;
        Swap(r1, c1, r2, c2);
        if (FindMatches().Count > 0) return true;
        Swap(r1, c1, r2, c2);
        return false;
    }

    void Swap(int r1, int c1, int r2, int c2) =>
        (cells[r1, c1], cells[r2, c2]) = (cells[r2, c2], cells[r1, c1]);

    public HashSet<(int r, int c)> FindMatches()
    {
        var set = new HashSet<(int r, int c)>();
        for (int r = 0; r < Rows; r++)
        {
            int run = 1;
            for (int c = 1; c <= Cols; c++)
            {
                bool same = c < Cols && cells[r, c] != Empty && cells[r, c] == cells[r, c - 1];
                if (same) { run++; continue; }
                if (run >= 3) for (int k = c - run; k < c; k++) set.Add((r, k));
                run = 1;
            }
        }
        for (int c = 0; c < Cols; c++)
        {
            int run = 1;
            for (int r = 1; r <= Rows; r++)
            {
                bool same = r < Rows && cells[r, c] != Empty && cells[r, c] == cells[r - 1, c];
                if (same) { run++; continue; }
                if (run >= 3) for (int k = r - run; k < r; k++) set.Add((k, c));
                run = 1;
            }
        }
        return set;
    }

    public void Clear(HashSet<(int r, int c)> set)
    {
        foreach (var (r, c) in set) { ClearedCounts[cells[r, c]]++; cells[r, c] = Empty; }
    }

    // Row 0 is the top. Tiles fall toward higher row numbers.
    public List<(int col, int fromRow, int toRow)> ApplyGravity()
    {
        var moves = new List<(int col, int fromRow, int toRow)>();
        for (int c = 0; c < Cols; c++)
        {
            int write = Rows - 1;
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (cells[r, c] == Empty) continue;
                if (r != write)
                {
                    cells[write, c] = cells[r, c];
                    cells[r, c] = Empty;
                    moves.Add((c, r, write));
                }
                write--;
            }
        }
        return moves;
    }

    // Returns new tiles as (row, col, how many new tiles are in that column).
    public List<(int row, int col, int count)> Refill()
    {
        var spawned = new List<(int row, int col, int count)>();
        for (int c = 0; c < Cols; c++)
        {
            int empties = 0;
            while (empties < Rows && cells[empties, c] == Empty) empties++;
            for (int r = 0; r < empties; r++)
            {
                cells[r, c] = (int)roll();
                spawned.Add((r, c, empties));
            }
        }
        return spawned;
    }

    public bool HasPossibleMove()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
            {
                if (c + 1 < Cols && WouldMatch(r, c, r, c + 1)) return true;
                if (r + 1 < Rows && WouldMatch(r, c, r + 1, c)) return true;
            }
        return false;
    }

    bool WouldMatch(int r1, int c1, int r2, int c2)
    {
        Swap(r1, c1, r2, c2);
        bool m = FindMatches().Count > 0;
        Swap(r1, c1, r2, c2);
        return m;
    }
}