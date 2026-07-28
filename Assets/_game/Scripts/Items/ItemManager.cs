using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns a random item for a random player side after a random delay.
/// Only one item on screen at a time.
/// </summary>
public class ItemManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject[] itemPrefabs;

    [Header("Timing")]
    [Tooltip("In Seconds")]
    [SerializeField] float minSpawnDelay = 4f;
    [Tooltip("In Seconds")]
    [SerializeField] float maxSpawnDelay = 10f;

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
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("ItemManager: no item prefabs assigned.");
            return;
        }

        GaugeSides side = Random.value < 0.5f ? GaugeSides.Top : GaugeSides.Bottom;
        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        Vector3 pos = GetSpawnPosition(side);

        currentItem = Instantiate(prefab, pos, Quaternion.identity);

        if (!currentItem.TryGetComponent<PickupItem>(out var pickup))
        {
            Debug.LogError($"Item prefab '{prefab.name}' needs a PickupItem component.");
            Destroy(currentItem);
            currentItem = null;
            return;
        }

        pickup.Init(side, this);
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
    public void ApplyHoldDebuff(GaugeSides victim, float holdMultiplier, float durationSeconds, float sizeMultiplier, bool altGauge)
    {
        if (debuffRoutine != null)
            StopCoroutine(debuffRoutine);

        BlackBoard.SetHoldMultiplier(victim, holdMultiplier);
        BlackBoard.SetGaugeSizeMultiplier(victim, sizeMultiplier);
        BlackBoard.SetAltGauge(victim, altGauge);
        EventManager.Instance.TriggerDelegate("ItemModifierChanged", victim);

        debuffRoutine = StartCoroutine(ClearDebuffAfter(victim, durationSeconds));
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
