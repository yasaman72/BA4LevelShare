using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class ShareLevel : MonoBehaviour
{
    string SharedLevelAmountKey = "SharedLevelAmount";
    int sharedLevelsAmount;


    delegate void AfterGettingLevelAmountDelegate();
    AfterGettingLevelAmountDelegate _afterGettingLevelAmountDelegate;

    public void ShareTheLevel()
    {
        // first get the amount of created levels by this user. The number is then used for the SharedLevelAmountKey of the saving level
        _afterGettingLevelAmountDelegate = OnShare;
        // Get shared level amount
        PlayFabController.instance.GetUserData(SharedLevelAmountKey, OnFinishedGettingLevelAmount);
    }

    private void OnShare()
    {
        if (sharedLevelsAmount == -1)
            return;

        string newLevelKey = "sharedLevelNo" + (sharedLevelsAmount + 1);
        string newLevelValue = SaveLevel.instance.GetCurrentLevelCode();

        PlayFabController.instance.SetUserData(newLevelKey, newLevelValue);
        //increment the SharedLevelAmountKey value by one
        PlayFabController.instance.SetUserData(SharedLevelAmountKey, (sharedLevelsAmount + 1).ToString());
    }
    
    // for debugging
    public void LoadLastSharedLevel()
    {
        _afterGettingLevelAmountDelegate = OnLoad;
        // Get shared level amount
        PlayFabController.instance.GetUserData(SharedLevelAmountKey, OnFinishedGettingLevelAmount);
        }

    private void OnLoad()
    {
        if (sharedLevelsAmount == -1)
            return;

        string LevelKey = "sharedLevelNo" + sharedLevelsAmount;
        Debug.Log("this is the level key: " + LevelKey);
        PlayFabController.instance.GetUserData(LevelKey, OnFinishedGettingLevelData);
    }

    private void OnFinishedGettingLevelAmount(GetUserDataResultHolder levelCountRequestResult)
    {
        switch (levelCountRequestResult.response)
        {
            case Response.Error:
                Debug.Log("Error in getting shared levels amount");
                sharedLevelsAmount = -1;
                break;
            case Response.noKey:
                PlayFabController.instance.SetUserData(SharedLevelAmountKey, "0");
                sharedLevelsAmount = 0;

                break;
            case Response.result:
                sharedLevelsAmount = int.Parse(levelCountRequestResult.value);
                break;
        }

        _afterGettingLevelAmountDelegate.Invoke();
    }

    private void OnFinishedGettingLevelData(GetUserDataResultHolder levelCountRequestResult)
    {
        switch (levelCountRequestResult.response)
        {
            case Response.Error:
                Debug.Log("Error in getting levels data.");
                break;
            case Response.noKey:
                Debug.Log("No level found with this key.");
                break;
            case Response.result:
                LoadLevel.instance.LoadLevelWithCode(levelCountRequestResult.value);
                break;
        }
    }
}
