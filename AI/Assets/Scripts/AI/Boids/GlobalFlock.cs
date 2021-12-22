using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject fishPrefab;

    [Header("Flock Settings")]
    public Vector3 volumeSize;
    public int numFish = 50;

    [Tooltip("The amount of time before the goal pos changes (in seconds)")]
    [Range(1, 30)]
    public float goalPosWaitTime = 3f;
    [Tooltip("Adds a deviation to the wait time before and after (in seconds)")]
    public float goalWaitTimeDeviation = 1f;

    [HideInInspector]
    public GameObject[] fish;

    public Vector3 goalPos { get; private set; }

    [HideInInspector]
    public float xboundMax, yboundMax, zboundMax;
    [HideInInspector]
    public float xboundMin, yboundMin, zboundMin;

    float currentWaitTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        fish = new GameObject[numFish];
        goalPos = NewGoalPosition();

        currentWaitTime = NewWaitTime(goalPosWaitTime, goalWaitTimeDeviation);

        xboundMax = transform.position.x + volumeSize.x / 2;
        yboundMax = transform.position.y + volumeSize.y / 2;
        zboundMax = transform.position.z + volumeSize.z / 2;

        xboundMin = transform.position.x - volumeSize.x / 2;
        yboundMin = transform.position.y - volumeSize.y / 2;
        zboundMin = transform.position.z - volumeSize.z / 2;

        for (int i = 0; i < numFish; i++)
        {
            Vector3 pos = transform.TransformPoint(Random.Range(-volumeSize.x / 2, volumeSize.x / 2),
                                                   Random.Range(-volumeSize.y / 2, volumeSize.y / 2),
                                                   Random.Range(-volumeSize.z / 2, volumeSize.z / 2));

            fish[i] = (GameObject)Instantiate(fishPrefab, pos, Quaternion.LookRotation(new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1))));
            FlockEntity fe = fish[i].GetComponent<FlockEntity>();
            fe.flock = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWaitTime <= 0)
        {
            goalPos = NewGoalPosition();
            currentWaitTime = NewWaitTime(goalPosWaitTime, goalWaitTimeDeviation);

            Debug.Log(currentWaitTime);
        }

        currentWaitTime -= Time.deltaTime;
    }

    Vector3 NewGoalPosition()
    {
        return new Vector3(Random.Range((-volumeSize.x / 2) + transform.position.x, (volumeSize.x / 2) + transform.position.x),
                              Random.Range((-volumeSize.y / 2) + transform.position.y, (volumeSize.y / 2) + transform.position.y),
                              Random.Range((-volumeSize.z / 2) + transform.position.z, (volumeSize.z / 2) + transform.position.z));
    }

    float NewWaitTime(float waitTime, float deviation)
    {
        float minWait = waitTime - deviation;
        float maxWait = waitTime + deviation;

        minWait = Mathf.Clamp(minWait, 0.1f, waitTime);

        return Random.Range(minWait, maxWait);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, volumeSize);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(goalPos, 0.2f);
    }
}
