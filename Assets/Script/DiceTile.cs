using System;

public class DiceTile : Tile
{
    protected override Type GetTileViewType() => typeof(DiceTileView);
}
