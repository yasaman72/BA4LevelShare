using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLevel : MonoBehaviour
{
    [SerializeField] private GameObject m_levelGrid;
    [SerializeField] private SetLogText m_setLogText;

    private string m_levelCode;

    public void SaveCurrentLevel()
    {
        m_levelCode = "";
        foreach (Transform child in m_levelGrid.transform)
        {
            string id = child.GetComponent<ToggleTile>().currenTileID.ToString();
            m_levelCode += id;
        }

        PlayerPrefs.SetString("lastSavedLevel", m_levelCode);

        m_setLogText.ShowMessage("Level has been saved!");
    }
}
