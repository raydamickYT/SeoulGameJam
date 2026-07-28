using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This script will update the gauge's visual state according to how long the player has been holding the button.
/// Each gauge will have it's own manager
/// </summary>
public class GaugeManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public GaugeSides side = GaugeSides.Top;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.Instance.AddDelegateListener(GaugeEvents.UpdateGauge, (Action<float, GaugeSides>)OnUpdateGauge);
        EventManager.Instance.AddUnityEventListener(GaugeEvents.ResetGauge, OnResetGauge);
    }

    string AnimatorGaugeAmountString = "GaugeAmount";
    void OnUpdateGauge(float value, GaugeSides btnSide)
    {
        if (side != btnSide)
            return;

        animator.SetFloat(AnimatorGaugeAmountString, value);
    }

    void OnResetGauge()
    {
        StartCoroutine(WaitForSeconds(0.5f));
    }

    IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        animator.SetFloat(AnimatorGaugeAmountString, 0f);
        
    }
}


public static class GaugeEvents
{
    public const string UpdateGauge = "UpdateGauge";
    public const string ResetGauge = "ResetGauge";
}

public enum GaugeSides
{
    Top,
    Bottom,
}