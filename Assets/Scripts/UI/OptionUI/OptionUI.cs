using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : BaseUI
{
    private static OptionUI instance;

    public event EventHandler OnRebindKeyButtonPressed;

    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicEffectButton;
    [SerializeField] private Button rebindKeyButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicEffectText;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }

        soundEffectButton.onClick.AddListener(() =>
        {
            SoundManager.GetInstance().SoundVolumeChange();
            UpdateVisual();
        });
        musicEffectButton.onClick.AddListener(() =>
        {
            MusicManager.GetInstance().MusicVolumeChange();
            UpdateVisual();
        });
        rebindKeyButton.onClick.AddListener(() =>
        {
            OnRebindKeyButtonPressed?.Invoke(this, EventArgs.Empty);
            Hide();
        });
        closeButton.onClick.AddListener(() =>
        {
            CookingGameManager.GetInstance().SyncResumeGameFromPuase();
        });
    }

    private void Start()
    {
        GamePuaseUI.GetInstance().OnOptionButtonPressed += GamePuaseUI_OnOptionButtonPressed;
        Hide();
    }

    private void GamePuaseUI_OnOptionButtonPressed(object sender, System.EventArgs e)
    {
        UIStack.uiStack.Push(this);
        Show();
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        soundEffectText.text = "Sound volume: " + ((int)(SoundManager.GetInstance().GetVolume() * 10f)).ToString("");
        musicEffectText.text = "Music volume: " + ((int)(MusicManager.GetInstance().GetVolume() * 10f)).ToString("");
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        soundEffectButton.Select();
    }

    public static OptionUI GetInstance()
    {
        return instance;
    }
}
