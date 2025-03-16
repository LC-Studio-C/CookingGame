using TMPro;
using UnityEngine;

public class CountdownToStartUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownToStartText;

    private float soundPlayTimer = 0f;

    private void Start()
    {
        CookingGameManager.GetInstance().OnGameStateChanged += CookingGameManager_OnGameStateChanged;
        Hide();
    }

    private void CookingGameManager_OnGameStateChanged(object sender, System.EventArgs e)
    {
        if (CookingGameManager.GetInstance().IsCountdownToStart() == true)
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
        countdownToStartText.text = Mathf.Ceil(CookingGameManager.GetInstance().GetCountdownToStartTimer()).ToString();
        soundPlayTimer -= Time.deltaTime;
        if (soundPlayTimer < 0f)
        {
            SoundManager.GetInstance().PlayCountdownSound(Camera.main.transform.position);
            soundPlayTimer = 1.01f;
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
