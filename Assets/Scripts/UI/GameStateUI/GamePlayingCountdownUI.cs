using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingCountdownUI : MonoBehaviour
{

    [SerializeField] private Image countdownImage;
    [SerializeField] private TextMeshProUGUI countdownText;


    private void Start()
    {
        CookingGameManager.GetInstance().OnGameStateChanged += CookingGameManager_OnGameStateChanged;
        Hide();
    }

    private void CookingGameManager_OnGameStateChanged(object sender, System.EventArgs e)
    {
        if (CookingGameManager.GetInstance().IsGamePlaying())
        {
            Show();
            
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        float gameCountdown = CookingGameManager.GetInstance().GetGamePlayingTimer();
        countdownImage.fillAmount = CookingGameManager.GetInstance().GetGamePlayingTimerNormalization();
        int minute = (int)gameCountdown / 60;
        int second = (int)gameCountdown % 60;
        countdownText.text = minute.ToString() + ":" + second.ToString("D2");
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
