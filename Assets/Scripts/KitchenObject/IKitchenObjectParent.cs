using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent
{
    /// <summary>
    /// 返回父物体的Transform
    /// </summary>
    /// <returns></returns>
    public Transform GetKitchenObjFollwPoint();
    /// <summary>
    /// 设置父物体的KitchenObject
    /// </summary>
    /// <param name="kitchenObject"></param>
    public void SetKitchenObject(KitchenObject kitchenObject);
    /// <summary>
    /// 返回当前父物体上的KitchenObject
    /// </summary>
    /// <returns></returns>
    public KitchenObject GetKitchenObject();
    /// <summary>
    /// 清楚当前父物体上的KitchenObject
    /// </summary>
    public void ClearKitchenObject();
    /// <summary>
    /// 当前父物体是否有KitchenObject
    /// </summary>
    /// <returns>有为true反之为false</returns>
    public bool IsHasKitchenObject();

    public NetworkObject GetNetworkObject();
}
