using UnityEngine;

public static class BlackBoard
{
    public static float TopGaugeValue, BottomGaugeValue;
    public static float VisualMeterValue;

    public static void SetGaugeValue(GaugeSides side, float value)
    {
        Debug.Log(side);
        var normalized = Mathf.Clamp01(value / 4); //*! 4 moet je updaten naar een ander getal als de float in de animator verandert

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
}
