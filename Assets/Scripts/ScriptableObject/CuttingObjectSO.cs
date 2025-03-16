using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class CuttingObjectSO : ScriptableObject
{
    public KitchenObjectSO inputSO;
    public KitchenObjectSO outputSO;
    public int cuttingProgressMax;
}
