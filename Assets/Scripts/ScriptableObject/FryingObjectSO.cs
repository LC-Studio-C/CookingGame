using UnityEngine;

[CreateAssetMenu()]
public class FryingObjectSO : ScriptableObject
{
    public KitchenObjectSO inputKitchenObjSO;
    public KitchenObjectSO outputKitchenObjSO;
    public float fryingTimeMax;
}
