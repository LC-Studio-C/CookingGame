using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        /*Transform iconTemp = Instantiate(iconTemplate, transform);
        iconTemp.GetComponent<IconTemplate>().SetKitObjSOSprite(e.kitchenObjectSO);
        iconTemp.gameObject.SetActive(true);*/
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in transform)
        {
            if (child == iconTemplate)
            {
                continue;
            }
            Destroy(child.gameObject);
        }

        foreach (var kitchenObjSO in plateKitchenObject.GetKitchenObjectSOList())
        {
            Transform iconTemp = Instantiate(iconTemplate, transform);
            iconTemp.gameObject.SetActive(true);
            iconTemp.GetComponent<IconTemplate>().SetKitObjSOSprite(kitchenObjSO.sprite);
        }
    }
}
