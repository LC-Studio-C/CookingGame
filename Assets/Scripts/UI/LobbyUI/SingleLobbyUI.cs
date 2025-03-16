using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleLobbyUI : MonoBehaviour
{
    private Button joinButton;

    private void Awake()
    {
        joinButton = GetComponent<Button>();
        joinButton.onClick.AddListener(() =>
        {
        });
    }
}
