using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will update the gauge's visual state according to how long the player has been holding the button.
/// Each gauge will have it's own manager
/// </summary>
public class GaugeManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public GaugeSides side = GaugeSides.Top;
    public float MaxGaugeAmount = 31;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //*onbutton pressed
        EventManager.Instance.AddDelegateListener(GaugeEvents.UpdateGauge, (Action<float, GaugeSides>)OnUpdateGauge);

        //*onbutton released
        EventManager.Instance.AddDelegateListener(ButtonEvents.OnButtonPointeUp, (Action<GaugeSides>)UpdateBlackBoard);
    }

    string AnimatorGaugeAmountString = "GaugeAmount";
    float gaugeValue;
    void OnUpdateGauge(float value, GaugeSides btnSide)
    {
        if (side != btnSide)
            return;

        gaugeValue = value;

        animator.SetFloat(AnimatorGaugeAmountString, Mathf.Clamp(value, 0, MaxGaugeAmount)); //make sure the value doesnt exceed the max amount by more than 0.1f
    }
    void UpdateBlackBoard(GaugeSides btnSide)
    {
        if (side != btnSide) return;

        BlackBoard.SetGaugeValue(side, gaugeValue); //*set gauge value in blackboard 
        EventManager.Instance.TriggerUnityEvent(BackgroundManagerEvents.UpdateMeter); //*? deze functie wordt gecalled wnr je de knop released dus hier zou het moeten werken

        StartCoroutine(WaitForSeconds(0.5f)); //reset gauge
    }

    //delay de reset met een kleine tijdA
    IEnumerator WaitForSeconds(float seconds) //*reset gauges
    {
        yield return new WaitForSeconds(seconds);

        BlackBoard.ResetGauge(side);
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