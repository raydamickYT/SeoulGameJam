using System;
using System.Data;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] float VisualProgress = 0; //this is the current position of the middle meter. -1 is down, 1 is up
    [SerializeField] float progressVelocity = 0.15f, smoothingTime = 0.15f;
    private float targetProgress = 0, yMin = -5, yMax = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        EventManager.Instance.AddUnityEventListener(BackgroundManagerEvents.UpdateMeter, UpdateGauge);
    }

    // Update is called once per frame
    void Update()
    {
        targetProgress = Mathf.Clamp(targetProgress, yMin, yMax); //we moeten zeker weten dat hij niet meer of minder wordt dan de min/max waarden

        VisualProgress = Mathf.SmoothDamp(VisualProgress, targetProgress, ref progressVelocity, smoothingTime); //optioneel: 5e parameter; maxSpeed

        transform.localPosition = new Vector3(transform.localPosition.x, VisualProgress, transform.localPosition.z);
    }

    void UpdateGauge()
    {
        //Als top 0 is, maar bottom -1. dan wint -1. en andersom. beide zullen vrijwel nooit tegelijk 1 en -1 zijn. en zo wel dan cancelen ze
        float delta = BlackBoard.BottomGaugeValue - BlackBoard.TopGaugeValue;
        Debug.Log(BlackBoard.BottomGaugeValue + " " + BlackBoard.TopGaugeValue);
        
        targetProgress += delta;
        targetProgress = Mathf.Clamp(targetProgress, yMin, yMax);
    }
}

public static class BackgroundManagerEvents
{
    public static string UpdateMeter = "UpdateMeter";
}
