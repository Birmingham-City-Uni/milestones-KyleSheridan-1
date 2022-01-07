using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSearchState : State
{
    //constructor
    public CameraSearchState(CameraAgent owner) { this.owner = owner; stateName = "Search"; }

    //min / max angles for the camera to turn (local space)
    public float maxTurn = 310f;
    public float minTurn = 50f;

    //speed for the camera to turn
    public float turnSpeed = 10f;

    //start rotation of the camera on the x axis
    public float startRotX = 40;

    public override void Enter()
    {
        Debug.Log("Entering Search");
        owner.lc.ChangeColour(owner.searchColour);
    }

    public override void Execute()
    {
        //if camera reaches min / max angle then change rotation direction
        if(owner.transform.localEulerAngles.y >= minTurn && owner.transform.localEulerAngles.y < maxTurn - 10 && turnSpeed > 0)
        {
            turnSpeed *= -1;
        }
        else if(owner.transform.localEulerAngles.y <= maxTurn && owner.transform.localEulerAngles.y > minTurn + 10 && turnSpeed < 0)
        {
            turnSpeed *= -1;
        }

        // if the camera rotation is out of normal range on y axis, lerp to original value. else rotate normally
        if (owner.transform.localEulerAngles.y > minTurn + 10 && owner.transform.localEulerAngles.y < maxTurn - 10)
        {
            float angle = Mathf.LerpAngle(owner.transform.localEulerAngles.y, 0, 0.01f);
            owner.transform.rotation = Quaternion.Euler(owner.transform.localEulerAngles.x, angle, owner.transform.localEulerAngles.z);
        }
        else
        {
            owner.transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f, Space.World);
        }

        // if camera rotation is not at the normal value on y axis, lerp to the original value
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
