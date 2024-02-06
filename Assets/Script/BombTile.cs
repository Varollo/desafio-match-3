using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BombTile : Tile
{
    private const int radius = 2;

    protected override Type GetTileViewType() => typeof(BombTileView);

    public override TileCallbackData OnTileDestroyed(Vector2Int tile, BoardData boardData)
    {        
        return new TileCallbackData()
        {
            toDestroy = GetCircle(tile, radius, new Vector2Int(boardData.width, boardData.height)),
        };
    }

    private static bool IsInCircle(Vector2Int point, int radius, Vector2Int center = default) => radius * (radius + 1) >
        ((center.x - point.x) * (center.x - point.x)) + 
        ((center.y - point.y) * (center.y - point.y));

    private static bool IsInBounds(Vector2Int point, Vector2Int bounds) => 
        point.x >= 0 && point.x < bounds.x &&
        point.y >= 0 && point.y < bounds.y;

    private static List<Vector2Int> GetCircle(Vector2Int center, int radius, Vector2Int bounds)
    {
        List<Vector2Int> circle = new List<Vector2Int>();
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                Vector2Int point = new Vector2Int(center.x+x, center.y+y);
                if (IsInBounds(point, bounds) && IsInCircle(point, radius, center))
                    circle.Add(point);
            }
        }
        return circle;
    }
}
