using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

public class ShowOnlineLevels : MonoBehaviour
{
    [SerializeField] private Transform listHolder;
    [SerializeField] private GameObject listItems;
    [SerializeField] private LoadingScreenManager _loadingScreenManager;

    string SharedLevelAmountKey = "SharedLevelAmount";

    string playerLevelCode, playerLevelRating;

    public void ShowLevelsLeaderboard()
    {
        _loadingScreenManager.ShowLoading();

        foreach (Transform child in listHolder.transform)
        {
            Destroy(child.gameObject);
        }

        PlayFabController.instance.GetLeaderboardDate("SharedLevelAmount", OnFinishedGettingLeaderboardDate);
    }

    private void OnFinishedGettingLeaderboardDate(List<PlayerLeaderboardEntry> playersEntry)
    {
        foreach (var entry in playersEntry)
        {
            _loadingScreenManager.ShowLoading();
            PlayFabController.instance.GetUserData(SharedLevelAmountKey
                , entry.PlayFabId,
                (levelCountRequestResult) =>
                {
                    int sharedLevelsAmount = 0;
                    switch (levelCountRequestResult.response)
                    {
                        case Response.Error:
                            Debug.Log("Error in getting shared levels amount");
                            return;
                            break;
                        case Response.noKey:
                            Debug.Log("Found no key");
                            return;
                            break;
                        case Response.result:
                            sharedLevelsAmount = int.Parse(levelCountRequestResult.value);
                            break;
                    }

                    LevelData levelData = new LevelData
                    {
                        DesignerName = entry.DisplayName,
                        OwnerPlayFabId = entry.PlayFabId
                    };

                    OnFinishedGettingLevelAmount(sharedLevelsAmount, levelData);
                });
        }
    }

    private void OnFinishedGettingLevelAmount(int sharedLevelsAmount, LevelData levelData)
    {
        int randomLevelIndex = Random.Range(1, sharedLevelsAmount + 1);

        levelData.levelKey = "sharedLevelNo" + randomLevelIndex;
        levelData.levelRateKey = "sharedLevelNo" + randomLevelIndex + "Rating";
        levelData.RatingCountKey = "sharedLevelNo" + randomLevelIndex + "RatingCountKey";


        Debug.Log("random level index: " + randomLevelIndex);
        PlayFabController.instance.GetUserData(levelData.levelKey,
            (levelRequestResult) =>
        {
            if (levelRequestResult.response == Response.result)
            {
                levelData.LevelCode = levelRequestResult.value;
                GetLevelRatingCount(levelData);
            }
        });
    }

    private void GetLevelRatingCount(LevelData levelData)
    {
        PlayFabController.instance.GetUserReadOnlyData(levelData.OwnerPlayFabId, levelData.RatingCountKey, (arg) =>
        {
            if (arg.response == Response.result)
            {
                levelData.RateCount = arg.value;
                GetLevelRatingVaue(levelData);
            }
        });
    }

    private void GetLevelRatingVaue(LevelData levelData)
    {
        PlayFabController.instance.GetUserReadOnlyData(levelData.OwnerPlayFabId, levelData.levelRateKey,
            (levelRatingRequestResult) =>
            {
                if (levelRatingRequestResult.response == Response.result)
                {
                    levelData.Rating = levelRatingRequestResult.value;
                    GenerateLevelHolder(levelData);
                }
            });
    }

    private void GenerateLevelHolder(LevelData levelData)
    {
        GameObject item = Instantiate(listItems, listHolder);
        item.GetComponent<LoadOnlineLevel>().SetLevelData(levelData);
        _loadingScreenManager.HideLoading();
    }


}
