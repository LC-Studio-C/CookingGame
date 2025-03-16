using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    void Update()
    {
        Debug.Log(GetComponent<CuttingCounter>().GetKitchenObject());
        Debug.Log(GetComponent<CuttingCounter>().GetKitchenObject().GetKitchenObjParent());
    }
}
