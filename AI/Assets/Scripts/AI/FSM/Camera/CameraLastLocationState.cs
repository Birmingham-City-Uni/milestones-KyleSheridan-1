using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLastLocationState : State
{
    public CameraLastLocationState(CameraAgent owner, Transform target)
    {
        this.owner = owner;
        this.target = target.position;
        stateName = "LastLocation";
    }

    public Vector3 target;

    private Vector3 searchLocation;

    private Quaternion lookRotation;

    float rotationSpeed = 1.3f;

    public override void Enter()
    {
        Debug.Log("Entering Follow");
        owner.lc.ChangeColour(owner.lastLocationColour);

        searchLocation = RandomSearchLocation(target);

        Vector3 direction = (searchLocation - owner.transform.position).normalized;

        lookRotation = Quaternion.LookRotation(direction);
    }

    public override void Execute()
    {
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        if (owner.transform.rotation == lookRotation)
        {
            searchLocation = RandomSearchLocation(target);

            Vector3 direction = (searchLocation - owner.transform.position).normalized;

            lookRotation = Quaternion.LookRotation(direction);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Follow");
    }

    Vector3 RandomSearchLocation(Vector3 start)
    {
        float randX = Random.Range(-3, 3);
        float randY = Random.Range(-3, 3);
        float randZ = Random.Range(-3, 3);

        return new Vector3(start.x + randX, start.y + randY, start.z + randZ);
    }
}
