using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform containerTemp;
    [SerializeField] private Transform recipeTemp;


    private void Start()
    {
        DeliveryManager.GetInstance().OnRecipeSpawned += DeliveryManagerUI_OnRecipeSpawned;
        DeliveryManager.GetInstance().OnRecipeCompleted += DeliveryManagerUI_OnRecipeCompleted;
        recipeTemp.gameObject.SetActive(false);
    }

    private void DeliveryManagerUI_OnRecipeCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManagerUI_OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in containerTemp.transform)
        {
            if (child == recipeTemp)
            {
                continue;
            }
            Destroy(child.gameObject);
        }

        List<RecipeSO> waitRecipeSOList = DeliveryManager.GetInstance().GetWaitRecipeSOList();

        foreach (var waitRecipeSO in waitRecipeSOList)
        {
            Transform recipeTemplate = Instantiate(recipeTemp, containerTemp);
            recipeTemplate.gameObject.SetActive(true);
            recipeTemplate.GetComponent<DeliveryManagerSingleUI>().SetRepiceTextAndIcon(waitRecipeSO);
        }
    }
}
