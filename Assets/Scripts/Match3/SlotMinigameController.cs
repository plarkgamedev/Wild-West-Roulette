using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotMinigameController : MonoBehaviour
{
    enum State { Match3, Busy, Spinning, Done }

    [Header("References")]
    [SerializeField] Match3Config config;
    [SerializeField] RectTransform boardArea;
    [SerializeField] TileView tilePrefab;
    [SerializeField] Sprite bulletSprite;
    [SerializeField] TMP_Text movesText;
    [SerializeField] ResultsPanel results;

    [Header("Layout (canvas units, preview in Scene view when selected)")]
    [SerializeField] float padLeft;
    [SerializeField] float padRight;
    [SerializeField] float padTop;
    [SerializeField] float padBottom;
    [SerializeField] Vector2 spacing = new Vector2(8f, 8f);
    [SerializeField, Range(0.2f, 1f)] float tileScale = 0.85f;
    [Tooltip("On: cells are squares and the grid is centered. Off: cells stretch to fill the area.")]
    [SerializeField] bool squareCells = true;
    [Tooltip("Optional per-column vertical shift. Positive moves up. Leave empty for none.")]
    [SerializeField] float[] columnYOffset;
    [SerializeField] float[] RowXOffset;

    [Header("Flow")]
    [SerializeField] float resultsDelay = 0.8f;

    Match3Board board;
    TileView[,] views;
    System.Random rng;
    State state;
    int movesLeft;
    TileView selected;
    Vector2 cellSize;
    Vector2 origin;     // top-left of the grid, measured from boardArea's top-left corner

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        rng = new System.Random();
        ComputeLayout();
        board = new Match3Board(config.rows, config.columns, () => config.Roll(rng));
        views = new TileView[config.rows, config.columns];
        BuildViews();
        movesLeft = config.moves;
        UpdateHud();
        state = State.Match3;
    }

    // ---------- layout ----------
    void ComputeLayout()
    {
        int cols = config.columns, rows = config.rows;
        var rect = boardArea.rect;
        float usableW = rect.width - padLeft - padRight;
        float usableH = rect.height - padTop - padBottom;
        float cw = (usableW - spacing.x * (cols - 1)) / cols;
        float ch = (usableH - spacing.y * (rows - 1)) / rows;
        if (squareCells) { float s = Mathf.Min(cw, ch); cw = ch = s; }
        cellSize = new Vector2(cw, ch);
        float gridW = cw * cols + spacing.x * (cols - 1);
        float gridH = ch * rows + spacing.y * (rows - 1);
        origin = new Vector2(padLeft + (usableW - gridW) / 2f, padTop + (usableH - gridH) / 2f);
    }

    Vector2 CellCenter(int r, int c)
    {
        float x = origin.x + c * (cellSize.x + spacing.x) + cellSize.x / 2f;
        float y = -(origin.y + r * (cellSize.y + spacing.y) + cellSize.y / 2f);
        if (columnYOffset != null && c < columnYOffset.Length) y += columnYOffset[c];
        if (RowXOffset != null && c < RowXOffset.Length) x += RowXOffset[c];
        return new Vector2(x, y);
    }

    void OnDrawGizmosSelected()
    {
        if (!boardArea || !config) return;
        ComputeLayout();
        var rect = boardArea.rect;
        Vector3 P(Vector2 p) => boardArea.TransformPoint(new Vector3(rect.xMin + p.x, rect.yMax + p.y, 0f));
        Vector2 half = cellSize * tileScale * 0.5f;
        Gizmos.color = Color.yellow;
        for (int r = 0; r < config.rows; r++)
            for (int c = 0; c < config.columns; c++)
            {
                Vector2 m = CellCenter(r, c);
                Vector3 a = P(m + new Vector2(-half.x, half.y)), b = P(m + new Vector2(half.x, half.y));
                Vector3 d = P(m + new Vector2(-half.x, -half.y)), e = P(m + new Vector2(half.x, -half.y));
                Gizmos.DrawLine(a, b); Gizmos.DrawLine(b, e); Gizmos.DrawLine(e, d); Gizmos.DrawLine(d, a);
            }
    }

    // ---------- views ----------
    TileView Spawn(Rarity r, Vector2 pos)
    {
        var v = Instantiate(tilePrefab, boardArea);
        var rt = v.Rect;
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = cellSize * tileScale;
        rt.anchoredPosition = pos;
        v.Init(bulletSprite, config.Tint(r), OnTileClicked);
        return v;
    }

    void BuildViews()
    {
        for (int r = 0; r < config.rows; r++)
            for (int c = 0; c < config.columns; c++)
            {
                if (views[r, c]) Destroy(views[r, c].gameObject);
                views[r, c] = Spawn(board.Get(r, c), CellCenter(r, c));
            }
    }

    void UpdateHud() => movesText.text = $"Moves: {movesLeft}";

    bool Find(TileView v, out int row, out int col)
    {
        for (int r = 0; r < config.rows; r++)
            for (int c = 0; c < config.columns; c++)
                if (views[r, c] == v) { row = r; col = c; return true; }
        row = col = -1;
        return false;
    }

    // ---------- match-3 state ----------
    void OnTileClicked(TileView v)
    {
        if (state != State.Match3) return;
        if (selected == null) { selected = v; v.SetSelected(true); return; }
        if (selected == v) { v.SetSelected(false); selected = null; return; }

        Find(selected, out int r1, out int c1);
        Find(v, out int r2, out int c2);
        selected.SetSelected(false);
        selected = null;

        if (!board.AreAdjacent(r1, c1, r2, c2)) { selected = v; v.SetSelected(true); return; }
        StartCoroutine(SwapRoutine(r1, c1, r2, c2));
    }

    IEnumerator SwapRoutine(int r1, int c1, int r2, int c2)
    {
        state = State.Busy;
        bool valid = board.TrySwap(r1, c1, r2, c2);
        var a = views[r1, c1];
        var b = views[r2, c2];

        yield return Move(new List<(TileView, Vector2)> { (a, CellCenter(r2, c2)), (b, CellCenter(r1, c1)) }, config.swapTime);

        if (!valid)
        {
            yield return Move(new List<(TileView, Vector2)> { (a, CellCenter(r1, c1)), (b, CellCenter(r2, c2)) }, config.swapTime);
            state = State.Match3;
            yield break;
        }

        views[r1, c1] = b;
        views[r2, c2] = a;
        movesLeft--;
        UpdateHud();

        yield return Cascade();

        if (movesLeft <= 0) { StartCoroutine(SlotRoutine()); yield break; }

        if (!board.HasPossibleMove())
        {
            board.Fill();
            BuildViews();
        }
        state = State.Match3;
    }

    IEnumerator Cascade()
    {
        while (true)
        {
            var matches = board.FindMatches();
            if (matches.Count == 0) yield break;

            var popped = new List<TileView>();
            foreach (var (r, c) in matches) { popped.Add(views[r, c]); views[r, c] = null; }
            board.Clear(matches);
            yield return Shrink(popped, config.clearTime);
            foreach (var v in popped) Destroy(v.gameObject);

            var moves = new List<(TileView, Vector2)>();
            foreach (var (col, fromRow, toRow) in board.ApplyGravity())
            {
                var v = views[fromRow, col];
                views[fromRow, col] = null;
                views[toRow, col] = v;
                moves.Add((v, CellCenter(toRow, col)));
            }
            foreach (var (row, col, count) in board.Refill())
            {
                var v = Spawn(board.Get(row, col), CellCenter(row - count, col));
                views[row, col] = v;
                moves.Add((v, CellCenter(row, col)));
            }
            yield return Move(moves, config.fallTime);
        }
    }

    IEnumerator Move(List<(TileView view, Vector2 target)> moves, float duration)
    {
        var starts = new List<Vector2>();
        foreach (var m in moves) starts.Add(m.view.Rect.anchoredPosition);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            for (int i = 0; i < moves.Count; i++)
                moves[i].view.Rect.anchoredPosition = Vector2.Lerp(starts[i], moves[i].target, k);
            yield return null;
        }
        foreach (var m in moves) m.view.Rect.anchoredPosition = m.target;
    }

    IEnumerator Shrink(List<TileView> list, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float s = 1f - t / duration;
            foreach (var v in list) v.Rect.localScale = Vector3.one * s;
            yield return null;
        }
    }

    // ---------- slot state ----------
    IEnumerator SlotRoutine()
    {
        state = State.Spinning;
        movesText.text = "SPIN!";
        foreach (var v in views) v.SetDim(0.4f);
        yield return new WaitForSeconds(config.spinStartDelay);

        var result = new Rarity[config.columns];
        int running = config.columns;
        for (int c = 0; c < config.columns; c++)
        {
            int col = c;
            StartCoroutine(SpinColumn(col, r => { result[col] = r; running--; }));
        }
        while (running > 0) yield return null;

        GameManager.Instance.Run.LastSlotResult = result;
        state = State.Done;
        movesText.text = "Done!";

        yield return new WaitForSeconds(resultsDelay);
        results.Show(result, Continue);
    }

    IEnumerator SpinColumn(int col, System.Action<Rarity> done)
    {
        int rows = config.rows;
        int target = rng.Next(rows);
        int hops = rows * (2 + col) + target;
        int current = -1;

        for (int i = 1; i <= hops; i++)
        {
            if (current >= 0) views[current, col].SetDim(0.4f);
            current = i % rows;
            views[current, col].SetDim(1f);
            float k = (float)i / hops;
            yield return new WaitForSeconds(Mathf.Lerp(config.spinStepFast, config.spinStepSlow, k * k));
        }

        views[target, col].Rect.localScale = Vector3.one * 1.2f;
        done(board.Get(target, col));
    }

    void Continue()
    {
        GameManager.Instance.Run.Advance();
        SceneLoader.Instance.Load("Hub");
    }
}