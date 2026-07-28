using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// this script manages the event calls from the visual buttons.
/// </summary>
public class OnGameButtonPressed : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private float maxTime = 5f;

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

        var routine = WaitForSeconds(maxTime, btn);
        if (btn == btnTop)
            TopButtonCoroutine = routine;
        else
            BottomButtonCoroutine = routine;

        StartCoroutine(routine);
    }

    void OnButtonReleased(PointerUpEvent e)
    {
        var btn = e.target as Button;
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

        StopCoroutine(routine);
    }


    IEnumerator WaitForSeconds(float seconds, Button btn)
    {
        float elapsedTime = 0f;
        Debug.Log("button name: " + btn.name);

        while (elapsedTime < seconds)
        {
            elapsedTime += Time.deltaTime;
            // Debug.Log("clamped time: " + Mathf.FloorToInt(elapsedTime) + " seconds");
            EventManager.Instance.TriggerDelegate("GaugeTime", Mathf.FloorToInt(elapsedTime), btn); //update the guages.
            yield return null;
        }
    }


}
