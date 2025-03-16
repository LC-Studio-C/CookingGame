using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{

    private void Start()
    {
        DeliveryManager.GetInstance().OnDeliverySucceed += DeliveryManager_OnDeliverySucceed;
        DeliveryManager.GetInstance().OnDeliveryFailed += DeliveryManager_OnDeliveryFailed;
    }

    private void DeliveryManager_OnDeliveryFailed(object sender, EventArgs e)
    {
        SoundManager.GetInstance().PlayDeliveryFailSound(transform.position);
    }

    private void DeliveryManager_OnDeliverySucceed(object sender, EventArgs e)
    {
        SoundManager.GetInstance().PlayDeliverySuccessSound(transform.position);
    }

    public override void Interact(PlayerControl player)
    {
        if (player.IsHasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.GetInstance().Delivery(plateKitchenObject);
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }

}
