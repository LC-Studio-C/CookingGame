using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{

    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyTextObject;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshProUGUI playerNameText;

    private PlayerVisual playerVisual;

    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = CookingGameMultiplayer.GetInstance().GetPlayerDataByPlayerIndex(playerIndex);
            CookingGameMultiplayer.GetInstance().KickPlayer(playerData.clientId);
            CookingLobbyManager.GetInstance().KickPlayer(playerData.playerId.ToString());
        });
    }

    private void Start()
    {
        CookingGameMultiplayer.GetInstance().OnPlayerDataListChanged += CookingGameMultiplayer_OnPlayerDataListChanged;
        CharacterSelectReady.GetInstance().OnPlayerReadyChanged += CharacterSelectReady_OnPlayerReadyChanged;
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        if (playerIndex == 0f)
        {
            kickButton.gameObject.SetActive(false);
        }
        playerVisual = GetComponent<PlayerVisual>();
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnPlayerReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void CookingGameMultiplayer_OnPlayerDataListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (CookingGameMultiplayer.GetInstance().IsPlayerShow(playerIndex))
        {
            Show();
            PlayerData playerData = CookingGameMultiplayer.GetInstance().GetPlayerDataByPlayerIndex(playerIndex);
            playerNameText.text = playerData.playerName.ToString();
            readyTextObject.SetActive(CharacterSelectReady.GetInstance().IsPlayerReady(playerData.clientId));
            playerVisual.SetPlayerColor(CookingGameMultiplayer.GetInstance().GetPlayerColor(playerIndex));
        }
        else
        {
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

    private void OnDestroy()
    {
        CookingGameMultiplayer.GetInstance().OnPlayerDataListChanged -= CookingGameMultiplayer_OnPlayerDataListChanged;
    }
}
