using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class PlayFabController : MonoBehaviour
{
    public int playerLevel;
    public string currentPlayerPlayfabID;

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

    public void SetStatistics(string statisticName, int value)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = statisticName, Value = value}
                }
        },
            result => { Debug.Log("User's statistic updated! statistic: " + statisticName + " value: " + value); },
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

    public void StartCloudUpdateSharedLevelsAmount(int value)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "updateSharedLevelsAmount", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { newValue = value }, // The parameter provided to your function
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

    public void SetUserData(string key, string value)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() { { key, value } },
        },
            result =>
            {
                Debug.Log("Successfully updated user data for key: " + key + " and value: " + value);
            },
            error =>
            {
                Debug.Log("Got error setting user data");
                Debug.Log(error.GenerateErrorReport());
            });
    }

    public void GetUserData(string key, Action<GetUserDataResultHolder> OnFinished = null)
    {
        GetUserDataResultHolder finalResultHolder = new GetUserDataResultHolder();
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = currentPlayerPlayfabID,
            Keys = null
        }, result =>
        {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey(key))
            {
                Debug.Log("No key for " + key);
                finalResultHolder.response = Response.noKey;
            }
            else
            {
                //Debug.Log("key: " + result.Data[key].Value);
                finalResultHolder.response = Response.result;
                finalResultHolder.value = result.Data[key].Value;
            }
            OnFinished?.Invoke(finalResultHolder);

        }, (error) =>
        {
            Debug.Log("Got error retrieving user data: ");
            Debug.Log(error.GenerateErrorReport());
            finalResultHolder.response = Response.Error;
            OnFinished?.Invoke(finalResultHolder);
        });

    }

    public void GetUserData(string key, string playerPlayFabID, Action<GetUserDataResultHolder> OnFinished = null)
    {
        GetUserDataResultHolder finalResultHolder = new GetUserDataResultHolder();
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = playerPlayFabID,
            Keys = null
        }, result =>
        {
            Debug.Log("Got user " + playerPlayFabID + " data:");
            if (result.Data == null || !result.Data.ContainsKey(key))
            {
                Debug.Log("No key for " + key + " for user " + playerPlayFabID);
                finalResultHolder.response = Response.noKey;
            }
            else
            {
                //Debug.Log("key: " + result.Data[key].Value);
                finalResultHolder.response = Response.result;
                finalResultHolder.value = result.Data[key].Value;
            }
            OnFinished?.Invoke(finalResultHolder);

        }, (error) =>
        {
            Debug.Log("Got error retrieving " + playerPlayFabID + " user data: ");
            Debug.Log(error.GenerateErrorReport());
            finalResultHolder.response = Response.Error;
            OnFinished?.Invoke(finalResultHolder);
        });

    }

    #region ReadOnlyPlayerData

    public void UpdateUserReadOnlyData(string playFabId, string key, string value, Action OnFinished = null)
    {
        PlayFabServerAPI.UpdateUserReadOnlyData(new PlayFab.ServerModels.UpdateUserDataRequest()
        {
            PlayFabId = playFabId,
            Data = new Dictionary<string, string>() {
                    {key, value}
                },
            Permission = PlayFab.ServerModels.UserDataPermission.Public
        },
            result =>
            {
                Debug.Log("Set read-only user data successful");
                OnFinished?.Invoke();
            },
            error =>
            {
                Debug.Log("Got error updating read-only user data:");
                Debug.Log(error.GenerateErrorReport());
                OnFinished?.Invoke();
            });
    }

    public void GetUserReadOnlyData(string playFabId, string key, Action<GetUserDataResultHolder> onFinished = null)
    {
        GetUserDataResultHolder finalResultHolder = new GetUserDataResultHolder();

        PlayFabServerAPI.GetUserReadOnlyData(new PlayFab.ServerModels.GetUserDataRequest()
        {
            PlayFabId = playFabId,
            Keys = new List<string>() { key },
        },
            result =>
            {
                if (result.Data == null || !result.Data.ContainsKey(key))
                {
                    Debug.Log("No " + key);
                    finalResultHolder.response = Response.noKey;
                }
                else
                {
                    Debug.Log(key + ": " + result.Data[key].Value);
                    finalResultHolder.response = Response.result;
                    finalResultHolder.value = result.Data[key].Value;
                }

                onFinished?.Invoke(finalResultHolder);
            },
            error =>
            {
                Debug.Log("Got error getting read-only user data:");
                Debug.Log(error.GenerateErrorReport());
                finalResultHolder.response = Response.Error;
                onFinished?.Invoke(finalResultHolder);
            });
    }
    #endregion


    #region Leaderboard
    //private static async Task DoReadLeaderboard()
    //{
    //    // Get Leaderboard Request
    //    var result = await PlayFabClientAPI.GetLeaderboardAsync(new GetLeaderboardRequest()
    //    {
    //        // Specify your statistic name here
    //        StatisticName = "TestScore",
    //        // Override Player Profile View Constraints and fetch player DisplayName and AvatarUrl
    //        ProfileConstraints = new PlayerProfileViewConstraints()
    //        {
    //            ShowDisplayName = true,
    //            ShowAvatarUrl = true
    //        }
    //    });

    //    // Start printing the leaderboard
    //    Console.WriteLine("=== LEADERBOARD ===");

    //    if (result.Error != null)
    //    {
    //        // Handle error if any
    //        //Console.WriteLine(result.Error.GenerateErrorReport());
    //    }
    //    else
    //    {
    //        // Traverse the leaderboard list
    //        foreach (var entry in result.Result.Leaderboard)
    //        {
    //            // Print regular leaderboard entry information
    //            Console.WriteLine($"{entry.Position + 1} {entry.PlayFabId} {entry.StatValue}");

    //            // Additionally print display name and avatar url that comes from player profile
    //            Console.WriteLine($"    {entry.Profile.DisplayName} | {entry.Profile.AvatarUrl}");
    //        }
    //    }
    //}

    public void GetLeaderboardDate(string statisticName, Action<List<PlayerLeaderboardEntry>> OnFinished = null)
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest()
        {
            MaxResultsCount = 7,
            StartPosition = 0,
            StatisticName = statisticName
        }, result =>
        {
            Debug.Log("Getting leaderboard");

            OnFinished?.Invoke(result.Leaderboard);

        }, (error) =>
        {
            Debug.Log("Got error retrieving leaderboard: ");
            Debug.Log(error.GenerateErrorReport());
            OnFinished?.Invoke(null);
        });
    }

    #endregion

    #region UpdateDisplayName

    public void UpdateDisplayName(string displayName)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = displayName
        },
            result =>
        {
            Debug.Log("Updated display name");
        },
        (error) =>
        {
            Debug.Log("Got error updating display name: ");
            Debug.Log(error.GenerateErrorReport());

        });
    }

    #endregion
}

public struct GetUserDataResultHolder
{
    public string value;
    public Response response;
}

public enum Response
{
    result,
    noKey,
    Error
}
