using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAgent : MonoBehaviour
{
    //state manager for the agent
    StateManager sm = new StateManager();

    //reference to the sensor used to detect player
    RayBundleSensor sensor;

    //reference to player
    public Transform player;

    //buffer time between states
    public float bufferTime = 0.5f;
    //length of time of LastLocation state
    public float searchTime = 10f;

    //colours of the camera light to represent state
    [Header("State Colours")]
    public Color searchColour;
    public Color followColour;
    public Color lastLocationColour;

    //script to control light
    [HideInInspector]
    public LightController lc;

    //for buffer between states
    private bool canChange = true;
    //to check if state can change to search
    private bool lostPlayer = false;

    //used to reset the wait time for LastLocation state
    private Coroutine searchCoroutine;

    private void Awake()
    {
        //set references
        sensor = GetComponent<RayBundleSensor>();
        lc = GetComponent<LightController>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        //start on search state
        sm.ChangeState(new CameraSearchState(this));
    }

    // Update is called once per frame
    void Update()
    {
        sm.Update();

        if (canChange)
        {
            //if sensor detects player change to follow state
            if(sensor.Hit && sm.currentState.stateName != "Follow")
            {
                //end timer for lastLocation state (if needed)
                if(searchCoroutine != null)
                {
                    StopCoroutine(searchCoroutine);
                }
                sm.ChangeState(new CameraFollowState(this, player));
                StartCoroutine(Buffer(bufferTime));
            }
            //if in follow state and sensor does not detect player, change to LastLocation state
            if (!sensor.Hit && sm.currentState.stateName == "Follow")
            {
                sm.ChangeState(new CameraLastLocationState(this, player));
                StartCoroutine(Buffer(bufferTime));
                searchCoroutine = StartCoroutine(ChangeToSearch(searchTime));
            }
            //lostPlayer will only be true if in LastLocation state for searchTime
            if (lostPlayer)
            {
                lostPlayer = false;
                sm.ChangeState(new CameraSearchState(this));
                StartCoroutine(Buffer(bufferTime));
            }
        }
    }

    //Buffer between states
    IEnumerator Buffer(float waitTime)
    {
        canChange = false;

        yield return new WaitForSeconds(waitTime);

        canChange = true;
    }

    //change to search state after some time (coroutine will be stopped if state changes during)
    IEnumerator ChangeToSearch(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        lostPlayer = true;
    }
}
