using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTestingUI : NetworkBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private void Start()
    {
        createGameButton.onClick.AddListener(() =>
        {
            CookingGameMultiplayer.GetInstance().StartHost();
            Loader.LoadSceneNetwork(Loader.Scene.CharacterSelectScene);
        });
        joinGameButton.onClick.AddListener(() =>
        {
            CookingGameMultiplayer.GetInstance().StartClient();
        });
    }

}
