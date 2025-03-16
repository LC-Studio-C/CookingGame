using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RebindKeyUI : BaseUI
{
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAltButton;
    [SerializeField] private Button gamePadInteractButton;
    [SerializeField] private Button gamePadInteractAltButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltText;
    [SerializeField] private TextMeshProUGUI gamePadInteractText;
    [SerializeField] private TextMeshProUGUI gamePadInteractAltText;
    [SerializeField] private Transform tipsTransform;

    private void Awake()
    {
        moveUpButton.onClick.AddListener(() =>
        {
            ReBindKey(GameInput.Binding.MoveUp);
        });
        moveDownButton.onClick.AddListener(() =>
        {
            ReBindKey(GameInput.Binding.MoveDown);
        });
        moveLeftButton.onClick.AddListener(() =>
        {
            ReBindKey(GameInput.Binding.MoveLeft);
        });
        moveRightButton.onClick.AddListener(() =>
        {
            ReBindKey(GameInput.Binding.MoveRight);
        });
        interactButton.onClick.AddListener(() =>
        {
            ReBindKey(GameInput.Binding.Interact);
        });
        interactAltButton.onClick.AddListener(() =>
        {
            ReBindKey(GameInput.Binding.InteractAlt);
        });
        gamePadInteractButton.onClick.AddListener(() =>
        {
            ReBindKey(GameInput.Binding.GamePadInteract);
        });
        gamePadInteractAltButton.onClick.AddListener(() =>
        {
            ReBindKey(GameInput.Binding.GamePadInteractAlt);
        });
        backButton.onClick.AddListener(() =>
        {
            CookingGameManager.GetInstance().SyncResumeGameFromPuase();
        });
    }

    private void Start()
    {
        OptionUI.GetInstance().OnRebindKeyButtonPressed += OptionUI_OnRebindKeyButtonPressed;
        Hide();
    }

    private void OptionUI_OnRebindKeyButtonPressed(object sender, System.EventArgs e)
    {
        UIStack.uiStack.Push(this);
        Show();
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        moveUpText.text = GameInput.GetInstance().GetBindingKeyText(GameInput.Binding.MoveUp);
        moveDownText.text = GameInput.GetInstance().GetBindingKeyText(GameInput.Binding.MoveDown);
        moveLeftText.text = GameInput.GetInstance().GetBindingKeyText(GameInput.Binding.MoveLeft);
        moveRightText.text = GameInput.GetInstance().GetBindingKeyText(GameInput.Binding.MoveRight);
        interactText.text = GameInput.GetInstance().GetBindingKeyText(GameInput.Binding.Interact);
        interactAltText.text = GameInput.GetInstance().GetBindingKeyText(GameInput.Binding.InteractAlt);
        gamePadInteractText.text = GameInput.GetInstance().GetBindingKeyText(GameInput.Binding.GamePadInteract);
        gamePadInteractAltText.text = GameInput.GetInstance().GetBindingKeyText(GameInput.Binding.GamePadInteractAlt);
    }


    private void ReBindKey(GameInput.Binding binding)
    {
        ShowTips();
        GameInput.GetInstance().RebindBindings(binding, () =>
        {
            HideTips();
            UpdateVisual();
        });
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        moveUpButton.Select();
    }

    private void ShowTips()
    {
        tipsTransform.gameObject.SetActive(true);
    }

    private void HideTips()
    {
        tipsTransform.gameObject.SetActive(false);
    }
}
