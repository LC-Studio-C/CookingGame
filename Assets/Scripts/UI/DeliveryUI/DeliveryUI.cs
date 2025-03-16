using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUI : MonoBehaviour
{
    [SerializeField] private DeliveryCounter deliveryCounter;

    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;
    [SerializeField] private TextMeshProUGUI diliveryText;
    [SerializeField] private Color successBackgroundColor;
    [SerializeField] private Color failBackgroundColor;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image deliveryIcon;

    private const string SUCCESS_TEXT = "DELIVERY\nSUCCESS";
    private const string Fail_TEXT = "DELIVERY\nFAIL";

    private float showTimer = 1.5f;
    private float showTime = 1.5f;

    private void Start()
    {
        DeliveryManager.GetInstance().OnDeliverySucceed += DeliveryManager_OnDeliverySucceed;
        DeliveryManager.GetInstance().OnDeliveryFailed += DeliveryManager_OnDeliveryFailed;
        Hide();
    }

    private void DeliveryManager_OnDeliveryFailed(object sender, System.EventArgs e)
    {
        Show();
        diliveryText.text = Fail_TEXT;
        backgroundImage.color = failBackgroundColor;
        deliveryIcon.sprite = failSprite;
    }

    private void DeliveryManager_OnDeliverySucceed(object sender, System.EventArgs e)
    {
        Show();
        diliveryText.text = SUCCESS_TEXT;
        backgroundImage.color = successBackgroundColor;
        deliveryIcon.sprite = successSprite;
    }

    private void Update()
    {
        showTimer -= Time.deltaTime;
        if (showTimer < 0f)
        {
            showTimer = showTime;
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
