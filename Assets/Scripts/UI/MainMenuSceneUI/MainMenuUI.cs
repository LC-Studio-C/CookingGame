using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button singleButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        singleButton.onClick.AddListener(() =>
        {
            CookingGameMultiplayer.isMultiplayer = false;
            Loader.LoadScene(Loader.Scene.LobbyScene);
        });
        multiplayerButton.onClick.AddListener(() =>
        {
            CookingGameMultiplayer.isMultiplayer = true;
            Loader.LoadScene(Loader.Scene.LobbyScene);
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        singleButton.Select();
    }

}
