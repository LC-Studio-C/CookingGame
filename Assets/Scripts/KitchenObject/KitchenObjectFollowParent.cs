using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObjectFollowParent : MonoBehaviour
{
    [SerializeField] private Transform targerTransform;

    private void LateUpdate()
    {
        if (targerTransform == null)
        {
            return;
        }

        transform.position = targerTransform.position;
        transform.rotation = targerTransform.rotation;
    }

    public void SetTargerTransform(Transform targerTransform)
    {
        this.targerTransform = targerTransform;
    }
}
