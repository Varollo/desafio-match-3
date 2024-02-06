using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class TileFactoryTests
{
    [Test]
    public void TileFactoryGetTileTests()
    {
        Assert.NotNull(TileFactory.MakeTile(-1, 0), "Error trying to get tile of GROUP = 0.");
        Assert.NotNull(TileFactory.MakeTile(-1, -1), "Error trying to get tile of GROUP = -1.");
        Assert.NotNull(TileFactory.MakeTile(-1, 42069), "Error trying to get INVALID tile of GROUP = 42069.");
    }

    [Test]
    public void TileFactoryGetRandomTileTests()
    {
        Type[][] filters = new Type[][]
        {
            null,
            new Type[0],
            new Type[]  { typeof(Tile)                             },
            new Type[]  { typeof(BombTile),       typeof(StarTile) },
            new Type[]  { typeof(Tile),           typeof(BombTile) },
            new Type[]  { typeof(GameController), typeof(Tile)     },
        };

        int[][] groups = new int[][]
        {
            null,
            new int[0],
            new int[] {-1, -2},
            new int[] {0,   1},
        };

        foreach(var group in groups)
            foreach (var filter in filters)
            {
                var tile = TileFactory.RandomTile(-1, group, filter);                                
                
                Assert.NotNull(tile);

                Assert.IsTrue(filter == null || filter.Length == 0 || 
                    !filter.All(t => t == typeof(Tile) || t.IsSubclassOf(typeof(Tile))) ||
                     filters.Any(f => f != null && f.Contains(tile.GetType())));

                Assert.IsTrue(group  == null || group.Length  == 0 ||
                    groups.Any (g => g != null && g.Contains(tile.group)));
            }
    }
}
