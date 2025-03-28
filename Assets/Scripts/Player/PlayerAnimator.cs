using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "IsWalking";

    private Animator animator;

    [SerializeField] private PlayerControl player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (IsOwner == false)
        {
            return;
        }

        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
