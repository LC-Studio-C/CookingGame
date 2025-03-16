using System;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    /// <summary>
    /// TrashCounterThrowAni����
    /// </summary>
    public event EventHandler OnThrowing;
    public new static event EventHandler OnGetedSomething;

    new public static void ResetStaticData()
    {
        OnGetedSomething = null;
    }

    public override void Interact(PlayerControl player)
    {
        if (IsHasKitchenObject() == false)
        {
            if (player.IsHasKitchenObject() == true)
            {
                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetParent(this);
                InteractLogicServerRpc();
                DestroyKitchenObjectServerRpc(kitchenObject.GetNetworkObject());
            }
            else
            {
                //ʲô������
            }
        }
        else
        {
            if (player.IsHasKitchenObject() == true)
            {
                //ʲô������
            }
            else
            {
                //ʲô������
            }
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
        OnThrowing?.Invoke(this, EventArgs.Empty);
        OnGetedSomething?.Invoke(this, EventArgs.Empty);
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
        kitchenObject.DestroySelfDelay(.3f);
    }

    [ClientRpc]
    private void DestroyKitchenObjectClientRpc(NetworkObjectReference kitchenObjectNetworkReference)
    {
        kitchenObjectNetworkReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        kitchenObjectNetworkObject.GetComponent<KitchenObject>().ClearKitchenObjectParentOnKitchenObject();
    }
}
