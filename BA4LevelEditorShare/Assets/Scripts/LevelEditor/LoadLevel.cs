using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private GameObject m_levelGrid;
    [SerializeField] private SetLogText m_setLogText;

    public void LoadLastLevel()
    {
        string levelString;
        if (PlayerPrefs.HasKey("lastSavedLevel"))
        {
            levelString = PlayerPrefs.GetString("lastSavedLevel");
        }
        else
        {
            m_setLogText.ShowMessage("You have no saved level.");
            return;
        }

        LoadLevelWithCode(levelString);

        m_setLogText.ShowMessage("Loaded last level.");
    }

    public void LoadLevelWithCode(string levelString)
    {
        for (int i = 0; i < m_levelGrid.transform.childCount; i++)
        {
            int tileId = int.Parse(levelString[i].ToString());
            m_levelGrid.transform.GetChild(i).GetComponent<ToggleTile>().SetTileToTileId(tileId);
        }
    }
}
