using System;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] float VisualProgress = 0; // current Y of the middle meter
    [SerializeField] float progressVelocity = 0.15f, smoothingTime = 0.15f;
    [SerializeField] float yMin = -5f, yMax = 5f;

    float targetProgress;

    // --- AGENT: singleton-achtige ref zodat items de tug kunnen flippen ---
    public static BackgroundManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        EventManager.Instance.AddUnityEventListener(BackgroundManagerEvents.UpdateMeter, UpdateGauge);
        EventManager.Instance.AddDelegateListener(
            BackgroundManagerEvents.KoFlip,
            (Action<GaugeSides, float>)OnKoFlip);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        targetProgress = Mathf.Clamp(targetProgress, yMin, yMax);

        VisualProgress = Mathf.SmoothDamp(VisualProgress, targetProgress, ref progressVelocity, smoothingTime);

        transform.localPosition = new Vector3(transform.localPosition.x, VisualProgress, transform.localPosition.z);

        // Andere systemen (ItemManager) kunnen lezen hoe dicht iemand bij verlies is
        BlackBoard.TugTargetProgress = targetProgress;
        BlackBoard.TugYMin = yMin;
        BlackBoard.TugYMax = yMax;
    }

    void UpdateGauge()
    {
        float delta = BlackBoard.BottomGaugeValue - BlackBoard.TopGaugeValue;
        targetProgress += delta;
        targetProgress = Mathf.Clamp(targetProgress, yMin, yMax);
    }

    // --- AGENT: KO item — plotseling naar 80% (of flipAmount) richting beneficiary ---
    // Top wint richting yMin (-), Bottom wint richting yMax (+), zie delta = Bottom - Top.
    void OnKoFlip(GaugeSides beneficiary, float flipAmount)
    {
        float amount = Mathf.Clamp01(flipAmount);
        float snapped = beneficiary == GaugeSides.Top
            ? yMin * amount   // bv. -5 * 0.8 = -4
            : yMax * amount;  // bv. +5 * 0.8 = +4

        targetProgress = snapped;
        VisualProgress = snapped; // "opeens" — niet alleen target laten smoothen
        progressVelocity = 0f;
    }
}

public static class BackgroundManagerEvents
{
    public static string UpdateMeter = "UpdateMeter";
    // --- AGENT ---
    public const string KoFlip = "KoFlip";
}
