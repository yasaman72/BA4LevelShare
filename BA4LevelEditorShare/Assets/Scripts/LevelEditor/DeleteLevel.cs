using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteLevel : MonoBehaviour
{
    [SerializeField] private GameObject m_levelGrid;

    [SerializeField] private SetLogText m_setLogText;


    public void DeleteTheLevel()
    {
        foreach (Transform child in m_levelGrid.transform)
        {
            child.GetComponent<ToggleTile>().SetStartingTile();
            m_setLogText.ShowMessage("Deleted the level.");
        }

        CurrentLevelData.instance.thisLevelData = new LevelData();
        CurrentLevelData.instance.ChangeRateButtonVisibility(false);
    }
}
