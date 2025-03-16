using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    //[SerializeField] private ClearCounter clearCounter;
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;
    private List<KitchenObjectSO> kitchenObjectSOList;


    [SerializeField] private GameObject plateComplete;
    [SerializeField] private Transform kitchenObjFollowPoint;

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    private void Start()
    {
        //clearCounter.OnPlating += ClearCounter_OnPlating;
    }

    /*private void ClearCounter_OnPlating(object sender, ClearCounter.OnPlatingEventArgs e)
    {
        Instantiate(plateComplete, kitchenObjFollowPoint);
    }*/


    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (validKitchenObjectSOList.Contains(kitchenObjectSO) == false)
        {
            return false;
        }
        if (kitchenObjectSOList.Contains(kitchenObjectSO) == false)
        {
            int kitchenObjectSOIndex = GetKitchenObjectSOIndex(kitchenObjectSO);
            AddIngredientServerRpc(kitchenObjectSOIndex);
            return true;
        }
        else
        {
            return false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOByIndex(kitchenObjectSOIndex);
        kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs()
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return validKitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    private KitchenObjectSO GetKitchenObjectSOByIndex(int kitchenObjectSOIndex)
    {
        return validKitchenObjectSOList[kitchenObjectSOIndex];
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return this.kitchenObjectSOList;
    }
}
