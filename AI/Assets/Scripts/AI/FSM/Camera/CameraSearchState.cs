using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSearchState : State
{
    public CameraSearchState(CameraAgent owner) { this.owner = owner; stateName = "Search"; }

    public float maxTurn = 310f;
    public float minTurn = 50f;
    public float turnSpeed = 10f;
    public float startRotX = 40;

    public override void Enter()
    {
        Debug.Log("Entering Search");
        owner.lc.ChangeColour(owner.searchColour);
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

        if(owner.transform.eulerAngles.x < startRotX - 1 || owner.transform.eulerAngles.x > startRotX + 1)
        {
            float angle = Mathf.LerpAngle(owner.transform.eulerAngles.x, startRotX, 0.01f);
            Debug.Log(angle);
            owner.transform.rotation = Quaternion.Euler(angle, owner.transform.eulerAngles.y, owner.transform.eulerAngles.z);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Search");
    }
}
