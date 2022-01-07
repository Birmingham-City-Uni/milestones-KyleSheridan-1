using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayBundleSensor : MonoBehaviour
{
    //where rays start
    public Transform startPoint;

    //what rays can hit
    public LayerMask hitMask;

    //length of rays
    public static float raycastLength = 30f;

    //the amount of rays on x and y axis
    [Range(2, 20)]
    public int rayResX = 5;
    [Range(2, 20)]
    public int rayResY = 5;
    //the angle of the arc that the rays will be projected from
    [Range(10, 360)]
    public int searchArc = 120;

    //true if the was a hit that frame
    public bool Hit { get; private set; }
    //info of the hit
    public RaycastHit info = new RaycastHit();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Scan();
    }

    //function to cast rays in cone shape
    public bool Scan()
    {
        Hit = false;

        float searchArcRad = searchArc * Mathf.Deg2Rad / 2;
        float angleDifY = searchArcRad * 2 / (rayResY - 1);

        for (int y = 0; y < rayResY; y++)
        {
            // works out the search arc for that value of y (so casts as a cone)
            float newSearchArc = searchArcRad * Mathf.Sin(((((float)rayResY - 1) - y) / ((float)rayResY - 1)) * Mathf.PI);

            float angleDifX = newSearchArc * 2 / (rayResX - 1);
            for (int x = 0; x < rayResX; x++)
            {
                if (!Hit)
                {
                    Vector3 rayDir = new Vector3(
                        Mathf.Sin(newSearchArc - (angleDifX * x)),
                        Mathf.Sin(searchArcRad - (angleDifY * y)),
                        Mathf.Cos(newSearchArc - (angleDifX * x)) /* * Mathf.Cos(searchArcRad - (angleDifY * y)) */
                        );
                    rayDir.Normalize();
                    rayDir = startPoint.TransformDirection(rayDir);
                    if (Physics.Linecast(startPoint.position, startPoint.position + rayDir * raycastLength, out info, hitMask, QueryTriggerInteraction.Ignore))
                    {
                        if (info.transform.CompareTag("Player"))
                        {
                            Hit = true;
                        }
                    }
                }
            }
        }

        if (Hit) return true;

        return false;
    }

    //draw lines to visualise rays
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Scan();
        if (Hit) Gizmos.color = Color.red;
        Gizmos.matrix *= Matrix4x4.TRS(startPoint.position, startPoint.rotation, Vector3.one);
        float length = raycastLength;

        if (Hit) length = Vector3.Distance(this.startPoint.position, info.point);

        float searchArcRad = searchArc * Mathf.Deg2Rad / 2;
        float angleDifY = searchArcRad * 2 / (rayResY - 1);

        for (int y = 0; y < rayResY; y++)
        {
            float newSearchArc = searchArcRad * Mathf.Sin(((((float)rayResY - 1) - y) / ((float)rayResY - 1)) * Mathf.PI);

            float angleDifX = newSearchArc * 2 / (rayResX - 1);
            for (int x = 0; x < rayResX; x++)
            {
                Vector3 rayDir = new Vector3(
                    Mathf.Sin(newSearchArc - (angleDifX * x)),
                    Mathf.Sin(searchArcRad - (angleDifY * y)),
                    Mathf.Cos(newSearchArc - (angleDifX * x)) /* * Mathf.Cos(searchArcRad - (angleDifY * y)) */
                    );
                rayDir.Normalize();
                Gizmos.DrawLine(Vector3.zero, rayDir * length);
            }
        }

        Vector3 cubePoint = startPoint.InverseTransformPoint(info.point);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(cubePoint, new Vector3(0.02f, 0.02f, 0.02f));
    }
}
