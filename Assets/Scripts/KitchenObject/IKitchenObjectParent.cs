using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent
{
    /// <summary>
    /// ���ظ������Transform
    /// </summary>
    /// <returns></returns>
    public Transform GetKitchenObjFollwPoint();
    /// <summary>
    /// ���ø������KitchenObject
    /// </summary>
    /// <param name="kitchenObject"></param>
    public void SetKitchenObject(KitchenObject kitchenObject);
    /// <summary>
    /// ���ص�ǰ�������ϵ�KitchenObject
    /// </summary>
    /// <returns></returns>
    public KitchenObject GetKitchenObject();
    /// <summary>
    /// �����ǰ�������ϵ�KitchenObject
    /// </summary>
    public void ClearKitchenObject();
    /// <summary>
    /// ��ǰ�������Ƿ���KitchenObject
    /// </summary>
    /// <returns>��Ϊtrue��֮Ϊfalse</returns>
    public bool IsHasKitchenObject();

    public NetworkObject GetNetworkObject();
}
