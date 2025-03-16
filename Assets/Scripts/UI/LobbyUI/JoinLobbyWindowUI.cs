using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyWindowUI : BaseUI
{
    private void Start()
    {
        AuthenticationService.Instance.SignedIn += AuthenticationService_SignedIn;
    }

    private void OnDestroy()
    {
        AuthenticationService.Instance.SignedIn -= AuthenticationService_SignedIn;
    }

    private void AuthenticationService_SignedIn()
    {
        CookingLobbyManager.GetInstance().ListLobbise();
    }
}
