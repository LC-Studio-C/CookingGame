using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : BaseUI
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
        closeButton.Select();
    }

    private void Start()
    {
        CookingGameMultiplayer.GetInstance().OnTryingToJoinGame += CookingGameMultiplayer_OnTryJoinGamed;
        CookingGameMultiplayer.GetInstance().OnJoinGameFailed += CookingGameMultiplayer_OnFailJoinGamed;
        CookingLobbyManager.GetInstance().OnCreateingLobby += CookingLobbyManager_OnCreateingLobby;
        CookingLobbyManager.GetInstance().OnCreateFailedLobby += CookingLobbyManager_OnCreateFailedLobby;
        CookingLobbyManager.GetInstance().OnJoiningLobby += CookingLobbyManager_OnJoiningLobby;
        CookingLobbyManager.GetInstance().OnJoinFailedLobby += CookingLobbyManager_OnJoinFailedLobby;
        AuthenticationService.Instance.SignedIn += AuthenticationService_SignedIn;
        AuthenticationService.Instance.SignInFailed += AuthenticationService_SignInFailed;
        ShowMessage("SignIn...");
        closeButton.gameObject.SetActive(false);
    }


    private void OnDestroy()
    {
        CookingGameMultiplayer.GetInstance().OnTryingToJoinGame -= CookingGameMultiplayer_OnTryJoinGamed;
        CookingGameMultiplayer.GetInstance().OnJoinGameFailed -= CookingGameMultiplayer_OnFailJoinGamed;
        CookingLobbyManager.GetInstance().OnCreateingLobby -= CookingLobbyManager_OnCreateingLobby;
        CookingLobbyManager.GetInstance().OnCreateFailedLobby -= CookingLobbyManager_OnCreateFailedLobby;
        CookingLobbyManager.GetInstance().OnJoiningLobby -= CookingLobbyManager_OnJoiningLobby;
        CookingLobbyManager.GetInstance().OnJoinFailedLobby -= CookingLobbyManager_OnJoinFailedLobby;
        AuthenticationService.Instance.SignedIn -= AuthenticationService_SignedIn;
        AuthenticationService.Instance.SignInFailed -= AuthenticationService_SignInFailed;
    }

    private void CookingGameMultiplayer_OnTryJoinGamed(object sender, System.EventArgs e)
    {
        Show();
        ShowMessage("Connecting...");
        closeButton.gameObject.SetActive(false);
    }

    private void CookingGameMultiplayer_OnFailJoinGamed(object sender, System.EventArgs e)
    {
        Show();
        ShowMessage(NetworkManager.Singleton.DisconnectReason);
        closeButton.gameObject.SetActive(true);
    }

    private void CookingLobbyManager_OnCreateingLobby(object sender, System.EventArgs e)
    {
        Show();
        ShowMessage("Lobby Createing...");
        closeButton.gameObject.SetActive(false);
    }

    private void CookingLobbyManager_OnCreateFailedLobby(object sender, System.EventArgs e)
    {
        Show();
        ShowMessage("Lobby Create Failed!");
        closeButton.gameObject.SetActive(true);
    }

    private void CookingLobbyManager_OnJoiningLobby(object sender, System.EventArgs e)
    {
        Show();
        ShowMessage("Lobby Joining...");
        closeButton.gameObject.SetActive(false);
    }

    private void CookingLobbyManager_OnJoinFailedLobby(object sender, System.EventArgs e)
    {
        Show();
        ShowMessage("Lobby Join Failed!");
        closeButton.gameObject.SetActive(true);
    }
    private void AuthenticationService_SignedIn()
    {
        Hide();
    }

    private void AuthenticationService_SignInFailed(Unity.Services.Core.RequestFailedException obj)
    {
        ShowMessage("SignInFailed!");
        closeButton.gameObject.SetActive(true);
    }

    private void ShowMessage(string msg)
    {
        messageText.text = msg;
    }
    
}
