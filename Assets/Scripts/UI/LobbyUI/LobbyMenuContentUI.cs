using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenuContentUI : MonoBehaviour
{
    [SerializeField] private Transform signleLobbyButton;
    [SerializeField] private TMP_InputField playerNameInputField;

    private List<Lobby> listLobbies;

    private void Start()
    {
        CookingLobbyManager.GetInstance().OnQueriedLobbies += CookingLobbyManager_OnQueriedLobbies;
    }

    private void OnDestroy()
    {
        CookingLobbyManager.GetInstance().OnQueriedLobbies -= CookingLobbyManager_OnQueriedLobbies;
    }

    private void CookingLobbyManager_OnQueriedLobbies(object sender, CookingLobbyManager.OnQueriedLobbiesEventArgs e)
    {
        listLobbies = e.listLobbies;
        UpdateLobbyMenu();
    }

    private void UpdateLobbyMenu()
    {
        foreach (Transform child in transform)
        {
            if (child == signleLobbyButton)
            {
                continue;
            }
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in listLobbies)
        {
            Transform singleLobby = Instantiate(signleLobbyButton,transform);
            singleLobby.gameObject.SetActive(true);
            singleLobby.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
            int playerCount = lobby.MaxPlayers - lobby.AvailableSlots;
            singleLobby.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerCount + "/" + lobby.MaxPlayers;
            singleLobby.GetComponent<Button>().onClick.AddListener(() =>
            {
                CookingLobbyManager.GetInstance().JoinLobbyById(lobby.Id);
                /*if (playerNameInputField.text != "")
                {
                    CookingGameMultiplayer.GetInstance().SetPlayerName(playerNameInputField.text);
                }*/
            });
        }
    }
}
