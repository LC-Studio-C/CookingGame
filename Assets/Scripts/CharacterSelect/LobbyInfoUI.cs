using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake()
    {
        Lobby lobby = CookingLobbyManager.GetInstance().GetLobby();

        lobbyNameText.text = "Lobby Name : " + lobby.Name;
        lobbyCodeText.text = "Lobby Code : " + lobby.LobbyCode;
    }
}
