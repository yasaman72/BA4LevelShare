using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLevelData : MonoBehaviour
{
    [SerializeField] private GameObject m_rateButton;
    public LevelData thisLevelData;

    public static CurrentLevelData instance { get; private set; }
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

    public void ChangeRateButtonVisibility(bool isOn)
    {
        m_rateButton.SetActive(isOn);
    }
}