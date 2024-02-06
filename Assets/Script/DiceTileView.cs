using UnityEngine;

public class DiceTileView : TileView
{
    [SerializeField] private Sprite[] dices;

    protected override Sprite GetSprite(Tile tile) => dices[tile.group];
}
