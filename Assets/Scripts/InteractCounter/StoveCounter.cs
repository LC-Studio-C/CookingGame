using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;



public class StoveCounter : BaseCounter,IProgress
{
    /// <summary>
    /// 烤盘状态机
    /// </summary>
    public enum FryingState
    {
        Idle,
        Frying,
        Burning,
        Burned
    }

    [SerializeField] FryingObjectSO[] fryingObjectSOArray;
    [SerializeField] BurningObjectSO[] burningObjectSOArray; 
    private FryingObjectSO fryingObjectSO;
    private BurningObjectSO burningObjectSO;

    /// <summary>
    /// 烤盘当前状态
    /// </summary>
    private NetworkVariable<FryingState> fryingState = new NetworkVariable<FryingState>(FryingState.Idle);
    /// <summary>
    /// 煎烤计时器
    /// </summary>
    private NetworkVariable<float> fryingTimer = new (0f);
    /// <summary>
    /// 燃烧计时器
    /// </summary>
    private NetworkVariable<float> burningTimer = new (0f);

    /// <summary>
    /// 煎烤时触发，控制烤盘特效
    /// </summary>
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public FryingState fryingStateArgs;
    }

    public event EventHandler<IProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        fryingState.OnValueChanged += FryingState_OnValueChanged;
    }

    private void FryingTimer_OnValueChanged(float previousValue,float newValue)
    {
        float fryingTimeMax = fryingObjectSO != null ? fryingObjectSO.fryingTimeMax : 1f;
        OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs()
        {
            progressArgs = this.fryingTimer.Value / fryingTimeMax
        });
    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimeMax = burningObjectSO != null ? burningObjectSO.burningTimeMax : 1f;
        OnProgressChanged?.Invoke(this, new IProgress.OnProgressChangedEventArgs()
        {
            progressArgs = this.burningTimer.Value / burningTimeMax
        });
    }

    private void FryingState_OnValueChanged(FryingState previousValue,FryingState newValue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() { fryingStateArgs = fryingState.Value });
    }

    private void Start()
    {
        fryingState.Value = FryingState.Idle;
    }

    private void Update()
    {
        if (IsServer == false)
        {
            return;
        }
        switch (fryingState.Value)
        {
            case FryingState.Idle:
                break;
            case FryingState.Frying:
                fryingTimer.Value += Time.deltaTime;
                if (fryingTimer.Value > fryingObjectSO.fryingTimeMax)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    KitchenObject.ProductKitchenObject(fryingObjectSO.outputKitchenObjSO, this);
                    fryingTimer.Value = 0f;
                    SetBurningKitchenObjectSOServerRpc();
                    fryingState.Value = FryingState.Burning;
                }
                break;
            case FryingState.Burning:
                burningTimer.Value += Time.deltaTime;
                if (burningTimer.Value > burningObjectSO.burningTimeMax)
                {
                    if (IsHasKitchenObject() == true)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.ProductKitchenObject(burningObjectSO.outputKitchenObjSO, this);
                        burningTimer.Value = 0f;
                        fryingState.Value = FryingState.Burned;
                    }
                }
                break;
            case FryingState.Burned:
                break;
        }
    }

    public override void Interact(PlayerControl player)
    {
        if (IsHasKitchenObject() == false)
        {
            if (player.IsHasKitchenObject() == true)
            {
                if (IsCanFrying(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetParent(this);
                    InteractFryingServerRpc();
                }
                else if (IsCanBurning(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetParent(this);
                    InteractBurningServerRpc();
                }
            }
            else
            {
                //什么都不做
            }
        }
        else
        {
            if (player.IsHasKitchenObject() == true)
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        InteractLogicServerRpc();
                    }
                }
                else
                {
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                            InteractLogicServerRpc();
                        }
                    }
                }
            }
            else
            {
                GetKitchenObject().SetParent(player);
                InteractLogicServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SetBurningKitchenObjectSOServerRpc()
    {
        SetBurningKitchenObjectSOClientRpc();
    }

    [ClientRpc]
    private void SetBurningKitchenObjectSOClientRpc()
    {
        burningObjectSO = GetBurningObjectSO(fryingObjectSO.outputKitchenObjSO);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractFryingServerRpc()
    {
        fryingTimer.Value = 0;
        fryingState.Value = FryingState.Frying;
        InteractFryingClientRpc();
    }

    [ClientRpc]
    private void InteractFryingClientRpc()
    {
        fryingObjectSO = GetFryingObjectSO(GetKitchenObject().GetKitchenObjectSO());
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractBurningServerRpc()
    {
        burningTimer.Value = 0f;
        fryingState.Value = FryingState.Burning;
        InteractBurningClientRpc();
    }

    [ClientRpc]
    private void InteractBurningClientRpc()
    {
        burningObjectSO = GetBurningObjectSO(GetKitchenObject().GetKitchenObjectSO());
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        fryingState.Value = FryingState.Idle;
        fryingTimer.Value = 0f;
        burningTimer.Value = 0f;
    }

    public FryingObjectSO GetFryingObjectSO(KitchenObjectSO inputKitchenObjSO)
    {
        foreach (var fryingObjSO in this.fryingObjectSOArray)
        {
            if (fryingObjSO.inputKitchenObjSO == inputKitchenObjSO)
            {
                return fryingObjSO;
            }
        }
        return null;
    }

    public KitchenObjectSO GetOutPutKitchenObjectSO(KitchenObjectSO inputKitchenObjSO)
    {
        FryingObjectSO fryingObjectSO = GetFryingObjectSO(inputKitchenObjSO);
        return fryingObjectSO.outputKitchenObjSO;
    }

    public bool IsCanFrying(KitchenObjectSO inputKitchenObjSO)
    {
        FryingObjectSO fryingObjectSO = GetFryingObjectSO(inputKitchenObjSO);
        return fryingObjectSO != null;
    }

    public BurningObjectSO GetBurningObjectSO(KitchenObjectSO inputKitchenObjSO)
    {
        foreach (var burningObj in this.burningObjectSOArray)
        {
            if (burningObj.inputKitchenObjSO == inputKitchenObjSO)
            {
                return burningObj;
            }
        }
        return null;
    }

    public bool IsCanBurning(KitchenObjectSO inputKitchenObjSO)
    {
        BurningObjectSO burningObjectSO = GetBurningObjectSO(inputKitchenObjSO);
        return burningObjectSO != null;
    }

    public FryingState GetStoveFryState()
    {
        return this.fryingState.Value;
    }

    public float GetBurningTimerProgress()
    {
        return burningTimer.Value / burningObjectSO.burningTimeMax;
    }
}
