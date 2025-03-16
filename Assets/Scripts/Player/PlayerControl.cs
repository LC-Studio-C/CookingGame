using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerControl : NetworkBehaviour, IKitchenObjectParent
{
    private static PlayerControl localInstance;

    [SerializeField] private PlayerVisual playerVisual;

    [SerializeField] private Transform playerHand;
    private KitchenObject kitchenObject;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private LayerMask moveLayerMask;

    private bool isWalking = false;

    private BaseCounter selectedCounter;

    [SerializeField] private List<Vector3> playerSpawnPositionList = new();

    //选择counter的事件
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    /// <summary>
    /// OnSelectedCounterChanged事件中消息传递类，负责把被选中Counter传递给事件处理器
    /// </summary>
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    /// <summary>
    /// SelectedCounterVisual订阅
    /// </summary>
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
        OnAnyPickedSomething = null;
    }

    

    public override void OnNetworkSpawn()
    {
        if (IsOwner == true)
        {
            localInstance = this;
        }
        transform.position = playerSpawnPositionList[CookingGameMultiplayer.GetInstance().GetPlayerIndexByClientId(OwnerClientId)];
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId && IsHasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void Awake()
    {
        GameInput.GetInstance().OnInteractAction += GameInput_OnInteractAction;
        GameInput.GetInstance().OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void Start()
    {
        PlayerData playerData = CookingGameMultiplayer.GetInstance().GetPlayerDataByClientId(OwnerClientId);
        playerVisual.SetPlayerColor(CookingGameMultiplayer.GetInstance().GetColor(playerData.colorId));
    }

    private void Update()
    {
        if (IsOwner == false)
        {
            return;
        }
        //HandleMovementServerAuth();
        HandleMovement();
        HandleInteractions();
    }

    /// <summary>
    /// 交互事件处理器，负责鼠标右键处理点击交互
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (CookingGameManager.GetInstance().IsGamePlaying() == false)
        {
            return;
        }
        
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    /// <summary>
    /// 交互事件处理器，负责鼠标左键处理点击交互
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (CookingGameManager.GetInstance().IsGamePlaying() == false)
        {
            return;
        }

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }


    /// <summary>
    /// 设置被选中的Counter，传递给事件处理器(事件在SelectedCounterVisual类中被订阅)
    /// </summary>
    /// <param name="baseCounter">要传递的Counter</param>
    private void SetSelectedCounter(BaseCounter baseCounter)
    {
        this.selectedCounter = baseCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs()
        {
            selectedCounter = this.selectedCounter
        });
    }

    /// <summary>
    /// 处理玩家交互
    /// </summary>
    private void HandleInteractions()
    {
        //交互距离
        float interactDistance = 2f;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit rayHitInfo, interactDistance, counterLayerMask))
        {
            if (rayHitInfo.transform.TryGetComponent<BaseCounter>(out BaseCounter baseCounter))
            {
                SetSelectedCounter(baseCounter);
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    /// <summary>
    /// 判断玩家是否在行走，用于传递给PlayerAnimator类控制行走动画
    /// </summary>
    /// <returns>行走中为true，反之为false</returns>
    public bool IsWalking()
    {
        return isWalking;
    }

    /// <summary>
    /// 判断玩家在当前方向是否可以移动
    /// </summary>
    /// <param name="moveDir">玩家移动方向</param>
    /// <returns>若当前移动方向有障碍则false，反之则true</returns>
    private bool IsCanMove(Vector3 moveDir)
    {
        float playerRadius = .5f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;
        return !Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * playerHeight), playerRadius, moveDir, moveDistance,moveLayerMask);
    }

    /*private void HandleMovementServerAuth()
    {
        Vector2 inputVector = GameInput.GetInstance().GetMovementVectorNormalized();
        HandleMovementServerRpc(inputVector);
    }


    [ServerRpc(RequireOwnership = false)]
    private void HandleMovementServerRpc(Vector2 inputVector)
    {
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);
        }

        isWalking = moveDir != Vector3.zero;

        float moveDistance = moveSpeed * Time.deltaTime;

        bool isCanMove = IsCanMove(moveDir);

        if (CookingGameManager.GetInstance().IsGamePlaying() == false)
        {
            return;
        }

        if (isCanMove == false)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            isCanMove = IsCanMove(moveDirX);
            if (isCanMove == true)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                isCanMove = IsCanMove(moveDirZ);
                if (isCanMove == true)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if (isCanMove == true)
        {
            transform.position += moveDir * moveDistance;
        }
    }*/

    /// <summary>
    /// 处理玩家移动
    /// </summary>
    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.GetInstance().GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);
        }

        isWalking = moveDir != Vector3.zero;

        float moveDistance = moveSpeed * Time.deltaTime;

        bool isCanMove = IsCanMove(moveDir);

        if (CookingGameManager.GetInstance().IsGamePlaying() == false)
        {
            return;
        }

        if (isCanMove == false)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            isCanMove = IsCanMove(moveDirX);
            if (isCanMove == true)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                isCanMove = IsCanMove(moveDirZ);
                if (isCanMove == true)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if (isCanMove == true)
        {
            transform.position += moveDir * moveDistance;
        }
    }

    public Transform GetKitchenObjFollwPoint()
    {
        return playerHand;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (this.kitchenObject != null)
        {
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return this.kitchenObject;
    }

    public void ClearKitchenObject()
    {
        this.kitchenObject = null;
    }

    public bool IsHasKitchenObject()
    {
        return this.kitchenObject != null;
    }


    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    /// <summary>
    /// 获取一个全局访问单例
    /// </summary>
    /// <returns>返回PlayerControl单例</returns>
    public static PlayerControl GetLocalInstance()
    {
        return localInstance;
    }

}
