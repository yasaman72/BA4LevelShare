﻿using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFabLogin : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject RecoveryLoginPanel;
    [SerializeField] private GameObject RecoveryButton;
    [SerializeField] private bool autoLogin;

    private string userEmail;
    private string userPassword;
    private string username;

    public void LogOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerPrefs.DeleteAll();
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Start()
    {
        loginPanel.SetActive(false);
        RecoveryButton.SetActive(false);
        RecoveryLoginPanel.SetActive(false);
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "C14AC"; // Please change this value to your own titleId from PlayFab Game Manager
        }

        if (PlayerPrefs.HasKey("EMAIL"))
        {
            if (autoLogin)
            {
                userEmail = PlayerPrefs.GetString("EMAIL");
                userPassword = PlayerPrefs.GetString("PASSWORD");
                var request = new LoginWithEmailAddressRequest {Email = userEmail, Password = userPassword};
                PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
            }
        }
        else
        {

#if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID(), CreateAccount  = true};
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnMobileLoginSuccess, OnMobileLoginFailure);
            RecoveryButton.SetActive(true);
#elif UNITY_IOS
            var requestIos = new LoginWithIOSDeviceIDRequest { DeviceId = ReturnMobileID(), CreateAccount  = true};
            PlayFabClientAPI.LoginWithIOSDeviceID(requestIos, OnMobileLoginSuccess, OnMobileLoginFailure); 
            RecoveryButton.SetActive(true);
#else
            loginPanel.SetActive(true);
#endif
        }
    }

    public void SetUserEmail(string email)
    {
        userEmail = email;
    }
    public void SetUserPassword(string password)
    {
        userPassword = password;
    }
    public void SetUsername(string name)
    {
        username = name;
    }

    public void LoginRequest()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");

        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);

        loginPanel.SetActive(false);
        // storing current player playfab ID
        PlayFabController.instance.currentPlayerPlayfabID = result.PlayFabId;

        PlayFabController.instance.GetStatistics();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnMobileLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");

        loginPanel.SetActive(false);

        // storing current player playfab ID
        PlayFabController.instance.currentPlayerPlayfabID = result.PlayFabId;

        PlayFabController.instance.GetStatistics();
    }

    private void OnMobileLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with log in.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    public static string ReturnMobileID()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        return deviceId;
    }

    public void RegisterNewUser()
    {
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = username };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Registration successful!");

        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        // storing current player playfab ID
        PlayFabController.instance.currentPlayerPlayfabID = result.PlayFabId;

        UpdateUserDisplayName();

        loginPanel.SetActive(false);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with registration.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void OnClickAddRecoveryData()
    {
        var addRecoveryRequest = new AddUsernamePasswordRequest { Email = userEmail, Password = userPassword, Username = username };
        PlayFabClientAPI.AddUsernamePassword(addRecoveryRequest, OnRegisterSuccess, OnRegisterFailure);

        UpdateUserDisplayName();
    }

    private void OnRegisterSuccess(AddUsernamePasswordResult result)
    {
        Debug.Log("Registration successful!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);

        loginPanel.SetActive(false);
    }

    public void UpdateUserDisplayName()
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = username },
            onDisplayName => { Debug.Log("your display name is now: " + username); },
            OnLoginFailure);
    }

    public void UpdateUserDisplayName(string username)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = username },
            onDisplayName => { Debug.Log("your display name is now: " + username); },
            OnLoginFailure);
    }
}