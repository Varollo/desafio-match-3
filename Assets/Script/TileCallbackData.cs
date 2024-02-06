using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct TileCallbackData
{
    public List<Vector2Int> toDestroy;
    public Tween beforeSequence;

    public static TileCallbackData Empty => new TileCallbackData();

    public static TileCallbackData operator +(TileCallbackData a, TileCallbackData b) => new TileCallbackData()
    {
        // Joins the lists. If one list is null, result is the other.
        toDestroy = a.toDestroy != null && b.toDestroy != null ?
            a.toDestroy.Union(b.toDestroy).ToList() :
            a.toDestroy ?? b.toDestroy,

        beforeSequence = DOTween.Sequence().Append(a.beforeSequence).Join(b.beforeSequence)
    };
}