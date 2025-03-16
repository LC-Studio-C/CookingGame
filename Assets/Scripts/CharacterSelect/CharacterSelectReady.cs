using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;

public class CharacterSelectReady : NetworkBehaviour
{
    private static CharacterSelectReady instance;

    private Dictionary<ulong, bool> playerReadyDictionary;

    public event EventHandler OnPlayerReadyChanged;

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
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        PlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SyncPlayerReadyDictionaryClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool isAllPlayerReady = true;
        foreach (ulong ClientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerReadyDictionary.ContainsKey(ClientId) == false || playerReadyDictionary[ClientId] == false)
            {
                isAllPlayerReady = false;
            }
        }

        if (isAllPlayerReady)
        {
            Loader.LoadSceneNetwork(Loader.Scene.GameScene);
            CookingLobbyManager.GetInstance().DeleteLobby();
        }
    }

    [ClientRpc]
    private void SyncPlayerReadyDictionaryClientRpc(ulong ClientId)
    {
        playerReadyDictionary[ClientId] = true;
        OnPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId] == true;
    }

    public static CharacterSelectReady GetInstance()
    {
        return instance;
    }


}
