﻿using DG.Tweening;
using UnityEngine;

public class TileView : MonoBehaviour
{
    /// <summary>
    /// Called when tile moves as a result of a chain reaction, NOT when swapped.
    /// </summary>
    public virtual Tween OnTileMove(MovedTileInfo moveInfo, BoardView board) => TweenUtils.GetBlankTween();

    /// <summary>
    /// Called when tile is destroyed as a result of a match, when score is computed.
    /// </summary>
    public virtual Tween OnTileDestroyed(Vector2Int tile, BoardView board) => TweenUtils.GetBlankTween();
}
