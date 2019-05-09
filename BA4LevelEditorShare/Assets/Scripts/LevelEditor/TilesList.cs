using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TileList", menuName = "GameAssets/TileList", order = 1)]
public class TilesList : ScriptableObject
{
    public Tile[] tiles;
}
