using UnityEngine;

/// <summary>
/// Long item: benefits intended side by debuffing the opponent
/// (hold time x4 + alt gauge look).
/// </summary>
public class LongItem : PickupItem
{
    [SerializeField] float holdMultiplier = 4f;
    [SerializeField] float durationSeconds = 8f;
    [SerializeField] float SizeMultiplier = 2;

    protected override void OnCollected()
    {
        GaugeSides victim = Opposite(intendedSide);
        manager.ApplyHoldDebuff(victim, holdMultiplier, durationSeconds, SizeMultiplier, altGauge: true);
    }
}
