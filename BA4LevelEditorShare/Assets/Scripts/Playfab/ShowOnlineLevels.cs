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
            PlayFabController.instance.GetUserData(SharedLevelAmountKey,
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

                    LevelData levelData = new LevelData();
                    levelData.DesignerName = entry.DisplayName;
                    levelData.OwnerPlayFabId = entry.PlayFabId;

                    OnFinishedGettingLevelAmount(sharedLevelsAmount, levelData);
                }
                , entry.PlayFabId);
        }
    }

    private void OnFinishedGettingLevelAmount(int sharedLevelsAmount, LevelData levelData)
    {
        int randomLevelIndex = Random.Range(1, sharedLevelsAmount + 1);

        string LevelKey = "sharedLevelNo" + randomLevelIndex;
        string newLevelRatingKey = "sharedLevelNo" + randomLevelIndex + "Rating";

        Debug.Log("random level index: " + randomLevelIndex);
        PlayFabController.instance.GetUserData(LevelKey,
            (levelRequestResult) =>
        {
            if (levelRequestResult.response == Response.result)
            {
                levelData.LevelCode = levelRequestResult.value;
                GetLevelRating(newLevelRatingKey, levelData);
            }
        });
    }

    private void GetLevelRating(string levelRatingKey, LevelData levelData)
    {
        PlayFabController.instance.GetUserData(levelRatingKey,
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
        item.GetComponent<LoadOnlineLevel>().SetLevelData(levelData.LevelCode, levelData.DesignerName, levelData.Rating, levelData.OwnerPlayFabId);
        _loadingScreenManager.HideLoading();
    }

    struct LevelData
    {
        public string LevelCode;
        public string DesignerName;
        public string Rating;
        public string OwnerPlayFabId;
    }
}
