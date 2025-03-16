using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(PlayerControl player)
    {
        if (IsHasKitchenObject() == false)
        {
            if (player.IsHasKitchenObject() == true)
            {
                player.GetKitchenObject().SetParent(this);
            }
            else
            {
                //什么都不做
            }
        }
        else
        {
            if (player.IsHasKitchenObject() == true)
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else
                {
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    }
                }
            }
            else
            {
                GetKitchenObject().SetParent(player);
            }
        }
    }

}
