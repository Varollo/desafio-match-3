using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BombTileView : TileView
{
    [SerializeField] private int radius = 2;

    /// <summary>
    /// Calculates if a point(<paramref name="x"/>,<paramref name="y"/>) is inside the circle of given <paramref name="radius"/>.
    /// <para>Center of the circle is assumed to be at point(0,0), in relation to given coordinates.</para>
    /// </summary>
    private static bool IsInCircle(int x, int y, int radius) => (x * x) + (y * y) < radius * (radius + 1);
    private static bool IsInBounds(int x, int y, Vector2Int bounds) => x >= 0 && x < bounds.x && y >= 0 && y < bounds.y;

    public override Tween OnTileSwap(Vector2Int tile, BoardSequence currentSequence, BoardView board)
    {
        currentSequence.ClearTiles(GetCircle(tile, radius, board.GetSize()));
        return base.OnTileSwap(tile, currentSequence, board);
    }

    private static IEnumerable<Vector2Int> GetCircle(Vector2Int center, int radius, Vector2Int bounds)
    {
        List<Vector2Int> circle = new List<Vector2Int>();
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (IsInBounds(x, y, bounds) && IsInCircle(x, y, radius))
                    circle.Add(new Vector2Int(center.x + x, center.y + y));
            }
        }
        return circle;
    }
}
