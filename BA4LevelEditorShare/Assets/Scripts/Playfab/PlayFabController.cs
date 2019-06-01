using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class PlayFabController : MonoBehaviour
{
    public int playerLevel;

    public static PlayFabController instance { get; private set; }


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

    /// <summary>
    /// this function can be used to set player statistics from client.
    /// Playfab doesn't allow this by default.
    /// For it to work project settings should change in Playfab cosnsole
    /// </summary>
    public void SetStatistics()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {StatisticName = "playerLevel", Value = playerLevel}
                //,new StatisticUpdate {StatisticName = "playerLevel", Value = playerLevel}
            }
        },
            result => { Debug.Log("User's statistic updated!"); },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    public void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch (eachStat.StatisticName)
            {
                case "playerLevel":
                    playerLevel = eachStat.Value;
                    break;
                default:
                    break;
            }
        }
    }



    // cloud script execution
    public void StartCloudUpdatePlayerStats()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "updatePlayerStats", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { level = playerLevel }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnCloudUpdatePlayerStats, OnErrorShared);
    }

    private static void OnCloudUpdatePlayerStats(ExecuteCloudScriptResult result)
    {
        // Cloud Script returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        Debug.Log(PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).SerializeObject(result.FunctionResult));
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in Cloud Script
        Debug.Log((string)messageValue);
    }

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
