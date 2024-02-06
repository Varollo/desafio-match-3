using System;
using System.Collections.Generic;

public static class TileFactory
{
    #region Make Tile
    public static Tile MakeTile(Type type, int id, int group)
    {
        if (type == null || (type != typeof(Tile) && !type.IsSubclassOf(typeof(Tile))))
            throw new Exception($"Provided type \"{type}\" not a valid tile type.");

        Tile result = Activator.CreateInstance(type) as Tile;
        result.id = id;
        result.group = group;
        return result;
    }

    public static TTile MakeTile<TTile>(int id, int group) where TTile : Tile =>
        MakeTile(typeof(TTile), id, group) as TTile;

    public static Tile MakeTile(int id, int group) => MakeTile<Tile>(id, group);

    #endregion

    #region Random Tile
    private static readonly WeightMap<Type> _tileWeightMap = new WeightMap<Type>()
    {
        { typeof(DiceTile),   1.00f },
        { typeof(CherryTile), 0.25f },
        { typeof(StarTile),   0.17f },
        { typeof(BombTile),   0.08f },
    };

    public static Tile RandomTile(int id, int group, params Type[] filterTypes) =>
        MakeTile(_tileWeightMap[RNGManager.Value, filterTypes], id, group);

    public static Tile RandomTile(int id, ICollection<int> groups, params Type[] filterTypes) =>
        RandomTile(id, RNGManager.From(groups), filterTypes); 

    #endregion

    public static Tile CopyTile(Tile toCopy) => MakeTile(toCopy.GetType(), toCopy.id, toCopy.group);
    public static Tile EmptyTile() => MakeTile(-1, -1);
}
