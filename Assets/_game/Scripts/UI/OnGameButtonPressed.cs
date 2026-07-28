using UnityEngine;
using UnityEngine.UIElements;

public class OnGameButtonPressed : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = _uiDocument.rootVisualElement;

        var btnTop = root.Q<Button>("TopButton");
        var btnBottom = root.Q<Button>("BottomButton");

        btnTop.clicked += TopButtonWasPressed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TopButtonWasPressed()
    {

    }

    void BottomButtonWasPressed()
    {

    }
}
