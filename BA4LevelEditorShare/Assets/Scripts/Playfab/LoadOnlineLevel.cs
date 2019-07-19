using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadOnlineLevel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_levelCodeText, m_levelDesignerText, m_levelRatingText;

    private string m_levelCode;
    private string m_ownerPlayFabID;

    public void LoadTheLevel()
    {
        GameObject.FindGameObjectWithTag("LevelList").SetActive(false);
        Debug.Log("loaded online level");

        //TODO: load level with its code
        LoadLevel.instance.LoadLevelWithCode(m_levelCode);
    }

    public void SetLevelData(string levelCode, string levelDesigner, string levelRating, string ownerPlayFabId)
    {
        m_levelCode = levelCode;
        m_ownerPlayFabID = ownerPlayFabId;

        m_levelCodeText.text = levelCode;
        m_levelDesignerText.text = levelDesigner;
        m_levelRatingText.text = levelRating;
    }
}
