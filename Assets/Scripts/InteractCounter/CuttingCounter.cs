using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter,IProgress
{
    /// <summary>
    /// �в��䷽
    /// </summary>
    [SerializeField] private CuttingObjectSO[] cuttingRecipeObjSOArray;
    /// <summary>
    /// ��ǰ�в˽���
    /// </summary>
    [SerializeField] private int currentCuttingProgress;

    /// <summary>
    /// ���в˵�ʱ����,CuttingCounterVisual����
    /// </summary>
    public event EventHandler OnCutting;

    public static event EventHandler OnCutSomething;
    new public static void ResetStaticData()
    {
        OnCutSomething = null;
    }

    /// <summary>
    /// ���в˽��ȸı�֮ʱ��
    /// </summary>
    //public event EventHandler<CuttingProgressEventArgs> OnCuttingProgressChanged;
    public event EventHandler<IProgress.OnProgressChangedEventArgs> OnProgressChanged;

    /// <summary>
    /// �в˽��ȸı�ʱ���ݵ���Ϣ
    /// </summary>
    /*public class CuttingProgressEventArgs : EventArgs
    {
        public float cuttingProgressArgs;
    }*/

    /// <summary>
    /// �������
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
                //ʲô������
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
    /// �Ҽ�����
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
    /// ����inputKitchenObjSO,���䷽�д������ö�Ӧ��outputKitchenObjSO
    /// </summary>
    /// <param name="inputKitchenObjectSO"></param>
    /// <returns></returns>
    private KitchenObjectSO GetOutputKitchenObjSO(KitchenObjectSO inputKitchenObjSO)
    {
        CuttingObjectSO cuttingObjectSO = GetCuttingObjectSO(inputKitchenObjSO);
        return cuttingObjectSO.outputSO;
    }

    /// <summary>
    /// �����䷽�����䷽���д����KitchenObjectSO���򷵻�true,����false
    /// </summary>
    /// <param name="inputKitchenObjSO"></param>
    /// <returns></returns>
    private bool IsCanCutting(KitchenObjectSO inputKitchenObjSO)
    {
        CuttingObjectSO cuttingObjectSO = GetCuttingObjectSO(inputKitchenObjSO);
        return cuttingObjectSO != null;
    }

    /// <summary>
    /// ����inputKitchenObjSO���䷽�д��ڣ��򷵻�cuttingObjSO������inputKitchenObjSO��outputinputKitchenObjSO��
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
