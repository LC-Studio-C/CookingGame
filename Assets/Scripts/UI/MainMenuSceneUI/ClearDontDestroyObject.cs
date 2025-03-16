using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClearDontDestroyObject : MonoBehaviour
{
    private void Awake()
    {
        if (CookingGameMultiplayer.GetInstance() != null)
        {
            Destroy(CookingGameMultiplayer.GetInstance().gameObject);
        }
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }
}
