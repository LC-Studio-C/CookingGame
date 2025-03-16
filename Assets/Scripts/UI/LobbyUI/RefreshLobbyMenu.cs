using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshLobbyMenu : MonoBehaviour
{

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            CookingLobbyManager.GetInstance().ListLobbise();
        });
    }
}
