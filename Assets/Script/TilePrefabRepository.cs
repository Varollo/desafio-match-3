using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TilePrefabRepository", menuName = "Gameplay/TilePrefabRepository")]
public class TilePrefabRepository : ScriptableObject
{
    [SerializeField] private TileView[] tileTypePrefabList;
    
    private Dictionary<Type, TileView> _tileTypeDictionary;

    public TileView CreateTileInstance(Type tileViewType) => Instantiate((
        _tileTypeDictionary ??= tileTypePrefabList.ToDictionary(view => view.GetType())) [tileViewType]);
}