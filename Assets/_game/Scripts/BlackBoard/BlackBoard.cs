using UnityEngine;

public static class BlackBoard
{
    public static float TopGaugeValue, BottomGaugeValue;
    public static float VisualMeterValue;
    public static bool TopGaugeIsBroken, BottomGaugeIsBroken;

    public static void SetGaugeValue(GaugeSides side, float value)
    {
        // Debug.Log(side);
        var normalized = Mathf.Clamp01(value / 31); //*! 4 moet je updaten naar een ander getal als de float in de animator verandert

        if (side == GaugeSides.Top)
            TopGaugeValue = normalized;
        if (side == GaugeSides.Bottom)
            BottomGaugeValue = normalized;

    }

    public static void ResetGauge(GaugeSides side)
    {
        if (side == GaugeSides.Top)
            TopGaugeValue = 0;
        if (side == GaugeSides.Bottom)
            BottomGaugeValue = 0;
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
}
