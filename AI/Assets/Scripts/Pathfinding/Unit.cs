using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;

    //public Transform target;
    public float speed = 5f;
    public float turnSpeed = 3;
    public float turnDist = 5;

    public float stoppingDst = 10;

    public float yOffset = 0;

    Path path;

    private Coroutine updateCoroutine;
    private Coroutine followCoroutine;

    public void StartPath(Vector3 target)
    {
        StopPath();
        updateCoroutine = StartCoroutine(UpdatePath(target));
    }
    public void StartPath(Transform target)
    {
        StopPath();
        updateCoroutine = StartCoroutine(UpdatePath(target));
    }

    public void StopPath()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
        }
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position, turnDist, stoppingDst);
            if (followCoroutine != null)
            {
                StopCoroutine(followCoroutine);
            }
            followCoroutine = StartCoroutine(FollowPath());
        }
    }

    IEnumerator UpdatePath(Vector3 target)
    {
        if(Time.timeSinceLevelLoad < .3)
        {
            yield return new WaitForSeconds(.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, target, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if((target - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target, OnPathFound));
                targetPosOld = target;
            }
        }
    }
    IEnumerator UpdatePath(Transform target)
    {
        if (Time.timeSinceLevelLoad < .3)
        {
            yield return new WaitForSeconds(.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
            }
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        Vector3 targetPoint = new Vector3(path.lookPoints[pathIndex].x, path.lookPoints[pathIndex].y + yOffset, path.lookPoints[pathIndex].z);
        transform.LookAt(targetPoint);

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundries[pathIndex].HasCrossedLine(pos2D))
            {
                if(pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if(pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    if(speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }

                targetPoint = new Vector3(path.lookPoints[pathIndex].x, path.lookPoints[pathIndex].y + yOffset, path.lookPoints[pathIndex].z);

                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }
            
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if(path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
