using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class GamePuaseUI : BaseUI
{
    private static GamePuaseUI instance;

    public event EventHandler OnOptionButtonPressed;

    public event EventHandler OnHelperButtonPressed;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button helperButton;
    [SerializeField] private Transform buttons;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        CookingGameManager.GetInstance().OnGamePuased += CookingGameManager_OnGamePuased;
        //CookingGameManager.GetInstance().OnGameResumed += CookingGameManager_OnGameResumed;
        resumeButton.onClick.AddListener(() =>
        {
            CookingGameManager.GetInstance().SyncResumeGameFromPuase();
        });
        helperButton.onClick.AddListener(() =>
        {
            OnHelperButtonPressed?.Invoke(this, EventArgs.Empty);
            Hide();
        });
        optionButton.onClick.AddListener(() =>
        {
            OnOptionButtonPressed?.Invoke(this, EventArgs.Empty);
            Hide();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            AuthenticationService.Instance.SignOut();
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });
        Hide();
    }

    private void CookingGameManager_OnGamePuased(object sender, System.EventArgs e)
    {
        UIStack.uiStack.Push(this);
        Show();
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        resumeButton.Select();
    }

    public static GamePuaseUI GetInstance()
    {
        return instance;
    }
}
