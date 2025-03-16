using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : NetworkBehaviour
{ 
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button readyButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
                CookingLobbyManager.GetInstance().LeaveLobby();
                AuthenticationService.Instance.SignOut();
            }
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });
        readyButton.onClick.AddListener(() =>
        {
            CharacterSelectReady.GetInstance().SetPlayerReady();
        });
        readyButton.Select();
    }
}
