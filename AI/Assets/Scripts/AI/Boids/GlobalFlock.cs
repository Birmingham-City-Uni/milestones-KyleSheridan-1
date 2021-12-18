using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject fishPrefab;
    public Vector3 volumeSize;
    public int numFish = 50;
    public GameObject[] fish;

    public Vector3 goalPos { get; private set; }

    [HideInInspector]
    public float xboundMax, yboundMax, zboundMax;
    [HideInInspector]
    public float xboundMin, yboundMin, zboundMin;

    // Start is called before the first frame update
    void Start()
    {
        fish = new GameObject[numFish];
        goalPos = NewGoalPosition();

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
        //IMPROVE THIS
        if(Random.Range(0, 100) < 0.1)
        {
            goalPos = NewGoalPosition();
        }
    }

    Vector3 NewGoalPosition()
    {
        return new Vector3(Random.Range((-volumeSize.x / 2) + transform.position.x, (volumeSize.x / 2) + transform.position.x),
                              Random.Range((-volumeSize.y / 2) + transform.position.y, (volumeSize.y / 2) + transform.position.y),
                              Random.Range((-volumeSize.z / 2) + transform.position.z, (volumeSize.z / 2) + transform.position.z));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, volumeSize);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(goalPos, 0.2f);
    }
}
