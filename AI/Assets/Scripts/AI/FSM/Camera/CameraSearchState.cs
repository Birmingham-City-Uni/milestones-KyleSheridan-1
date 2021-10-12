using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSearchState : State
{
    public float maxTurn = 310f;
    public float minTurn = 50f;
    public float turnSpeed = 10f;

    CameraAgent owner;
    public CameraSearchState(CameraAgent owner) { this.owner = owner; }

    public override void Enter()
    {
        Debug.Log("Entering Search");
    }

    public override void Execute()
    {
        //Debug.Log("Executing Search");
        if(owner.transform.eulerAngles.y >= minTurn && owner.transform.eulerAngles.y < maxTurn - 10 && turnSpeed > 0)
        {
            turnSpeed *= -1;
        }
        else if(owner.transform.eulerAngles.y <= maxTurn && owner.transform.eulerAngles.y > minTurn + 10 && turnSpeed < 0)
        {
            turnSpeed *= -1;
        }

        owner.transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f, Space.World);
    }

    public override void Exit()
    {
        Debug.Log("Exiting Search");
    }
}
