using UnityEngine;

/// <summary>
/// This class's sole purpose is to make sure there are no more references in the event manager
/// </summary>
public class EvenManagerReset : MonoBehaviour
{
    void OnDestroy()
    {
        EventManager.Instance.RemoveAllListeners();
    }
}
