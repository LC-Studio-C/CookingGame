using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter,IProgress
{
    /// <summary>
    /// 切菜配方
    /// </summary>
    [SerializeField] private CuttingObjectSO[] cuttingRecipeObjSOArray;
    /// <summary>
    /// 当前切菜进度
    /// </summary>
    [SerializeField] private int currentCuttingProgress;

    /// <summary>
    /// 当切菜的时候做,CuttingCounterVisual订阅
    /// </summary>
    public event EventHandler OnCutting;

    public static event EventHandler OnCutSomething;
    new public static void ResetStaticData()
    {
        OnCutSomething = null;
    }

    /// <summary>
    /// 当切菜进度改变之时做
    /// </summary>
    //public event EventHandler<CuttingProgressEventArgs> OnCuttingProgressChanged;
    public event EventHandler<IProgress.OnProgressChangedEventArgs> OnProgressChanged;

    /// <summary>
    /// 切菜进度改变时传递的消息
    /// </summary>
    /*public class CuttingProgressEventArgs : EventArgs
    {
        public float cuttingProgressArgs;
    }*/

    /// <summary>
    /// 左键交互
    /// </summary>
    /// <param name="player"></param>
    public override void Interact(PlayerControl player)
    {
        if (IsHasKitchenObject() == false)
        {
            if (player.IsHasKitchenObject() == true && IsCanCutting(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetParent(this);
                InteractServerRpc();
                CuttingObjectSO cuttingObjectSO = GetCuttingObjectSO(kitchenObject.GetKitchenObjectSO());
                OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs()
                {
                    progressArgs = (float)this.currentCuttingProgress / cuttingObjectSO.cuttingProgressMax
                });
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
                InteractServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc()
    {
        InteractClientRpc();
    }

    [ClientRpc]
    private void InteractClientRpc()
    {
        currentCuttingProgress = 0;
        OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs()
        {
            progressArgs = currentCuttingProgress
        });
    }

    /// <summary>
    /// 右键交互
    /// </summary>
    /// <param name="player"></param>
    public override void InteractAlternate(PlayerControl player)
    {
        if (IsHasKitchenObject() == true && IsCanCutting(GetKitchenObject().GetKitchenObjectSO()))
        {
            InteractAltServerRpc();
            
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractAltServerRpc()
    {
        if (IsHasKitchenObject() == true && IsCanCutting(GetKitchenObject().GetKitchenObjectSO()))
        {
            InteractAltClientRpc();
            CuttingCompletedServerRpc();
        }
    }

    [ClientRpc]
    private void InteractAltClientRpc()
    {
        currentCuttingProgress++;

        OnCutting?.Invoke(this, EventArgs.Empty);

        CuttingObjectSO cuttingObjectSO = GetCuttingObjectSO(GetKitchenObject().GetKitchenObjectSO());
        OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs()
        {
            progressArgs = (float)this.currentCuttingProgress / cuttingObjectSO.cuttingProgressMax
        });
        OnCutSomething?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CuttingCompletedServerRpc()
    {
        if (IsHasKitchenObject() == true && IsCanCutting(GetKitchenObject().GetKitchenObjectSO()))
        {
            if (currentCuttingProgress >= GetCuttingObjectSO(GetKitchenObject().GetKitchenObjectSO()).cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenSO = GetOutputKitchenObjSO(GetKitchenObject().GetKitchenObjectSO());
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
                ClearKitchenObject();
                KitchenObject.ProductKitchenObject(outputKitchenSO, this);
                currentCuttingProgress = 0;
            }
        }
    }


    /// <summary>
    /// 传入inputKitchenObjSO,若配方中存在则获得对应的outputKitchenObjSO
    /// </summary>
    /// <param name="inputKitchenObjectSO"></param>
    /// <returns></returns>
    private KitchenObjectSO GetOutputKitchenObjSO(KitchenObjectSO inputKitchenObjSO)
    {
        CuttingObjectSO cuttingObjectSO = GetCuttingObjectSO(inputKitchenObjSO);
        return cuttingObjectSO.outputSO;
    }

    /// <summary>
    /// 遍历配方，若配方中有传入的KitchenObjectSO，则返回true,否则false
    /// </summary>
    /// <param name="inputKitchenObjSO"></param>
    /// <returns></returns>
    private bool IsCanCutting(KitchenObjectSO inputKitchenObjSO)
    {
        CuttingObjectSO cuttingObjectSO = GetCuttingObjectSO(inputKitchenObjSO);
        return cuttingObjectSO != null;
    }

    /// <summary>
    /// 传入inputKitchenObjSO若配方中存在，则返回cuttingObjSO（包含inputKitchenObjSO，outputinputKitchenObjSO）
    /// </summary>
    /// <param name="inputKitchenObjSO"></param>
    /// <returns></returns>
    private CuttingObjectSO GetCuttingObjectSO(KitchenObjectSO inputKitchenObjSO)
    {
        foreach (var cuttingObjSO in this.cuttingRecipeObjSOArray)
        {
            if (cuttingObjSO.inputSO == inputKitchenObjSO)
            {
                return cuttingObjSO;
            }
        }
        return null;
    }
}
