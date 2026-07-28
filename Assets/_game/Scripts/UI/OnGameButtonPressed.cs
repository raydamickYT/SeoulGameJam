using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// this script manages the event calls from the visual buttons.
/// </summary>
public class OnGameButtonPressed : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private float maxTime = 5f, maxTimeRandomVariation = 0.5f;

    private Button btnTop, btnBottom;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        var root = _uiDocument.rootVisualElement;

        btnTop = root.Q<Button>("TopButton");
        btnBottom = root.Q<Button>("BottomButton");

        btnTop.RegisterCallback<PointerDownEvent>(OnButtonPressed, TrickleDown.TrickleDown);
        btnTop.RegisterCallback<PointerUpEvent>(OnButtonReleased);

        btnBottom.RegisterCallback<PointerDownEvent>(OnButtonPressed, TrickleDown.TrickleDown);
        btnBottom.RegisterCallback<PointerUpEvent>(OnButtonReleased);
    }

    void OnDisable()
    {
        btnTop.UnregisterCallback<PointerDownEvent>(OnButtonPressed, TrickleDown.TrickleDown);
        btnTop.UnregisterCallback<PointerUpEvent>(OnButtonReleased);

        btnBottom.UnregisterCallback<PointerDownEvent>(OnButtonPressed, TrickleDown.TrickleDown);
        btnBottom.UnregisterCallback<PointerUpEvent>(OnButtonReleased);
    }

    IEnumerator TopButtonCoroutine;
    IEnumerator BottomButtonCoroutine;
    void OnButtonPressed(PointerDownEvent e)
    {
        var btn = e.target as Button;
        GaugeSides side = btn == btnTop ? GaugeSides.Top : GaugeSides.Bottom;

        if (BlackBoard.IsBroken(side)) return;

        var routine = WaitForSeconds(btn);
        if (btn == btnTop)
            TopButtonCoroutine = routine;
        else
            BottomButtonCoroutine = routine;

        StartCoroutine(routine);
    }

    void OnButtonReleased(PointerUpEvent e)
    {
        var btn = e.target as Button;
        GaugeSides side = btn == btnTop ? GaugeSides.Top : GaugeSides.Bottom;

        IEnumerator routine = null;
        if (btn == btnTop)
        {
            routine = TopButtonCoroutine;
            TopButtonCoroutine = null;
        }
        else
        {
            routine = BottomButtonCoroutine;
            BottomButtonCoroutine = null;
        }

        EventManager.Instance.TriggerDelegate(ButtonEvents.OnButtonPointeUp, side);
        if (routine != null)
            StopCoroutine(routine);
    }

    //** Count seconds pressed
    IEnumerator WaitForSeconds(Button btn)
    {
        GaugeSides side = btn == btnTop ? GaugeSides.Top : GaugeSides.Bottom;
        float holdTime = 0f;
        float mult = BlackBoard.GetHoldMultiplier(side);
        float randMaxHoldTime = UnityEngine.Random.Range(maxTime - 0.5f, maxTime + 0.5f) * mult;
        float maxGauge = 31f;
        while (holdTime < randMaxHoldTime)
        {
            if (BlackBoard.IsBroken(side)) yield break;

            holdTime += Time.deltaTime;
            float value = (holdTime / randMaxHoldTime) * maxGauge;

            EventManager.Instance.TriggerDelegate(GaugeEvents.UpdateGauge, value, side);
            yield return null;
        }
    }
}

public static class ButtonEvents
{
    public const string OnButtonPointeUp = "OnButtonPointerup";
}