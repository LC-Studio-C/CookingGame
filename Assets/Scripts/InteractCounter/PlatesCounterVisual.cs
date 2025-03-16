using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private List<GameObject> plateList;

    private void Awake()
    {
        platesCounter.OnPlateProduced += PlatesCounter_OnPlateProduced;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
        plateList = new List<GameObject>();
    }

    private void PlatesCounter_OnPlateRemoved(object sender, System.EventArgs e)
    {
        GameObject willRemovePlate = plateList[plateList.Count - 1];
        plateList.Remove(plateList[plateList.Count - 1]);
        Destroy(willRemovePlate);
    }

    private void PlatesCounter_OnPlateProduced(object sender, System.EventArgs e)
    {
        Transform plate = Instantiate(kitchenObjectSO.Prefab, platesCounter.GetKitchenObjFollwPoint());
        float plateProductOffsetY = .1f;
        plate.localPosition = new Vector3(0, plateProductOffsetY,0)*plateList.Count;
        plateList.Add(plate.gameObject);
    }
}
