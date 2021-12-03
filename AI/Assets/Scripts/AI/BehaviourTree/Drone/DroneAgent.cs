using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAgent : MonoBehaviour, BehaviourTreeDelegates
{
    public Vector3[] patrolPoints;
    public float moveSpeed;

    private SequenceNode rootNode;
    private SelectorNode atLocationCheck;
    private LeafNode isAtLocation;
    private LeafNode findRandomPatrolPoint;
    private LeafNode checkDistanceToNext;
    private LeafNode moveToNextPoint;

    private Vector3 nextPatrolPoint;

    private bool atLocation = true;

    // Start is called before the first frame update
    void Start()
    {
        //root node
        rootNode = new SequenceNode();

        //check if at location
        /**/atLocationCheck = new SelectorNode();
        /**/rootNode.AddChildNode(atLocationCheck);

        //if at location return false
        /*  */BehaviourTreeDelegates.LeafNodeDelegate isAtLocationDelegate = IsAtLocation;
        /*  */isAtLocation = new LeafNode(isAtLocationDelegate);
        /*  */atLocationCheck.AddChildNode(isAtLocation);

        //find random patrol point
        /*  */BehaviourTreeDelegates.LeafNodeDelegate findRandomPatrolPointDelegate = FindRandomPatrolPoint;
        /*  */findRandomPatrolPoint = new LeafNode(findRandomPatrolPointDelegate);
        /*  */atLocationCheck.AddChildNode(findRandomPatrolPoint);

        //check distance to next patrol point
        /**/BehaviourTreeDelegates.LeafNodeDelegate checkDistanceToNextDelegate = CheckDistanceToNext;
        /**/checkDistanceToNext = new LeafNode(checkDistanceToNextDelegate);
        /**/rootNode.AddChildNode(checkDistanceToNext);

        //move to the next patrol point
        /**/BehaviourTreeDelegates.LeafNodeDelegate moveToNextPointDelegate = MoveToNextPoint;
        /**/moveToNextPoint = new LeafNode(moveToNextPointDelegate);
        /**/rootNode.AddChildNode(moveToNextPoint);
    }

    // Update is called once per frame
    void Update()
    {
        rootNode.Execute();
    }

    public bool IsAtLocation()
    {
        return !atLocation;
    }

    public bool FindRandomPatrolPoint()
    {
        int randomIndx = Random.Range(0, patrolPoints.Length);

        while(patrolPoints[randomIndx] == nextPatrolPoint)
        {
            randomIndx = Random.Range(0, patrolPoints.Length);
        }

        nextPatrolPoint = patrolPoints[randomIndx];

        if(nextPatrolPoint != null)
        {
            atLocation = false;
            return true;
        }
        else
        {
            Debug.Log("Error assigning next patrol point");
            return false;
        }
    }

    public bool CheckDistanceToNext()
    {
        if (Mathf.Abs(Vector3.Distance(transform.position, nextPatrolPoint)) < 0.1f)
        {
            atLocation = true;
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool MoveToNextPoint()
    {
        Debug.Log(nextPatrolPoint);

        Vector3 dir = (nextPatrolPoint - transform.position).normalized;

        transform.Translate(dir * moveSpeed * Time.deltaTime);

        return true;
    }
}
