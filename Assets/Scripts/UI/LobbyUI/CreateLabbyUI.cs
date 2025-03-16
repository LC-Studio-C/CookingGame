using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLabbyUI : BaseUI
{
    [SerializeField] private Button publicLobbyButton;
    [SerializeField] private Button privateLobbyButton;
    [SerializeField] private TMP_InputField lobbyNameInput;

    private void Awake()
    {
        publicLobbyButton.onClick.AddListener(() =>
        {
            CookingLobbyManager.GetInstance().CreateLobby(lobbyNameInput.text, false);
        });
        privateLobbyButton.onClick.AddListener(() =>
        {
            CookingLobbyManager.GetInstance().CreateLobby(lobbyNameInput.text, true);
        });
        publicLobbyButton.Select();
    }

    private void Start()
    {
        Hide();
    }
}
