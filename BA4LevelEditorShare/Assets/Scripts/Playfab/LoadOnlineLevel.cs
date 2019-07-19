using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadOnlineLevel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_levelCodeText, m_levelDesignerText, m_levelRatingText;

    private string m_levelCode;
    private string m_ownerPlayFabID;
    private string m_levelKey;
    private string m_levelRateKey;

    private LevelData m_levelData;

    public void LoadTheLevel()
    {
        CurrentLevelData.instance.ChangeRateButtonVisibility(true);
        CurrentLevelData.instance.thisLevelData = m_levelData;
        Debug.Log("m_levelCode: " + m_levelCode);

        GameObject.FindGameObjectWithTag("LevelList").SetActive(false);

        // load level with its code
        LoadLevel.instance.LoadLevelWithCode(m_levelData.LevelCode);
    }

    public void SetLevelData(LevelData levelData)
    {
        m_levelData = levelData;

        m_levelCodeText.text = m_levelData.LevelCode;
        m_levelDesignerText.text = m_levelData.DesignerName;
        m_levelRatingText.text = m_levelData.Rating;
    }
}
