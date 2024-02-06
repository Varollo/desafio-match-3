using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardSequence
{
    public Tween beforeSequence = TweenUtils.GetBlankTween();
    public List<Vector2Int> matchedPosition = new List<Vector2Int>();
    public List<AddedTileInfo> addedTiles = new List<AddedTileInfo>();
    public List<MovedTileInfo> movedTiles = new List<MovedTileInfo>();

    public override string ToString()
    {
        string log;
        log = "matchedPosition: \n";
        for (int i = 0; i < matchedPosition.Count; i++)
        {
            log += $"{matchedPosition[i]}, ";
        }

        log += "\naddedTiles: \n";
        for (int i = 0; i < addedTiles.Count; i++)
        {
            log += $"{addedTiles[i].position}, ";
        }

        log += $"\nmovedTiles: {movedTiles.Count}\n";
        for (int i = 0; i < movedTiles.Count; i++)
        {
            log += $"{movedTiles[i].from} - {movedTiles[i].to}, ";
        }

        //log = $"matchedPosition: {matchedPosition.Count} - addedTiles: {addedTiles.Count} - movedTiles: {movedTiles.Count}";
        return log;
    }
}