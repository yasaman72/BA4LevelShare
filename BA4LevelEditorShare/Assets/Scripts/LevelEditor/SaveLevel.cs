using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLevel : MonoBehaviour
{
    [SerializeField] private GameObject m_levelGrid;
    [SerializeField] private SetLogText m_setLogText;

    private string m_levelCode;

    public static SaveLevel instance { get; private set; }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveCurrentLevel()
    {
        SetLevelCode();

        PlayerPrefs.SetString("lastSavedLevel", m_levelCode);

        m_setLogText.ShowMessage("Level has been saved!");
    }

    public string GetCurrentLevelCode()
    {
        SetLevelCode();
        return m_levelCode;
    }

    private void SetLevelCode()
    {
        m_levelCode = "";
        foreach (Transform child in m_levelGrid.transform)
        {
            string id = child.GetComponent<ToggleTile>().currenTileID.ToString();
            m_levelCode += id;
        }
    }
}
