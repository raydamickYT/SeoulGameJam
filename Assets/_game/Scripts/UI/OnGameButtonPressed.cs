using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// this script manages the event calls from the visual buttons.
/// </summary>
public class OnGameButtonPressed : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = _uiDocument.rootVisualElement;

        var btnTop = root.Q<Button>("TopButton");
        var btnBottom = root.Q<Button>("BottomButton");

        btnTop.clicked += OnTopButtonPressed;
        btnBottom.clicked += OnBottomButtonPressed;
    }

    void OnTopButtonPressed()
    {
        Debug.Log("Testing the top button");
    }

    void OnBottomButtonPressed()
    {
        Debug.Log("Testing the Bottom button");
    }
}
