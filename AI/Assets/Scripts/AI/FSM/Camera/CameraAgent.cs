using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAgent : MonoBehaviour
{
    StateManager sm = new StateManager();

    RayBundleSensor sensor;

    public Transform player;

    [Header("State Colours")]
    public Color searchColour;
    public Color followColour;
    public Color lastLocationColour;

    [HideInInspector]
    public LightController lc;

    private void Awake()
    {
        sensor = GetComponent<RayBundleSensor>();
        lc = GetComponent<LightController>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        sm.ChangeState(new CameraSearchState(this));
    }

    // Update is called once per frame
    void Update()
    {
        sm.Update();
        if(sensor.Hit && sm.currentState.stateName != "Follow")
        {
            sm.ChangeState(new CameraFollowState(this, player));
        }
        if (!sensor.Hit && sm.currentState.stateName == "Follow")
        {
            sm.ChangeState(new CameraLastLocationState(this, player));
        }
    }
}
