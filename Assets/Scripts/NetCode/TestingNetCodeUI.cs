using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetCodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() =>
        {
            CookingGameMultiplayer.GetInstance().StartHost();
            Hide();
        });
        startClientButton.onClick.AddListener(() =>
        {
            CookingGameMultiplayer.GetInstance().StartClient();
            Hide();
        });
        startHostButton.Select();
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
