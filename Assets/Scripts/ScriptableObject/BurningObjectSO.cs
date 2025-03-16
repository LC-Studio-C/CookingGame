using UnityEngine;

[CreateAssetMenu()]
public class BurningObjectSO : ScriptableObject
{
    public KitchenObjectSO inputKitchenObjSO;
    public KitchenObjectSO outputKitchenObjSO;
    public float burningTimeMax;
}
