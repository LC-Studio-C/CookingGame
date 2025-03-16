using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private KitchenObjectFollowParent targerTransform;

    /// <summary>
    /// 当前物体的Parent
    /// </summary>
    private IKitchenObjectParent kitchenObjParent;

    private Rigidbody rigidbd;

    private string kitchenObjLayerName = "KitchenObject";

    protected virtual void Awake()
    {
        //rigidbd = gameObject.AddComponent<Rigidbody>();
        gameObject.layer = LayerMask.NameToLayer(kitchenObjLayerName);
        targerTransform = GetComponent<KitchenObjectFollowParent>();
    }

    private void Update()
    {
        /*if (GetKitchenObjParent() != null)
        {
            rigidbd.isKinematic = true;
        }
        else
        {
            rigidbd.isKinematic = false;
        }*/
    }

    /// <summary>
    /// 设置当前kitchenObject的Parent
    /// </summary>
    /// <param name="kitchenObjParent"></param>
    public void SetParent(IKitchenObjectParent kitchenObjParent)
    {
        SetTargerTransformServerRpc(kitchenObjParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetTargerTransformServerRpc(NetworkObjectReference targerTransformNetworkObjectReference)
    {
        SetTargerTransformClientRpc(targerTransformNetworkObjectReference);
    }

    [ClientRpc]
    private void SetTargerTransformClientRpc(NetworkObjectReference targerTransformNetworkObjectReference)
    {
        targerTransformNetworkObjectReference.TryGet(out NetworkObject targerTransformNetworkObject);
        IKitchenObjectParent kitchenObjectParent = targerTransformNetworkObject.GetComponent<IKitchenObjectParent>();
  
        //Debug.Log(kitchenObjectParent);
        //Debug.Log(kitchenObjectParent.GetKitchenObject());
        if (kitchenObjectParent.IsHasKitchenObject() == true)
        {
            return;
        }
        if (this.kitchenObjParent != null)
        {
            this.kitchenObjParent.ClearKitchenObject();
        }
        this.kitchenObjParent = kitchenObjectParent;
        kitchenObjectParent.SetKitchenObject(this);
        Transform targerTransform = kitchenObjectParent.GetKitchenObjFollwPoint();
        this.targerTransform.SetTargerTransform(targerTransform);
    }

    /// <summary>
    /// 返回当前KitchenObject的Parent
    /// </summary>
    /// <returns></returns>
    public IKitchenObjectParent GetKitchenObjParent()
    {
        return this.kitchenObjParent;
    }

    /// <summary>
    /// 返回ScriptableObject
    /// </summary>
    /// <returns></returns>
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }


    /// <summary>
    /// 销毁当前kitchenObject
    /// </summary>
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearKitchenObjectParentOnKitchenObject()
    {
        this.kitchenObjParent.ClearKitchenObject();
    }

    /// <summary>
    /// 延迟销毁当前kitchenObject
    /// </summary>
    /// <param name="time"></param>
    public void DestroySelfDelay(float time)
    {
        Destroy(gameObject,time);   
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }


    /// <summary>
    /// 生成传入的kitchenObjSO预制件，并设置它的父物体为传入的kitchenObjParent
    /// </summary>
    /// <param name="kitchenObjSO">要生成的kitchenObjSO</param>
    /// <param name="kitchenObjParent">要设置的父物体</param>
    /// <returns></returns>
    public static void ProductKitchenObject(KitchenObjectSO kitchenObjSO,IKitchenObjectParent kitchenObjParent)
    {
        CookingGameMultiplayer.GetInstance().ProductKitchenObjectForNetwork(kitchenObjSO, kitchenObjParent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        CookingGameMultiplayer.GetInstance().DestroyKitchenObjectForNetwork(kitchenObject);
    }
}
