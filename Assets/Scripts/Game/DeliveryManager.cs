using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    private static DeliveryManager instance;

    /// <summary>
    /// DeliveryManagerUI¶©ÔÄ
    /// </summary>
    public event EventHandler OnRecipeSpawned;
    /// <summary>
    /// DeliveryManagerUI¶©ÔÄ
    /// </summary>
    public event EventHandler OnRecipeCompleted;
    /// <summary>
    /// DeliveryUI¶©ÔÄ
    /// </summary>
    public event EventHandler OnDeliverySucceed;
    /// <summary>
    /// DeliveryUI¶©ÔÄ
    /// </summary>
    public event EventHandler OnDeliveryFailed;

    [SerializeField] private RecipeListSO recipeList;

    private List<RecipeSO> waitRecipeSOList;

    private float productRecipeTimer = 4f;
    private float productRecipeTimeMax = 4f;

    private int currentRecipeCount = 0;
    private int recipeCountMax = 4;

    private int deliverySuccessAmount;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }

        waitRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        if (IsServer == false)
        {
            return;
        }

        if (CookingGameManager.GetInstance().IsGamePlaying())
        {
            productRecipeTimer -= Time.deltaTime;
            if (productRecipeTimer < 0)
            {
                productRecipeTimer = productRecipeTimeMax;
                if (currentRecipeCount < recipeCountMax)
                {
                    currentRecipeCount++;
                    int waitRecipeSOIndex = UnityEngine.Random.Range(0, recipeList.recipeListSO.Count);
                    WaitRecipeSOClientRpc(waitRecipeSOIndex);
                }
            }
        }
    }

    [ClientRpc()]
    private void WaitRecipeSOClientRpc(int waitRecipeSOIndex)
    {
        RecipeSO recipeSO = recipeList.recipeListSO[waitRecipeSOIndex];
        waitRecipeSOList.Add(recipeSO);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public bool Delivery(PlateKitchenObject plateKitchenObj)
    {
        for (int i = 0; i < waitRecipeSOList.Count; i++)
        {
            if (waitRecipeSOList[i].kitchenObjSOList.Count == plateKitchenObj.GetKitchenObjectSOList().Count)
            {
                bool plateContentsMatchesRecipe = true;
                foreach (var recipeKitchenObjSO in waitRecipeSOList[i].kitchenObjSOList)
                {
                    bool isIngredientFound = false;
                    foreach (var plateKitchenObjSO in plateKitchenObj.GetKitchenObjectSOList())
                    {
                        if (recipeKitchenObjSO == plateKitchenObjSO)
                        {
                            isIngredientFound = true;
                            break;
                        }
                    }
                    if (isIngredientFound == false)
                    {
                        plateContentsMatchesRecipe = false;
                    }
                }
                if (plateContentsMatchesRecipe == true)
                {
                    DeliveryRecipeSuccessServerRpc(i);
                    /*waitRecipeSOList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnDeliverySucceed?.Invoke(this, EventArgs.Empty);
                    currentRecipeCount--;
                    deliverySuccessAmount++;*/
                    return true;
                }
            }
        }
        DeliveryRecipeFailServerRpc();
        //OnDeliveryFailed?.Invoke(this, EventArgs.Empty);
        return false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryRecipeSuccessServerRpc(int deliveryRecipeIndex)
    {
        DeliveryRecipeSuccessClientRpc(deliveryRecipeIndex);
    }

    [ClientRpc]
    private void DeliveryRecipeSuccessClientRpc(int deliveryRecipeIndex)
    {
        waitRecipeSOList.RemoveAt(deliveryRecipeIndex);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnDeliverySucceed?.Invoke(this, EventArgs.Empty);
        currentRecipeCount--;
        deliverySuccessAmount++;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryRecipeFailServerRpc()
    {
        DeliveryRecipeFailClientRpc();
    }

    [ClientRpc]
    private void DeliveryRecipeFailClientRpc()
    {
        OnDeliveryFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitRecipeSOList()
    {
        return waitRecipeSOList;
    }

    public int GetDeliverySeccessAmount()
    {
        return deliverySuccessAmount;
    }

    public static DeliveryManager GetInstance()
    {
        return instance;
    }
}
