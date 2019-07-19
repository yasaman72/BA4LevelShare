using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelRatingManager : MonoBehaviour
{
    [SerializeField] private GameObject m_rateButton;
    [SerializeField] private TextMeshProUGUI m_rateText;
    [SerializeField] private TMP_InputField m_inputField;
    [SerializeField] private LoadingScreenManager _loadingScreenManager;
    [SerializeField] private DeleteLevel _deleteLevel;


    public void CheckEnteredRate(string entry)
    {
        if (int.TryParse(entry, out var value))
        {
            Debug.Log("value: " + value);
            if (value > 10)
            {
                m_inputField.text = "10";
            }
            else if (value < 0)
            {
                m_inputField.text = "0";
            }
        }
    }

    public void FinishedEntringRate(string entry)
    {
        m_rateButton.SetActive(!string.IsNullOrEmpty(entry));
    }

    public void RateLevel()
    {
        LevelData thisLevelData = CurrentLevelData.instance.thisLevelData;

        string ownerId = thisLevelData.OwnerPlayFabId;
        string levelRateKey = thisLevelData.levelRateKey;
        string levelRateCountKey = thisLevelData.RatingCountKey;

        Debug.Log("thisLevelData.RateCount: " + thisLevelData.RateCount 
                  + " | thisLevelData.Rating): " + thisLevelData.Rating);

        int.TryParse(thisLevelData.RateCount, out int levelRatingAmount);
        int.TryParse(thisLevelData.Rating, out int levelRatingValue);
        int.TryParse(m_inputField.text, out int newRate);

        // calculate average of rating
        int finalLevelRate = ((levelRatingValue * levelRatingAmount) + newRate) / (levelRatingAmount + 1);

        _loadingScreenManager.ShowLoading();
        // send the rating and rate count for the level to the server
        PlayFabController.instance.UpdateUserReadOnlyData(ownerId, levelRateKey, finalLevelRate.ToString(), () =>
         {
             PlayFabController.instance.UpdateUserReadOnlyData(ownerId, levelRateCountKey, (levelRatingAmount + 1).ToString(),
                () =>
                {
                    // clear scene
                    CurrentLevelData.instance.ChangeRateButtonVisibility(false);
                    _deleteLevel.DeleteTheLevel();
                    _loadingScreenManager.HideLoading();
                });

         });
    }
}
