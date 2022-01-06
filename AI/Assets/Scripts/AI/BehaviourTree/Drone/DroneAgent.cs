using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAgent : MonoBehaviour, BehaviourTreeDelegates
{
    public Transform player;
    public Vector3[] patrolPoints;
    public float moveSpeed;
    public float searchDist = 5f;
    public float searchTime = 10f;

    //colours of the camera light to represent state
    [Header("State Colours")]
    public Color patrolColour;
    public Color pursueColour;
    public Color lastLocationColour;

    [HideInInspector]
    public Vector3 lastKnownLocation;

    LightController lc;

    //reference to the sensor used to detect player
    RayBundleSensor sensor;

    //reference to the unit component used for pathfinding
    Unit unitComponent;

    //root node
    private SelectorNode rootNode;

    //pursue player
    private SequenceNode pursueRoot;
    private LeafNode canSeePlayer;
    private LeafNode moveToPlayer;

    //Search last known location
    private SequenceNode lastLocationRoot;
    private LeafNode lastLocationKnown;
    private SequenceNode searchLastLocation;
    private SelectorNode atLastLocationCheck;
    private LeafNode isAtLastLocation;
    private LeafNode findRandomSearchPoint;
    private LeafNode checkDistanceToSearchPoint;
    private LeafNode moveToSearchPoint;

    //patrol
    private SequenceNode patrolRoot;
    private SelectorNode atLocationCheck;
    private LeafNode isAtLocation;
    private LeafNode findRandomPatrolPoint;
    private LeafNode checkDistanceToNext;
    private LeafNode moveToNextPoint;

    private Vector3 nextPatrolPoint;
    private Vector3 nextSearchPoint;

    private float patrolPointRange = 0.8f;
    private float yOffset = 5f;

    private bool atLocation = true;
    private bool atLastLocation = true;
    private bool moving = false;
    private bool pursuingPlayer = false;
    private bool lastLocation = false;

    // Start is called before the first frame update
    void Start()
    {
        sensor = GetComponent<RayBundleSensor>();
        unitComponent = GetComponent<Unit>();
        lc = GetComponent<LightController>();

        //root node of behaviour tree
        rootNode = new SelectorNode();

        //root of pursue state --------------------
        /**/pursueRoot = new SequenceNode();
        /**/rootNode.AddChildNode(pursueRoot);

        //check if sensor detects player
        /*  */BehaviourTreeDelegates.LeafNodeDelegate canSeePlayerDelegate = CanSeePlayer;
        /*  */canSeePlayer = new LeafNode(canSeePlayerDelegate);
        /*  */pursueRoot.AddChildNode(canSeePlayer);

        //move towards the player
        /*  */BehaviourTreeDelegates.LeafNodeDelegate moveToPlayerDelegate = MoveToPlayer;
        /*  */moveToPlayer = new LeafNode(moveToPlayerDelegate);
        /*  */pursueRoot.AddChildNode(moveToPlayer);

        //root of last location state -------------
        /**/lastLocationRoot = new SequenceNode();
        /**/rootNode.AddChildNode(lastLocationRoot);

        //check if last location is known
        /*  */BehaviourTreeDelegates.LeafNodeDelegate lastLocationKnownDelegate = LastLocationKnown;
        /*  */lastLocationKnown = new LeafNode(lastLocationKnownDelegate);
        /*  */lastLocationRoot.AddChildNode(lastLocationKnown);

        //search around the last known location
        /*  */searchLastLocation = new SequenceNode();
        /*  */lastLocationRoot.AddChildNode(searchLastLocation);

        //check if at location
        /*    */atLastLocationCheck = new SelectorNode();
        /*    */searchLastLocation.AddChildNode(atLastLocationCheck);

        //if at location return false
        /*      */BehaviourTreeDelegates.LeafNodeDelegate isAtLastLocationDelegate = IsAtLastLocation;
        /*      */isAtLastLocation = new LeafNode(isAtLastLocationDelegate);
        /*      */atLastLocationCheck.AddChildNode(isAtLastLocation);

        //find random search point
        /*      */BehaviourTreeDelegates.LeafNodeDelegate findRandomSearchPointDelegate = FindRandomSearchPoint;
        /*      */findRandomSearchPoint = new LeafNode(findRandomSearchPointDelegate);
        /*      */atLastLocationCheck.AddChildNode(findRandomSearchPoint);

        //check distance to the next search point
        /*    */BehaviourTreeDelegates.LeafNodeDelegate checkDistanceToSearchPointDelegate = CheckDistanceToSearchPoint;
        /*    */checkDistanceToSearchPoint = new LeafNode(checkDistanceToSearchPointDelegate);
        /*    */searchLastLocation.AddChildNode(checkDistanceToSearchPoint);

        //move to search point
        /*    */BehaviourTreeDelegates.LeafNodeDelegate moveToSearchPointDelegate = MoveToSearchPoint;
        /*    */moveToSearchPoint = new LeafNode(moveToSearchPointDelegate);
        /*    */searchLastLocation.AddChildNode(moveToSearchPoint);

        //root of patrol state --------------------
        /**/patrolRoot = new SequenceNode();
        /**/rootNode.AddChildNode(patrolRoot);

        //check if at location
        /*  */atLocationCheck = new SelectorNode();
        /*  */patrolRoot.AddChildNode(atLocationCheck);

        //if at location return false
        /*    */BehaviourTreeDelegates.LeafNodeDelegate isAtLocationDelegate = IsAtLocation;
        /*    */isAtLocation = new LeafNode(isAtLocationDelegate);
        /*    */atLocationCheck.AddChildNode(isAtLocation);

        //find random patrol point
        /*    */BehaviourTreeDelegates.LeafNodeDelegate findRandomPatrolPointDelegate = FindRandomPatrolPoint;
        /*    */findRandomPatrolPoint = new LeafNode(findRandomPatrolPointDelegate);
        /*    */atLocationCheck.AddChildNode(findRandomPatrolPoint);

        //check distance to next patrol point
        /*  */BehaviourTreeDelegates.LeafNodeDelegate checkDistanceToNextDelegate = CheckDistanceToNext;
        /*  */checkDistanceToNext = new LeafNode(checkDistanceToNextDelegate);
        /*  */patrolRoot.AddChildNode(checkDistanceToNext);

        //move to the next patrol point
        /*  */BehaviourTreeDelegates.LeafNodeDelegate moveToNextPointDelegate = MoveToNextPatrolPoint;
        /*  */moveToNextPoint = new LeafNode(moveToNextPointDelegate);
        /*  */patrolRoot.AddChildNode(moveToNextPoint);
    }

    // Update is called once per frame
    void Update()
    {
        rootNode.Execute();
    }

    public bool CanSeePlayer()
    {
        if (!sensor.Hit) 
        {
            if (pursuingPlayer)
            {
                pursuingPlayer = false;
                moving = false;
                lastLocation = true;
                lastKnownLocation = player.position;
                nextSearchPoint = new Vector3(lastKnownLocation.x, yOffset, lastKnownLocation.z);
                lc.ChangeColour(lastLocationColour);
                return false;
            }
            if (!lastLocation)
            {
                lc.ChangeColour(patrolColour);
            }
            return false;
        }

        if (!pursuingPlayer)
        {
            lc.ChangeColour(pursueColour);
            moving = false;
        }
        return true;
    }

    public bool MoveToPlayer()
    {
        if(pursuingPlayer) { return true; }
        if (player != null && !moving)
        {
            unitComponent.StartPath(player);
            pursuingPlayer = true;
            moving = true;
            return true;
        }
        return false;
    }

    public bool LastLocationKnown()
    {
        return lastLocation;
    }

    public bool IsAtLastLocation()
    {
        return !atLastLocation;
    }

    //find new search point in area
    public bool FindRandomSearchPoint()
    {
        float randX = Random.Range(-searchDist, searchDist);
        float randZ = Random.Range(-searchDist, searchDist);

        float searchX = lastKnownLocation.x + randX;
        float searchZ = lastKnownLocation.x + randZ;

        nextSearchPoint = new Vector3(searchX, yOffset, searchZ);

        if (nextSearchPoint != null)
        {
            atLocation = false;
            return true;
        }
        else
        {
            Debug.Log("Error assigning next search point");
            return false;
        }
    }

    //check distance to search point - return false when near
    public bool CheckDistanceToSearchPoint()
    {
        if (Mathf.Abs(Vector3.Distance(transform.position, nextSearchPoint)) < patrolPointRange)
        {
            Debug.Log("pls Help :(");
            atLocation = true;
            moving = false;
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool MoveToSearchPoint()
    {
        //move to search point
        if (!moving)
        {
            unitComponent.StartPath(nextSearchPoint);
            moving = true;
        }
        return true;
    }

    public bool IsAtLocation()
    {
        Debug.Log(!atLocation);
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
        if (Mathf.Abs(Vector3.Distance(transform.position, nextPatrolPoint)) < patrolPointRange)
        {
            atLocation = true;
            moving = false;
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool MoveToNextPatrolPoint()
    {
        if (!moving)
        {
            Debug.Log(nextPatrolPoint);

            unitComponent.StartPath(nextPatrolPoint);

            moving = true;
        }
        return true;
    }

    private IEnumerator StartLastLocationTimer()
    {
        lastLocation = true;

        yield return new WaitForSeconds(searchTime);

        lastLocation = false;
    }

    void OnDrawGizmos()
    {
        foreach(Vector3 point in patrolPoints)
        {
            if(point == nextPatrolPoint)
            {
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.color = Color.blue;
            }

            Gizmos.DrawWireSphere(point, patrolPointRange);
        }

        if (lastLocation)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(nextSearchPoint, patrolPointRange);
        }

        if (pursuingPlayer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, 1f);
        }
    }
}
