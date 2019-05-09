using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;

[CreateAssetMenu(fileName = "Tile", menuName = "GameAssets/Tile", order = 1)]
public class Tile : ScriptableObject
{
    public int ID;
    public Sprite sprite;
    public bool blocksMove;
}
