using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CameraAgent : MonoBehaviour
{
    //state manager for the agent
    StateManager sm = new StateManager();

    //reference to the sensor used to detect player
    RayBundleSensor sensor;

    AudioSource alarmSound;

    //reference to player
    public Transform player;

    //buffer time between states
    public float bufferTime = 0.5f;
    //length of time of LastLocation state
    public float searchTime = 10f;
    //the max distance to alert drones
    public float droneAlertDist = 30f;
    //amount of time between drones being notified
    public float refreshTime = 2f;

    //colours of the camera light to represent state
    [Header("State Colours")]
    public Color searchColour;
    public Color followColour;
    public Color lastLocationColour;

    //script to control light
    [HideInInspector]
    public LightController lc;

    //list of all drone agents in the scene
    [HideInInspector]
    public List<DroneAgent> drones;

    //last postion of target alerted to drones (for gizmos)
    private Vector3 lastPos;

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
        alarmSound = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //start on search state
        sm.ChangeState(new CameraSearchState(this));

        //find all drones
        GameObject[] droneObjs = GameObject.FindGameObjectsWithTag("Drone");

        foreach(GameObject drone in droneObjs)
        {
            drones.Add(drone.GetComponent<DroneAgent>());
        }
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
                sm.ChangeState(new CameraFollowState(this, player, alarmSound));
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

    //alerts nearby drones every (refreshTime) seconds
    private IEnumerator AlertDrones()
    {
        while (true)
        {
            foreach (DroneAgent drone in drones)
            {
                if (Vector3.Distance(transform.position, drone.transform.position) <= droneAlertDist)
                {
                    drone.SetLastLocationState();
                }
            }
            lastPos = player.position;
            yield return new WaitForSeconds(refreshTime);
        }
    }

    public void StartAlert()
    {
        StartCoroutine("AlertDrones");
    }

    public void StopAlert()
    {
        StopCoroutine("AlertDrones");
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if(sm.currentState.stateName == "Follow")
            {
                Gizmos.color = Color.magenta;

                Gizmos.DrawWireSphere(lastPos, 2f);
            }
        }
    }
}
