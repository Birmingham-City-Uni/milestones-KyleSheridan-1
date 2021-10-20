using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayBundleSensor : MonoBehaviour
{
    public Transform startPoint;

    public LayerMask hitMask;

    public float raycastLength = 1.0f;

    [Range(2, 20)]
    public int rayResX = 5;
    [Range(2, 20)]
    public int rayResY = 5;
    [Range(10, 360)]
    public int searchArc = 120;

    public bool Hit { get; private set; }
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

    public bool Scan()
    {
        Hit = false;

        float searchArcRad = searchArc * Mathf.Deg2Rad / 2;
        //float angleDifX = searchArcRad * 2 / (rayResX - 1);
        float angleDifY = searchArcRad * 2 / (rayResY - 1);

        for (int y = 0; y < rayResY; y++)
        {
            float newSearchArc = searchArcRad * Mathf.Sin(((((float)rayResY - 1) - y) / ((float)rayResY - 1)) * Mathf.PI);
            Debug.Log(newSearchArc);

            float angleDifX = newSearchArc * 2 / (rayResX - 1);
            //float angleDifY = newSearchArc * 2 / (rayResY - 1);
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
        //float searchArcRad = searchArc * Mathf.Deg2Rad / 2;
        //float angleDifX = searchArcRad * 2 / (rayResX - 1);

        //for (int y = 0; y < rayResY; y++)
        //{
        //    for (int x = 0; x < rayResX; x++)
        //    {
        //        if (!Hit)
        //        {
        //            Vector3 rayDir = new Vector3(Mathf.Sin(searchArcRad - (angleDifX * x)), 0, Mathf.Cos(searchArcRad - (angleDifX * x)));
        //            rayDir.Normalize();
        //            Debug.Log(rayDir);
        //            rayDir = startPoint.TransformDirection(rayDir);
        //            if (Physics.Linecast(startPoint.position, startPoint.position + rayDir * raycastLength, out info, hitMask, QueryTriggerInteraction.Ignore))
        //            {
        //                if(info.transform.CompareTag("Player"))
        //                {
        //                    Hit = true;
        //                }
        //            }
        //        }
        //    }
        //}

        if (Hit) return true;

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Scan();
        if (Hit) Gizmos.color = Color.red;
        Gizmos.matrix *= Matrix4x4.TRS(startPoint.position, startPoint.rotation, Vector3.one);
        float length = raycastLength;

        if (Hit) length = Vector3.Distance(this.startPoint.position, info.point);

        float searchArcRad = searchArc * Mathf.Deg2Rad / 2;
        //float angleDifX = searchArcRad * 2 / (rayResX - 1);
        float angleDifY = searchArcRad * 2 / (rayResY - 1);

        for (int y = 0; y < rayResY; y++)
        {
            float newSearchArc = searchArcRad * Mathf.Sin(((((float)rayResY - 1) - y) / ((float)rayResY - 1)) * Mathf.PI);
            Debug.Log(newSearchArc);

            float angleDifX = newSearchArc * 2 / (rayResX - 1);
            //float angleDifY = newSearchArc * 2 / (rayResY - 1);
            for (int x = 0; x < rayResX; x++)
            {
                Vector3 rayDir = new Vector3(
                    Mathf.Sin(newSearchArc - (angleDifX * x)),
                    Mathf.Sin(searchArcRad - (angleDifY * y)),
                    Mathf.Cos(newSearchArc - (angleDifX * x)) /* * Mathf.Cos(searchArcRad - (angleDifY * y)) */
                    );
                rayDir.Normalize();
                //Debug.Log(rayDir);
                Gizmos.DrawLine(Vector3.zero, rayDir * length);
            }
        }
        //for (int y = 0; y < rayResY; y++)
        //{
        //    for (int x = 0; x < rayResX; x++)
        //    {
        //        Vector3 rayDir = new Vector3(Mathf.Sin(searchArcRad - (angleDifX * x)), Mathf.Sin(searchArcRad - (angleDifY * y)), Mathf.Cos(searchArcRad - (angleDifX * x)) /** Mathf.Cos(searchArcRad - (angleDifY * y))*/);
        //        rayDir.Normalize();
        //        Debug.Log(rayDir);
        //        Gizmos.DrawLine(Vector3.zero, rayDir * length);
        //    }
        //}

        Vector3 cubePoint = startPoint.InverseTransformPoint(info.point);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(cubePoint, new Vector3(0.02f, 0.02f, 0.02f));
    }
}
