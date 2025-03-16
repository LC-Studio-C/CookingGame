using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounterThrowAni : MonoBehaviour
{
    [SerializeField] TrashCounter trashCounter;
    private Animator animator;
    private const string THROW = "Throw";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        trashCounter.OnThrowing += TrashCounter_OnThrow;
    }

    private void TrashCounter_OnThrow(object sender, System.EventArgs e)
    {
        animator.SetTrigger(THROW);
    }
}
