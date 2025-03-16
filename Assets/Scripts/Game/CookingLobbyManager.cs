using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class CookingLobbyManager : MonoBehaviour
{
    private static CookingLobbyManager instance;

    private const string KEY_RELAY_JOIN_CODE = "KeyRelayJoinCode";

    private Lobby joinedLobby;

    private float heartbeatTimer = 15f;
    private float heartbeatTimerMax = 15f;

    private float refreshListLobbiesTimer = 3f;
    private float refreshListLobbiesTimerMax = 3f;

    public event EventHandler OnCreateingLobby;
    public event EventHandler OnCreateFailedLobby;
    public event EventHandler OnJoiningLobby;
    public event EventHandler OnJoinFailedLobby;

    public event EventHandler<OnQueriedLobbiesEventArgs> OnQueriedLobbies;
    public class OnQueriedLobbiesEventArgs : EventArgs
    {
        public List<Lobby> listLobbies;
    }

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
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private void Update()
    {
        HandleHeartbeat();
        HandleRefreshLisyLobbies();
    }

    private void HandleHeartbeat()
    {
        if (IsHostLobby() == true)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                heartbeatTimer = heartbeatTimerMax;
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private void HandleRefreshLisyLobbies()
    {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn == true)
        {
            refreshListLobbiesTimer -= Time.deltaTime;
            if (refreshListLobbiesTimer < 0f)
            {
                refreshListLobbiesTimer = refreshListLobbiesTimerMax;
                ListLobbise();
            }
        }
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());
            await UnityServices.InitializeAsync(initializationOptions);
        }

        try
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log(AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e);
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        int maxConnections = CookingGameMultiplayer.MAX_PLAYER_AMOUNT;
        try
        {
            Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
            return allocation;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            String relayJoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string relayJoinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await Relay.Instance.JoinAllocationAsync(relayJoinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    public async void CreateLobby(string name, bool isPrivate)
    {
        int maxPlayer = CookingGameMultiplayer.MAX_PLAYER_AMOUNT;
        CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
        {
            IsPrivate = isPrivate
        };
        OnCreateingLobby?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayer, createLobbyOptions);

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>()
                {
                    {
                        KEY_RELAY_JOIN_CODE,
                        new DataObject
                        (
                            DataObject.VisibilityOptions.Member,
                            relayJoinCode
                        )
                    }
                }
            };
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, updateLobbyOptions);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            CookingGameMultiplayer.GetInstance().StartHost();
            Loader.LoadSceneNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateFailedLobby?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void QuickJoinLobby()
    {
        OnJoiningLobby?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            CookingGameMultiplayer.GetInstance().StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailedLobby?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinLobbyById(string lobbyId)
    {
        OnJoiningLobby?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            CookingGameMultiplayer.GetInstance().StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailedLobby?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        OnJoiningLobby?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            CookingGameMultiplayer.GetInstance().StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailedLobby?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void ListLobbise()
    {
        int queryResultCount = 5;
        QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
        {
            Count = queryResultCount,
            Filters = new System.Collections.Generic.List<QueryFilter>()
            {
                new QueryFilter
                (
                    QueryFilter.FieldOptions.AvailableSlots,
                    "0",
                    QueryFilter.OpOptions.GE
                )
            }
        };

        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            OnQueriedLobbies?.Invoke(this, new OnQueriedLobbiesEventArgs()
            {
                listLobbies = queryResponse.Results
            });
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (joinedLobby != null)
        {
            if (IsHostLobby() == true)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
                    joinedLobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }
    }

    private bool IsHostLobby()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    public static CookingLobbyManager GetInstance()
    {
        return instance;
    }
}
