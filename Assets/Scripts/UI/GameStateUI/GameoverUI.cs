using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class GameoverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button playAgainButton;

    private void Awake()
    {
        playAgainButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            AuthenticationService.Instance.SignOut();
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });
        playAgainButton.Select();
    }

    private void Start()
    {
        CookingGameManager.GetInstance().OnGameStateChanged += CookingGameManager_OnGameStateChanged;
        Hide();
    }

    private void CookingGameManager_OnGameStateChanged(object sender, System.EventArgs e)
    {
        if (CookingGameManager.GetInstance().IsGameover() == true)
        {
            Show();
            countText.text = DeliveryManager.GetInstance().GetDeliverySeccessAmount().ToString();
        }
        else
        {
            Hide();
        }
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
