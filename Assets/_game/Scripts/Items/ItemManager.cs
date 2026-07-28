using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns a random item for a random player side after a random delay.
/// Only one item on screen at a time.
/// KO items only spawn for a side that is close to losing.
/// </summary>
public class ItemManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject[] itemPrefabs;
    [Tooltip("Optional. Only spawns when someone is close to losing, for that underdog side.")]
    [SerializeField] GameObject koItemPrefab;

    [Header("Timing")]
    [Tooltip("In Seconds")]
    [SerializeField] float minSpawnDelay = 4f;
    [Tooltip("In Seconds")]
    [SerializeField] float maxSpawnDelay = 10f;

    [Header("KO spawn rules")]
    [Tooltip("How close to the win/lose edge (0-1). 0.75 = 75% toward a win = other side almost lost.")]
    [SerializeField, Range(0.5f, 0.95f)] float nearLossThreshold = 0.75f;
    [Tooltip("Chance to pick KO instead of a normal item when near-loss is true.")]
    [SerializeField, Range(0f, 1f)] float koSpawnChance = 0.5f;
    // --- AGENT: chaos / clutch KO targeting ---
    [Tooltip("If KO is for the underdog: chance it visually spawns on the winner's side (fake-out).")]
    [SerializeField, Range(0f, 1f)] float koFakeOutSpawnChance = 0.35f;
    [Tooltip("Chance the KO is actually meant for the winning side (then it ALWAYS spawns on their side).")]
    [SerializeField, Range(0f, 1f)] float koMeantForWinnerChance = 0.10f;

    [Header("Spawn positions (world)")]
    [SerializeField] Vector2 topSpawnCenter = new Vector2(0f, 3f);
    [SerializeField] Vector2 bottomSpawnCenter = new Vector2(0f, -3f);
    [SerializeField] Vector2 spawnJitter = new Vector2(1.5f, 0.4f);

    GameObject currentItem;
    Coroutine debuffRoutine;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(wait);

            if (currentItem != null)
                continue;

            SpawnRandomItem();
        }
    }

    void SpawnRandomItem()
    {
        // --- AGENT: KO alleen bij near-loss + koSpawnChance; daarna underdog/winner + spawn-side chaos ---
        if (koItemPrefab != null
            && TryGetNearLossSide(out GaugeSides underdogSide)
            && Random.value <= koSpawnChance)
        {
            SpawnKoItem(underdogSide);
            return;
        }

        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("ItemManager: no item prefabs assigned.");
            return;
        }

        GaugeSides side = Random.value < 0.5f ? GaugeSides.Top : GaugeSides.Bottom;
        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        SpawnItem(prefab, intendedSide: side, spawnSide: side);
    }

    // --- AGENT: KO beneficiary + waar hij visueel ligt ---
    void SpawnKoItem(GaugeSides underdogSide)
    {
        GaugeSides winnerSide = Opposite(underdogSide);

        // 10% (default): KO is ECHT voor de winnaar → spawn ALTIJD aan winnaars kant
        bool meantForWinner = Random.value <= koMeantForWinnerChance;
        GaugeSides intendedSide = meantForWinner ? winnerSide : underdogSide;

        GaugeSides spawnSide;
        if (meantForWinner)
        {
            spawnSide = winnerSide;
        }
        else
        {
            // Underdog-KO: soms fake-out op de kant van de winnaar (chaos)
            bool fakeOut = Random.value <= koFakeOutSpawnChance;
            spawnSide = fakeOut ? winnerSide : underdogSide;
        }

        SpawnItem(koItemPrefab, intendedSide, spawnSide);
    }

    static GaugeSides Opposite(GaugeSides side)
    {
        return side == GaugeSides.Top ? GaugeSides.Bottom : GaugeSides.Top;
    }

    // --- AGENT: Top verliest bijna als tug richting +yMax zit; Bottom bijna als richting yMin ---
    bool TryGetNearLossSide(out GaugeSides underdog)
    {
        float p = BlackBoard.TugTargetProgress;
        float topLossLine = BlackBoard.TugYMax * nearLossThreshold;
        float bottomLossLine = BlackBoard.TugYMin * nearLossThreshold;

        if (p >= topLossLine)
        {
            underdog = GaugeSides.Top;
            return true;
        }

        if (p <= bottomLossLine)
        {
            underdog = GaugeSides.Bottom;
            return true;
        }

        underdog = GaugeSides.Top;
        return false;
    }

    void SpawnItem(GameObject prefab, GaugeSides intendedSide, GaugeSides spawnSide)
    {
        Vector3 pos = GetSpawnPosition(spawnSide);
        currentItem = Instantiate(prefab, pos, Quaternion.identity);

        if (!currentItem.TryGetComponent<PickupItem>(out var pickup))
        {
            Debug.LogError($"Item prefab '{prefab.name}' needs a PickupItem component.");
            Destroy(currentItem);
            currentItem = null;
            return;
        }

        // intendedSide = voor wie het effect is; spawnSide = alleen waar hij ligt
        pickup.Init(intendedSide, this);
    }

    Vector3 GetSpawnPosition(GaugeSides side)
    {
        Vector2 center = side == GaugeSides.Top ? topSpawnCenter : bottomSpawnCenter;
        float x = center.x + Random.Range(-spawnJitter.x, spawnJitter.x);
        float y = center.y + Random.Range(-spawnJitter.y, spawnJitter.y);
        return new Vector3(x, y, 0f);
    }

    public void NotifyItemCollected(PickupItem item)
    {
        currentItem = null;
    }

    /// <summary>
    /// Debuff the victim (usually the opponent of the item's intended side).
    /// </summary>
    public void ApplyLongGaugeDebuff(GaugeSides victim, float holdMultiplier, float durationSeconds, float sizeMultiplier, bool altGauge)
    {
        if (debuffRoutine != null)
            StopCoroutine(debuffRoutine);

        BlackBoard.SetHoldMultiplier(victim, holdMultiplier);
        BlackBoard.SetGaugeSizeMultiplier(victim, sizeMultiplier);
        BlackBoard.SetAltGauge(victim, altGauge);
        EventManager.Instance.TriggerDelegate("ItemModifierChanged", victim);

        debuffRoutine = StartCoroutine(ClearDebuffAfter(victim, durationSeconds));
    }

    // --- AGENT: KO flip via BackgroundManager event ---
    public void ApplyKoFlip(GaugeSides beneficiary, float flipAmount)
    {
        EventManager.Instance.TriggerDelegate(BackgroundManagerEvents.KoFlip, beneficiary, flipAmount);
    }

    IEnumerator ClearDebuffAfter(GaugeSides victim, float duration)
    {
        yield return new WaitForSeconds(duration);

        BlackBoard.SetHoldMultiplier(victim, 1f);
        BlackBoard.SetGaugeSizeMultiplier(victim, 1f);
        BlackBoard.SetAltGauge(victim, false);
        EventManager.Instance.TriggerDelegate("ItemModifierChanged", victim);
        debuffRoutine = null;
    }
}

public static class ItemEvents
{
    public const string ModifierChanged = "ItemModifierChanged";
}
