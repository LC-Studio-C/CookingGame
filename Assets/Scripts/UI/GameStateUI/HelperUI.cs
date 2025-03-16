using TMPro;
using UnityEngine;

public class HelperUI : BaseUI
{

    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltText;

    private void Start()
    {
        Show();
        CookingGameManager.GetInstance().OnPlayerReadyChanged += CookingGameManagers_OnPlayerReadyChanged;
        GamePuaseUI.GetInstance().OnHelperButtonPressed += GamePuaseUI_OnHelperButtonPressed;
    }

    private void CookingGameManagers_OnPlayerReadyChanged(object sender, System.EventArgs e)
    {
        if (CookingGameManager.GetInstance().IsLocalPlayerReady() == true)
        {
            Hide();
        }
    }

    private void GamePuaseUI_OnHelperButtonPressed(object sender, System.EventArgs e)
    {
        UIStack.uiStack.Push(this);
        Show();
    }

    public override void Show()
    {
        gameObject.SetActive(true);
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
    }
}
