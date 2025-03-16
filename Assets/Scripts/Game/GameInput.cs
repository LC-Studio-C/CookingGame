using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private static GameInput instance;

    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlt,
        GamePadInteract,
        GamePadInteractAlt
    }

    /// <summary>
    /// PlayerControl/CookingGameManager订阅
    /// </summary>
    public event EventHandler OnInteractAction;

    /// <summary>
    /// PlayerControl订阅
    /// </summary>
    public event EventHandler OnInteractAlternateAction;

    /// <summary>
    /// CookingGameManager订阅
    /// </summary>
    public event EventHandler OnPuaseAction;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        inputActions.Player.Interact.performed += Interact_performed;
        inputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        inputActions.Player.Puased.performed += Puased_performed;
    }

    private void OnDestroy()
    {
        inputActions.Player.Interact.performed -= Interact_performed;
        inputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        inputActions.Player.Puased.performed -= Puased_performed;

        inputActions.Dispose();
    }

    private void Puased_performed(InputAction.CallbackContext obj)
    {
        OnPuaseAction?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    private void InteractAlternate_performed(InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 玩家点击鼠标左键（键盘E键）时的事件处理器，负责开启OnInteractAction事件
    /// </summary>
    /// <param name="obj"></param>
    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 获取当前玩家的WASD（等同类型）输入，并把它归一化
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMovementVectorNormalized()
    {
        return inputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    public string GetBindingKeyText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.MoveUp:
                return inputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.MoveDown:
                return inputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.MoveLeft:
                return inputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.MoveRight:
                return inputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return inputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.InteractAlt:
                return inputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.GamePadInteract:
                return inputActions.Player.Interact.bindings[2].ToDisplayString();
            case Binding.GamePadInteractAlt:
                return inputActions.Player.InteractAlternate.bindings[2].ToDisplayString();
        }
    }

    public void RebindBindings(Binding binding,Action rebindKeyCallBack)
    {
        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Binding.MoveUp:
                inputAction = inputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.MoveDown:
                inputAction = inputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.MoveLeft:
                inputAction = inputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.MoveRight:
                inputAction = inputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = inputActions.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.InteractAlt:
                inputAction = inputActions.Player.InteractAlternate;
                bindingIndex = 1;
                break;
            case Binding.GamePadInteract:
                inputAction = inputActions.Player.Interact;
                bindingIndex = 2;
                break;
            case Binding.GamePadInteractAlt:
                inputAction = inputActions.Player.InteractAlternate;
                bindingIndex = 2;
                break;
        }


        inputActions.Player.Disable();
        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
        {
            callback.Dispose();
            inputActions.Player.Enable();
            rebindKeyCallBack();
        }).Start();
    }

    public static GameInput GetInstance()
    {
        return instance;
    }
}
