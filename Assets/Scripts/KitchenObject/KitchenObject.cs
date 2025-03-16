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
    /// ��ǰ�����Parent
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
    /// ���õ�ǰkitchenObject��Parent
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
    /// ���ص�ǰKitchenObject��Parent
    /// </summary>
    /// <returns></returns>
    public IKitchenObjectParent GetKitchenObjParent()
    {
        return this.kitchenObjParent;
    }

    /// <summary>
    /// ����ScriptableObject
    /// </summary>
    /// <returns></returns>
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }


    /// <summary>
    /// ���ٵ�ǰkitchenObject
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
    /// �ӳ����ٵ�ǰkitchenObject
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
    /// ���ɴ����kitchenObjSOԤ�Ƽ������������ĸ�����Ϊ�����kitchenObjParent
    /// </summary>
    /// <param name="kitchenObjSO">Ҫ���ɵ�kitchenObjSO</param>
    /// <param name="kitchenObjParent">Ҫ���õĸ�����</param>
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
