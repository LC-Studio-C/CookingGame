using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconTemplate : MonoBehaviour
{
    [SerializeField] private Image image;


    public void SetKitObjSOSprite(Sprite kitchenObjSprite)
    {
        image.sprite = kitchenObjSprite;
    }
}
