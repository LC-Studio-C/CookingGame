using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CookingGameMultiplayer : NetworkBehaviour
{
    private static CookingGameMultiplayer instance;

    public const int MAX_PLAYER_AMOUNT = 4;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnJoinGameFailed;

    [SerializeField] private KitchenObjectSOList kitchenObjectSOList;

    private NetworkList<PlayerData> playerDataList;
    public event EventHandler OnPlayerDataListChanged;//CharPlayer,CharSigle订阅

    [SerializeField] private List<Color> playerColorList;

    private const string PREFER_PLAYER_NAME = "PreferPlayerName";
    private const string PLAYER_NAME = "PlayerName";
    private string playerName;

    public static bool isMultiplayer = true;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
        playerDataList = new NetworkList<PlayerData>();
        playerDataList.OnListChanged += PlayerDataList_OnListChanged;

        playerName = PlayerPrefs.GetString(PREFER_PLAYER_NAME, PLAYER_NAME + UnityEngine.Random.Range(100, 1000));
    }

    private void Start()
    {
        if (isMultiplayer == false)
        {
            StartHost();
            Loader.LoadSceneNetwork(Loader.Scene.GameScene);
        }
    }

    private void PlayerDataList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        playerDataList.Remove(GetPlayerDataByClientId(clientId));
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataList.Add(new PlayerData()
        {
            clientId = clientId,
            colorId = SetPlayerDefaultColor()
        });
        SetPlayerIdServerRpc();
        SetPlayerNameServerRpc(GetPlayerName());
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            response.Approved = false;
            response.Reason = "Cannot join while the game is in progress!!!";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            response.Approved = false;
            response.Reason = "Room Fulled!!!";
            return;
        }
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {

        int playerIndex = GetPlayerIndexByClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataList[playerIndex];

        playerData.playerName = playerName;

        playerDataList[playerIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(ServerRpcParams serverRpcParams = default)
    {

        int playerIndex = GetPlayerIndexByClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataList[playerIndex];

        playerData.playerId = AuthenticationService.Instance.PlayerId;

        playerDataList[playerIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong obj)
    {
        OnJoinGameFailed?.Invoke(this, EventArgs.Empty);
    }

    public void ProductKitchenObjectForNetwork(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        ProductKitchenObjectServerRpc(GetKichenObjectIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void ProductKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        if (kitchenObjectParent.IsHasKitchenObject())
        {
            return;
        }

        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOByIndex(kitchenObjectSOIndex);
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.Prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        kitchenObjectTransform.GetComponent<KitchenObject>().SetParent(kitchenObjectParent);
    }


    public void DestroyKitchenObjectForNetwork(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkReference)
    {
        kitchenObjectNetworkReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        if (kitchenObjectNetworkObject == null)
        {
            return;
        }

        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        DestroyKitchenObjectClientRpc(kitchenObjectNetworkReference);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void DestroyKitchenObjectClientRpc(NetworkObjectReference kitchenObjectNetworkReference)
    {
        kitchenObjectNetworkReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        kitchenObjectNetworkObject.GetComponent<KitchenObject>().ClearKitchenObjectParentOnKitchenObject();
    }

    private int GetKichenObjectIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectSOList.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    private KitchenObjectSO GetKitchenObjectSOByIndex(int kitchenObjectSOIndex)
    {
        return kitchenObjectSOList.kitchenObjectSOList[kitchenObjectSOIndex];
    }

    public bool IsPlayerShow(int playerIndex)
    {
        return playerIndex < playerDataList.Count;
    }

    public PlayerData GetPlayerDataByPlayerIndex(int playerIndex)
    {
        return playerDataList[playerIndex];
    }

    /// <summary>
    /// 通过CharacterSelectSingleUI的colorId获得playerColorList中的颜色
    /// </summary>
    /// <param name="colorId"></param>
    /// <returns></returns>
    public Color GetColor(int colorId)
    {
        return playerColorList[colorId];
    }

    /// <summary>
    /// 通过客户端ID获得客户端PlayerData
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public PlayerData GetPlayerDataByClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    /// <summary>
    /// 获得本地玩家的colorId
    /// </summary>
    /// <returns></returns>
    public int GetPlayerColorId()
    {
        return GetPlayerDataByClientId(NetworkManager.Singleton.LocalClientId).colorId;
    }

    public Color GetPlayerColor(int playerIndex)
    {
        return playerColorList[GetPlayerDataByPlayerIndex(playerIndex).colorId];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (IsPlayerColorAvailable(colorId) == false)
        {
            return;
        }

        int playerIndex = GetPlayerIndexByClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataList[playerIndex];

        playerData.colorId = colorId;

        playerDataList[playerIndex] = playerData;
    }

    /// <summary>
    /// 通过客户端Id获得玩家索引
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public int GetPlayerIndexByClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataList.Count; i++)
        {
            if (playerDataList[i].clientId == clientId)
            {
                return i;
            }
        }
        return default;
    }

    private bool IsPlayerColorAvailable(int colorId)
    {
        foreach (PlayerData playerData in playerDataList)
        {
            if (playerData.colorId == colorId)
            {
                return false;
            }
        }
        return true;
    }

    private int SetPlayerDefaultColor()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsPlayerColorAvailable(i) == true)
            {
                return i;
            }
        }
        return -1;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;

        PlayerPrefs.SetString(PREFER_PLAYER_NAME, playerName);
    }

    public static CookingGameMultiplayer GetInstance()
    {
        return instance;
    }
}
