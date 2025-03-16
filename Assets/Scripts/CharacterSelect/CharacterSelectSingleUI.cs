using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject select;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            //CookingGameMultiplayer.GetInstance().OnPlayerDataListChanged += CookingGameMultiplayer_OnPlayerDataListChanged;
            CookingGameMultiplayer.GetInstance().ChangePlayerColor(colorId);
            UpdateIsColor();
        });
    }

    private void Start()
    {
        CookingGameMultiplayer.GetInstance().OnPlayerDataListChanged += CookingGameMultiplayer_OnPlayerDataListChanged;
        image.color = CookingGameMultiplayer.GetInstance().GetColor(colorId);
        UpdateIsColor();
    }

    private void OnDestroy()
    {
        CookingGameMultiplayer.GetInstance().OnPlayerDataListChanged -= CookingGameMultiplayer_OnPlayerDataListChanged;
    }

    private void CookingGameMultiplayer_OnPlayerDataListChanged(object sender, System.EventArgs e)
    {
        UpdateIsColor();
    }

    private void UpdateIsColor()
    {
        if (CookingGameMultiplayer.GetInstance().GetPlayerColorId() == colorId)
        {
            select.SetActive(true);
        }
        else
        {
            select.SetActive(false);
        }
    }

    
}
