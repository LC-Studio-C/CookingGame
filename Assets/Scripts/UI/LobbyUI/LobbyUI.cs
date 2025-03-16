using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : BaseUI
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinLobbyButton;
    [SerializeField] private Button joinLobbyByCodeButton;

    [SerializeField] private CreateLabbyUI createLobbyUI;
    [SerializeField] private JoinLobbyWindowUI joinLobbyWindowUI;

    [SerializeField] private TMP_InputField lobbyCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            AuthenticationService.Instance.SignOut();
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });
        createLobbyButton.onClick.AddListener(() =>
        {
            createLobbyUI.Show();
            if (playerNameInputField.text != "")
            {
                CookingGameMultiplayer.GetInstance().SetPlayerName(playerNameInputField.text);
            }
            Hide();
        });
        quickJoinLobbyButton.onClick.AddListener(() =>
        {
            CookingLobbyManager.GetInstance().QuickJoinLobby();
            if (playerNameInputField.text != "")
            {
                CookingGameMultiplayer.GetInstance().SetPlayerName(playerNameInputField.text);
            }
        });
        joinLobbyByCodeButton.onClick.AddListener(() =>
        {
            if (lobbyCodeInputField.text != "")
            {
                CookingLobbyManager.GetInstance().JoinLobbyByCode(lobbyCodeInputField.text);
            }
            if (playerNameInputField.text != "")
            {
                CookingGameMultiplayer.GetInstance().SetPlayerName(playerNameInputField.text);
            }
        });
        quickJoinLobbyButton.Select();
    }
}
