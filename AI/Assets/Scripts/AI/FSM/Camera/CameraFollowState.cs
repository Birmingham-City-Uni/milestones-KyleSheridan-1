using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CameraFollowState : State
{
    public CameraFollowState(CameraAgent owner, Transform target, AudioSource audio) 
    {
        this.owner = owner;
        this.target = target;
        stateName = "Follow";
        alarmSound = audio;
    }

    //reference to target for camera to follow
    public Transform target;

    AudioSource alarmSound;

    //speed that camera rotates
    float rotationSpeed = 1.3f;

    public override void Enter()
    {
        Debug.Log("Entering Follow");
        owner.lc.ChangeColour(owner.followColour);
        owner.StartAlert();
        alarmSound.Play();
    }

    public override void Execute()
    {
        Vector3 direction = (target.position - owner.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    public override void Exit()
    {
        Debug.Log("Exiting Follow");
        owner.StopAlert();
        alarmSound.Stop();
    }
}
