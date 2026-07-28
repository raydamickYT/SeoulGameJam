using UnityEngine;

/// <summary>
/// Comeback item: only meant for a side that is close to losing.
/// On pickup, snaps the tug meter to 80% toward the beneficiary's win side.
/// </summary>
public class KOItem : PickupItem
{
    // public ItemManager manager2;
    [SerializeField, Range(0.1f, 1f)]
    float flipAmount = 0.8f; // 80% naar de winnende kant van intendedSide

    //*! for debugging
    // void Awake()
    // {
    //     if (manager2 != null)
    //         manager = manager2;
    // }

    protected override void OnCollected()
    {
        // intendedSide = de speler voor wie dit item gespawnd is (de underdog)
        manager.ApplyKoFlip(intendedSide, flipAmount);
    }
}
