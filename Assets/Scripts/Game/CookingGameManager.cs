using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

public class CookingGameManager : NetworkBehaviour
{
    private static CookingGameManager instance;

    public enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        Gameover
    }

    /// <summary>
    /// CountdownToStartUI/GameoverUI/GamePlayingUI/WaitingToStartUI¶©ÔÄ
    /// </summary>
    public event EventHandler OnGameStateChanged;

    /// <summary>
    /// HelperUI/WaitingToStartUI¶©ÔÄ
    /// </summary>
    public event EventHandler OnPlayerReadyChanged;

    /// <summary>
    /// GamePuaseUI¶©ÔÄ
    /// </summary>
    public event EventHandler OnGamePuased;

    private NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.WaitingToStart);

    /// <summary>
    /// µ¹¼ÆÊ±¼ÆÊ±Æ÷
    /// </summary>
    [SerializeField] private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);

    /// <summary>
    /// ÓÎÏ·Ê±¼ä¼ÆÊ±Æ÷
    /// </summary>
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    [SerializeField] private float gamePlayingMaxTime = 0f;

    private NetworkVariable<bool> isGamePuased = new(false);
    private Dictionary<ulong, bool> playerResumeDictionary;
    public event EventHandler OnPlayerGameResumed;//WaitResumeUI¶©ÔÄ
    private bool isLocalPlayerResume = false;
    private bool autoSingleResume = false;

    private Dictionary<ulong, bool> playerReadyDictionary;
    private bool isLocalPlayerReady = false;

    [SerializeField] private Transform playerPrefab;

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
        playerResumeDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        GameInput.GetInstance().OnPuaseAction += CookingGameManager_OnPuaseAction;
        GameInput.GetInstance().OnInteractAction += GameInput_OnInteractAction;
    }

    private void Update()
    {
        if (IsServer == false)
        {
            return;
        }

        switch (gameState.Value)
        {
            case GameState.WaitingToStart:
                break;
            case GameState.CountdownToStart:
                CountdownToStart();
                break;
            case GameState.GamePlaying:
                GamePlaying();
                break;
            case GameState.Gameover:
                break;
        }
    }

    private void LateUpdate()
    {
        if (autoSingleResume == true)
        {
            autoSingleResume = false;
            ResumeGameServerRpc();
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (gameState.Value == GameState.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            PlayerReadyServerRpc();
        }
    }

    public override void OnNetworkSpawn()
    {
        gameState.OnValueChanged += GameState_OnValueChanged;
        if (IsServer)
        {
            NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(client);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong ClientId)
    {
        if (isGamePuased.Value == true)
        {
            autoSingleResume = true;
        }
    }

    private void GameState_OnValueChanged(GameState previousValue, GameState newValue)
    {
        OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
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
            gameState.Value = GameState.CountdownToStart;
        }
    }

    private void CookingGameManager_OnPuaseAction(object sender, EventArgs e)
    {
        if (isGamePuased.Value == false)
        {
            PuaseGameServerRpc();
        }
        else
        {
            if (isLocalPlayerResume == true)
            {
                return;
            }
            UIStack.uiStack.Pop().Hide();
            if (UIStack.uiStack.Count > 0)
            {
                UIStack.uiStack.Peek().Show();
            }
            if (UIStack.uiStack.Count == 0)
            {
                isLocalPlayerResume = true;
                OnPlayerGameResumed?.Invoke(this, EventArgs.Empty);
                ResumeGameServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PuaseGameServerRpc()
    {
        isGamePuased.Value = true;
        PuaseGameClientRpc();
    }

    [ClientRpc]
    private void PuaseGameClientRpc()
    {
        OnGamePuased?.Invoke(this, EventArgs.Empty);
        Time.timeScale = 0f;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResumeGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerResumeDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool isAllPlayerResume = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerResumeDictionary.ContainsKey(clientId) == false || playerResumeDictionary[clientId] == false)
            {
                isAllPlayerResume = false;
            }
        }

        if(isAllPlayerResume == true)
        {
            isGamePuased.Value = false;
            playerResumeDictionary.Clear();
            ResumeGameClientRpc();
        }
    }

    [ClientRpc]
    private void ResumeGameClientRpc()
    {
        Time.timeScale = 1f;
        isLocalPlayerResume = false;
        foreach (BaseUI baseUI in UIStack.uiStack)
        {
            baseUI.Hide();
        }
        UIStack.uiStack.Clear();
    }

    /// <summary>
    /// ÔÝÍ£
    /// </summary>
    public void SyncResumeGameFromPuase()
    {
        if(isGamePuased.Value == true)
        {
            UIStack.uiStack.Pop().Hide();
            if (UIStack.uiStack.Count > 0)
            {
                UIStack.uiStack.Peek().Show();
            }
            if (UIStack.uiStack.Count == 0)
            {
                isLocalPlayerResume = true;
                OnPlayerGameResumed?.Invoke(this, EventArgs.Empty);
                ResumeGameServerRpc();
            }
        }
    }

    private void CountdownToStart()
    {
        countdownToStartTimer.Value -= Time.deltaTime;
        if (countdownToStartTimer.Value < 0)
        {
            gamePlayingTimer.Value = gamePlayingMaxTime;
            gameState.Value = GameState.GamePlaying;
        }
    }

    private void GamePlaying()
    {
        gamePlayingTimer.Value -= Time.deltaTime;
        if (gamePlayingTimer.Value < 0)
        {
            gameState.Value = GameState.Gameover;
        }
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public float GetGamePlayingTimerNormalization()
    {
        return gamePlayingTimer.Value / gamePlayingMaxTime;
    }

    public float GetGamePlayingTimer()
    {
        return gamePlayingTimer.Value;
    }

    public bool IsWaitingToStart()
    {
        return gameState.Value == GameState.WaitingToStart;
    }

    public bool IsCountdownToStart()
    {
        return gameState.Value == GameState.CountdownToStart;
    }

    public bool IsGameover()
    {
        return gameState.Value == GameState.Gameover;
    }

    public bool IsGamePlaying()
    {
        return gameState.Value == GameState.GamePlaying;
    }

    public bool IsGamePuased()
    {
        return isGamePuased.Value;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public bool IsLocalPlayerResume()
    {
        return isLocalPlayerResume;
    }

    public static CookingGameManager GetInstance()
    {
        return instance;
    }
}
