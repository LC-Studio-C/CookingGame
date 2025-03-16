using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    [SerializeField] CuttingCounter cuttingCounter;
    private Animator animator;
    private const string CUT = "Cut";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cuttingCounter.OnCutting += CuttingCounter_OnCutting;
    }

    private void CuttingCounter_OnCutting(object sender, System.EventArgs e)
    {
        animator.SetTrigger(CUT);
    }
}
