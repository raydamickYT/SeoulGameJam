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
    Vector3 baseScale;
    bool isHolding = false;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Start()
    {
        EventManager.Instance.AddDelegateListener(GaugeEvents.UpdateGauge, (Action<float, GaugeSides>)OnUpdateGauge);
        EventManager.Instance.AddDelegateListener(ButtonEvents.OnButtonPointeUp, (Action<GaugeSides>)UpdateBlackBoard);
        EventManager.Instance.AddDelegateListener("ItemModifierChanged", (Action<GaugeSides>)OnModifierChanged);

        ApplyAltGaugeVisual(BlackBoard.GetAltGauge(side));
        ApplySizeMultiplier(BlackBoard.GetGaugeSizeMultiplier(side));
    }

    void OnModifierChanged(GaugeSides changedSide)
    {
        if (changedSide != side) return;
        ApplyAltGaugeVisual(BlackBoard.GetAltGauge(side));
        ApplySizeMultiplier(BlackBoard.GetGaugeSizeMultiplier(side));
    }

    void ApplySizeMultiplier(float multiplier)
    {
        transform.localScale = baseScale * multiplier;
    }

    void ApplyAltGaugeVisual(bool alt)
    {
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

    IEnumerator BreakGraceCoroutine;
    [SerializeField]float breakGrace = 0.5f;
    void OnUpdateGauge(float value, GaugeSides btnSide)
    {
        if (isbroken) return;
        if (side != btnSide)
            return;
        isHolding = true;

        gaugeValue = value;

        animator.SetFloat(AnimatorGaugeAmountString, Mathf.Clamp(value, 0, MaxGaugeAmount));

        // --- AGENT: start overheat-grace 1x als gauge vol is (breakPending voorkomt spam) ---
        if (value >= MaxGaugeAmount - 0.01f && !breakPending)
        {
            BreakGraceCoroutine = WaitForSecondsToBreak(breakGrace);
            StartCoroutine(BreakGraceCoroutine);
        }
    }

    void BreakGauge()
    {
        if (isbroken) return;
        isbroken = true;
        isHolding = false;
        animator.SetBool("GaugeIsBroken", true);
        BlackBoard.SetGaugeIsBroken(side, isbroken);
        StartCoroutine(WaitForSecondsToResetGauge(BrokenTime));
    }

    void UpdateBlackBoard(GaugeSides btnSide)
    {
        if (isbroken) return;
        if (side != btnSide) return;

        // --- AGENT: cancel overheat-grace meteen bij release (niet pas na 0.5s reset) ---
        // Anders kan WaitForSecondsToBreak na 0.6s nog BreakGauge() aanroepen terwijl de speler al losliet.
        CancelBreakGrace();

        BlackBoard.SetGaugeValue(side, gaugeValue);
        EventManager.Instance.TriggerUnityEvent(BackgroundManagerEvents.UpdateMeter);

        StartCoroutine(WaitForSecondsToResetGauge(0.5f));
    }

    // --- AGENT: grace stoppen + flags resetten zodat break later weer mogelijk is ---
    void CancelBreakGrace()
    {
        isHolding = false;

        if (BreakGraceCoroutine != null)
        {
            StopCoroutine(BreakGraceCoroutine);
            BreakGraceCoroutine = null;
        }

        breakPending = false;
    }

    IEnumerator WaitForSecondsToResetGauge(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        BlackBoard.ResetGauge(side);
        animator.SetFloat(AnimatorGaugeAmountString, 0f);
        // isHolding / breakPending worden al in CancelBreakGrace() bij release gezet

        if (isbroken)
        {
            animator.SetBool("GaugeIsBroken", false);
            isbroken = false;
            BlackBoard.SetGaugeIsBroken(side, isbroken);
        }
    }

    bool breakPending = false;
    IEnumerator WaitForSecondsToBreak(float seconds)
    {
        breakPending = true; // voorkomt dat OnUpdateGauge elke frame een nieuwe grace start
        yield return new WaitForSeconds(seconds);
        breakPending = false;

        // Alleen breaken als ze na de grace-tijd nog steeds vasthouden
        if (isHolding)
            BreakGauge();
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
