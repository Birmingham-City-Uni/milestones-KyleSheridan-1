using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAgent : MonoBehaviour
{
    StateManager sm = new StateManager();

    SphereCastSensor sensor;

    public Transform player;

    [Header("State Colours")]
    public Color searchColour;
    public Color followColour;

    [HideInInspector]
    public LightController lc;

    private void Awake()
    {
        sensor = GetComponent<SphereCastSensor>();
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
        if (!sensor.Hit && sm.currentState.stateName != "Search")
        {
            sm.ChangeState(new CameraSearchState(this));
        }
    }
}
