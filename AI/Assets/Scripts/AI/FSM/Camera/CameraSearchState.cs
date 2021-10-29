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
        if(owner.transform.localEulerAngles.y >= minTurn && owner.transform.localEulerAngles.y < maxTurn - 10 && turnSpeed > 0)
        {
            turnSpeed *= -1;
        }
        else if(owner.transform.localEulerAngles.y <= maxTurn && owner.transform.localEulerAngles.y > minTurn + 10 && turnSpeed < 0)
        {
            turnSpeed *= -1;
        }

        if (owner.transform.localEulerAngles.y > minTurn + 10 && owner.transform.localEulerAngles.y < maxTurn - 10)
        {
            float angle = Mathf.LerpAngle(owner.transform.localEulerAngles.y, 0, 0.01f);
            owner.transform.rotation = Quaternion.Euler(owner.transform.localEulerAngles.x, angle, owner.transform.localEulerAngles.z);
        }
        else
        {
            owner.transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f, Space.World);
        }

        //owner.transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f, Space.World);

        if (owner.transform.localEulerAngles.x < startRotX - 1 || owner.transform.localEulerAngles.x > startRotX + 1)
        {
            float angle = Mathf.LerpAngle(owner.transform.localEulerAngles.x, startRotX, 0.01f);
            owner.transform.rotation = Quaternion.Euler(angle, owner.transform.localEulerAngles.y, owner.transform.localEulerAngles.z);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Search");
    }
}
