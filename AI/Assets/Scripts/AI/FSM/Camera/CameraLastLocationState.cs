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

    // the location of the target when state was entered
    public Vector3 target;

    // a random locataion near target for the camera to rotate towards
    private Vector3 searchLocation;

    //the rotation pointing towards searchLocation
    private Quaternion lookRotation;

    //speed to rotate
    float rotationSpeed = 1.3f;

    public override void Enter()
    {
        Debug.Log("Entering LastLocation");
        owner.lc.ChangeColour(owner.lastLocationColour);

        //set new searchLocation
        searchLocation = RandomSearchLocation(target);

        //set lookRotation to match searchLocation
        Vector3 direction = (searchLocation - owner.transform.position).normalized;

        lookRotation = Quaternion.LookRotation(direction);
    }

    public override void Execute()
    {
        //rotate towards searchLocation, if reached set new searchLocation
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
        Debug.Log("Exiting LastLocation");
    }

    //set search location to random point near target
    Vector3 RandomSearchLocation(Vector3 start)
    {
        float randX = Random.Range(-3, 3);
        float randY = Random.Range(-3, 3);
        float randZ = Random.Range(-3, 3);

        return new Vector3(start.x + randX, start.y + randY, start.z + randZ);
    }
}
