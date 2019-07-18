using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOnlineLevel : MonoBehaviour
{
    [SerializeField] private GameObject m_levelList;
    [SerializeField] private LoadLevel m_loadLevel;

    public void LoadTheLevel()
    {
        m_levelList.SetActive(false);
        Debug.Log("loaded online level");

        //TODO: load level with its code
    }
}
