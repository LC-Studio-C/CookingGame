using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO plateKitchenObjSO;
    private float productPlateTimer;
    private float productPlateTimeMax = 4f;
    private float plateCount;
    private float plateCountMax = 3f;
    
    /// <summary>
    /// PlatesCounterVisual¶©ÔÄ
    /// </summary>
    public event EventHandler OnPlateProduced;
    /// <summary>
    /// PlatesCounterVisual¶©ÔÄ
    /// </summary>
    public event EventHandler OnPlateRemoved;

    private void Update()
    {
        if (IsServer == false)
        {
            return;
        }

        if (CookingGameManager.GetInstance().IsGamePlaying())
        {
            productPlateTimer += Time.deltaTime;
            if (productPlateTimer > productPlateTimeMax)
            {
                productPlateTimer = 0;
                if (plateCount < plateCountMax)
                {
                    ProductPlateServerRpc();
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ProductPlateServerRpc()
    {
        ProductPlateClientRpc();
    }

    [ClientRpc]
    private void ProductPlateClientRpc()
    {
        plateCount++;
        OnPlateProduced?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(PlayerControl player)
    {
        if (plateCount > 0)
        {
            if (player.IsHasKitchenObject() == false)
            {
                productPlateTimer = 0;
                KitchenObject.ProductKitchenObject(plateKitchenObjSO, player);
                InteractLogicServerRpc();
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
        plateCount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
