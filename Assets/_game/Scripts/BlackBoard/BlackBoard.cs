using UnityEngine;

public static class BlackBoard
{
    public static float TopGaugeValue, BottomGaugeValue;
    public static float VisualMeterValue;
    public static bool TopGaugeIsBroken, BottomGaugeIsBroken;

    public static float TopHoldMultiplier = 1f;
    public static float BottomHoldMultiplier = 1f;
    public static float TopGaugeSizeMultiplier = 1f;
    public static float BottomGaugeSizeMultiplier = 1f;
    public static bool TopAltGauge, BottomAltGauge;

    public static void SetGaugeValue(GaugeSides side, float value)
    {
        var normalized = Mathf.Clamp01(value / 31f);

        if (side == GaugeSides.Top)
            TopGaugeValue = normalized;
        if (side == GaugeSides.Bottom)
            BottomGaugeValue = normalized;
    }

    public static void ResetGauge(GaugeSides side)
    {
        if (side == GaugeSides.Top)
            TopGaugeValue = 0f;
        if (side == GaugeSides.Bottom)
            BottomGaugeValue = 0f;
    }

    public static void SetGaugeIsBroken(GaugeSides side, bool isBroken)
    {
        if (side == GaugeSides.Top)
            TopGaugeIsBroken = isBroken;
        if (side == GaugeSides.Bottom)
            BottomGaugeIsBroken = isBroken;
    }

    public static bool IsBroken(GaugeSides side)
    {
        if (side == GaugeSides.Top)
            return TopGaugeIsBroken;
        if (side == GaugeSides.Bottom)
            return BottomGaugeIsBroken;

        return false;
    }

    public static void SetHoldMultiplier(GaugeSides side, float multiplier)
    {
        if (side == GaugeSides.Top)
            TopHoldMultiplier = multiplier;
        else
            BottomHoldMultiplier = multiplier;
    }

    public static float GetHoldMultiplier(GaugeSides side)
    {
        return side == GaugeSides.Top ? TopHoldMultiplier : BottomHoldMultiplier;
    }

    public static void SetAltGauge(GaugeSides side, bool enabled)
    {
        if (side == GaugeSides.Top)
            TopAltGauge = enabled;
        else
            BottomAltGauge = enabled;
    }

    public static bool GetAltGauge(GaugeSides side)
    {
        return side == GaugeSides.Top ? TopAltGauge : BottomAltGauge;
    }

    public static void SetGaugeSizeMultiplier(GaugeSides side, float multiplier)
    {
        if (side == GaugeSides.Top)
            TopGaugeSizeMultiplier = multiplier;
        else
            BottomGaugeSizeMultiplier = multiplier;
    }

    public static float GetGaugeSizeMultiplier(GaugeSides side)
    {
        return side == GaugeSides.Top ? TopGaugeSizeMultiplier : BottomGaugeSizeMultiplier;
    }
}
