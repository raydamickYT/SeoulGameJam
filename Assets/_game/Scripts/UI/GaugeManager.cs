using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This script will update the gauge's visual state according to how long the player has been holding the button.
/// Each gauge will have it's own manager
/// </summary>
public class GaugeManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public GaugeSides side = GaugeSides.Top;
    public float MaxGaugeAmount = 31;
    public float BrokenTime = 10;
    private bool isbroken = false;

    const string AnimatorGaugeAmountString = "GaugeAmount";
    const string AnimatorAltGaugeString = "GaugeAlt";

    float gaugeValue;

    void Start()
    {
        EventManager.Instance.AddDelegateListener(GaugeEvents.UpdateGauge, (Action<float, GaugeSides>)OnUpdateGauge);
        EventManager.Instance.AddDelegateListener(ButtonEvents.OnButtonPointeUp, (Action<GaugeSides>)UpdateBlackBoard);
        EventManager.Instance.AddDelegateListener("ItemModifierChanged", (Action<GaugeSides>)OnModifierChanged);

        ApplyAltGaugeVisual(BlackBoard.GetAltGauge(side));
    }

    void OnModifierChanged(GaugeSides changedSide)
    {
        if (changedSide != side) return;
        ApplyAltGaugeVisual(BlackBoard.GetAltGauge(side));
    }

    void ApplyAltGaugeVisual(bool alt)
    {
        // Optional animator bool — add "GaugeAlt" to the controller when you have the alt look ready.
        if (animator == null) return;

        foreach (var p in animator.parameters)
        {
            if (p.name == AnimatorAltGaugeString && p.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(AnimatorAltGaugeString, alt);
                return;
            }
        }
    }

    void OnUpdateGauge(float value, GaugeSides btnSide)
    {
        if (isbroken) return;
        if (side != btnSide)
            return;

        gaugeValue = value;

        animator.SetFloat(AnimatorGaugeAmountString, Mathf.Clamp(value, 0, MaxGaugeAmount));

        if (value >= MaxGaugeAmount - 0.01f)
            BreakGauge();
    }

    void BreakGauge()
    {
        if (isbroken) return;
        isbroken = true;
        animator.SetBool("GaugeIsBroken", true);
        BlackBoard.SetGaugeIsBroken(side, isbroken);
        StartCoroutine(WaitForSecondsToResetGauge(BrokenTime));
    }

    void UpdateBlackBoard(GaugeSides btnSide)
    {
        if (isbroken) return;
        if (side != btnSide) return;

        BlackBoard.SetGaugeValue(side, gaugeValue);
        EventManager.Instance.TriggerUnityEvent(BackgroundManagerEvents.UpdateMeter);

        StartCoroutine(WaitForSecondsToResetGauge(0.5f));
    }

    IEnumerator WaitForSecondsToResetGauge(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        BlackBoard.ResetGauge(side);
        animator.SetFloat(AnimatorGaugeAmountString, 0f);

        if (isbroken)
        {
            animator.SetBool("GaugeIsBroken", false);
            isbroken = false;
            BlackBoard.SetGaugeIsBroken(side, isbroken);
        }
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
