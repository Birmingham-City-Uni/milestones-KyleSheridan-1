using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCastSensor : MonoBehaviour
{
    public LayerMask hitMask;

    public Transform startPoint;

    public float spherecastRadius = 1.0f;
    public float spherecastLength = 1.0f;

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
        Vector3 dir = transform.forward;

        if (Physics.SphereCast(new Ray(startPoint.position, dir), spherecastRadius, out info, spherecastLength, hitMask, QueryTriggerInteraction.Ignore))
        {
            Hit = true;
            //Debug.Log("Hit!!");
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (Hit) Gizmos.color = Color.red;

        Gizmos.matrix *= Matrix4x4.TRS(startPoint.position, transform.rotation, Vector3.one);
        float length = spherecastLength;

        Gizmos.DrawWireSphere(Vector3.zero, spherecastRadius);

        if (Hit)
        {
            Vector3 ballCenter = info.point + info.normal * spherecastRadius;
            length = Vector3.Distance(startPoint.position, ballCenter);
        }

        Gizmos.DrawWireSphere(Vector3.zero + Vector3.forward * length, spherecastRadius);

        Gizmos.DrawLine(Vector3.zero + Vector3.up * spherecastRadius, (Vector3.zero + Vector3.forward * length) + Vector3.up * spherecastRadius);
        Gizmos.DrawLine(Vector3.zero + Vector3.right * spherecastRadius, (Vector3.zero + Vector3.forward * length) + Vector3.right * spherecastRadius);
        Gizmos.DrawLine(Vector3.zero + Vector3.down * spherecastRadius, (Vector3.zero + Vector3.forward * length) + Vector3.down * spherecastRadius);
        Gizmos.DrawLine(Vector3.zero + Vector3.left * spherecastRadius, (Vector3.zero + Vector3.forward * length) + Vector3.left * spherecastRadius);
    }
}
