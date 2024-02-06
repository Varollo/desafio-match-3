using UnityEngine;

[CreateAssetMenu(fileName = "TileColorRepository", menuName = "Gameplay/TileColorRepository")]
public class TileColorRepository : ScriptableObject
{
    [SerializeField] private Color[] _colorList;

    public Color GetColor(int index) => _colorList[index];
}
