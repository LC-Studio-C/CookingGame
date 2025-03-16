using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameUI;
    [SerializeField] private Transform icons;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }


    public void SetRepiceTextAndIcon(RecipeSO recipeSO)
    {
        recipeNameUI.text = recipeSO.recipeName;
        foreach(Transform child in icons)
        {
            if (child == iconTemplate)
            {
                continue;
            }
            Destroy(child.gameObject);
        }

        foreach (var kitchenObjSO in recipeSO.kitchenObjSOList)
        {
            Transform icon = Instantiate(iconTemplate, icons);
            icon.gameObject.SetActive(true);
            icon.GetComponent<Image>().sprite = kitchenObjSO.sprite;
        }
    }
}
