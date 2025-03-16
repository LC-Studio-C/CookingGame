using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjSO;

    /// <summary>
    /// SelectedCounterVisual����
    /// </summary>
    public event EventHandler OnPlayerClose;

    public override void Interact(PlayerControl player)
    {
        if (player.IsHasKitchenObject() == false)
        {
            KitchenObject.ProductKitchenObject(kitchenObjSO, player);
            InteractLogicServerRpc();
        }
        else
        {
            //ʲô������
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnPlayerClose?.Invoke(this, EventArgs.Empty);
    }
}
