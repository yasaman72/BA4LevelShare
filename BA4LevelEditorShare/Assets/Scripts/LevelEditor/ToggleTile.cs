using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTile : MonoBehaviour
{
    public TilesList tileList;
    public bool unchangeAble;
    public int currenTileID;

    private Image image;
    private Button button;


    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
        button = gameObject.GetComponent<Button>();

        SetStartingTile();
    }

    public void SetStartingTile()
    {
        if (unchangeAble)
        {
            bool findBlockingTile = false;
            foreach (var tile in tileList.tiles)
            {
                if (tile.blocksMove)
                {
                    currenTileID = tile.ID;
                    button.interactable = false;

                    findBlockingTile = true;
                    break;
                }
            }

            if (!findBlockingTile)
                Debug.LogError("Couldn't find any tile that blocks moving!");
        }
        else
        {
            currenTileID = tileList.tiles[0].ID;
        }

        SetTileAppearance();
    }

    public void ToggleTileType()
    {
        if (unchangeAble) return;

        if (currenTileID + 1 < tileList.tiles.Length)
        {
            currenTileID++;
        }
        else
        {
            currenTileID = 0;
        }
        SetTileAppearance();
    }

    private void SetTileAppearance()
    {
        image.sprite = tileList.tiles[currenTileID].sprite;
    }

    public void SetTileToTileId(int tileId)
    {
        if (tileId >= tileList.tiles.Length)
        {
            Debug.Log("the "+ tileId + " tile id is higher than available tiles");
            return;
        }
        currenTileID = tileId;
        SetTileAppearance();
    }
}
