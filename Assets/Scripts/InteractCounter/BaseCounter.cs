using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    /// <summary>
    /// SoundManager����
    /// </summary>
    public static event EventHandler OnGetedSomething;

    public static void ResetStaticData()
    {
        OnGetedSomething = null;
    }

    protected enum DestorySelf
    {
        DestorySelf,
        DestorySelfDelay
    }

    [SerializeField] private Transform counterTopPoint;

    /// <summary>
    /// ��ǰCounter�ϵ�����
    /// </summary>
    private KitchenObject kitchenObject;

    /// <summary>
    /// Counter������ʱ���еĲ���
    /// </summary>
    public virtual void Interact(PlayerControl player)
    {
        Debug.Log("BaseInteract");
    }

    public virtual void InteractAlternate(PlayerControl player)
    {
        Debug.Log("BaseInteractAlternate");
    }

    /// <summary>
    /// ����KitchenObject���ɵ��Transform
    /// </summary>
    /// <returns></returns>
    public Transform GetKitchenObjFollwPoint()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (this.kitchenObject != null)
        {
            OnGetedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return this.kitchenObject;
    }

    public void ClearKitchenObject()
    {
        this.kitchenObject = null;
    }

    public bool IsHasKitchenObject()
    {
        return this.kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
