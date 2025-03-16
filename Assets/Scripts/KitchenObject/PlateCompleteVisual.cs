using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjSOAndGameObj
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private List<KitchenObjSOAndGameObj> kitchenObjSOAndGameObjList;

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    //[SerializeField] private GameObject[] childObjects;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        foreach (var kitchenObjSOAndGameObj in kitchenObjSOAndGameObjList)
        {
            kitchenObjSOAndGameObj.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        //ShowKitchenObject(e.kitchenObjectSO);
        foreach (var kitchenObjSOAndGameObj in kitchenObjSOAndGameObjList)
        {
            if (kitchenObjSOAndGameObj.kitchenObjectSO == e.kitchenObjectSO)
            {
                kitchenObjSOAndGameObj.gameObject.SetActive(true);
            }
        }
    }

    /*private void ShowKitchenObject(KitchenObjectSO kitchenObjectSO)
    {
        foreach (var childObject in childObjects)
        {
            if (childObject.name == kitchenObjectSO.objectName)
            {
                childObject.SetActive(true);
            }
        }
    }*/

    
}
